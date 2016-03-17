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
    export const noPatternMatch = "no pattern match";

    export const outOfRange = (val: any, min: any, max: any, filter: ILocalFilter) => `Value is outside the range ${filter.filter(min) || "unlimited"} to ${filter.filter(max) || "unlimited"}`;
}