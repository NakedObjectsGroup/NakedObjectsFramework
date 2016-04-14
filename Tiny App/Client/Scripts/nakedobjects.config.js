// Copyright 2013-2014 Naked Objects Group Ltd
// Licensed under the Apache License, Version 2.0(the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
var NakedObjects;
(function (NakedObjects) {
    // custom configuration for a particular implementation 
    // path to Restful Objects server 
    var appPath = "http://localhost:59798/";
    function getAppPath() {
        if (appPath.charAt(appPath.length - 1) === "/") {
            return appPath.length > 1 ? appPath.substring(0, appPath.length - 1) : "";
        }
        return appPath;
    }
    NakedObjects.getAppPath = getAppPath;
    NakedObjects.defaultPageSize = 20; // can be overriden by server 
    NakedObjects.listCacheSize = 5;
    NakedObjects.shortCutMarker = "___";
    NakedObjects.urlShortCuts = ["http://nakedobjectsrodemo.azurewebsites.net", "AdventureWorksModel"];
    NakedObjects.keySeparator = "--";
    NakedObjects.objectColor = "object-color";
    NakedObjects.linkColor = "link-color";
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.config.js.map