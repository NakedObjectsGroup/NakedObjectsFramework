// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.MemberPeer
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object.reflect
{
  [JavaInterface]
  public interface MemberPeer
  {
    void debugData(DebugString debugString);

    string getDescription();

    object getExtension(Class cls);

    Class[] getExtensions();

    string getHelp();

    MemberIdentifier getIdentifier();

    string getName();

    bool isAuthorised(Session session);

    Consent isAvailable(NakedReference target);

    Consent isVisible(NakedReference target);
  }
}
