// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NUnit.Framework;
using NakedObjects;
using NakedObjects.Security;

namespace RestfulObjects.Test.Data {
    public class WithActionViewModel : WithAction, IViewModel {
        private MostSimple ms1;
        private WithDateTimeKey dt1;
        private MostSimpleViewModel vm1;

        [Key, Title, ConcurrencyCheck]
        public virtual int Id { get; set; }

        [NakedObjectsIgnore]
        public string[] DeriveKeys() {
            return new[] { Id.ToString() };
        }

        [NakedObjectsIgnore]
        public void PopulateUsingKeys(string[] keys) {
            Id = int.Parse(keys.First());

            this.ms1 = Container.Instances<MostSimple>().Single(x => x.Id == 1);
            this.dt1 = Container.Instances<WithDateTimeKey>().FirstOrDefault();
            this.vm1 = Container.NewViewModel<MostSimpleViewModel>();
            vm1.Id = 1;
            
        }

        public override MostSimpleViewModel AnActionReturnsViewModel() {
            return vm1;
        }


        [QueryOnly]
        public override WithDateTimeKey AnActionReturnsWithDateTimeKey() {
            return dt1;
        }

        [AuthorizeAction(Users = "ViewUser")]
        public override MostSimple AUserDisabledAction() {
            return ms1;
        }

        public override MostSimple AnActionReturnsNull() {
            return null;
        }

        public override MostSimpleViewModel AnActionReturnsNullViewModel() {
            return null;
        }


        public override MostSimple AnActionWithOptionalParm([Optionally, Named("Optional Parm"), DescribedAs("an optional parm"), System.ComponentModel.DataAnnotations.MaxLength(101), RegEx(Validation = @"[A-Z]")] string parm) {
            return ms1;
        }

        [QueryOnly]
        public override MostSimple AnActionWithOptionalParmQueryOnly([Optionally] string parm) {
            return ms1;
        }

        [QueryOnly]
        public override MostSimple AnActionAnnotatedQueryOnly() {
            return ms1;
        }

        [QueryOnly]
        public override  MostSimpleViewModel AnActionAnnotatedQueryOnlyReturnsViewModel() {
            return vm1;
        }

        [QueryOnly]
        public override MostSimple AnActionAnnotatedQueryOnlyReturnsNull() {
            return null;
        }

        [Idempotent]
        public override MostSimple AnActionAnnotatedIdempotent() {
            return ms1;
        }

        [Idempotent]
        public override MostSimpleViewModel AnActionAnnotatedIdempotentReturnsViewModel() {
            return vm1;
        }

        [Idempotent]
        public override MostSimple AnActionAnnotatedIdempotentReturnsNull() {
            return null;
        }

        [Hidden]
        public override MostSimple AHiddenAction() {
            return ms1;
        }

        [Disabled]
        public override MostSimple ADisabledAction() {
            return ms1;
        }

        [Disabled]
        public override IQueryable<MostSimple> ADisabledQueryAction() {
            return Container.Instances<MostSimple>().Where(x => x.Id == 1);
        }

        [Disabled]
        public override ICollection<MostSimple> ADisabledCollectionAction() {
            return Container.Instances<MostSimple>().Where(x => x.Id == 1).ToList();
        }

        public override int AnActionReturnsScalar() {
            return 999;
        }

        public override string AnActionReturnsScalarEmpty() {
            return "";
        }

        public override string AnActionReturnsScalarNull() {
            return null;
        }


        public override void AnActionReturnsVoid() { }

        public override IQueryable<MostSimple> AnActionReturnsQueryable() {
            return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2);
        }

        public override ICollection<MostSimple> AnActionReturnsCollection() {
            return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToList();
        }

        public override ICollection<MostSimple> AnActionReturnsCollectionEmpty() {
            return new List<MostSimple>();
        }

        public override ICollection<MostSimple> AnActionReturnsCollectionNull() {
            return null;
        }

        public override void AnActionWithDateTimeParm(DateTime parm) { }

        public override IQueryable<MostSimple> AnActionReturnsQueryableWithScalarParameters(int parm1, string parm2) {
            Assert.AreEqual(100, parm1);
            Assert.AreEqual("fred", parm2);
            return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2);
        }

        public override ICollection<MostSimple> AnActionReturnsCollectionWithScalarParameters(int parm1, string parm2) {
            Assert.AreEqual(100, parm1);
            Assert.AreEqual("fred", parm2);
            return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToList();
        }

        public override IQueryable<MostSimple> AnActionReturnsQueryableWithParameters(int parm1, MostSimple parm2) {
            Assert.AreEqual(101, parm1);
            Assert.AreEqual(Container.Instances<MostSimple>().First(), parm2);
            return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2);
        }

        public override ICollection<MostSimple> AnActionReturnsCollectionWithParameters(int parm1, MostSimple parm2) {
            Assert.AreEqual(101, parm1);
            Assert.AreEqual(Container.Instances<MostSimple>().First(), parm2);
            return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToList();
        }

        public override int AnActionReturnsScalarWithParameters(int parm1, MostSimple parm2) {
            Assert.AreEqual(101, parm1);
            Assert.AreEqual(Container.Instances<MostSimple>().First(), parm2);
            return 555;
        }

        [DescribedAs("an action for testing")]
        [MemberOrder(Sequence = "1")]
        public override void AnActionReturnsVoidWithParameters(int parm1, MostSimple parm2) {
            Assert.AreEqual(101, parm1);
            Assert.AreEqual(Container.Instances<MostSimple>().First(), parm2);
        }

        public override MostSimple AnActionReturnsObjectWithParameters(int parm1, MostSimple parm2) {
            Assert.AreEqual(101, parm1);
            Assert.AreEqual(Container.Instances<MostSimple>().First(), parm2);
            return Container.Instances<MostSimple>().First();
        }

        [QueryOnly]
        public override MostSimple AnActionReturnsObjectWithParametersAnnotatedQueryOnly(int parm1, MostSimple parm2) {
            Assert.AreEqual(101, parm1);
            Assert.AreEqual(Container.Instances<MostSimple>().First(), parm2);
            return Container.Instances<MostSimple>().First();
        }

        [QueryOnly]
        public override MostSimple AnActionReturnsObjectWithParameterAnnotatedQueryOnly(int parm1) {
            Assert.AreEqual(101, parm1);
            return Container.Instances<MostSimple>().First();
        }

        [Idempotent]
        public override MostSimple AnActionReturnsObjectWithParametersAnnotatedIdempotent(int parm1, MostSimple parm2) {
            Assert.AreEqual(101, parm1);
            Assert.AreEqual(Container.Instances<MostSimple>().First(), parm2);
            return Container.Instances<MostSimple>().First();
        }

        public override MostSimple AnActionWithValueParameter(int parm1) {
            return Container.Instances<MostSimple>().Single(x => x.Id == parm1);
        }

        public override MostSimple AnActionWithValueParameterWithChoices(int parm3) {
            return Container.Instances<MostSimple>().Single(x => x.Id == parm3);
        }

        public override MostSimple AnActionWithValueParameterWithDefault(int parm5) {
            return Container.Instances<MostSimple>().Single(x => x.Id == parm5);
        }

        public override IList<int> Choices0AnActionWithValueParameterWithChoices() {
            return new[] { 1, 2, 3 };
        }

        public override int Default0AnActionWithValueParameterWithDefault() {
            return 4;
        }

        public override MostSimple AnActionWithReferenceParameter(MostSimple parm2) {
            return Container.Instances<MostSimple>().Single(x => x.Id == parm2.Id);
        }

        public override MostSimple AnActionWithReferenceParameterWithChoices(MostSimple parm4) {
            return Container.Instances<MostSimple>().Single(x => x.Id == parm4.Id);
        }

        public override MostSimple AnActionWithReferenceParameterWithDefault(MostSimple parm6) {
            return Container.Instances<MostSimple>().Single(x => x.Id == parm6.Id);
        }

        public override IList<MostSimple> Choices0AnActionWithReferenceParameterWithChoices() {
            return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToList();
        }

        public override MostSimple Default0AnActionWithReferenceParameterWithDefault() {
            return Container.Instances<MostSimple>().First();
        }

        public override MostSimple AnActionWithParametersWithChoicesWithDefaults(int parm1, int parm7, MostSimple parm2, MostSimple parm8) {
            return Container.Instances<MostSimple>().First();
        }

        public override IList<int> Choices1AnActionWithParametersWithChoicesWithDefaults() {
            return new[] { 1, 2, 3 };
        }

        public override int Default1AnActionWithParametersWithChoicesWithDefaults() {
            return 4;
        }

        public override IList<MostSimple> Choices3AnActionWithParametersWithChoicesWithDefaults() {
            return Container.Instances<MostSimple>().Where(ms => ms.Id == 1 || ms.Id == 2).ToList();
        }

        public override MostSimple Default3AnActionWithParametersWithChoicesWithDefaults() {
            return Container.Instances<MostSimple>().First();
        }

        public override int AnError() {
            throw new DomainException("An error exception");
        }

        public override IQueryable<MostSimple> AnErrorQuery() {
            throw new DomainException("An error exception");
        }

        public override ICollection<MostSimple> AnErrorCollection() {
            throw new DomainException("An error exception");
        }

    }
}