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
    // user message constants
    NakedObjects.noResultMessage = "no result found";
    NakedObjects.obscuredText = "*****";
    NakedObjects.tooShort = "Too short";
    NakedObjects.tooLong = "Too long";
    NakedObjects.notAnInteger = "Not an integer";
    NakedObjects.notANumber = "Not a number";
    NakedObjects.mandatory = "Mandatory";
    NakedObjects.noPatternMatch = "Invalid entry";
    NakedObjects.closeActions = "Close actions";
    NakedObjects.noActions = "No actions available";
    NakedObjects.openActions = "Open actions";
    NakedObjects.mandatoryFieldsPrefix = "Missing mandatory fields: ";
    NakedObjects.invalidFieldsPrefix = "Invalid fields: ";
    NakedObjects.unknownFileTitle = "UnknownFile";
    NakedObjects.unknownCollectionSize = "Unknown Size";
    NakedObjects.emptyCollectionSize = "Empty";
    NakedObjects.outOfRange = function (val, min, max, filter) { return ("Value is outside the range " + (filter.filter(min) || "unlimited") + " to " + (filter.filter(max) || "unlimited")); };
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.userMessages.config.js.map