// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Objects {
    public interface IMessageBroker {
        string[] PeekMessages { get; }

        string[] PeekWarnings { get; }

        string[] Messages { get; }

        string[] Warnings { get; }

        void AddWarning(string message);

        void AddMessage(string message);

        void ClearWarnings();

        void ClearMessages();

        void EnsureEmpty();
    }

    // Copyright (c) Naked Objects Group Ltd.
}