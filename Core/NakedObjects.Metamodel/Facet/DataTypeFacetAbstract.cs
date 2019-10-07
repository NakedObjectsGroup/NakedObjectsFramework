// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public abstract class DataTypeFacetAbstract : FacetAbstract, IDataTypeFacet {
        private readonly string customDataType;
        private readonly DataType dataType;

        private DataTypeFacetAbstract(DataType dataType, string customDataType, ISpecification holder)
            : base(Type, holder) {
            this.dataType = dataType;
            this.customDataType = customDataType;
        }

        protected DataTypeFacetAbstract(DataType dataType, ISpecification holder)
            : this(dataType, "", holder) { }

        protected DataTypeFacetAbstract(string customDataType, ISpecification holder)
            : this(System.ComponentModel.DataAnnotations.DataType.Custom, customDataType, holder) { }

        public static Type Type => typeof(IDataTypeFacet);

        #region IDataTypeFacet Members

        public DataType DataType() {
            return dataType;
        }

        public string CustomDataType() {
            return customDataType;
        }

        #endregion
    }
}