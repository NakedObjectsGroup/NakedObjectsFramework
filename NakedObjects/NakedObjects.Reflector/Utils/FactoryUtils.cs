﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using NakedFramework.Core.Error;

namespace NakedObjects.Reflector.Utils {
    public static class FactoryUtils {
        
        public static T Invoke<T>(this Func<object, object[], object> methodDelegate, MethodInfo method, object target,  object[] parms) {
            try {
                return methodDelegate is not null ? (T) methodDelegate(target, parms) : (T) method.Invoke(target, parms);
            }
            catch (InvalidCastException) {
                throw new NakedObjectDomainException($"Must return {typeof(T)} from  method: {method.Name}");
            }
        }
    }
}