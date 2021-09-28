// Decompiled with JetBrains decompiler
// Type: org.xml.sax.Attributes
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.xml.sax
{
  [JavaInterface]
  public interface Attributes
  {
    int getLength();

    string getURI(int index);

    string getLocalName(int index);

    string getQName(int index);

    string getType(int index);

    string getValue(int index);

    int getIndex(string uri, string localName);

    int getIndex(string qName);

    string getType(string uri, string localName);

    string getType(string qName);

    string getValue(string uri, string localName);

    string getValue(string qName);
  }
}
