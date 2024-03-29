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
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Framework;
using NOF2.About;
using NOF2.Container;

namespace NOF2.Reflector.Helpers;

public static class AboutHelpers {
    public enum AboutType {
        Action,
        Field
    }

    public static object[] GetParameters(this MethodInfo method, INakedFramework framework, object about, bool substitute, params object[] proposedValues) {
        var parameterCount = method.GetParameters().Length;
        var parameters = new List<object> { about };

        if (parameterCount > 1) {
            var hasContainer = method.GetParameters().Last().ParameterType.IsAssignableTo(typeof(IContainer));
            var frameworkParameters = hasContainer ? 2 : 1;
            var parmPlaceholders = new object[parameterCount - frameworkParameters];
            var containerPlaceholder = hasContainer ? new object[] { null } : Array.Empty<object>();

            var actualParameters = (proposedValues?.Any() == true ? proposedValues : parmPlaceholders).Concat(containerPlaceholder);
            parameters.AddRange(actualParameters);
        }

        var rawParameters = parameters.ToArray();

        return substitute ? NOF2Helpers.SubstituteNullsAndContainer(rawParameters, method, framework) : NOF2Helpers.InjectContainer(rawParameters, method, framework);
    }

    public static IAbout AboutFactory(this AboutType aboutType, AboutTypeCodes aboutTypeCode, INakedFramework framework) {
        var factory = framework.ServiceProvider.GetService<IAboutFactory>();
        return aboutType is AboutType.Action
            ? factory.NewActionAbout(aboutTypeCode)
            : factory.NewFieldAbout(aboutTypeCode);
    }
}