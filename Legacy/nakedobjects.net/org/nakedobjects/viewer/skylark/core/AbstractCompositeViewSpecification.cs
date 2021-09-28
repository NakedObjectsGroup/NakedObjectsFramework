// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.AbstractCompositeViewSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.skylark.core
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/CompositeViewSpecification;")]
  public abstract class AbstractCompositeViewSpecification : CompositeViewSpecification
  {
    [JavaFlags(4)]
    public CompositeViewBuilder builder;

    public virtual View createView(Content content, ViewAxis axis)
    {
      ViewAxis axis1 = this.builder.createViewAxis() ?? axis;
      return this.builder.createCompositeView(content, (CompositeViewSpecification) this, axis1);
    }

    public virtual CompositeViewBuilder getSubviewBuilder() => this.builder;

    public virtual View decorateSubview(View subview) => subview;

    public virtual bool isOpen() => true;

    public virtual bool isReplaceable() => true;

    public virtual bool isSubView() => false;

    [JavaFlags(4)]
    public virtual void resolveObject(Content content)
    {
      NakedObject @object = ((ObjectContent) content).getObject();
      if (@object.getResolveState().isResolved())
        return;
      NakedObjects.getObjectPersistor().resolveImmediately(@object);
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AbstractCompositeViewSpecification viewSpecification = this;
      ObjectImpl.clone((object) viewSpecification);
      return ((object) viewSpecification).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    public abstract bool canDisplay(Content content);

    public abstract string getName();
  }
}
