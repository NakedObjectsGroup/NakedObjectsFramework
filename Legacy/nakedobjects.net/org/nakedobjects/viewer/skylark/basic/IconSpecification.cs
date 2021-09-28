// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.IconSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.nakedobjects.viewer.skylark.basic
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/ViewSpecification;")]
  public abstract class IconSpecification : ViewSpecification
  {
    private bool isSubView;
    private bool isReplaceable;

    public IconSpecification()
      : this(true, true)
    {
    }

    [JavaFlags(0)]
    public IconSpecification(bool isSubView, bool isReplaceable)
    {
      this.isSubView = isSubView;
      this.isReplaceable = isReplaceable;
    }

    public virtual bool canDisplay(Content content) => content.isObject() && content.getNaked() != null;

    public virtual View createView(Content content, ViewAxis axis) => (View) new ObjectBorder((View) new IconOpenAction((View) new IconView(content, (ViewSpecification) this, axis, Style.NORMAL)));

    public virtual string getName() => "Icon";

    public virtual bool isSubView() => this.isSubView;

    public virtual bool isReplaceable() => this.isReplaceable;

    public virtual View decorateSubview(View subview) => subview;

    public virtual bool isOpen() => false;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      IconSpecification iconSpecification = this;
      ObjectImpl.clone((object) iconSpecification);
      return ((object) iconSpecification).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
