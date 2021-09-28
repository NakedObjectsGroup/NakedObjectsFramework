// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.value.CharValue
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;

namespace org.nakedobjects.@object.value
{
  [JavaInterface]
  [JavaInterfaces("1;org/nakedobjects/object/NakedValue;")]
  public interface CharValue : NakedValue
  {
    char charValue();

    void setValue(char value);
  }
}
