// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading;
using Common.Logging;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Util;
using NakedObjects.Objects;
using NakedObjects.Resources;

namespace NakedObjects.Reflector.I18n.Resourcebundle {
    public class ResourceBasedI18nManager : II18nManager {
        private const string Action = "action";
        private const string Description = "description";
        private const string Help = "help";
        private const string Name = "name";
        private const string Parameter = "parameter";
        private const string Property = "property";
        private static readonly ILog Log;

        //private ResXResourceWriter resourceWriter;

        private readonly IDictionary<string, string> keyCache = new Dictionary<string, string>();
        private readonly IMessageBroker messageBroker;
        private readonly string resourceFile;
        private readonly IDictionary<string, string> resources;

        static ResourceBasedI18nManager() {
            Log = LogManager.GetLogger(typeof (ResourceBasedI18nManager));
        }

        public ResourceBasedI18nManager(string resourceFile, IMessageBroker messageBroker) {
            this.resourceFile = resourceFile;
            this.messageBroker = messageBroker;
            Log.Info(this);
            if (resourceFile != null && resources == null) {
                try {
                    Log.Info("Creating localization resource file: " + resourceFile);
                    if (File.Exists(resourceFile)) {
                        File.Delete(resourceFile);
                    }
                    resources = new Dictionary<string, string>();
                }
                catch (Exception e) {
                    Log.ErrorFormat("Failed to create resource file: {0} {1}", resourceFile, e.Message);
                }
            }
        }

        #region II18nManager Members

        public virtual string GetName(IIdentifier identifier, string original) {
            return GetText(identifier, Name, original);
        }

        public virtual string GetDescription(IIdentifier identifier, string original) {
            return GetText(identifier, Description, original);
        }

        public virtual string GetHelp(IIdentifier identifier) {
            return null; // GetText(identifier, HELP, "");
        }

        // TODO allow description and help to be found for parameters
        public string GetParameterName(IIdentifier identifier, int index, string original) {
            string key = identifier.ToIdentityString(IdentifierDepth.ClassNameParams) + Action + "/" + Parameter + (index + 1) + "/" + Name;
            return GetText(key, original);
        }

        public string GetParameterDescription(IIdentifier identifier, int index, string original) {
            string key = identifier.ToIdentityString(IdentifierDepth.ClassNameParams) + Action + "/" + Parameter + (index + 1) + "/" + Description;
            return GetText(key, original);
        }

        #endregion

        private string GetText(IIdentifier identifier, string type, string original) {
            string form = identifier.IsField ? Property : Action;
            string key = identifier.ToIdentityString(IdentifierDepth.ClassNameParams) + ":" + form + "/" + type;
            return GetText(key, original);
        }

        private void AddTranslatorRunningMessage() {
            if (resources != null) {
                string automatedTranslatorRunning = "Writing resourcefile: " + resourceFile;
                string[] existingMessages = messageBroker.PeekMessages;
                if (!existingMessages.Any(s => s == automatedTranslatorRunning)) {
                    messageBroker.AddMessage(automatedTranslatorRunning);
                }
            }
        }

        private string GetText(string key, string original) {
            AddTranslatorRunningMessage();

            if (key.StartsWith("System.") || key.StartsWith("NakedObjects.")) {
                return null;
            }

            string keyWithUnderscore = ReplaceAllPunctuation(key);

            WriteTranslation(keyWithUnderscore, original);

            try {
                return Model.ResourceManager.GetString(keyWithUnderscore);
            }
            catch (MissingManifestResourceException e) {
                Log.WarnFormat("Missing manifest resource (culture {0}) for the key {1} ({2}) ->  {3}", Thread.CurrentThread.CurrentUICulture, key, keyWithUnderscore, e.Message);
                return null;
            }
        }

        private string ReplaceAllPunctuation(string key) {
            if (keyCache.ContainsKey(key)) {
                return keyCache[key];
            }

            string newKey = key.Replace('.', '_').Replace(':', '_').Replace('#', '_').Replace('/', '_').Replace('(', '_').Replace(')', '_').Replace(',', '_').Replace('?', '_');

            keyCache[key] = newKey;

            return newKey;
        }

        private void WriteTranslation(string key, string original) {
            if (resources != null && original != null) {
                if (!resources.ContainsKey(key)) {
                    resources[key] = original;
                }
            }
        }

        ~ResourceBasedI18nManager() {
            try {
                if (resources != null) {
                    using (var resourceWriter = new ResXResourceWriter(resourceFile)) {
                        resources.ForEach(kvp => resourceWriter.AddResource(kvp.Key, kvp.Value));
                    }
                }
            }
            catch (Exception) {
                //do nothing
            }
        }
    }
}

// Copyright (c) Naked Objects Group Ltd.