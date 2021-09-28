// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.Content
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.skylark
{
  [JavaInterface]
  public interface Content
  {
    Consent canDrop(Content sourceContent);

    void contentMenuOptions(UserActionSet options);

    void debugDetails(DebugString debug);

    Naked drop(Content sourceContent);

    string getDescription();

    string getHelp();

    string getIconName();

    Image getIconPicture(int iconHeight);

    string getId();

    Naked getNaked();

    NakedObjectSpecification getSpecification();

    bool isCollection();

    bool isDerived();

    bool isObject();

    bool isPersistable();

    bool isTransient();

    bool isValue();

    [JavaThrownExceptions("1;org/nakedobjects/object/InvalidEntryException;")]
    void parseTextEntry(string entryText);

    string title();

    void viewMenuOptions(UserActionSet options);

    string windowTitle();
  }
}
