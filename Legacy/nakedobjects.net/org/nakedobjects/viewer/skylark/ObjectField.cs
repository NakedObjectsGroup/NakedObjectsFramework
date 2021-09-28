// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.ObjectField
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.skylark
{
  [JavaFlags(48)]
  public sealed class ObjectField
  {
    private readonly NakedObjectField field;
    private readonly NakedObject parent;

    [JavaFlags(0)]
    public ObjectField(NakedObject parent, NakedObjectField field)
    {
      this.parent = parent;
      this.field = field;
    }

    public virtual void debugDetails(DebugString debug)
    {
      debug.appendln("field", (object) this.getFieldReflector());
      debug.appendln("name", (object) this.getName());
      debug.appendln("specification", (object) this.getSpecification());
      debug.appendln("parent", (object) this.parent);
    }

    public virtual string getDescription() => this.field.getDescription();

    public virtual string getHelp() => this.field.getHelp();

    public virtual NakedObjectField getFieldReflector() => this.field;

    [JavaFlags(17)]
    public string getName() => this.field.getName();

    public virtual NakedObject getParent() => this.parent;

    public virtual NakedObjectSpecification getSpecification() => this.field.getSpecification();

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      ObjectField objectField = this;
      ObjectImpl.clone((object) objectField);
      return ((object) objectField).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
