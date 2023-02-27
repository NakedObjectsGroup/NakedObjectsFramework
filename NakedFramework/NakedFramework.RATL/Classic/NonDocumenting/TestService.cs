using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NakedFramework.RATL.Classic.Interface;
using NakedFramework.RATL.Classic.TestCase;
using ROSI.Records;

namespace NakedFramework.RATL.Classic.NonDocumenting
{
    internal class TestService : TestHasActions, ITestService
    {
        public TestService(DomainObject service, AcceptanceTestCase acceptanceTestCase) : base(service, acceptanceTestCase) {
            //NakedObject = NakedObjectsContext.ObjectPersistor.GetAdapterFor(service);
        }

        #region ITestService Members

        public override string Title => throw new NotImplementedException(); //NakedObject.TitleString();

        #endregion
    }
    }

