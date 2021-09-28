// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.NakedValue
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;

namespace org.nakedobjects.@object
{
  [JavaInterfaces("1;org/nakedobjects/object/Naked;")]
  [JavaInterface]
  public interface NakedValue : Naked
  {
    sbyte[] asEncodedString();

    void clear();

    bool canClear();

    int getMaximumLength();

    int getMinumumLength();

    bool isEmpty();

    [JavaThrownExceptions("1;org/nakedobjects/object/InvalidEntryException;")]
    void parseTextEntry(string text);

    void restoreFromEncodedString(sbyte[] data);
  }
}
