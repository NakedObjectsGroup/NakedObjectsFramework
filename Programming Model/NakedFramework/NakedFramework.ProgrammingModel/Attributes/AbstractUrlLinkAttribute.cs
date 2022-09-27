﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedFramework;

[AttributeUsage(AttributeTargets.Property)]
public abstract class AbstractUrlLinkAttribute : Attribute {
    protected AbstractUrlLinkAttribute() { }

    protected AbstractUrlLinkAttribute(bool alwaysOpenInNewTab) => AlwaysOpenInNewTab = alwaysOpenInNewTab;

    protected AbstractUrlLinkAttribute(string displayAs) => DisplayAs = displayAs;

    protected AbstractUrlLinkAttribute(bool alwaysOpenInNewTab, string displayAs) : this(alwaysOpenInNewTab) => DisplayAs = displayAs;

    public bool AlwaysOpenInNewTab { get; }
    public string DisplayAs { get; }
}