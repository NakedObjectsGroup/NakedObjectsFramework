// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.table.AbstractTableSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.@object.value;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.special;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.table
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/special/SubviewSpec;")]
  public abstract class AbstractTableSpecification : AbstractCompositeViewSpecification, SubviewSpec
  {
    private static readonly Logger LOG;
    private ViewSpecification rowSpecification;

    public AbstractTableSpecification() => this.builder = (CompositeViewBuilder) new StackLayout((CompositeViewBuilder) new CollectionElementBuilder((SubviewSpec) this, true));

    public override bool canDisplay(Content content)
    {
      if (!content.isCollection())
        return false;
      NakedObjectField[] forCollectiveView = ((TypedNakedCollection) ((CollectionContent) content).getCollection()).getElementSpecification().getAccessibleFieldsForCollectiveView();
      for (int index = 0; index < forCollectiveView.Length; ++index)
      {
        if (forCollectiveView[index].isObject() || forCollectiveView[index].isValue())
          return true;
      }
      return false;
    }

    public virtual View createSubview(Content content, ViewAxis axis) => this.rowSpecification.createView(content, axis);

    [JavaFlags(17)]
    public override sealed View createView(Content content, ViewAxis axis)
    {
      TableAxis tableAxis = new TableAxis(this.tableFields(((TypedNakedCollection) ((CollectionContent) content).getCollection()).getElementSpecification().getAccessibleFieldsForCollectiveView()));
      tableAxis.setupColumnWidths((ColumnWidthStrategy) new TypeBasedColumnWidthStrategy());
      View view = base.createView(content, (ViewAxis) tableAxis);
      tableAxis.setRoot(view);
      this.rowSpecification = (ViewSpecification) new TableRowSpecification();
      return this.doCreateView(view, content, axis);
    }

    [JavaFlags(1028)]
    public abstract View doCreateView(View table, Content content, ViewAxis axis);

    public override string getName() => "Standard Table";

    public override bool isReplaceable() => false;

    private NakedObjectField[] tableFields(NakedObjectField[] viewFields)
    {
      for (int index = 0; index < viewFields.Length; ++index)
      {
        if (!viewFields[index].getSpecification().isOfType(NakedObjects.getSpecificationLoader().loadSpecification(Class.FromType(typeof (ImageValue)))) && !viewFields[index].isHidden() && AbstractTableSpecification.LOG.isDebugEnabled())
          AbstractTableSpecification.LOG.debug((object) new StringBuffer().append("column ").append((object) viewFields[index].getSpecification()).ToString());
      }
      int length1 = viewFields.Length;
      NakedObjectField[] nakedObjectFieldArray1 = length1 >= 0 ? new NakedObjectField[length1] : throw new NegativeArraySizeException();
      int num1 = 0;
      for (int index1 = 0; index1 < viewFields.Length; ++index1)
      {
        if (!(viewFields[index1] is OneToManyAssociation))
        {
          NakedObjectField[] nakedObjectFieldArray2 = nakedObjectFieldArray1;
          int num2;
          num1 = (num2 = num1) + 1;
          int index2 = num2;
          NakedObjectField viewField = viewFields[index1];
          nakedObjectFieldArray2[index2] = viewField;
        }
      }
      int length2 = num1;
      NakedObjectField[] nakedObjectFieldArray3 = length2 >= 0 ? new NakedObjectField[length2] : throw new NegativeArraySizeException();
      java.lang.System.arraycopy((object) nakedObjectFieldArray1, 0, (object) nakedObjectFieldArray3, 0, num1);
      return nakedObjectFieldArray3;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static AbstractTableSpecification()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
