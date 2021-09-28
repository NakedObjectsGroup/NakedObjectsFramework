// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.special.CollectionElementBuilder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.core;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.special
{
  public class CollectionElementBuilder : AbstractViewBuilder
  {
    private static readonly org.apache.log4j.Logger LOG;
    private bool canDragView;
    private SubviewSpec subviewDesign;
    private bool showAll;

    public CollectionElementBuilder(SubviewSpec subviewDesign, bool showAll)
    {
      this.canDragView = true;
      this.subviewDesign = subviewDesign;
      this.showAll = showAll;
    }

    public override void build(View view)
    {
      Assert.assertEquals((object) view.getView(), (object) view);
      Content content = view.getContent();
      OneToManyAssociation association = !(content is OneToManyField) ? (OneToManyAssociation) null : ((OneToManyField) content).getOneToManyAssociation();
      if (CollectionElementBuilder.LOG.isDebugEnabled())
        CollectionElementBuilder.LOG.debug((object) new StringBuffer().append("rebuild view ").append((object) view).append(" for ").append((object) content).ToString());
      CollectionContent collectionContent = (CollectionContent) content;
      Enumeration enumeration = !this.showAll ? collectionContent.allElements() : collectionContent.allElements();
      View[] subviews = view.getSubviews();
      int length = subviews.Length;
      Naked[] nakedArray = length >= 0 ? (Naked[]) new NakedObject[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < subviews.Length; ++index)
      {
        view.removeView(subviews[index]);
        nakedArray[index] = subviews[index].getContent().getNaked();
      }
      while (enumeration.hasMoreElements())
      {
        NakedObject nakedObject = (NakedObject) enumeration.nextElement();
        View view1 = (View) null;
        for (int index = 0; index < subviews.Length; ++index)
        {
          if (nakedArray[index] == nakedObject)
          {
            view1 = subviews[index];
            break;
          }
        }
        if (view1 == null)
          view1 = this.subviewDesign.createSubview(association != null ? (Content) new OneToManyFieldElement(((OneToManyField) view.getContent()).getParent(), nakedObject, association) : (Content) new CollectionElement(nakedObject), view.getViewAxis());
        if (view1 != null)
          view.addView(view1);
      }
    }

    public override View createCompositeView(
      Content content,
      CompositeViewSpecification specification,
      ViewAxis axis)
    {
      CompositeView compositeView = new CompositeView(content, specification, axis);
      compositeView.setCanDragView(this.canDragView);
      return (View) compositeView;
    }

    public virtual void setCanDragView(bool canDragView) => this.canDragView = canDragView;

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static CollectionElementBuilder()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
