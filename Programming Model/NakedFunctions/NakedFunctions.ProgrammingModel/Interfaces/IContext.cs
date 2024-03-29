﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;

namespace NakedFunctions; 

public interface IContext {
    //Obtains a queryable of a given domain type, from the persistor.
    public IQueryable<T> Instances<T>() where T : class;

    //Gets a service that has been configured in services configuration.
    public T GetService<T>();

    public IContext WithNew<T>(T newObj) where T : class;

    public IContext WithUpdated<T>(T original, T updated) where T : class;

    public IContext WithDeleted<T>(T deleteObj) where T : class;

    IContext WithDeferred(Func<IContext, IContext> function);

    public T Reload<T>(T unsaved) where T : class;

    public T Resolve<T>(T unResolved) where T : class;

    public IContext RaiseError(string message);

    public IContext RaiseError(Exception exception);
}