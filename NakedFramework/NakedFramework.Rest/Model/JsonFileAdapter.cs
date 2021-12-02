// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.IO;
using NakedFramework.Facade.Interface;

namespace NakedFramework.Rest.Model;

public class JsonFileAdapter : IAttachmentFacade {
    private readonly string data;

    public JsonFileAdapter(string data, string type, string name) {
        this.data = data;
        ContentType = type;
        FileName = name;
    }

    #region IAttachmentFacade Members

    public Stream InputStream {
        get {
            // This assumes the data url is base64 encoded  
            var body = data[(data.IndexOf(",", StringComparison.InvariantCulture) + 1)..];
            var bytes = Convert.FromBase64String(body);
            return new MemoryStream(bytes);
        }
    }

    public string ContentType { get; }

    public string FileName { get; }

    #endregion
}