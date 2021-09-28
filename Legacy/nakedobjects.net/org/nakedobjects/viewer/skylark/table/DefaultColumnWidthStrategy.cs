// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.table.DefaultColumnWidthStrategy
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.skylark.table
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/table/ColumnWidthStrategy;")]
  public class DefaultColumnWidthStrategy : ColumnWidthStrategy
  {
    private int minimum;
    private int preferred;
    private int maximum;

    public DefaultColumnWidthStrategy()
      : this(18, 70, 250)
    {
    }

    public DefaultColumnWidthStrategy(int minimum, int preferred, int maximum)
    {
      if (minimum <= 0)
        throw new IllegalArgumentException("minimum width must be greater than zero");
      if (preferred <= minimum || preferred >= maximum)
        throw new IllegalArgumentException("preferred width must be greater than minimum and less than maximum");
      this.minimum = minimum;
      this.preferred = preferred;
      this.maximum = maximum;
    }

    public virtual int getMinimumWidth(int i, NakedObjectField specification) => this.minimum;

    public virtual int getPreferredWidth(int i, NakedObjectField specification) => this.preferred;

    public virtual int getMaximumWidth(int i, NakedObjectField specification) => this.maximum;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      DefaultColumnWidthStrategy columnWidthStrategy = this;
      ObjectImpl.clone((object) columnWidthStrategy);
      return ((object) columnWidthStrategy).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
