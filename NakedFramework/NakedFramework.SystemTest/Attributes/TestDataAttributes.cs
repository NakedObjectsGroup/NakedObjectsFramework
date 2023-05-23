// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using NakedFramework;
using NakedObjects;
using NakedObjects.SystemTest;
using SystemTest.Attributes;

#pragma warning disable 612

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedVariable

namespace NakedFramework.SystemTest.Attributes {
    #region Classes used in test

    public class AttributesDatabaseInitializer : DropCreateDatabaseAlways<AttributesDbContext> {
        protected override void Seed(AttributesDbContext context) {
            context.Default1s.Add(new Default1 { Id = 1 });
            context.DescribedAs1s.Add(new Describedas1 { Id = 1 });
            context.DescribedAs2s.Add(new Describedas2 { Id = 1 });
            context.Description1s.Add(new Description1 { Id = 1 });
            context.Description2s.Add(new Description2 { Id = 1 });
            context.Disabled1s.Add(new Disabled1 { Id = 1 });
            context.Displayname1s.Add(new Displayname1 { Id = 1 });
            context.Hidden1s.Add(new Hidden1 { Id = 1 });
            //context.Iconname1s.Add(new Iconname1 { Id = 1 });
            //context.Iconname2s.Add(new Iconname2 { Id = 1 });
            //context.Iconname3s.Add(new Iconname3 { Id = 1 });
            //context.Iconname4s.Add(new Iconname4 { Id = 1 });
            context.Immutable1s.Add(new Immutable1 { Id = 1 });
            context.Immutable2s.Add(new Immutable2 { Id = 1 });
            context.Immutable3s.Add(new Immutable3 { Id = 1 });
            context.Mask1s.Add(new Mask1 { Id = 1, Prop1 = new DateTime(2009, 9, 23), Prop2 = new DateTime(2009, 9, 24) });
            context.Mask2s.Add(new Mask2 { Id = 1 });
            context.Maxlength1s.Add(new Maxlength1 { Id = 1 });
            context.Maxlength2s.Add(new Maxlength2 { Id = 1 });
            context.NakedObjectsIgnore1s.Add(new NakedObjectsIgnore1 { Id = 1 });
            //context.NakedObjectsIgnore2s.Add(new NakedObjectsIgnore2 { Id = 1 });
            //context.NakedObjectsIgnore3s.Add(new NakedObjectsIgnore3 { Id = 1 });
            //context.NakedObjectsIgnore4s.Add(new NakedObjectsIgnore4 { Id = 1 });
            //context.NakedObjectsIgnore5s.Add(new NakedObjectsIgnore5 { Id = 1 });
            context.Named1s.Add(new Named1 { Id = 1 });
            context.Range1s.Add(new Range1 { Id = 1, Prop25 = DateTime.Today, Prop26 = DateTime.Today });
            context.Regex1s.Add(new Regex1 { Id = 1 });
            context.Regex2s.Add(new Regex2 { Id = 1 });
            context.Memberorder1s.Add(new Memberorder1 { Id = 1 });
            context.Memberorder2s.Add(new Memberorder2 { Id = 1 });
            context.Stringlength1s.Add(new Stringlength1 { Id = 1 });
            var t1 = context.Title1s.Add(new Title1 { Id = 1, Prop1 = "Foo" });
            var t2 = context.Title2s.Add(new Title2 { Id = 1, Prop1 = "Baz" });
            context.Title3s.Add(new Title3 { Id = 1, Prop1 = "Qux" });
            var t4 = context.Title4s.Add(new Title4 { Id = 1, Prop1 = "Bar" });
            //context.Title5s.Add(new Title5 { Id = 1 });
            context.Title6s.Add(new Title6 { Id = 1, Prop1 = "Foo" });
            context.Title7s.Add(new Title7 { Id = 1, Prop1 = t4 });
            context.Title8s.Add(new Title8 { Id = 1, Prop1 = t1 });
            context.Title9s.Add(new Title9 { Id = 1, Prop1 = t2 });
            context.ValidateProgrammaticUpdates1s.Add(new Validateprogrammaticupdates1 { Id = 1 });
            context.ValidateProgrammaticUpdates2s.Add(new Validateprogrammaticupdates2 { Id = 1 });
            context.Contributees.Add(new Contributee { Id = 1 });
            context.Contributee2s.Add(new Contributee2 { Id = 1 });
            context.Contributee3s.Add(new Contributee3 { Id = 1 });
            //context.Exclude1s.Add(new FinderAction1 { Id = 1 });

            context.SaveChanges();
        }
    }

    public class AttributesDbContext : DbContext {
        public const string DatabaseName = "TestAttributes";

        private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;Encrypt=False;";
        public AttributesDbContext() : base(Cs) { }

        public DbSet<Default1> Default1s { get; set; }
        public DbSet<Describedas1> DescribedAs1s { get; set; }
        public DbSet<Describedas2> DescribedAs2s { get; set; }
        public DbSet<Description1> Description1s { get; set; }
        public DbSet<Description2> Description2s { get; set; }
        public DbSet<Disabled1> Disabled1s { get; set; }
        public DbSet<Displayname1> Displayname1s { get; set; }
        public DbSet<Hidden1> Hidden1s { get; set; }
        public DbSet<Iconname1> Iconname1s { get; set; }
        public DbSet<Iconname2> Iconname2s { get; set; }
        public DbSet<Iconname3> Iconname3s { get; set; }
        public DbSet<Iconname4> Iconname4s { get; set; }
        public DbSet<Immutable1> Immutable1s { get; set; }
        public DbSet<Immutable2> Immutable2s { get; set; }
        public DbSet<Immutable3> Immutable3s { get; set; }
        public DbSet<Mask1> Mask1s { get; set; }
        public DbSet<Mask2> Mask2s { get; set; }
        public DbSet<Maxlength1> Maxlength1s { get; set; }
        public DbSet<Maxlength2> Maxlength2s { get; set; }
        public DbSet<NakedObjectsIgnore1> NakedObjectsIgnore1s { get; set; }
        public DbSet<NakedObjectsIgnore2> NakedObjectsIgnore2s { get; set; }
        public DbSet<NakedObjectsIgnore3> NakedObjectsIgnore3s { get; set; }
        public DbSet<NakedObjectsIgnore4> NakedObjectsIgnore4s { get; set; }
        public DbSet<NakedObjectsIgnore5> NakedObjectsIgnore5s { get; set; }
        public DbSet<Named1> Named1s { get; set; }
        public DbSet<Range1> Range1s { get; set; }
        public DbSet<Regex1> Regex1s { get; set; }
        public DbSet<Regex2> Regex2s { get; set; }
        public DbSet<Memberorder1> Memberorder1s { get; set; }
        public DbSet<Memberorder2> Memberorder2s { get; set; }
        public DbSet<Stringlength1> Stringlength1s { get; set; }
        public DbSet<Title1> Title1s { get; set; }
        public DbSet<Title2> Title2s { get; set; }
        public DbSet<Title3> Title3s { get; set; }
        public DbSet<Title4> Title4s { get; set; }
        public DbSet<Title5> Title5s { get; set; }
        public DbSet<Title6> Title6s { get; set; }
        public DbSet<Title7> Title7s { get; set; }
        public DbSet<Title8> Title8s { get; set; }
        public DbSet<Title9> Title9s { get; set; }
        public DbSet<Validateprogrammaticupdates1> ValidateProgrammaticUpdates1s { get; set; }
        public DbSet<Validateprogrammaticupdates2> ValidateProgrammaticUpdates2s { get; set; }
        public DbSet<Contributee> Contributees { get; set; }
        public DbSet<Contributee2> Contributee2s { get; set; }
        public DbSet<Contributee3> Contributee3s { get; set; }
        public DbSet<FinderAction1> Exclude1s { get; set; }

        public static void Delete() => Database.Delete(Cs);

        protected override void OnModelCreating(DbModelBuilder modelBuilder) => Database.SetInitializer(new AttributesDatabaseInitializer());
    }

    #region Default

    public class Default1 {
        public virtual int Id { get; set; }

        [DefaultValue(8)]
        public virtual int Prop1 { get; set; }

        [DefaultValue("Foo")]
        public virtual string Prop2 { get; set; }

        public virtual void DoSomething([DefaultValue(8)] int param0,
                                        [DefaultValue("Foo")] string param1) { }
    }

    #endregion

    #region DescribedAs

    [DescribedAs("Foo")]
    public class Describedas1 {
        public virtual int Id { get; set; }

        [DescribedAs("Bar")]
        public virtual string Prop1 { get; set; }

        [DescribedAs("Hex")]
        public void DoSomething([DescribedAs("Yop")] string param1) { }
    }

    public class Describedas2 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }
        public void DoSomething(string param1) { }
    }

    #endregion

    #region Description

    [Description("Foo")]
    public class Description1 {
        public virtual int Id { get; set; }

        [Description("Bar")]
        public virtual string Prop1 { get; set; }

        [Description("Hex")]
        public void DoSomething([Description("Yop")] string param1) { }
    }

    public class Description2 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }
        public void DoSomething(string param1) { }
    }

    #endregion

    #region Disabled

    public class Disabled1 {
        public Disabled1() => Prop0 = Prop1 = Prop2 = Prop3 = Prop4 = Prop5 = Prop6 = "";

        public virtual int Id { get; set; }

        public virtual string Prop0 { get; set; }

        [Disabled]
        public virtual string Prop1 { get; set; }

        [Disabled(WhenTo.OncePersisted)]
        public virtual string Prop2 { get; set; }

        [Disabled(WhenTo.UntilPersisted)]
        public virtual string Prop3 { get; set; }

        [Disabled(WhenTo.Never)]
        public virtual string Prop4 { get; set; }

        [Disabled(WhenTo.Always)]
        public virtual string Prop5 { get; set; }

        public virtual string Prop6 { get; set; }

        public string DisableProp6() => Prop4 == "Disable 6" ? "Disabled Message" : null;
    }

    #endregion

    #region DisplayName

    [DisplayName("Foo")]
    public class Displayname1 {
        public virtual int Id { get; set; }

        [DisplayName("Bar")]
        public virtual string Prop1 { get; set; }

        [DisplayName("Hex")]
        public virtual void DoSomething(string param1) { }
    }

    #endregion

    #region Hidden

    public class Hidden1 {
        public Hidden1() =>
            // initialise all fields 
            Prop0 = Prop1 = Prop2 = Prop3 = Prop4 = Prop5 = string.Empty;

        public virtual int Id { get; set; }

        public virtual string Prop0 { get; set; }

        [Hidden(WhenTo.Always)]
        public virtual string Prop1 { get; set; }

        [Hidden(WhenTo.OncePersisted)]
        public virtual string Prop2 { get; set; }

        [Hidden(WhenTo.UntilPersisted)]
        public virtual string Prop3 { get; set; }

        [Hidden(WhenTo.Never)]
        public virtual string Prop4 { get; set; }

        [Hidden(WhenTo.Always)]
        public virtual string Prop5 { get; set; }
    }

    #endregion

    #region IconName

    public class Iconname1 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }
    }

    public class Iconname2 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }
    }

    public class Iconname3 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public string IconName() => "Bar";
    }

    public class Iconname4 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public string IconName() => "Bar";
    }

    #endregion

    #region Immutable

    public class Immutable1 {
        public Immutable1() =>
            // initialise all fields 
            Prop0 = Prop1 = Prop2 = Prop3 = Prop4 = Prop5 = Prop6 = string.Empty;

        public virtual int Id { get; set; }

        public virtual string Prop0 { get; set; }

        [Disabled]
        public virtual string Prop1 { get; set; }

        [Disabled(WhenTo.OncePersisted)]
        public virtual string Prop2 { get; set; }

        [Disabled(WhenTo.UntilPersisted)]
        public virtual string Prop3 { get; set; }

        [Disabled(WhenTo.Never)]
        public virtual string Prop4 { get; set; }

        [Disabled(WhenTo.Always)]
        public virtual string Prop5 { get; set; }

        public virtual string Prop6 { get; set; }

        public string DisableProp6() => Prop4 == "Disable 6" ? "Disabled Message" : null;
    }

    [Immutable]
    public class Immutable2 : Immutable1 {
        public void ChangeProp1() {
            Prop1 = "Foo";
        }
    }

    [Immutable(WhenTo.OncePersisted)]
    public class Immutable3 : Immutable1 { }

    #endregion

    #region Mask

    public class Mask1 {
        public virtual int Id { get; set; }

        public virtual DateTime Prop1 { get; set; }

        [Mask("d")]

        public virtual DateTime Prop2 { get; set; }

        public void DoSomething([Mask("d")] DateTime d1) { }
    }

    public class Mask2 {
        public virtual int Id { get; set; }

        [Mask("c")]
        public virtual decimal Prop1 { get; set; } = 32.70M;
    }

    #endregion

    #region MaxLength

    public class Maxlength1 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        [MaxLength(7)]
        public virtual string Prop2 { get; set; }

        public void Action([MaxLength(8)] string parm) { }
    }

    public class Maxlength2 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        [MaxLength(7)]
        public virtual string Prop2 { get; set; }

        public void Action([MaxLength(8)] string parm) { }
    }

    #endregion

    #region Named

    [Named("Foo")]
    public class Named1 {
        public virtual int Id { get; set; }

        [Named("Bar")]
        public virtual string Prop1 { get; set; }

        [Named("Hex")]
        public void DoSomething([Named("Yop")] string param1) { }
    }

    #endregion

    #region NakedObjectsIgnore

    public class NakedObjectsIgnore1 {
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        public virtual int ValueProp1 { get; set; }

        public virtual NakedObjectsIgnore1 RefProp { get; set; }

        [NakedObjectsIgnore]
        public virtual NakedObjectsIgnore1 RefPropIgnored { get; set; }

        public virtual NakedObjectsIgnore2 RefPropToAnIgnoredType { get; set; }

        public ICollection<NakedObjectsIgnore1> Coll { get; set; } = new List<NakedObjectsIgnore1>();

        [NakedObjectsIgnore]
        public ICollection<NakedObjectsIgnore1> CollIgnored { get; set; } = new List<NakedObjectsIgnore1>();

        public ICollection<NakedObjectsIgnore2> CollOfIgnoredType { get; set; } = new List<NakedObjectsIgnore2>();

        public void Action() { }

        [NakedObjectsIgnore]
        public void ActionIgnored() { }

        public NakedObjectsIgnore2 ActionReturningIgnoredType() => null;

        public void ActionWithIgnoredTypeParam(NakedObjectsIgnore2 param1) { }
    }

    //[NakedObjectsType(ReflectOver.None)]
    public class NakedObjectsIgnore2 {
        public virtual int Id { get; set; }

        public virtual int ValueProp1 { get; set; }
    }

    //[NakedObjectsType(ReflectOver.All)]
    public class NakedObjectsIgnore3 {
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        public virtual int ValueProp1 { get; set; }

        public virtual NakedObjectsIgnore1 RefProp { get; set; }

        [NakedObjectsIgnore]
        public virtual NakedObjectsIgnore1 RefPropIgnored { get; set; }

        public virtual NakedObjectsIgnore2 RefPropToAnIgnoredType { get; set; }

        public ICollection<NakedObjectsIgnore1> Coll { get; set; }

        [NakedObjectsIgnore]
        public ICollection<NakedObjectsIgnore1> CollIgnored { get; set; }

        public ICollection<NakedObjectsIgnore2> CollOfIgnoredType { get; set; }

        public void Action() { }

        [NakedObjectsIgnore]
        public void ActionIgnored() { }

        public NakedObjectsIgnore2 ActionReturningIgnoredType() => null;

        public void ActionWithIgnoredTypeParam(NakedObjectsIgnore2 param1) { }
    }

    //[NakedObjectsType(ReflectOver.TypeOnlyNoMembers)]
    public class NakedObjectsIgnore4 {
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        public virtual int ValueProp1 { get; set; }

        public virtual NakedObjectsIgnore1 RefProp { get; set; }

        [NakedObjectsIgnore]
        public virtual NakedObjectsIgnore1 RefPropIgnored { get; set; }

        public virtual NakedObjectsIgnore2 RefPropToAnIgnoredType { get; set; }

        public ICollection<NakedObjectsIgnore1> Coll { get; set; }

        [NakedObjectsIgnore]
        public ICollection<NakedObjectsIgnore1> CollIgnored { get; set; }

        public ICollection<NakedObjectsIgnore2> CollOfIgnoredType { get; set; }

        public void Action() { }

        [NakedObjectsIgnore]
        public void ActionIgnored() { }

        public NakedObjectsIgnore2 ActionReturningIgnoredType() => null;

        public void ActionWithIgnoredTypeParam(NakedObjectsIgnore2 param1) { }
    }

    //[NakedObjectsType(ReflectOver.ExplicitlyIncludedMembersOnly)]
    public class NakedObjectsIgnore5 {
        public virtual int Id { get; set; }

        public virtual NakedObjectsIgnore1 RefProp { get; set; }

        //[NakedObjectsInclude]
        public virtual NakedObjectsIgnore1 RefProp2 { get; set; }

        //[NakedObjectsInclude] //Should have no impact if scope is AllMembers
        public virtual NakedObjectsIgnore2 RefPropToAnIgnoredType { get; set; }

        public ICollection<NakedObjectsIgnore1> Coll { get; set; }

        //[NakedObjectsInclude]
        public ICollection<NakedObjectsIgnore1> Coll2 { get; set; }

        public ICollection<NakedObjectsIgnore2> CollOfIgnoredType { get; set; }

        //[NakedObjectsInclude]
        public void Action() { }

        public void Action2() { }

        //[NakedObjectsInclude] //Should still be ignored, because return type is ignored
        public NakedObjectsIgnore2 ActionReturningIgnoredType() => null;

        public void ActionWithIgnoredTypeParam(NakedObjectsIgnore2 param1) { }
    }

    public class NakedObjectsIgnore6 : NakedObjectsIgnore4 {
        public virtual string Prop3 { get; set; }

        public void Action2() { }

        public void Action3() { }
    }

    public class NakedObjectsIgnore7 : NakedObjectsIgnore5 {
        public virtual string Prop3 { get; set; }

        //[NakedObjectsInclude]
        public virtual string Prop4 { get; set; }

        //[NakedObjectsInclude]
        public void Action3() { }

        public void Action4() { }
    }

    #endregion

    #region Range

    public class Range1 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        [Range(-1, 10)]
        public virtual short Prop3 { get; set; }

        [Range(-1, 10)]
        public virtual int Prop4 { get; set; }

        [Range(-1, 10)]
        public virtual long Prop5 { get; set; }

        //[System.ComponentModel.DataAnnotations.Range(1, 10)]
        //public virtual byte Prop6 { get; set; }

        //[System.ComponentModel.DataAnnotations.Range(1, 10)]
        //public virtual ushort Prop7 { get; set; }

        //[System.ComponentModel.DataAnnotations.Range(1, 10)]
        //public virtual uint Prop8 { get; set; }

        //[System.ComponentModel.DataAnnotations.Range(1, 10)]
        //public virtual ulong Prop9 { get; set; }

        [Range(-1, 10)]
        public virtual float Prop10 { get; set; }

        [Range(-1, 10)]
        public virtual double Prop11 { get; set; }

        [Range(-1, 10)]
        public virtual decimal Prop12 { get; set; }

        [Range(-1d, 10d)]
        public virtual short Prop14 { get; set; }

        [Range(-1d, 10d)]
        public virtual int Prop15 { get; set; }

        [Range(-1d, 10d)]
        public virtual long Prop16 { get; set; }

        //[System.ComponentModel.DataAnnotations.Range(1d, 10d)]
        //public virtual byte Prop17 { get; set; }

        //[System.ComponentModel.DataAnnotations.Range(1d, 10d)]
        //public virtual ushort Prop18 { get; set; }

        //[System.ComponentModel.DataAnnotations.Range(1d, 10d)]
        //public virtual uint Prop19 { get; set; }

        //[System.ComponentModel.DataAnnotations.Range(1d, 10d)]
        //public virtual ulong Prop20 { get; set; }

        [Range(-1.9d, 10.9d)]
        public virtual float Prop21 { get; set; }

        [Range(-1.9d, 10.9d)]
        public virtual double Prop22 { get; set; }

        [Range(-1.9d, 10.9d)]
        public virtual decimal Prop23 { get; set; }

        //[System.ComponentModel.DataAnnotations.Range(typeof(string), "1", "10")]
        public virtual int Prop24 { get; set; }

        //[System.ComponentModel.DataAnnotations.Range(-30, 0)]
        public virtual DateTime Prop25 { get; set; } = DateTime.Today;

        //[System.ComponentModel.DataAnnotations.Range(0, 30)]
        public virtual DateTime Prop26 { get; set; } = DateTime.Today;

        public void Action1([Range(5, 6)] sbyte parm) { }

        public void Action2([Range(5, 6)] short parm) { }

        public void Action3([Range(5, 6)] int parm) { }

        public void Action4([Range(5, 6)] long parm) { }

        public void Action5([Range(5, 6)] byte parm) { }

        public void Action6([Range(5, 6)] ushort parm) { }

        public void Action7([Range(5, 6)] uint parm) { }

        public void Action8([Range(5, 6)] ulong parm) { }

        public void Action9([Range(5, 6)] float parm) { }

        public void Action10([Range(5, 6)] double parm) { }

        public void Action11([Range(5, 6)] decimal parm) { }

        public void Action12([Range(5d, 6d)] sbyte parm) { }

        public void Action13([Range(5d, 6d)] short parm) { }

        public void Action14([Range(5d, 6d)] int parm) { }

        public void Action15([Range(5d, 6d)] long parm) { }

        public void Action16([Range(5d, 6d)] byte parm) { }

        public void Action17([Range(5d, 6d)] ushort parm) { }

        public void Action18([Range(5d, 6d)] uint parm) { }

        public void Action19([Range(5d, 6d)] ulong parm) { }

        public void Action20([Range(5d, 6d)] float parm) { }

        public void Action21([Range(5d, 6d)] double parm) { }

        public void Action22([Range(5d, 6d)] decimal parm) { }

        public void Action23([Range(typeof(string), "5", "6")] int parm) { }

        public void Action24([Range(-30, 0)] DateTime parm) { }

        public void Action25([Range(1, 30)] DateTime parm) { }
    }

    #endregion

    #region Regex

    public class Regex1 {
        public virtual int Id { get; set; }

        [RegEx(Validation = @"^[\-\w\.]+@[\-\w\.]+\.[A-Za-z]+$")]
        public virtual string Email { get; set; }
    }

    public class Regex2 {
        public virtual int Id { get; set; }

        [RegularExpression(@"^[\-\w\.]+@[\-\w\.]+\.[A-Za-z]+$")]
        public virtual string Email { get; set; }
    }

    #endregion

    #region MemberOrder

    public class Memberorder1 {
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        [MemberOrder(3)]
        public virtual string Prop1 { get; set; }

        [MemberOrder(1)]
        public virtual string Prop2 { get; set; }

        [MemberOrder(3)]
        public virtual void Action1() { }

        [MemberOrder(1)]
        public virtual void Action2() { }
    }

    public class Memberorder2 : Memberorder1 {
        [NakedObjectsIgnore]
        public override int Id { get; set; }

        [MemberOrder(4)]
        public virtual string Prop3 { get; set; }

        [MemberOrder(2)]
        public virtual string Prop4 { get; set; }

        [MemberOrder(4)]
        public void Action3() { }

        [MemberOrder(2)]
        public void Action4() { }
    }

    #endregion

    #region StringLength

    public class Stringlength1 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        [StringLength(7)]
        public virtual string Prop2 { get; set; }

        public void Action([StringLength(8)] string parm) { }
    }

    #endregion

    #region Title

    public class Title1 {
        public virtual int Id { get; set; }

        [Title]
        [Optionally]
        public virtual string Prop1 { get; set; }
    }

    public class Title2 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public override string ToString() => Prop1;
    }

    public class Title3 {
        public virtual int Id { get; set; }

        [Title]
        public virtual string Prop1 { get; set; }

        public override string ToString() => "Bar";
    }

    public class Title4 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public string Title() => Prop1;
    }

    public class Title5 {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public string Title() => Prop1;

        public override string ToString() => "Bar";
    }

    public class Title6 {
        public virtual int Id { get; set; }

        [Title]
        public virtual string Prop1 { get; set; }

        public string Title() => "Hex";

        public override string ToString() => "Bar";
    }

    public class Title7 {
        public virtual int Id { get; set; }

        [Title]
        public virtual Title4 Prop1 { get; set; }
    }

    public class Title8 {
        public virtual int Id { get; set; }

        [Title]
        public virtual Title1 Prop1 { get; set; }
    }

    public class Title9 {
        public virtual int Id { get; set; }

        [Title]
        public virtual Title2 Prop1 { get; set; }
    }

    #endregion

    #region ValidateProgrammaticUpdates

    [ValidateProgrammaticUpdates]
    public class Validateprogrammaticupdates1 {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        [Optionally]
        public virtual string Prop2 { get; set; }

        public string ValidateProp1(string prop1) => prop1 == "fail" ? "fail" : null;
    }

    [ValidateProgrammaticUpdates]
    public class Validateprogrammaticupdates2 {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        [Optionally]
        public virtual string Prop2 { get; set; }

        public string Validate(string prop1, string prop2) => prop1 == "fail" ? "fail" : null;
    }

    public class TestServiceValidateProgrammaticUpdates {
        public IDomainObjectContainer Container { set; protected get; }

        public Validateprogrammaticupdates1 GetObject1() => Container.NewTransientInstance<Validateprogrammaticupdates1>();

        public Validateprogrammaticupdates2 GetObject2() => Container.NewTransientInstance<Validateprogrammaticupdates2>();

        public void SaveObject1(Validateprogrammaticupdates1 obj, string newVal) {
            obj.Prop1 = newVal;
        }

        public void SaveObject2(Validateprogrammaticupdates2 obj, string newVal) {
            obj.Prop1 = newVal;
        }
    }

    #endregion

    #endregion
}

// Change the namespace of these test classes as if they start with 'NakedObjects' we will not introspect them

namespace SystemTest.Attributes {
    public class TestServiceContributedAction {
        public IDomainObjectContainer Container { set; protected get; }

        public void ContributedAction([ContributedAction("Test Service Contributed Action")] Contributee obj) { }

        public void NotContributedAction(Contributee obj) { }

        public IQueryable<Contributee2> AllContributee2() => Container.Instances<Contributee2>();

        public void CollectionContributedAction([ContributedAction] IQueryable<Contributee2> targets) { }

        public void CollectionContributedAction1([ContributedAction] IQueryable<Contributee2> targets, string parm2) { }

        public void CollectionContributedAction2([ContributedAction] IQueryable<Contributee2> targets, Contributee cont) { }

        public IQueryable<Contributee2> NotCollectionContributedAction1([ContributedAction] IQueryable<Contributee2> targets) => throw new NotImplementedException();

        public ICollection<Contributee2> NotCollectionContributedAction2([ContributedAction] IQueryable<Contributee2> targets) => throw new NotImplementedException();

        public void NotCollectionContributedAction3([ContributedAction] IEnumerable<Contributee2> targets) { }
    }

    public class Contributee {
        public virtual int Id { get; set; }
    }

    public class Contributee2 {
        public virtual int Id { get; set; }

        public void NativeAction() { }
    }

    public class Contributee3 : Contributee2 {
        public void NativeAction3() { }
    }

    public class TestServiceFinderAction {
        [FinderAction]
        public FinderAction1 FinderAction1() => null;

        [FinderAction]
        public ICollection<FinderAction1> FinderAction2(string s, int i) => null;

        [FinderAction]
        public FinderAction1 FinderAction3(string s, FinderAction1 obj) => null;

        public IList<FinderAction1> Choices1FinderAction3() => new List<FinderAction1>();

        //No annotation
        public FinderAction1 NotFinderAction1() => null;

        //No annotation
        public ICollection<FinderAction1> NotFinderAction2() => null;

        [FinderAction] //Non-parseable param without choices
        public FinderAction1 NotFinderAction3(string s, FinderAction1 obj) => null;

        [FinderAction] //Returns string
        public string NotFinderAction4() => null;

        internal FinderAction1 NewObject1() => new();
    }

    public class FinderAction1 {
        public virtual int Id { get; set; }
    }
}