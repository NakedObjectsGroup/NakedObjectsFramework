// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects;

namespace RestfulObjects.Test.Data {
    public class RestDataFixtureUnitTests {
        #region Injected Services

        // This region should contain properties to hold references to any services required by the
        // object.  Use the 'injs' shortcut to add a new service.

        public IDomainObjectContainer Container { set; protected get; }

        #endregion

        public void Install() {
            var ms1 = Container.NewTransientInstance<MostSimple>();
            ms1.Id = 1;
            Container.Persist(ref ms1);

            var ms2 = Container.NewTransientInstance<MostSimple>();
            ms2.Id = 2;
            Container.Persist(ref ms2);

            var ms3 = Container.NewTransientInstance<MostSimple>();
            ms3.Id = 3;
            Container.Persist(ref ms3);

            var wr1 = Container.NewTransientInstance<WithReference>();
            wr1.Id = 1;
            wr1.AReference = ms1;
            wr1.ADisabledReference = ms1;
            wr1.AChoicesReference = ms1;
            wr1.AnEagerReference = ms1;
            wr1.AnAutoCompleteReference = ms1;
            Container.Persist(ref wr1);

            var wr2 = Container.NewTransientInstance<WithReference>();
            wr2.Id = 2;
            wr2.AReference = ms1;
            wr2.ADisabledReference = ms1;
            wr2.AChoicesReference = ms1;
            wr2.AnEagerReference = ms1;
            wr2.AnAutoCompleteReference = ms1;
            Container.Persist(ref wr2);

            var wv1 = Container.NewTransientInstance<WithValue>();
            wv1.Id = 1;
            wv1.AValue = 100;
            wv1.ADisabledValue = 200;
            wv1.AStringValue = "";
            Container.Persist(ref wv1);

            var ws1 = Container.NewTransientInstance<WithScalars>();
            ws1.Id = 1;
            ws1.Bool = true;
            ws1.Byte = 1;
            ws1.ByteArray = new[] {(Byte) 2};
            ws1.Char = '3';
            ws1.CharArray = new[] {(Char) 4};
            ws1.Decimal = 5.1M;
            ws1.Double = 6.2;
            ws1.Float = 7.3F;
            ws1.Int = 8;
            ws1.Long = 9L;
            ws1.SByte = 10;
            ws1.SByteArray = new[] {(SByte) 11};
            ws1.Short = 12;
            ws1.String = "13";
            ws1.UInt = 14;
            ws1.ULong = 15;
            ws1.UShort = 16;
            Container.Persist(ref ws1);

            var wa1 = Container.NewTransientInstance<WithActionObject>();
            wa1.Id = 1;
            Container.Persist(ref wa1);

            var wc1 = Container.NewTransientInstance<WithCollection>();
            wc1.Id = 1;
            Container.Persist(ref wc1);

            var we1 = Container.NewTransientInstance<WithError>();
            we1.Id = 1;
            Container.Persist(ref we1);

            var wge1 = Container.NewTransientInstance<WithGetError>();
            wge1.Id = 1;
            Container.Persist(ref wge1);

            var i1 = Container.NewTransientInstance<Immutable>();
            i1.Id = 1;
            Container.Persist(ref i1);

            var vs1 = Container.NewTransientInstance<VerySimple>();
            vs1.Id = 1;
            Container.Persist(ref vs1);

            var vse1 = Container.NewTransientInstance<VerySimpleEager>();
            vse1.Id = 1;
            Container.Persist(ref vse1);

            var dt1 = Container.NewTransientInstance<WithDateTimeKey>();
            dt1.Id = new DateTime(634835232000000000).Date;
            Container.Persist(ref dt1);

            var rdo1 = Container.NewTransientInstance<RedirectedObject>();
            rdo1.Id = 1;
            rdo1.ServerName = "RedirectedToServer";
            rdo1.Oid = "RedirectedToOid"; 

            Container.Persist(ref rdo1);

            var wat1 = Container.NewTransientInstance<WithAttachments>();
            wat1.Id = 1;
            Container.Persist(ref wat1);
        }
    }
}