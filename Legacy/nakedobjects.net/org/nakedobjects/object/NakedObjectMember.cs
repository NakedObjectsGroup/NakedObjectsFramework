// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.NakedObjectMember
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object
{
  [JavaInterface]
  public interface NakedObjectMember
  {
    void debugData(DebugString debugString);

    string getDescription();

    object getExtension(Class cls);

    Class[] getExtensions();

    string getHelp();

    string getId();

    string getName();

    bool isAuthorised();

    Consent isAvailable(NakedReference target);

    Consent isVisible(NakedReference target);
  }
}
