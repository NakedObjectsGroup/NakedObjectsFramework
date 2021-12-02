// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.IO;

namespace NakedFramework.Facade.Contexts;

public class AttachmentContextFacade {
    public const string DefaultMimeType = "application/octet-stream";
    public const string DefaultContentDisposition = "attachment";
    public const string DefaultFileName = "unknown.txt";
    private Stream content;
    private string fileName;

    public Stream Content {
        get => content ?? new MemoryStream();
        set => content = value;
    }

    public string MimeType { get; set; }

    public string ContentDisposition { get; set; }

    public string FileName {
        get => string.IsNullOrWhiteSpace(fileName) ? DefaultFileName : fileName;
        set => fileName = value;
    }
}