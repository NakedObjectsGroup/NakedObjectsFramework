﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NOF2.Container;

public interface IContainer {
    public void AddMessageToBroker(string message);
    public void AddWarningToBroker(string message);
    public void ClearWarnings();
    public IEnumerable AllInstances(Type ofType);
    public IQueryable<T> AllInstances<T>() where T : class;
    public object DomainService(Type ofType);
    public T DomainService<T>();
    public object CreateTransientInstance(Type ofType);
    public T CreateTransientInstance<T>() where T : new();
    public void MakePersistent<T>(ref T transientObject);
    public bool IsPersistent(object obj);
    public IList<Type> AllTypes();
    public void Resolve(object obj);
    public T SystemService<T>();
}