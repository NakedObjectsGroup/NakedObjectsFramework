// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;

namespace NakedObjects.Surface {
    public interface INakedObjectActionParameterSurface : IScalarPropertyHolder, ISurfaceHolder {
        INakedObjectSpecificationSurface Specification { get; }
        INakedObjectActionSurface Action { get; }
        string Id { get; }
        bool IsChoicesEnabled { get; }
        bool IsAutoCompleteEnabled { get; }
        INakedObjectSurface[] GetChoices(INakedObjectSurface nakedObject, IDictionary<string, INakedObjectSurface> parameterNameValues);

        Tuple<INakedObjectSurface, string>[] GetChoicesAndTitles(INakedObjectSurface nakedObject, IDictionary<string, INakedObjectSurface> parameterNameValues);

        INakedObjectSurface[] GetCompletions(INakedObjectSurface nakedObject, string autoCompleteParm);
        bool DefaultTypeIsExplicit(INakedObjectSurface nakedObject);
        INakedObjectSurface GetDefault(INakedObjectSurface nakedObject);
        Tuple<string, INakedObjectSpecificationSurface>[] GetChoicesParameters();
    }
}