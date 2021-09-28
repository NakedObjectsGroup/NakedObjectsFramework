// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.NonBuildingSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.skylark.core
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/ViewSpecification;")]
  public class NonBuildingSpecification : ViewSpecification
  {
    private string name;

    public NonBuildingSpecification(View view)
    {
      string name = ObjectImpl.getClass((object) view).getName();
      this.name = StringImpl.substring(name, StringImpl.lastIndexOf(name, 46) + 1);
    }

    public virtual View createView(Content content, ViewAxis axis) => throw new UnexpectedCallException();

    public virtual string getName() => this.name;

    public virtual bool isOpen() => true;

    public virtual bool isReplaceable() => false;

    public virtual bool isSubView() => false;

    public virtual bool canDisplay(Content content) => false;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      NonBuildingSpecification buildingSpecification = this;
      ObjectImpl.clone((object) buildingSpecification);
      return ((object) buildingSpecification).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
