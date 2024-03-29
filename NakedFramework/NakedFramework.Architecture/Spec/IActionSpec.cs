// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Reflect;

namespace NakedFramework.Architecture.Spec;

/// <summary>
///     The specification for an action method on a domain object (or service).
/// </summary>
public interface IActionSpec : IMemberSpec {
    /// <summary>
    ///     Returns the specification for the type of object that this action can be invoked upon
    /// </summary>
    ITypeSpec OnSpec { get; }

    IObjectSpec ElementSpec { get; }

    /// <summary>
    ///     Return true if the action is run on a service object using the target object as a parameter
    /// </summary>
    bool IsContributedMethod { get; }

    bool IsStaticFunction { get; }

    /// <summary>
    ///     Returns the number of parameters used by this method
    /// </summary>
    int ParameterCount { get; }

    /// <summary>
    ///     Returns set of parameter information.
    /// </summary>
    /// <para>
    ///     Implementations may build this array lazily or eagerly as required
    /// </para>
    IActionParameterSpec[] Parameters { get; }

    /// <summary>
    ///     Returns true if the represented action returns something, else returns false
    /// </summary>
    bool HasReturn { get; }

    /// <summary>
    ///     Return true if the action is run on a service object and can be used as a finder
    /// </summary>
    bool GetIsFinderMethod(IMetamodel metamodel);

    /// <summary>
    ///     Determine the real target for this action. If this action represents an object action than the target
    ///     is returned. If this action is on a service then that service will be returned.
    /// </summary>
    INakedObjectAdapter RealTarget(INakedObjectAdapter target);

    /// <summary>
    ///     Invokes the action's method on the target object given the specified set of parameters
    /// </summary>
    INakedObjectAdapter Execute(INakedObjectAdapter target, INakedObjectAdapter[] parameterSet);

    /// <summary>
    ///     Whether the provided parameter set is valid
    /// </summary>
    IConsent IsParameterSetValid(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter[] parameterSet);

    INakedObjectAdapter[] RealParameters(INakedObjectAdapter target, INakedObjectAdapter[] parameterSet);

    string GetFinderMethodPrefix();
}

// Copyright (c) Naked Objects Group Ltd.