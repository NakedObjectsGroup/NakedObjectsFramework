// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Surface {
    public interface INakedObjectActionSurface : INakedObjectMemberSurface,  ISurfaceHolder {
        INakedObjectSpecificationSurface ReturnType { get; }
        int ParameterCount { get; }
        INakedObjectActionParameterSurface[] Parameters { get; }
        INakedObjectSpecificationSurface OnType { get; }
        bool IsVisible(INakedObjectSurface nakedObject);
        IConsentSurface IsUsable(INakedObjectSurface nakedObject);
    }
}