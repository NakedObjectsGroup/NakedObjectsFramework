// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Linq;
using NakedObjects.Surface.Nof2.Utility;
using NakedObjects.Facade.Nof2;
using org.nakedobjects.@object;

namespace NakedObjects.Surface.Nof2.Context {
    public class NakedObjectActionParameter {
        private readonly ActionWrapper action;
        private readonly object[] choices;
        private readonly object dflt;
        private readonly string id;
        private readonly int number;
        private readonly NakedObjectSpecification spec;

        public NakedObjectActionParameter(string id, int number, NakedObjectSpecification spec, ActionWrapper action, object[] choices, object dflt) {
            this.id = id;
            this.number = number;
            this.spec = spec;
            this.action = action;
            this.choices = choices;
            this.dflt = dflt;
        }

        public string getId() {
            return id;
        }

        public NakedObjectSpecification getSpecification() {
            return spec;
        }

        public string getName() {
            return SurfaceUtils.MakeSpaced(SurfaceUtils.Capitalize(id));
        }

        public string getDescription() {
            return "";
        }

        public bool isMandatory() {
            return true;
        }

        public int getNumber() {
            return number;
        }

        public ActionWrapper getAction() {
            return action;
        }

        public bool isChoicesEnabled() {
            return choices != null && choices.Any();
        }

        public Naked getDefault(Naked nakedObject) {
            return dflt == null ? null : org.nakedobjects.@object.NakedObjects.getObjectLoader().getAdapterFor(dflt);
        }

        public Naked[] getChoices(Naked nakedObject) {
            return choices.Select(o => org.nakedobjects.@object.NakedObjects.getObjectLoader().getAdapterFor(o)).Cast<Naked>().ToArray();
        }
    }
}