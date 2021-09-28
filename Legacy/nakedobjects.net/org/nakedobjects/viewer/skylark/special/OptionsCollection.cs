// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.special.OptionsCollection
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.basic;

namespace org.nakedobjects.viewer.skylark.special
{
  public class OptionsCollection : CollectionContent
  {
    private readonly NakedObject[] objects;

    public OptionsCollection(NakedObject[] objects)
    {
      this.objects = objects;
      this.setOrder((Comparator) new TitleComparator());
    }

    public override NakedObject[] elements() => this.objects;

    public override NakedCollection getCollection() => (NakedCollection) null;

    public override Consent canDrop(Content sourceContent) => (Consent) Veto.DEFAULT;

    public override void debugDetails(DebugString debug)
    {
      debug.appendln("options (array)");
      debug.indent();
      for (int index = 0; index < this.objects.Length; ++index)
        debug.appendln(this.objects[index].ToString());
      debug.unindent();
      base.debugDetails(debug);
    }

    public override Naked drop(Content sourceContent) => (Naked) null;

    public override string getIconName() => (string) null;

    public override Image getIconPicture(int iconHeight) => (Image) null;

    public override Naked getNaked() => throw new UnexpectedCallException();

    public override NakedObjectSpecification getSpecification() => throw new UnexpectedCallException();

    public override bool isTransient() => false;

    public override string title() => "";

    public override string getDescription() => "";

    public override string getHelp() => "";

    public override string getId() => "";
  }
}
