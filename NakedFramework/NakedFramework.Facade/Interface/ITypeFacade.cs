// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedFramework.Facade.Interface {
    public interface ITypeFacade : IFacadeHolder, IEquatable<ITypeFacade> {
        IAssociationFacade[] Properties { get; }
        IMenuFacade Menu { get; }
        string PresentationHint { get; }
        bool IsAlwaysImmutable { get; }
        bool IsImmutableOncePersisted { get; }
        bool IsComplexType { get; }
        bool IsParseable { get; }
        bool IsStream { get; }
        bool IsQueryable { get; }
        bool IsService { get; }
        bool IsVoid { get; }
        bool IsStatic { get; }
        bool IsDateTime { get; }
        bool IsCollection { get; }
        bool IsObject { get; }
        string FullName { get; }
        string ShortName { get; }
        string SingularName { get; }
        string PluralName { get; }
        string Description { get; }
        bool IsASet { get; }
        bool IsAggregated { get; }
        bool IsImage { get; }
        bool IsFileAttachment { get; }
        bool IsFile { get; }
        bool IsBoolean { get; }
        bool IsEnum { get; }
        ITypeFacade GetElementType(IObjectFacade objectFacade);
        bool IsImmutable(IObjectFacade objectFacade);
        IActionFacade[] GetActionLeafNodes();
        bool IsOfType(ITypeFacade otherSpec);
        Type GetUnderlyingType();
        IActionFacade[] GetCollectionContributedActions();
        IActionFacade[] GetLocallyContributedActions(ITypeFacade typeFacade, string id);
    }
}