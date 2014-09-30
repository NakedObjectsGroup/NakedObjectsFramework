// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets.Objects.TypicalLength;
using NakedObjects.Architecture.Facets.Propparam.MultiLine;
using NakedObjects.Architecture.Facets.Propparam.Validate.MaxLength;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.Spec {
    public class NakedObjectActionParameterParseable : NakedObjectActionParameterAbstract, IParseableEntryActionParameter {
        public NakedObjectActionParameterParseable(int index, INakedObjectAction action, INakedObjectActionParamPeer peer)
            : base(index, action, peer) {}

        #region IParseableEntryActionParameter Members

        public virtual int NoLines {
            get { return GetFacet<IMultiLineFacet>().NumberOfLines; }
        }

        public virtual int MaximumLength {
            get { return GetFacet<IMaxLengthFacet>().Value; }
        }

        public virtual int TypicalLineLength {
            get { return GetFacet<ITypicalLengthFacet>().Value; }
        }

        #endregion
    }
}