// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects;

namespace Expenses.Services {
    // This is a dummy email sender please replace with your own email sending implementation 

    public class DummyMailSender : IEmailSender {
        public IDomainObjectContainer Container { protected get; set; }

        #region IEmailSender Members

        public void SendTextEmail(string toEmailAddress, string text) {
            throw new Exception("Email not sent: no email service set up");
        }

        #endregion
    }
}