// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Testing.Dom;

namespace NakedObjects.Testing.Dom {
    /// <summary>
    /// A very simple INakedObject classes - contains only a single association
    /// </summary>
    public class Role {
        public string name;
        public Person person;

        public virtual Person Person {
            get { return person; }

            set { person = value; }
        }

        public virtual string Name {
            get { return name; }

            set { name = value; }
        }

        public virtual void modifyPerson(Person person) {
            Person = person;
        }

        public virtual void clearPerson(Person person) {
            Person = null;
        }
    }
}