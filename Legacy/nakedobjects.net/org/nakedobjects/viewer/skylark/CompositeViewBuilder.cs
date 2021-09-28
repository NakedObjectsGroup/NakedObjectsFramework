// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.CompositeViewBuilder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.nakedobjects.viewer.skylark
{
  [JavaInterface]
  public interface CompositeViewBuilder
  {
    void build(View view);

    View createCompositeView(
      Content content,
      CompositeViewSpecification specification,
      ViewAxis axis);

    ViewAxis createViewAxis();

    View decorateSubview(View subview);

    CompositeViewBuilder getReference();

    Size getRequiredSize(View view);

    bool isOpen();

    bool isReplaceable();

    bool isSubView();

    void layout(View view, Size maximumSize);

    void setReference(CompositeViewBuilder design);
  }
}
