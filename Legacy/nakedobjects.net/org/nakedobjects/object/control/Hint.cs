// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.control.Hint
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.io;
using org.nakedobjects.@object.control;

namespace org.nakedobjects.@object.control
{
  [JavaInterfaces("1;java/io/Serializable;")]
  [JavaInterface]
  public interface Hint : Serializable
  {
    Consent canAccess();

    Consent canUse();

    Consent isValid();

    string getDescription();

    string getName();

    string debug();
  }
}
