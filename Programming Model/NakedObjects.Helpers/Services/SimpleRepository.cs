// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

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
            var result = Container.FindByKey<T>(key);
            if (result == null) {
                WarnUser(ProgrammingModel.NoMatchSingular);
            }
            return result;
        }

        #endregion
    }
}