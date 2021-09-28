// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.table.TypeBasedColumnWidthStrategy
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.table
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/table/ColumnWidthStrategy;")]
  public class TypeBasedColumnWidthStrategy : ColumnWidthStrategy
  {
    private static readonly NakedObjectSpecification NAKEDOBJECT;
    private Hashtable types;

    public TypeBasedColumnWidthStrategy() => this.types = new Hashtable();

    public virtual void addWidth(NakedObjectSpecification specification, int width) => this.types.put((object) specification, (object) new Integer(width));

    public virtual int getMaximumWidth(int i, NakedObjectField specification) => 0;

    public virtual int getMinimumWidth(int i, NakedObjectField specification) => 15;

    public virtual int getPreferredWidth(int i, NakedObjectField specification)
    {
      NakedObjectSpecification specification1 = specification.getSpecification();
      if (specification1 == null)
        return 200;
      Integer integer = (Integer) this.types.get((object) specification1);
      if (integer != null)
        return integer.intValue();
      return specification1.isOfType(TypeBasedColumnWidthStrategy.NAKEDOBJECT) ? 120 : 100;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static TypeBasedColumnWidthStrategy()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      TypeBasedColumnWidthStrategy columnWidthStrategy = this;
      ObjectImpl.clone((object) columnWidthStrategy);
      return ((object) columnWidthStrategy).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
