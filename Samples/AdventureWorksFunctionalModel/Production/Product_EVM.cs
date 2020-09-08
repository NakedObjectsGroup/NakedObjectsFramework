// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFunctions;

namespace AdventureWorksModel
{
    [ViewModelEdit]
    public record ProductEVM
    {
        [NakedObjectsIgnore]
        public virtual int ProductID { get; init; }

        [MemberOrder(1)]
        public virtual string Name { get; init; }

        [MemberOrder(2)]
        public virtual string ProductNumber { get; init; }

        [MemberOrder(3)]
        public virtual string Color { get; init; }

        [MemberOrder(20)]
        public virtual bool Make { get; init; }

        [MemberOrder(21)]
        public virtual bool FinishedGoods { get; init; }

        [MemberOrder(22)]
        public virtual short SafetyStockLevel { get; init; }

        [MemberOrder(23)]
        public virtual short ReorderPoint { get; init; }

        [MemberOrder(90), Mask("C")]
        public virtual decimal StandardCost { get; init; }

        [MemberOrder(11), Mask("C")]
        public virtual decimal ListPrice { get; init; }

        public virtual string Size { get; init; }

        public virtual string SizeUnitMeasureCode { get; init; }

        public virtual UnitMeasure SizeUnit { get; init; }

        public virtual string WeightUnitMeasureCode { get; init; }

        public virtual decimal? Weight { get; init; }

        public virtual UnitMeasure WeightUnit { get; init; }

        [MemberOrder(24)] //TODO Range(1, 90)]
        public virtual int DaysToManufacture { get; init; }

        [MemberOrder(14)]
        public virtual string ProductLine { get; init; }

        [MemberOrder(19)]
        public virtual string Class { get; init; }

        [MemberOrder(18)]
        public virtual string Style { get; init; }

        [MemberOrder(81), Mask("d")]
        public virtual DateTime SellStartDate { get; init; }

        [MemberOrder(82), Mask("d")]
        public virtual DateTime? SellEndDate { get; init; }

        [MemberOrder(83), Mask("d")] //[Range(0, 10)] TODO
        public virtual DateTime? DiscontinuedDate { get; init; }

        [MemberOrder(10)]
        public virtual ProductModel ProductModel { get; init; }

        [MemberOrder(11)]
        public virtual ProductCategory ProductCategory { get; init; }

        [MemberOrder(12)]
        public virtual ProductSubcategory ProductSubcategory { get; init; }

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; init; }

        public override string ToString() => ProductID == 0 ? "New Product" : @"Editing {Name}";

    }
}