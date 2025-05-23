﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using NakedFramework.Core.Error;
using NakedFramework.Error;

namespace NakedFramework.Core.Util;

public static class DelegateUtils {
    // This is all based on http://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/comment-page-1/

    private const int MaxParameters = 6;

    private static string CreationFailed(string reason, MethodInfo method) =>
        $"Not creating delegate for method {method.DeclaringType}.{method}. It will be called using reflection rather than a delegate. This is because {reason}";

    public static (Func<object, object[], object>, string) CreateDelegate(MethodInfo method) {
        try {
            if (method.IsSecurityTransparent) {
                // don't seem to be able to bind delegates to these just return null
                return (null, CreationFailed("property 'IsSecurityTransparent' is true", method));
            }

            if (method.ContainsGenericParameters) {
                // don't seem to be able to bind delegates to these just return null
                return (null, CreationFailed("property 'ContainsGenericParameters' is true", method));
            }

            if (method.DeclaringType?.IsClass == false) {
                // don't seem to be able to bind delegates to these just return null
                return (null, CreationFailed("property 'DeclaringType.IsClass' is false", method));
            }

            var parameters = method.GetParameters();

            if (parameters.Length > MaxParameters) {
                // only support 6 parameters via delegates - return null and default to reflection
                return (null, CreationFailed($"it has more than {MaxParameters} parameters. Suggest: reducing the number of parameters if possible.", method));
            }

            var delegateHelper = MakeDelegateHelper(method.DeclaringType, method, parameters);

            // Now call it. The null argument is because it’s a static method.
            // Cast the result to the right kind of delegate
            var ret = (Func<object, object[], object>)delegateHelper.Invoke(null, new object[] { method });

            return (ret, "");
        }
        catch (Exception e) {
            return (null, e.Message);
        }
    }

    public static Action<object> CreateCallbackDelegate(MethodInfo method) {
        var genericHelper = typeof(DelegateUtils).GetMethod("CallbackHelper", BindingFlags.Static | BindingFlags.NonPublic);

        // Now supply the type arguments
        var typeArgs = new List<Type> { method.DeclaringType };
        var delegateHelper = genericHelper.MakeGenericMethod(typeArgs.ToArray());

        // Now call it. The null argument is because it’s a static method.
        var ret = delegateHelper.Invoke(null, new object[] { method });

        // Cast the result to the right kind of delegate and return it
        return (Action<object>)ret;
    }

    public static Type GetTypeAuthorizerType(Type type) {
        if (type is null) {
            return null;
        }

        return type.Name.StartsWith("ITypeAuthorizer")
            ? type
            : type.GetInterfaces().Select(GetTypeAuthorizerType).FirstOrDefault(t => t is not null);
    }

    public static Func<object, IPrincipal, object, string, bool> CreateObjectTypeAuthorizerDelegate(MethodInfo method) {
        var genericHelper = typeof(DelegateUtils).GetMethod("TypeAuthorizerHelper", BindingFlags.Static | BindingFlags.NonPublic);

        // Now supply the type arguments
        var typeArgs = new List<Type> { method.DeclaringType, Enumerable.First(GetTypeAuthorizerType(method.DeclaringType).GenericTypeArguments) };
        var delegateHelper = genericHelper.MakeGenericMethod(typeArgs.ToArray());

        // Now call it. The null argument is because it’s a static method.
        var ret = delegateHelper.Invoke(null, new object[] { method });

        // Cast the result to the right kind of delegate and return it
        return (Func<object, IPrincipal, object, string, bool>)ret;
    }

    private static MethodInfo MakeDelegateHelper(Type targetType, MethodInfo method, ParameterInfo[] parameters) {
        var prefix = method.IsStatic ? "Static" : "";
        var name = method.ReturnType == typeof(void) ? "ActionHelper" : "FuncHelper";
        var postfix = parameters.Length;

        var helperName = $"{prefix}{name}{postfix}";

        // First fetch the generic form
        var genericHelper = typeof(DelegateUtils).GetMethod(helperName, BindingFlags.Static | BindingFlags.NonPublic);

        // Now supply the type arguments
        var typeArgs = new List<Type> { targetType };
        typeArgs.AddRange(parameters.Select(p => p.ParameterType));

        if (name is "FuncHelper") {
            typeArgs.Add(method.ReturnType);
        }

        return genericHelper.MakeGenericMethod(typeArgs.ToArray());
    }

    private static T Cast<T>(object tocast) =>
        // ReSharper disable once MergeConditionalExpression
        // Do not simplify - this is to catch value parms and cast to their default value if they're being cleared
        // with a null. 
        (T)(tocast ?? default(T));

    private static Func<object, object[], object> WrapException(Func<object, object[], object> func) {
        return (target, param) => {
            try {
                return func(target, param);
            }
            catch (DomainException e) {
                // wrap this for compatibility with calls via invoke
                throw new NakedObjectDomainException(e.Message, e);
            }
            catch (ArgumentException e) {
                // wrap this so not confused with ArgumentExceptions from framework code
                throw new NakedObjectDomainException(e.Message, e);
            }
            catch (TargetParameterCountException e) {
                // wrap this so not confused with TargetParameterCountException from framework code
                throw new NakedObjectDomainException(e.Message, e);
            }
        };
    }

    // These are all called via reflection - so do not delete or change name without changing the Invoke above !
    // in each convert the slow MethodInfo into a fast, strongly typed, open delegate
    // then create a more weakly typed delegate which will call the strongly typed one
    // at some point in the future these may be replaced with generated code.  

    // ReSharper disable UnusedMember.Local
    private static Action<object> CallbackHelper<TTarget>(MethodInfo method) {
        var action = (Action<TTarget>)Delegate.CreateDelegate(typeof(Action<TTarget>), method);
        return target => action((TTarget)target);
    }

    private static Func<object, object[], object> ActionHelper0<TTarget>(MethodInfo method) {
        var action = (Action<TTarget>)Delegate.CreateDelegate(typeof(Action<TTarget>), method);
        return WrapException((target, param) => {
            action((TTarget)target);
            return null;
        });
    }

    private static Func<object, object[], object> ActionHelper1<TTarget, TParam0>(MethodInfo method) where TTarget : class {
        var action = (Action<TTarget, TParam0>)Delegate.CreateDelegate(typeof(Action<TTarget, TParam0>), method);
        return WrapException((target, param) => {
            action((TTarget)target, Cast<TParam0>(param[0]));
            return null;
        });
    }

    private static Func<object, object[], object> ActionHelper2<TTarget, TParam0, TParam1>(MethodInfo method) where TTarget : class {
        var action = (Action<TTarget, TParam0, TParam1>)Delegate.CreateDelegate(typeof(Action<TTarget, TParam0, TParam1>), method);
        return WrapException((target, param) => {
            action((TTarget)target, Cast<TParam0>(param[0]), Cast<TParam1>(param[1]));
            return null;
        });
    }

    private static Func<object, object[], object> ActionHelper3<TTarget, TParam0, TParam1, TParam2>(MethodInfo method) where TTarget : class {
        var action = (Action<TTarget, TParam0, TParam1, TParam2>)Delegate.CreateDelegate(typeof(Action<TTarget, TParam0, TParam1, TParam2>), method);
        return WrapException((target, param) => {
            action((TTarget)target, Cast<TParam0>(param[0]), Cast<TParam1>(param[1]), Cast<TParam2>(param[2]));
            return null;
        });
    }

    private static Func<object, object[], object> ActionHelper4<TTarget, TParam0, TParam1, TParam2, TParam3>(MethodInfo method) where TTarget : class {
        var action = (Action<TTarget, TParam0, TParam1, TParam2, TParam3>)Delegate.CreateDelegate(typeof(Action<TTarget, TParam0, TParam1, TParam2, TParam3>), method);
        return WrapException((target, param) => {
            action((TTarget)target, Cast<TParam0>(param[0]), Cast<TParam1>(param[1]), Cast<TParam2>(param[2]), Cast<TParam3>(param[3]));
            return null;
        });
    }

    private static Func<object, object[], object> ActionHelper5<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4>(MethodInfo method) where TTarget : class {
        var action = (Action<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4>)Delegate.CreateDelegate(typeof(Action<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4>), method);
        return (target, param) => {
            action((TTarget)target, Cast<TParam0>(param[0]), Cast<TParam1>(param[1]), Cast<TParam2>(param[2]), Cast<TParam3>(param[3]), Cast<TParam4>(param[4]));
            return null;
        };
    }

    private static Func<object, object[], object> ActionHelper6<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TParam5>(MethodInfo method) where TTarget : class {
        var action = (Action<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TParam5>)Delegate.CreateDelegate(typeof(Action<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TParam5>), method);
        return WrapException((target, param) => {
            action((TTarget)target, Cast<TParam0>(param[0]), Cast<TParam1>(param[1]), Cast<TParam2>(param[2]), Cast<TParam3>(param[3]), Cast<TParam4>(param[4]), Cast<TParam5>(param[5]));
            return null;
        });
    }

    private static Func<object, object[], object> StaticActionHelper1<TTarget, TParam0>(MethodInfo method) where TTarget : class {
        var action = (Action<TParam0>)Delegate.CreateDelegate(typeof(Action<TParam0>), method);
        return WrapException((target, param) => {
            action(Cast<TParam0>(param[0]));
            return null;
        });
    }

    private static Func<object, object[], object> StaticActionHelper2<TTarget, TParam0, TParam1>(MethodInfo method) where TTarget : class {
        var action = (Action<TParam0, TParam1>)Delegate.CreateDelegate(typeof(Action<TParam0, TParam1>), method);
        return WrapException((target, param) => {
            action(Cast<TParam0>(param[0]), Cast<TParam1>(param[1]));
            return null;
        });
    }

    private static Func<object, object[], object> StaticActionHelper3<TTarget, TParam0, TParam1, TParam2>(MethodInfo method) where TTarget : class {
        var action = (Action<TParam0, TParam1, TParam2>)Delegate.CreateDelegate(typeof(Action<TParam0, TParam1, TParam2>), method);
        return WrapException((target, param) => {
            action(Cast<TParam0>(param[0]), Cast<TParam1>(param[1]), Cast<TParam2>(param[2]));
            return null;
        });
    }

    private static Func<object, object[], object> StaticActionHelper4<TTarget, TParam0, TParam1, TParam2, TParam3>(MethodInfo method) where TTarget : class {
        var action = (Action<TParam0, TParam1, TParam2, TParam3>)Delegate.CreateDelegate(typeof(Action<TParam0, TParam1, TParam2, TParam3>), method);
        return WrapException((target, param) => {
            action(Cast<TParam0>(param[0]), Cast<TParam1>(param[1]), Cast<TParam2>(param[2]), Cast<TParam3>(param[3]));
            return null;
        });
    }

    private static Func<object, object[], object> StaticActionHelper5<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4>(MethodInfo method) where TTarget : class {
        var action = (Action<TParam0, TParam1, TParam2, TParam3, TParam4>)Delegate.CreateDelegate(typeof(Action<TParam0, TParam1, TParam2, TParam3, TParam4>), method);
        return (target, param) => {
            action(Cast<TParam0>(param[0]), Cast<TParam1>(param[1]), Cast<TParam2>(param[2]), Cast<TParam3>(param[3]), Cast<TParam4>(param[4]));
            return null;
        };
    }

    private static Func<object, object[], object> StaticActionHelper6<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TParam5>(MethodInfo method) where TTarget : class {
        var action = (Action<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5>)Delegate.CreateDelegate(typeof(Action<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5>), method);
        return WrapException((target, param) => {
            action(Cast<TParam0>(param[0]), Cast<TParam1>(param[1]), Cast<TParam2>(param[2]), Cast<TParam3>(param[3]), Cast<TParam4>(param[4]), Cast<TParam5>(param[5]));
            return null;
        });
    }

    private static Func<object, IPrincipal, object, string, bool> TypeAuthorizerHelper<TTarget, TAuth>(MethodInfo method) where TTarget : class {
        var func = (Func<TTarget, IPrincipal, TAuth, string, bool>)Delegate.CreateDelegate(typeof(Func<TTarget, IPrincipal, TAuth, string, bool>), method);
        return (target, principal, auth, name) => func((TTarget)target, principal, (TAuth)auth, name);
    }

    private static Func<object, object[], object> FuncHelper0<TTarget, TReturn>(MethodInfo method) where TTarget : class {
        var func = (Func<TTarget, TReturn>)Delegate.CreateDelegate(typeof(Func<TTarget, TReturn>), method);
        return WrapException((target, param) => func((TTarget)target));
    }

    private static Func<object, object[], object> FuncHelper1<TTarget, TParam0, TReturn>(MethodInfo method) where TTarget : class {
        var func = (Func<TTarget, TParam0, TReturn>)Delegate.CreateDelegate(typeof(Func<TTarget, TParam0, TReturn>), method);
        return WrapException((target, param) => func((TTarget)target, Cast<TParam0>(param[0])));
    }

    private static Func<object, object[], object> FuncHelper2<TTarget, TParam0, TParam1, TReturn>(MethodInfo method) where TTarget : class {
        var func = (Func<TTarget, TParam0, TParam1, TReturn>)Delegate.CreateDelegate(typeof(Func<TTarget, TParam0, TParam1, TReturn>), method);
        return WrapException((target, param) => func((TTarget)target, Cast<TParam0>(param[0]), Cast<TParam1>(param[1])));
    }

    private static Func<object, object[], object> FuncHelper3<TTarget, TParam0, TParam1, TParam2, TReturn>(MethodInfo method) where TTarget : class {
        var func = (Func<TTarget, TParam0, TParam1, TParam2, TReturn>)Delegate.CreateDelegate(typeof(Func<TTarget, TParam0, TParam1, TParam2, TReturn>), method);
        return WrapException((target, param) => func((TTarget)target, Cast<TParam0>(param[0]), Cast<TParam1>(param[1]), Cast<TParam2>(param[2])));
    }

    private static Func<object, object[], object> FuncHelper4<TTarget, TParam0, TParam1, TParam2, TParam3, TReturn>(MethodInfo method) where TTarget : class {
        var func = (Func<TTarget, TParam0, TParam1, TParam2, TParam3, TReturn>)Delegate.CreateDelegate(typeof(Func<TTarget, TParam0, TParam1, TParam2, TParam3, TReturn>), method);
        return WrapException((target, param) => func((TTarget)target, Cast<TParam0>(param[0]), Cast<TParam1>(param[1]), Cast<TParam2>(param[2]), Cast<TParam3>(param[3])));
    }

    private static Func<object, object[], object> FuncHelper5<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TReturn>(MethodInfo method) where TTarget : class {
        var func = (Func<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TReturn>)Delegate.CreateDelegate(typeof(Func<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TReturn>), method);
        return WrapException((target, param) => func((TTarget)target, Cast<TParam0>(param[0]), Cast<TParam1>(param[1]), Cast<TParam2>(param[2]), Cast<TParam3>(param[3]), Cast<TParam4>(param[4])));
    }

    private static Func<object, object[], object> FuncHelper6<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TReturn>(MethodInfo method) where TTarget : class {
        var func = (Func<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TReturn>)Delegate.CreateDelegate(typeof(Func<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TReturn>), method);
        return WrapException((target, param) => func((TTarget)target, Cast<TParam0>(param[0]), Cast<TParam1>(param[1]), Cast<TParam2>(param[2]), Cast<TParam3>(param[3]), Cast<TParam4>(param[4]), Cast<TParam5>(param[5])));
    }

    private static Func<object, object[], object> StaticFuncHelper0<TTarget, TReturn>(MethodInfo method) where TTarget : class {
        var func = (Func<TReturn>)Delegate.CreateDelegate(typeof(Func<TReturn>), method);
        return WrapException((target, param) => func());
    }

    private static Func<object, object[], object> StaticFuncHelper1<TTarget, TParam0, TReturn>(MethodInfo method) where TTarget : class {
        var func = (Func<TParam0, TReturn>)Delegate.CreateDelegate(typeof(Func<TParam0, TReturn>), method);
        return WrapException((target, param) => func(Cast<TParam0>(param[0])));
    }

    private static Func<object, object[], object> StaticFuncHelper2<TTarget, TParam0, TParam1, TReturn>(MethodInfo method) where TTarget : class {
        var func = (Func<TParam0, TParam1, TReturn>)Delegate.CreateDelegate(typeof(Func<TParam0, TParam1, TReturn>), method);
        return WrapException((target, param) => func(Cast<TParam0>(param[0]), Cast<TParam1>(param[1])));
    }

    private static Func<object, object[], object> StaticFuncHelper3<TTarget, TParam0, TParam1, TParam2, TReturn>(MethodInfo method) where TTarget : class {
        var func = (Func<TParam0, TParam1, TParam2, TReturn>)Delegate.CreateDelegate(typeof(Func<TParam0, TParam1, TParam2, TReturn>), method);
        return WrapException((target, param) => func(Cast<TParam0>(param[0]), Cast<TParam1>(param[1]), Cast<TParam2>(param[2])));
    }

    private static Func<object, object[], object> StaticFuncHelper4<TTarget, TParam0, TParam1, TParam2, TParam3, TReturn>(MethodInfo method) where TTarget : class {
        var func = (Func<TParam0, TParam1, TParam2, TParam3, TReturn>)Delegate.CreateDelegate(typeof(Func<TParam0, TParam1, TParam2, TParam3, TReturn>), method);
        return WrapException((target, param) => func(Cast<TParam0>(param[0]), Cast<TParam1>(param[1]), Cast<TParam2>(param[2]), Cast<TParam3>(param[3])));
    }

    private static Func<object, object[], object> StaticFuncHelper5<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TReturn>(MethodInfo method) where TTarget : class {
        var func = (Func<TParam0, TParam1, TParam2, TParam3, TParam4, TReturn>)Delegate.CreateDelegate(typeof(Func<TParam0, TParam1, TParam2, TParam3, TParam4, TReturn>), method);
        return WrapException((target, param) => func(Cast<TParam0>(param[0]), Cast<TParam1>(param[1]), Cast<TParam2>(param[2]), Cast<TParam3>(param[3]), Cast<TParam4>(param[4])));
    }

    private static Func<object, object[], object> StaticFuncHelper6<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TReturn>(MethodInfo method) where TTarget : class {
        var func = (Func<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TReturn>)Delegate.CreateDelegate(typeof(Func<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TReturn>), method);
        return WrapException((target, param) => func(Cast<TParam0>(param[0]), Cast<TParam1>(param[1]), Cast<TParam2>(param[2]), Cast<TParam3>(param[3]), Cast<TParam4>(param[4]), Cast<TParam5>(param[5])));
    }

    // ReSharper restore UnusedMember.Local
}