// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Reflect {
    public interface INakedObjectAction : INakedObjectMember {
        /// <summary>
        ///     Returns where the action should be executed: explicitly locally on the client; explicitly remotely on
        ///     the server; or where it normally should be executed. By default instance methods should execute on the
        ///     server, static methods should execute on the client.
        /// </summary>
        /// <seealso cref="NakedObjectActionConstants.LOCAL" />
        /// <seealso cref="NakedObjectActionConstants.REMOTE" />
        /// <seealso cref="NakedObjectActionConstants.DEFAULT" />
        Where Target { get; }

        /// <summary>
        ///     Returns the specification for the type of object that this action can be invoked upon
        /// </summary>
        INakedObjectSpecification OnType { get; }

        /// <summary>
        ///     Return true if the action is run on a service object using the target object as a parameter
        /// </summary>
        bool IsContributedMethod { get; }


        /// <summary>
        ///     Return true if the action is run on a service object and can be used as a finder
        /// </summary>
        bool IsFinderMethod { get; }

        /// <summary>
        ///     Returns the type of action: user, exploration or debug, or that it is a set of actions.
        /// </summary>
        /// <seealso cref="NakedObjectActionType.User" />
        /// <seealso cref="NakedObjectActionConstants.PROTOTYPE" />
        /// <seealso cref="NakedObjectActionConstants.DEBUG" />
        /// <seealso cref="NakedObjectActionConstants.SET" />
        NakedObjectActionType ActionType { get; }

        /// <summary>
        ///     Returns the specifications for the return type
        /// </summary>
        INakedObjectSpecification ReturnType { get; }

        /// <summary>
        ///     Lists the sub-actions that are available under this name. If any actions are returned then this action
        ///     is only a set and not an action itself.
        /// </summary>
        INakedObjectAction[] Actions { get; }

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
        INakedObjectActionParameter[] Parameters { get; }

        /// <summary>
        ///     Return true if the action is run on a service object using the target object as a parameter
        /// </summary>
        bool IsContributedTo(INakedObjectSpecification spec);

        bool PromptForParameters(INakedObject nakedObject);


        /// <summary>
        ///     Determine the real target for this action. If this action represents an object action than the target
        ///     is returned. If this action is on a service then that service will be returned.
        /// </summary>
        INakedObject RealTarget(INakedObject target, INakedObjectPersistor persistor);


        /// <summary>
        ///     Returns true if the represented action returns something, else returns false
        /// </summary>
        bool HasReturn();

        /// <summary>
        ///     Invokes the action's method on the target object given the specified set of parameters
        /// </summary>
        INakedObject Execute(INakedObject target, INakedObject[] parameterSet, INakedObjectPersistor persistor, ISession session);


        /// <summary>
        ///     Whether the provided parameter set is valid
        /// </summary>
        IConsent IsParameterSetValid(ISession session, INakedObject nakedObject, INakedObject[] parameterSet, INakedObjectPersistor persistor);


        /// <summary>
        ///     Returns set of parameter information matching the supplied filter.
        /// </summary>
        INakedObjectActionParameter[] GetParameters(INakedObjectActionParameterFilter filter);

        INakedObject[] RealParameters(INakedObject target, INakedObject[] parameterSet);
    }

    // Copyright (c) Naked Objects Group Ltd.
}