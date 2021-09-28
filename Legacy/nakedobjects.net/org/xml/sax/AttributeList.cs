// Decompiled with JetBrains decompiler
// Type: org.xml.sax.AttributeList
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using System;

namespace org.xml.sax
{
  [JavaInterface]
  [Obsolete(null, false)]
  public interface AttributeList
  {
    int getLength();

    string getName(int i);

    string getType(int i);

    string getValue(int i);

    string getType(string name);

    string getValue(string name);
  }
}
