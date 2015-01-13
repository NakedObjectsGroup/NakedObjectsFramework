// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Adapter;

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

        public static ActionResultModel Create(INakedObjectsFramework framework,  IActionSpec action, INakedObject nakedObject, int page, int pageSize, string format) {
            var result = (IEnumerable) nakedObject.Object;
            Type genericType = result.GetType().IsGenericType ? result.GetType().GetGenericArguments().First() : typeof (object);
            Type armGenericType = result is IQueryable ? typeof (ActionResultModelQ<>) : typeof (ActionResultModel<>);
            Type armType = armGenericType.MakeGenericType(genericType);
            var arm = (ActionResultModel) Activator.CreateInstance(armType, action, result);
            INakedObject noArm = framework.NakedObjectManager.CreateAdapter(arm, null, null);
            noArm.SetATransientOid(new CollectionMemento(framework.LifecycleManager, framework.NakedObjectManager, framework.MetamodelManager,  (CollectionMemento)nakedObject.Oid, new object[] { }));
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