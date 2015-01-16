// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;

namespace NakedObjects.Surface {
    public interface INakedObjectAssociationSurface : INakedObjectMemberSurface {
        INakedObjectSpecificationSurface Specification { get; }
        INakedObjectSpecificationSurface ElementSpecification { get; }
        bool IsChoicesEnabled { get; }
        bool IsAutoCompleteEnabled { get; }
        IConsentSurface IsUsable(INakedObjectSurface target);
        INakedObjectSurface GetNakedObject(INakedObjectSurface target);
        bool IsVisible(INakedObjectSurface nakedObject);
        bool IsEager(INakedObjectSurface nakedObject);
        INakedObjectSurface[] GetChoices(INakedObjectSurface target, IDictionary<string, INakedObjectSurface> parameterNameValues);

        Tuple<string, INakedObjectSpecificationSurface>[] GetChoicesParameters();

        Tuple<INakedObjectSurface, string>[] GetChoicesAndTitles(INakedObjectSurface target, IDictionary<string, INakedObjectSurface> parameterNameValues);

        INakedObjectSurface[] GetCompletions(INakedObjectSurface target, string autoCompleteParm);
        string GetTitle(INakedObjectSurface nakedObject);
        int Count(INakedObjectSurface target);
    }
}