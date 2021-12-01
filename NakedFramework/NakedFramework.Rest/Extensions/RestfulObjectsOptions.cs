// // Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// // Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// // You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// // Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// // WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// // See the License for the specific language governing permissions and limitations under the License.

namespace NakedFramework.Rest.Extensions; 

public class RestfulObjectsOptions {
    /// <summary>
    ///     The default value is true, meaning that when errors are encountered,
    ///     additional information may be passed through the RESTful API
    ///     so that it may be displayed on the client. For live deployment, it should be false
    ///     to avoid security risks.
    /// </summary>
    public bool DebugWarnings { get; set; } = true;

    /// <summary>
    ///     Set to true to limit availability of resources to those accessed via the HTTP Get method only
    ///     i.e. to 'read only' functionality.
    /// </summary>
    public bool IsReadOnly { get; set; } = false;

    /// <summary>
    ///     Permits cache settings  to be changed for: transactional, short-term, and long-term caches,
    ///     used for: domain objects, user credentials, and static application resources (e.g. menus).
    ///     Default values are 0 (no-caching) for each.
    /// </summary>
    public (int, int, int) CacheSettings { get; set; } = (0, 0, 0);

    /// <summary>
    ///     Default is true. Setting to false permits strict enforcement of Accept headers to be switched off
    ///     - which may be convenient during early stages of development only.
    /// </summary>
    public bool AcceptHeaderStrict { get; set; } = true;

    /// <summary>
    ///     Default is 20. If not specified by a PageSize attribite on the method this is the page size used.
    ///     Specifying 0 means 'unlimited'.
    /// </summary>
    public int DefaultPageSize { get; set; } = 20;

    /// <summary>
    ///     Defaults to true. Setting to false will decrease size of representations, but typically increase the number of Http
    ///     messages.
    /// </summary>
    public bool InlineDetailsInActionMemberRepresentations { get; set; } = true;

    /// <summary>
    ///     Defaults to true. Setting to false will decrease size of representations, but typically increase the number of Http
    ///     messages.
    /// </summary>
    public bool InlineDetailsInCollectionMemberRepresentations { get; set; } = true;

    /// <summary>
    ///     Defaults to true. Setting to false will decrease size of representations, but typically increase the number of Http
    ///     messages.
    /// </summary>
    public bool InlineDetailsInPropertyMemberRepresentations { get; set; } = true;

    /// <summary>
    ///     Defaults to true. Setting to false will decrease size of representations, but typically increase the number of Http
    ///     messages.
    /// </summary>
    public bool InlinedMemberRepresentations { get; set; } = true;

    /// <summary>
    ///     It is recommended that this flag remain set at the default (false).
    ///     It should only be set to true if necesssary for backwards-compatibility
    ///     with earlier versions of the framework.
    /// </summary>
    public bool AllowMutatingActionOnImmutableObject { get; set; } = false;

    /// <summary>
    ///     Defaults to true for Naked Objects, where 'proto-peristent objects' are referred to as 'transient objects'.
    /// </summary>
    public bool ProtoPersistentObjects { get; set; } = true;

    /// <summary>
    ///     Defaults to false - meaning that resources may not be deleted directly via the Http DEL method. It is still
    ///     possible to delete resources via purpose-written actions, however.
    /// </summary>
    public bool DeleteObjects { get; set; } = false;

    /// <summary>
    ///     Defaults to true. Permits the use of the x-ro-validate-only flag to validate parameters without attempting to
    ///     invoke the action.
    /// </summary>
    public bool ValidateOnly { get; set; } = true;

    /// <summary>
    ///     Defaults to false. Set this to true to allow blobs/clobs to be passed through the RESTful API.
    /// </summary>
    public bool BlobsClobs { get; set; } = false;
}