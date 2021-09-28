// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.table.InternalTableSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.viewer.skylark.metal;
using org.nakedobjects.viewer.skylark.special;

namespace org.nakedobjects.viewer.skylark.table
{
  public class InternalTableSpecification : AbstractTableSpecification
  {
    public InternalTableSpecification() => this.builder = (CompositeViewBuilder) new StackLayout((CompositeViewBuilder) new CollectionElementBuilder((SubviewSpec) this, true));

    public override View doCreateView(View view, Content content, ViewAxis axis)
    {
      ScrollBorder scrollBorder = new ScrollBorder(view);
      scrollBorder.setTopHeader((View) new TableHeader(content, view.getViewAxis()));
      scrollBorder.setFocusManager((FocusManager) new TableFocusManager((View) scrollBorder));
      return (View) scrollBorder;
    }

    public override string getName() => "Table";
  }
}
