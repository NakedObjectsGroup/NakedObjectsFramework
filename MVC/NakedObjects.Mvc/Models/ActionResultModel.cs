// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Web.Mvc.Models {
    public abstract class ActionResultModel : IEnumerable {
        protected ActionResultModel(IActionSpec action, IEnumerable result) {
            Action = action;
            Result = result;
        }

        public IActionSpec Action { get; private set; }
        public IEnumerable Result { get; private set; }

        public int PageSize { get; set; }
        public int Page { get; set; }
        public string Format { get; set; }

        public IEnumerator GetEnumerator() {
            return Result.GetEnumerator();
        }

        public static ActionResultModel Create(INakedObjectsFramework framework, IActionSpec action, INakedObjectAdapter nakedObject, int page, int pageSize, string format) {
            var result = (IEnumerable) nakedObject.Object;
            Type genericType = result.GetType().IsGenericType ? result.GetType().GetGenericArguments().First() : typeof (object);
            Type armGenericType = result is IQueryable ? typeof (ActionResultModelQ<>) : typeof (ActionResultModel<>);
            Type armType = armGenericType.MakeGenericType(genericType);
            var arm = (ActionResultModel) Activator.CreateInstance(armType, action, result);
            INakedObjectAdapter noArm = framework.NakedObjectManager.CreateAdapter(arm, null, null);
            var currentMemento = (ICollectionMemento) nakedObject.Oid;
            var newMemento = currentMemento.NewSelectionMemento(new object[] {}, false);
            noArm.SetATransientOid(newMemento);
            arm.Page = page;
            arm.PageSize = pageSize;
            arm.Format = format;
            return arm;
        }
    }

    public class ActionResultModel<T> : ActionResultModel, IEnumerable<T> {
        public ActionResultModel(IActionSpec action, IEnumerable<T> result) : base(action, result) {
            Result = result;
        }

        public new IEnumerable<T> Result { get; private set; }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return Result.GetEnumerator();
        }
    }

    public class ActionResultModelQ<T> : ActionResultModel<T>, IQueryable<T> {
        public ActionResultModelQ(IActionSpec action, IQueryable<T> result)
            : base(action, result) {
            Result = result;
        }

        public new IQueryable<T> Result { get; private set; }

        public Expression Expression {
            get { return Result.Expression; }
        }

        public Type ElementType {
            get { return Result.ElementType; }
        }

        public IQueryProvider Provider {
            get { return Result.Provider; }
        }
    }
}