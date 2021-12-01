// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedFramework.Facade.Contexts;

namespace NakedFramework.Facade.Error; 

public abstract class WithContextNOSException : NakedObjectsFacadeException {
    private readonly IList<ContextFacade> contexts;
    protected WithContextNOSException() { }
    protected WithContextNOSException(string message) : base(message) { }
    protected WithContextNOSException(string message, Exception e) : base(message, e) { }

    protected WithContextNOSException(string message, IList<ContextFacade> contexts) : this(message, null, contexts) { }

    protected WithContextNOSException(string message, ContextFacade context) : this(message, context, null) { }

    protected WithContextNOSException(string message, ContextFacade context, IList<ContextFacade> contexts) : this(message) {
        Contexts = contexts;
        ContextFacade = context;
    }

    public IList<ContextFacade> Contexts {
        get {
            if (contexts == null) {
                return ContextFacade == null ? Array.Empty<ContextFacade>() : new[] {ContextFacade};
            }

            return contexts;
        }
        private init => contexts = value;
    }

    public ContextFacade ContextFacade { get; }
}