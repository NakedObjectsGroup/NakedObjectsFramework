// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Reflector.DotNet.Facets.Ordering;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.DotNet.Reflect.Actions {
    // TODO (in all DotNet...Peer classes) make all methodsArray throw ReflectiveActionException when 
    // an exception occurs when calling a method reflectively (see execute method).  Then instead of 
    // calling invocationExcpetion() the exception will be passed though, and dealt with generally by 
    // the reflection package (which will be the same for all reflectors and will allow the message to
    // be better passed back to the client).


    public class DotNetNakedObjectActionPeer : DotNetNakedObjectMemberPeer, INakedObjectActionPeer {
        private readonly INakedObjectActionParamPeer[] parameters;

        public DotNetNakedObjectActionPeer(IIdentifier identifier, INakedObjectActionParamPeer[] parameters)
            : base(identifier) {
            this.parameters = parameters;
        }

        #region INakedObjectActionPeer Members

        public INakedObjectActionParamPeer[] Parameters {
            get { return parameters; }
        }

        #endregion

        public INakedObjectActionPeer Peer { get { return this; }}
        public OrderSet<INakedObjectActionPeer> Set { get { return null; } }
    }

    // Copyright (c) Naked Objects Group Ltd.
}