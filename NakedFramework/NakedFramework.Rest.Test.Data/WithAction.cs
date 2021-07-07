// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using NakedFramework;
using NakedFramework.Error;
using NakedObjects;
using NakedObjects.Security;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace RestfulObjects.Test.Data {
    public abstract class WithAction {
        public IDomainObjectContainer Container { set; protected get; }

        [PresentationHint("class5 class6")]
        public virtual MostSimple AnAction() {
            return Container.Instances<MostSimple>().Single(x => x.Id == 1);
        }

        public virtual MostSimpleViewModel AnActionReturnsViewModel() {
            var vm = Container.NewViewModel<MostSimpleViewModel>();
            vm.Id = 1;
            return vm;
        }

        public virtual RedirectedObject AnActionReturnsRedirectedObject() {
            return Container.Instances<RedirectedObject>().Single(x => x.Id == 1);
        }

        [QueryOnly]
        public virtual WithDateTimeKey AnActionReturnsWithDateTimeKeyQueryOnly() => Container.Instances<WithDateTimeKey>().FirstOrDefault();

        [AuthorizeAction(Users = "ViewUser")]
        public virtual MostSimple AUserDisabledAction() {
            return Container.Instances<MostSimple>().Single(x => x.Id == 1);
        }

        [MultiLine(NumberOfLines = 1)]
        public virtual MostSimple AnActionReturnsNull() => null;

        public virtual MostSimpleViewModel AnActionReturnsNullViewModel() => null;

        public virtual MostSimple AnActionWithOptionalParm([Optionally] [Named("Optional Parm")] [DescribedAs("an optional parm")] [MaxLength(101)] [RegEx(Validation = @"[A-Z]")]
                                                           string parm) {
            return Container.Instances<MostSimple>().Single(x => x.Id == 1);
        }

        [QueryOnly]
        public virtual MostSimple AnActionWithOptionalParmQueryOnly([Optionally] string parm) {
            return Container.Instances<MostSimple>().Single(x => x.Id == 1);
        }

        public virtual MostSimple AnActionWithFindMenuParameter([FindMenu] MostSimple parm2) {
            return Container.Instances<MostSimple>().Single(x => x.Id == 1);
        }

        [QueryOnly]
        public virtual MostSimple AnActionAnnotatedQueryOnly() {
            return Container.Instances<MostSimple>().Single(x => x.Id == 1);
        }

        [QueryOnly]
        public virtual MostSimpleViewModel AnActionAnnotatedQueryOnlyReturnsViewModel() => AnActionReturnsViewModel();

        [QueryOnly]
        public virtual MostSimple AnActionAnnotatedQueryOnlyReturnsNull() => null;

        [Idempotent]
        public virtual MostSimple AnActionAnnotatedIdempotent() {
            return Container.Instances<MostSimple>().Single(x => x.Id == 1);
        }

        [Idempotent]
        public virtual MostSimpleViewModel AnActionAnnotatedIdempotentReturnsViewModel() => AnActionReturnsViewModel();

        [Idempotent]
        public virtual MostSimple AnActionAnnotatedIdempotentReturnsNull() => null;

        [Hidden(WhenTo.Always)]
        public virtual MostSimple AHiddenAction() {
            return Container.Instances<MostSimple>().Single(x => x.Id == 1);
        }

        [Disabled]
        public virtual MostSimple ADisabledAction() {
            return Container.Instances<MostSimple>().Single(x => x.Id == 1);
        }

        [Disabled]
        public virtual IQueryable<MostSimple> ADisabledQueryAction() {
            return Container.Instances<MostSimple>().Where(x => x.Id == 1);
        }

        [Disabled]
        public virtual ICollection<MostSimple> ADisabledCollectionAction() {
            return Container.Instances<MostSimple>().Where(x => x.Id == 1).ToList();
        }

        public virtual int AnActionReturnsScalar() => 999;

        public virtual string AnActionReturnsScalarEmpty() => "";

        public virtual string AnActionReturnsScalarNull() => null;

        public virtual void AnActionReturnsVoid() { }

        [PageSize(0)]
        [Eagerly(Do.Rendering)]
        [TableView(true, nameof(MostSimple.Id))]
        public virtual IQueryable<MostSimple> AnActionReturnsQueryable() {
            return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2);
        }

        public virtual ICollection<MostSimple> AnActionReturnsCollection() {
            return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToList();
        }

        public virtual ICollection<MostSimple> AnActionReturnsCollectionEmpty() => new List<MostSimple>();

        public virtual ICollection<MostSimple> AnActionReturnsCollectionNull() => null;

        public virtual void AnActionWithDateTimeParm([Mask("d")] DateTime parm) { }

        public DateTime Default0AnActionWithDateTimeParm() => new DateTime(2016, 2, 16);

        public virtual IQueryable<MostSimple> AnActionReturnsQueryableWithScalarParameters(int parm1, string parm2) {
            Assert.AreEqual(100, parm1);
            Assert.AreEqual("fred", parm2);
            return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2);
        }

        public virtual ICollection<MostSimple> AnActionReturnsCollectionWithScalarParameters([PresentationHint("class9 class10")] int parm1, string parm2) {
            Assert.AreEqual(100, parm1);
            Assert.AreEqual("fred", parm2);
            return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToList();
        }

        public virtual IQueryable<MostSimple> AnActionReturnsQueryableWithParameters(int parm1, MostSimple parm2) {
            Assert.AreEqual(101, parm1);
            Assert.AreEqual(Container.Instances<MostSimple>().First(), parm2);
            return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2);
        }

        public virtual ICollection<MostSimple> AnActionReturnsCollectionWithParameters(int parm1, MostSimple parm2) {
            Assert.AreEqual(101, parm1);
            Assert.AreEqual(Container.Instances<MostSimple>().First(), parm2);
            return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToList();
        }

        public virtual int AnActionReturnsScalarWithParameters(int parm1, MostSimple parm2) {
            Assert.AreEqual(101, parm1);
            Assert.AreEqual(Container.Instances<MostSimple>().First(), parm2);
            return 555;
        }

        [DescribedAs("an action for testing")]
        [MemberOrder(Sequence = "1")]
        public virtual void AnActionReturnsVoidWithParameters(int parm1, MostSimple parm2) {
            Assert.AreEqual(101, parm1);
            Assert.AreEqual(Container.Instances<MostSimple>().First(), parm2);
        }

        public virtual MostSimple AnActionReturnsObjectWithParameters(int parm1, MostSimple parm2) {
            Assert.AreEqual(101, parm1);
            Assert.AreEqual(Container.Instances<MostSimple>().First(), parm2);
            return Container.Instances<MostSimple>().First();
        }

        [QueryOnly]
        public virtual MostSimple AnActionReturnsObjectWithParametersAnnotatedQueryOnly(int parm1, MostSimple parm2) {
            Assert.AreEqual(101, parm1);
            Assert.AreEqual(Container.Instances<MostSimple>().First(), parm2);
            return Container.Instances<MostSimple>().First();
        }

        [QueryOnly]
        public virtual MostSimple AnActionReturnsObjectWithParameterAnnotatedQueryOnly(int parm1) {
            Assert.AreEqual(101, parm1);
            return Container.Instances<MostSimple>().First();
        }

        [Idempotent]
        public virtual MostSimple AnActionReturnsObjectWithParametersAnnotatedIdempotent(int parm1, MostSimple parm2) {
            Assert.AreEqual(101, parm1);
            Assert.AreEqual(Container.Instances<MostSimple>().First(), parm2);
            return Container.Instances<MostSimple>().First();
        }

        public virtual MostSimple AnActionWithValueParameter(int parm1) {
            return Container.Instances<MostSimple>().Single(x => x.Id == parm1);
        }

        public virtual MostSimple AnActionWithValueParameterWithRange([Range(1, 500)] int parm1) {
            return Container.Instances<MostSimple>().Single(x => x.Id == parm1);
        }

        public virtual MostSimple AnActionWithValueParameterWithChoices(int parm3) {
            return Container.Instances<MostSimple>().Single(x => x.Id == parm3);
        }

        public virtual MostSimple AnActionWithValueParameterWithDefault(int parm5) {
            return Container.Instances<MostSimple>().Single(x => x.Id == parm5);
        }

        public virtual IList<int> Choices0AnActionWithValueParameterWithChoices() {
            return new[] {1, 2, 3};
        }

        public virtual int Default0AnActionWithValueParameterWithDefault() => 4;

        public virtual MostSimple AnActionWithValueParametersWithConditionalChoices(int parm3, string parm4) {
            return Container.Instances<MostSimple>().Single(x => x.Id == parm3);
        }

        public virtual IList<int> Choices0AnActionWithValueParametersWithConditionalChoices(int parm3, string parm4) {
            return new[] {parm3, parm4 == null ? 0 : int.Parse(parm4)};
        }

        public virtual IList<string> Choices1AnActionWithValueParametersWithConditionalChoices(int parm3, string parm4) {
            return new[] {parm3.ToString(CultureInfo.InvariantCulture), parm4};
        }

        public virtual MostSimple AnActionWithDisabledReferenceParameter([Disabled] MostSimple parm2) {
            return Container.Instances<MostSimple>().Single(x => x.Id == parm2.Id);
        }

        public virtual MostSimple AnActionWithReferenceParameter(MostSimple parm2) {
            return Container.Instances<MostSimple>().Single(x => x.Id == parm2.Id);
        }

        public virtual MostSimple AnActionWithReferenceParameterWithChoices(MostSimple parm4) {
            return Container.Instances<MostSimple>().Single(x => x.Id == parm4.Id);
        }

        public virtual MostSimple AnActionWithReferenceParameterWithConditionalChoices(MostSimple parm4) {
            return Container.Instances<MostSimple>().Single(x => x.Id == parm4.Id);
        }

        public virtual MostSimple AnActionWithReferenceParameterWithDefault(MostSimple parm6) {
            return Container.Instances<MostSimple>().Single(x => x.Id == parm6.Id);
        }

        public virtual IList<MostSimple> Choices0AnActionWithReferenceParameterWithChoices() {
            return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToList();
        }

        public virtual IList<MostSimple> Choices0AnActionWithReferenceParameterWithConditionalChoices(MostSimple parm4) {
            return parm4 == null ? new List<MostSimple>() : Container.Instances<MostSimple>().Where(ms => ms.Id != parm4.Id).ToList();
        }

        public virtual MostSimple Default0AnActionWithReferenceParameterWithDefault() => Container.Instances<MostSimple>().First();

        public virtual MostSimple AnActionWithParametersWithChoicesWithDefaults(int parm1, int parm7, MostSimple parm2, MostSimple parm8) => Container.Instances<MostSimple>().First();

        public virtual IList<int> Choices1AnActionWithParametersWithChoicesWithDefaults() {
            return new[] {1, 2, 3};
        }

        public virtual int Default1AnActionWithParametersWithChoicesWithDefaults() => 4;

        public virtual IList<MostSimple> Choices3AnActionWithParametersWithChoicesWithDefaults() {
            return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToList();
        }

        public virtual MostSimple Default3AnActionWithParametersWithChoicesWithDefaults() => Container.Instances<MostSimple>().First();

        public virtual int AnError() => throw new DomainException("An error exception");

        public virtual IQueryable<MostSimple> AnErrorQuery() => throw new DomainException("An error exception");

        public virtual ICollection<MostSimple> AnErrorCollection() => throw new DomainException("An error exception");

        public virtual int AnActionValidateParameters(int parm1, int parm2) => parm1 + parm2;

        public virtual string ValidateAnActionValidateParameters(int parm1) {
            if (parm1 == 0) {
                return "Fail validation parm1";
            }

            return null;
        }

        public virtual string ValidateAnActionValidateParameters(int parm1, int parm2) {
            if (parm1 > parm2) {
                return "Cross validation failed";
            }

            return null;
        }

        public virtual MostSimple AnActionWithReferenceParametersWithAutoComplete(MostSimple parm0, MostSimple parm1) => parm0;

        public virtual IQueryable<MostSimple> AutoComplete0AnActionWithReferenceParametersWithAutoComplete([MinLength(3)] string name) {
            return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2);
        }

        public virtual IQueryable<MostSimple> AutoComplete1AnActionWithReferenceParametersWithAutoComplete([MinLength(3)] string name) {
            return Container.Instances<MostSimple>().Where(ms => ms.Id == 2);
        }

       
        public void AnActionWithCollectionParameter(IEnumerable<string> parm) { }

        public string[] Choices0AnActionWithCollectionParameter() {
            return new[] {
                "string1",
                "string2",
                "string3"
            };
        }

        public string[] Default0AnActionWithCollectionParameter() {
            return new[] {
                "string2",
                "string3"
            };
        }

        public void AnActionWithCollectionParameterRef(IEnumerable<MostSimple> parm) { }

        public MostSimple[] Choices0AnActionWithCollectionParameterRef() {
            return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToArray();
        }

        public MostSimple[] Default0AnActionWithCollectionParameterRef() {
            return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToArray();
        }

        [CreateNew]
        public WithValue AnActionWithCreateNewAnnotation(int aValue) {
            return new WithValue();
        }

        [FinderAction]
        public WithValue FinderAction1(int aValue) => new();

        [FinderAction("aprefix")]
        public WithValue FinderAction2(int aValue) => new();
    }
}