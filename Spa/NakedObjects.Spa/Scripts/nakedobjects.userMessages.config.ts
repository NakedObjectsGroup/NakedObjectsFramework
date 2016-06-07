// Copyright 2013-2014 Naked Objects Group Ltd
// Licensed under the Apache License, Version 2.0(the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.


module NakedObjects {

    // user message constants

    export const noResultMessage = "no result found";
    export const obscuredText = "*****";
    export const tooShort = "Too short";
    export const tooLong = "Too long";
    export const notAnInteger = "Not an integer";
    export const notANumber = "Not a number";
    export const mandatory = "Mandatory";
    export const noPatternMatch = "Invalid entry";
    export const closeActions = "Close actions";
    export const noActions = "No actions available";
    export const openActions = "Open actions (Alt-a)";
    export const mandatoryFieldsPrefix = "Missing mandatory fields: ";
    export const invalidFieldsPrefix = "Invalid fields: ";
    export const unknownFileTitle = "UnknownFile";
    export const unknownCollectionSize = "Unknown Size";
    export const emptyCollectionSize = "Empty";
    export const noItemsSelected = "Must select items for collection contributed action";
    export const dropPrompt = "(drop here)";
    export const autoCompletePrompt = "(auto-complete or drop)";
    export const concurrencyError = "Object has been updated by another user\n. Object has been reloaded.";
    export const loadingMessage = "Loading...";


    export const outOfRange = (val: any, min: any, max: any, filter: ILocalFilter) => `Value is outside the range ${filter.filter(min) || "unlimited"} to ${filter.filter(max) || "unlimited"}`;

    export const pageMessage = (p : number, tp : number, c : number, tc : number) => `Page ${p} of ${tp}; viewing ${c} of ${tc} items`;

    export const logOffMessage = (u: string) => `Plesae confirm logoff of user: ${u}`;
}
