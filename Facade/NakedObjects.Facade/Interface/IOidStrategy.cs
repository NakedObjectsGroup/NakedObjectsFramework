﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Facade.Translation;

namespace NakedObjects.Facade {
    public interface IOidStrategy {
        // todo make public get while refactoring
        IFrameworkFacade FrameworkFacade { get; set; }
        IOidTranslator OidTranslator { get; }
        object GetDomainObjectByOid(IOidTranslation objectId);
        IObjectFacade GetObjectFacadeByOid(IOidTranslation objectId);
        object GetServiceByServiceName(IOidTranslation serviceName);
        ITypeFacade GetSpecificationByLinkDomainType(string linkDomainType);
        string GetLinkDomainTypeBySpecification(ITypeFacade spec);
        IOidFacade RestoreOid(OidTranslationSemiColonSeparatedList id);
        IOidFacade RestoreSid(OidTranslationSemiColonSeparatedList id);
        IOidFacade RestoreOid(OidTranslationSlashSeparatedTypeAndIds id);
        IOidFacade RestoreSid(OidTranslationSlashSeparatedTypeAndIds id);
    }
}