// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Menu;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Adapter;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.SemanticsProvider;
using NakedFramework.Metamodel.Serialization;
using NakedFramework.Value;

#pragma warning disable 618
#pragma warning disable SYSLIB0011

namespace NakedFramework.Metamodel.Component;

public sealed class ImmutableInMemorySpecCache : ISpecificationCache {
    private ImmutableList<IMenuImmutable> mainMenus = ImmutableList<IMenuImmutable>.Empty;
    private ImmutableDictionary<string, ITypeSpecImmutable> specs = ImmutableDictionary<string, ITypeSpecImmutable>.Empty;

    // constructor to use when reflecting
    public ImmutableInMemorySpecCache() { }

    // constructor to use when loading metadata from file
  

    public ImmutableInMemorySpecCache(string file) {
        if (file.EndsWith(".bin")) {
            BinaryDeserialize(file);
        }
        else {
            XmlDeserialize(file, Array.Empty<Type>());
        }
    }

    private Type[] createTypes() {

        var types = new List<Type>();

        var vtt = new[] { typeof(int),
            typeof(long),
            typeof(string),
            typeof(byte[]),
            typeof(byte),
            typeof(FileAttachment),
            typeof(Image),
            typeof(short),
            typeof(decimal),
            typeof(sbyte),
            typeof(uint),
            typeof(ulong),
            typeof(float),
            typeof(ushort),
            typeof(bool),
            typeof(DateTime),
            typeof(Guid),
            typeof(TimeSpan),
            typeof(char),
            typeof(Color),
            typeof(double),
        };

        var gtt = new[] { typeof(DefaultedFacetUsingDefaultsProvider<>), typeof(ParseableFacetUsingParser<>), typeof(TitleFacetUsingParser<>), typeof(ValueFacetFromSemanticProvider<>), typeof(ArrayValueSemanticsProvider<>) };

        foreach (var vt in vtt) {
            foreach (var gt in gtt) {
                var t = gt.MakeGenericType(vt);
                types.Add(t);
            }
        }

        return types.ToArray();
    }



    private Type[] KnownTypes() {
        var a = Assembly.GetAssembly(typeof(IdentifierImpl));
        var tt = a.GetTypes().Where(t => t.Namespace?.StartsWith("NakedFramework.Metamodel.Facet") == true ||
                                         t.Namespace?.StartsWith("NakedFramework.Metamodel.Adapter") == true ||
                                         t.Namespace?.StartsWith("NakedFramework.Metamodel.Spec") == true ||
                                         t.Namespace?.StartsWith("NakedFramework.Metamodel.SpecImmutable") == true ||
                                         t.Namespace?.StartsWith("NakedFramework.Metamodel.Menu") == true ||
                                         t.Namespace?.StartsWith("NakedFramework.Metamodel.Serialization") == true ||
                                         t.Namespace?.StartsWith("NakedFramework.Metamodel.SemanticsProvider") == true).ToArray();

       



        var createdTypes = createTypes();

        tt = tt.Union(createdTypes).ToArray();

        return tt.Where(t => t.IsPublic).ToArray();
    }



    private void BinaryDeserialize(string file) {
        var formatter = new BinaryFormatter();
        using var fs = File.Open(file, FileMode.Open);
        var data = (SerializedData)formatter.Deserialize(fs);
        specs = data.SpecKeys.Zip(data.SpecValues, (k, v) => new { k, v }).ToDictionary(a => a.k, a => a.v).ToImmutableDictionary();

        mainMenus = data.MenuValues.ToImmutableList();
    }

    private DataContractSerializerSettings Settings(Type[] additionalKnownTypes) {
        return new DataContractSerializerSettings() {
            KnownTypes = KnownTypes().Union(additionalKnownTypes),
            PreserveObjectReferences = true,
            
        };
    }



    private void XmlDeserialize(string file, Type[] additionalKnownTypes)
    {
       
        using var fs = File.Open(file, FileMode.Open);
      

        var deserializer = new DataContractSerializer(typeof(SerializedData),  Settings(additionalKnownTypes));
        var reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas() {MaxDepth = 100});
        var data = (SerializedData)deserializer.ReadObject(reader, true);


        specs = data.SpecKeys.Zip(data.SpecValues, (k, v) => new { k, v }).ToDictionary(a => a.k, a => a.v).ToImmutableDictionary();

        mainMenus = data.MenuValues.ToImmutableList();
    }


    #region ISpecificationCache Members

    public void Serialize(string file, Type[] additionalKnownTypes) {
        if (file.EndsWith(".bin")) {
            BinarySerialize(file);
        }
        else {
            XmlSerialize(file, additionalKnownTypes);
        }

    }

    private void BinarySerialize(string file) {
        var formatter = new BinaryFormatter();
        using var fs = File.Open(file, FileMode.OpenOrCreate);
        var data = new SerializedData { SpecKeys = specs.Keys.ToList(), SpecValues = specs.Values.ToList(), MenuValues = mainMenus.ToList() };
        formatter.Serialize(fs, data);
    }

    private void XmlSerialize(string file, Type[] additionalKnownTypes) {
        var formatter = new DataContractSerializer(typeof(SerializedData), Settings(additionalKnownTypes));
        using var fs = File.Open(file, FileMode.OpenOrCreate);
        var data = new SerializedData { SpecKeys = specs.Keys.ToList(), SpecValues = specs.Values.ToList(), MenuValues = mainMenus.ToList() };
        formatter.WriteObject(fs, data);
    }

    public ITypeSpecImmutable GetSpecification(string key) => specs.ContainsKey(key) ? specs[key] : null;

    public void Cache(string key, ITypeSpecImmutable spec) => specs = specs.Add(key, spec);

    public void Clear() => specs = specs.Clear();

    public ITypeSpecImmutable[] AllSpecifications() => specs.Values.ToArray();

    public void Cache(IMenuImmutable mainMenu) => mainMenus = mainMenus.Add(mainMenu);

    public IMenuImmutable[] MainMenus() => mainMenus.ToArray();

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.