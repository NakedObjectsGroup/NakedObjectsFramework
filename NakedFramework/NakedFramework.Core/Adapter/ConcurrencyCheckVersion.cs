﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Core.Util;

namespace NakedFramework.Core.Adapter;

public sealed class ConcurrencyCheckVersion : IVersion, IEncodedToStrings {
    private const string Wildcard = "*";
    private readonly DateTime time;
    private readonly object version;

    public ConcurrencyCheckVersion(string user, DateTime time, object version) {
        User = user;
        this.time = time;
        this.version = version;
    }

    public ConcurrencyCheckVersion(IMetamodelManager metamodel, ILoggerFactory loggerFactory, string[] strings) {
        var helper = new StringDecoderHelper(metamodel, loggerFactory, loggerFactory.CreateLogger<StringDecoderHelper>(), strings);
        User = helper.GetNextString();
        time = new DateTime(helper.GetNextLong());
        version = helper.GetNextObject();
    }

    public override bool Equals(object obj) => Equals(obj as ConcurrencyCheckVersion);

    public override int GetHashCode() => version.GetHashCode();

    public override string ToString() => $"Version: {version} (last read at : {Time} by : {User})";

    #region IEncodedToStrings Members

    public string[] ToEncodedStrings() {
        var helper = new StringEncoderHelper();

        helper.Add(User);
        helper.Add(time.Ticks);
        helper.Add(version);

        return helper.ToArray();
    }

    public string[] ToShortEncodedStrings() => ToEncodedStrings();

    #endregion

    #region IVersion Members

    public string User { get; }

    public DateTime? Time => time;

    public string Digest => version is not null ? IdentifierUtils.ComputeSHA256HashAsString(version.ToString()) : null;

    public bool IsDifferent(IVersion otherVersion) => !Equals(otherVersion);

    public bool IsDifferent(string digest) => digest != Wildcard && Digest != digest;

    public string AsSequence() => version.ToString();

    public bool Equals(IVersion other) => other is ConcurrencyCheckVersion entityVersion && version.Equals(entityVersion.version);

    #endregion
}