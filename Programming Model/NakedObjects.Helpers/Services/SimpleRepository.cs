// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Linq;
using System.Reflection;
using NakedObjects.Resources;
using NakedObjects.Util;

namespace NakedObjects.Services {
    /// <summary>
    ///     Simple repository of T - gives initial ability to create and retrieve instances of T
    /// </summary>
    public sealed class SimpleRepository<T> : AbstractFactoryAndRepository where T : class, new() {
        private readonly string title;

        /// <summary>
        ///     No arg constructor with default title
        /// </summary>
        public SimpleRepository()
            : this(NameUtils.PluralName(NameUtils.NaturalName(typeof (T).Name))) {}

        /// <summary>
        ///     Constructor for specifying title
        /// </summary>
        public SimpleRepository(string title) {
            this.title = title;
        }

        /// <summary>
        ///     Unique identifier for this service
        /// </summary>
        [Hidden]
        public override string Id {
            get { return "repository#" + typeof (T).FullName; }
        }

        /// <summary>
        ///     Icon name for service will be same as T.Name
        /// </summary>
        public string IconName() {
            return typeof (T).Name;
        }

        /// <summary>
        ///     Title of service
        /// </summary>
        public string Title() {
            return title;
        }

        /// <summary>
        ///     Title of service via ToString
        /// </summary>
        public override string ToString() {
            return Title();
        }

        #region Creators

        /// <summary>
        ///     Returns a new transient instance of T
        /// </summary>
        [MemberOrder(Sequence = "1")]
        public T NewInstance() {
            return NewTransientInstance<T>();
        }

        #endregion

        #region Finders

        /// <summary>
        ///     All Instances of T
        /// </summary>
        [MemberOrder(Sequence = "2")]
        public IQueryable<T> AllInstances() {
            return Container.Instances<T>();
        }

        [MemberOrder(Sequence = "3")]
        public T GetRandom() {
            return Random<T>();
        }

        [MemberOrder(Sequence = "4")]
        public T FindByKey(int key) {
            PropertyInfo keyProperty = Container.GetSingleKey(typeof (T));
            if (keyProperty.PropertyType != typeof (int)) {
                throw new DomainException(string.Format(ProgrammingModel.NoIntegerKey, typeof (T)));
            }
            T result = Container.FindByKey<T>(key);
            if (result == null) {
                WarnUser(ProgrammingModel.NoMatchSingular);
            }
            return result;
        }

        #endregion
    }
}