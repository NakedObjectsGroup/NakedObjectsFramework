// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;

namespace RestfulObjects.Test.Data {
    public class CodeFirstContextLocal : DbContext {
        public CodeFirstContextLocal(string cs) : base(cs) { }
        public DbSet<Immutable> Immutables { get; set; }
        public DbSet<MostSimple> MostSimples { get; set; }
        public DbSet<RedirectedObject> RedirectedObjects { get; set; }
        public DbSet<VerySimple> VerySimples { get; set; }
        public DbSet<VerySimpleEager> VerySimpleEagers { get; set; }
        public DbSet<WithActionObject> WithActionObjects { get; set; }
        public DbSet<WithAttachments> WithAttachments { get; set; }
        public DbSet<WithCollection> WithCollections { get; set; }
        public DbSet<WithDateTimeKey> WithDateTimeKeys { get; set; }
        public DbSet<WithGuidKey> WithGuidKeys { get; set; }
        public DbSet<WithError> WithErrors { get; set; }
        public DbSet<WithGetError> WithGetErrors { get; set; }
        public DbSet<WithReference> WithReferences { get; set; }
        public DbSet<WithScalars> WithScalarses { get; set; }
        public DbSet<WithValue> WithValues { get; set; }

        public DbSet<MostSimplePersist> MostSimplePersists { get; set; }
        public DbSet<VerySimplePersist> VerySimplePersists { get; set; }
        public DbSet<WithValuePersist> WithValuePersists { get; set; }
        public DbSet<WithReferencePersist> WithReferencePersists { get; set; }
        public DbSet<WithCollectionPersist> WithCollectionPersists { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            Database.SetInitializer(new CodeFirstInitializer());
        }
    }

    public class CodeFirstInitializer : DropCreateDatabaseAlways<CodeFirstContextLocal> {
        private static void SeedCodeFirstDatabase(CodeFirstContextLocal context) {
            var ms1 = new MostSimple {Id = 1};
            var ms2 = new MostSimple {Id = 2};
            var ms3 = new MostSimple {Id = 4};
            context.MostSimples.Add(ms1);

            context.MostSimples.Add(ms2);

            context.MostSimples.Add(ms3);

            var wr1 = new WithReference {Id = 1, AReference = ms1, ADisabledReference = ms1, AChoicesReference = ms1, AnEagerReference = ms1, AnAutoCompleteReference = ms1};
            var wr2 = new WithReference {Id = 2, AReference = ms1, ADisabledReference = ms1, AChoicesReference = ms1, AnEagerReference = ms1, AnAutoCompleteReference = ms1};
            var wr3 = new WithReference { Id = 3, AReference = ms1, ADisabledReference = ms1, AChoicesReference = ms1, AnEagerReference = ms1, AnAutoCompleteReference = ms1 };

            context.WithReferences.Add(wr1);

            context.WithReferences.Add(wr2);

            context.WithReferences.Add(wr3);

            var wv1 = new WithValue {Id = 1, AValue = 100, ADisabledValue = 200, AStringValue = ""};
            context.WithValues.Add(wv1);

            var wv2 = new WithValue {Id = 2, AValue = 100, ADisabledValue = 200, AStringValue = ""};
            context.WithValues.Add(wv2);

            var ws1 = new WithScalars {
                Id = 1,
                Bool = true,
                Byte = 1,
                ByteArray = new[] {
                    (byte) 2
                },
                Char = '3',
                CharArray = new[] {
                    (char) 4
                },
                DateTime = new DateTime(2012, 03, 27, 08, 42, 36, 0, DateTimeKind.Utc).ToUniversalTime(),
                Decimal = 5.1M,
                Double = 6.2,
                Float = 7.3F,
                Int = 8,
                Long = 9L,
                SByte = 10,
                SByteArray = new[] {
                    (sbyte) 11
                },
                Short = 12,
                String = "13",
                UInt = 14,
                ULong = 15,
                UShort = 16
            };

            context.WithScalarses.Add(ws1);

            var wa1 = new WithActionObject {Id = 1};
            context.WithActionObjects.Add(wa1);

            var wc1 = new WithCollection {Id = 1};
            wc1.ACollection.Add(ms1);
            wc1.ACollection.Add(ms2);
            wc1.ACollection.Add(ms3);

            wc1.ASet.Add(ms1);

            wc1.ASet.Add(ms2);

            wc1.ADisabledCollection.Add(ms1);
            wc1.ADisabledCollection.Add(ms2);
            wc1.AHiddenCollection.Add(ms1);
            wc1.AHiddenCollection.Add(ms2);
            wc1.AnEagerCollection.Add(ms1);
            wc1.AnEagerCollection.Add(ms2);
            context.WithCollections.Add(wc1);

            var we1 = new WithError {Id = 1};
            context.WithErrors.Add(we1);

            var we2 = new WithError {Id = 2};
            context.WithErrors.Add(we2);

            var we3 = new WithError {Id = 3};
            context.WithErrors.Add(we3);

            var we4 = new WithError {Id = 4};
            context.WithErrors.Add(we4);

            var wge1 = new WithGetError {Id = 1};
            context.WithGetErrors.Add(wge1);

            var i1 = new Immutable {Id = 1};
            context.Immutables.Add(i1);

            var vs1 = new VerySimple {Id = 1};
            context.VerySimples.Add(vs1);

            var vs2 = new VerySimple {Id = 2};
            context.VerySimples.Add(vs2);

            var vse1 = new VerySimpleEager {Id = 1};
            context.VerySimpleEagers.Add(vse1);

            var dt1 = new WithDateTimeKey {Id = (new DateTime(634835232000000000L)).Date};
            context.WithDateTimeKeys.Add(dt1);

            var rdo1 = new RedirectedObject {Id = 1, ServerName = "RedirectedToServer", Oid = "RedirectedToOid"};
            context.RedirectedObjects.Add(rdo1);

            var wat1 = new WithAttachments {Id = 1};
            context.WithAttachments.Add(wat1);

            var g1 = new WithGuidKey {Id = new Guid("CA761232-ED42-11CE-BACD-00AA0057B223")};
            context.WithGuidKeys.Add(g1);

            context.SaveChanges();

            wc1.ACollection.Remove(ms3);

            context.SaveChanges();
        }

        protected override void Seed(CodeFirstContextLocal context) {
            base.Seed(context);
            SeedCodeFirstDatabase(context);
        }
    }
}