// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Security;

namespace NakedObjects.Architecture.Resolve {
    public interface IResolveState {
        string Name { get; }
        string Code { get; }
        IResolveState Handle(IResolveEvent rEvent, INakedObject owner, IResolveStateMachine rsm, ISession s, INakedObjectPersistor p);
    }
}