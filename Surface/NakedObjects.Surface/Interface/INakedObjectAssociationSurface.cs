// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;

namespace NakedObjects.Surface {
    public interface INakedObjectAssociationSurface : INakedObjectMemberSurface,ISurfaceHolder {
        INakedObjectSpecificationSurface Specification { get; }
        IConsentSurface IsUsable(INakedObjectSurface target);
        INakedObjectSurface GetNakedObject(INakedObjectSurface target);
        bool IsVisible(INakedObjectSurface nakedObject);
        bool IsEager(INakedObjectSurface nakedObject);
        bool IsChoicesEnabled { get; }
        bool IsAutoCompleteEnabled { get; }
        INakedObjectSurface[] GetChoices(INakedObjectSurface target, IDictionary<string, INakedObjectSurface> parameterNameValues);

        Tuple<string, INakedObjectSpecificationSurface>[] GetChoicesParameters();

        Tuple<INakedObjectSurface, string>[] GetChoicesAndTitles(INakedObjectSurface target, IDictionary<string, INakedObjectSurface> parameterNameValues);

        INakedObjectSurface[] GetCompletions(INakedObjectSurface target, string autoCompleteParm);
        string GetTitle(INakedObjectSurface nakedObject);
        int Count(INakedObjectSurface target);
    }
}