// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.NakedObjectConfiguration
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.awt;
using java.util;

namespace org.nakedobjects.utility
{
  [JavaInterface]
  [JavaInterfaces("1;org/nakedobjects/utility/DebugInfo;")]
  public interface NakedObjectConfiguration : DebugInfo
  {
    void add(string name, string value);

    bool getBoolean(string name);

    bool getBoolean(string name, bool defaultValue);

    Color getColor(string name);

    Color getColor(string name, Color defaultValue);

    Font getFont(string name);

    Font getFont(string name, Font defaultValue);

    int getInteger(string name);

    int getInteger(string name, int defaultValue);

    string getString(string name);

    string getString(string name, string defaultValue);

    bool hasProperty(string name);

    string referedToAs(string name);

    NakedObjectConfiguration createSubset(string prefix);

    bool isEmpty();

    int size();

    Enumeration properties();
  }
}
