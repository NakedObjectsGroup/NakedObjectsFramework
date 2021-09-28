// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.table.TableRowSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.table
{
  public class TableRowSpecification : AbstractCompositeViewSpecification
  {
    public TableRowSpecification() => this.builder = (CompositeViewBuilder) new TableCellBuilder();

    public override bool canDisplay(Content content) => content.isObject();

    public override View createView(Content content, ViewAxis axis)
    {
      this.resolveObject(content);
      return (View) new TableRowBorder(base.createView(content, axis));
    }

    public override string getName() => "Table Row";

    public override bool isReplaceable() => false;

    public override bool isSubView() => true;
  }
}
