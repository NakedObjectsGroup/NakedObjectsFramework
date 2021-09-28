// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.RootCollection
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.util;

namespace org.nakedobjects.viewer.skylark
{
  public class RootCollection : CollectionContent
  {
    private readonly NakedCollection collection;

    public RootCollection(NakedCollection collection) => this.collection = collection;

    public override NakedObject[] elements()
    {
      int num = this.getCollection().size();
      int length = num;
      NakedObject[] nakedObjectArray = length >= 0 ? new NakedObject[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < num; ++index)
        nakedObjectArray[index] = this.getCollection().elementAt(index);
      return nakedObjectArray;
    }

    public virtual Consent canClear() => (Consent) Veto.DEFAULT;

    public virtual Consent canSet(NakedObject dragSource) => (Consent) Veto.DEFAULT;

    public virtual void clear() => throw new NakedObjectRuntimeException("Invalid call");

    public override void debugDetails(DebugString debug)
    {
      debug.appendln("collection", (object) this.collection);
      base.debugDetails(debug);
    }

    public override NakedCollection getCollection() => this.collection;

    public override bool isCollection() => true;

    public override string getDescription() => "";

    public override string getHelp() => "No help for this collection";

    public override string getIconName() => (string) null;

    public override string getId() => "";

    public override Naked getNaked() => (Naked) this.collection;

    public override NakedObjectSpecification getSpecification() => this.collection.getSpecification();

    public override bool isTransient() => this.collection != null;

    public virtual void setObject(NakedObject @object) => throw new NakedObjectRuntimeException("Invalid call");

    public override string title() => this.collection.titleString();

    public override string windowTitle() => this.collection.titleString();

    public override string ToString() => new StringBuffer().append("Root Collection: ").append((object) this.collection).ToString();

    public override Naked drop(Content sourceContent) => (Naked) null;

    public override Consent canDrop(Content sourceContent) => (Consent) Veto.DEFAULT;

    public override Image getIconPicture(int iconHeight) => ImageFactory.getInstance().loadIcon("root-collection", iconHeight);
  }
}
