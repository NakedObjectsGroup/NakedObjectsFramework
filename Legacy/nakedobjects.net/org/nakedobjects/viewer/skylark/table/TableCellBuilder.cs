// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.table.TableCellBuilder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.value;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.metal;
using org.nakedobjects.viewer.skylark.util;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.table
{
  [JavaFlags(32)]
  public class TableCellBuilder : AbstractViewBuilder
  {
    private static readonly org.apache.log4j.Logger LOG;

    private void addField(View view, NakedObject @object, NakedObjectField field)
    {
      try
      {
        Naked field1 = @object.getField(field);
        View fieldView = this.createFieldView(view, @object, field, field1);
        if (fieldView == null)
          return;
        view.addView(this.decorateSubview(fieldView));
      }
      catch (NakedObjectFieldException ex)
      {
        TableCellBuilder.LOG.error((object) "invalid field", (Throwable) ex);
        view.addView((View) new FieldErrorView(((Throwable) ex).getMessage()));
      }
    }

    public override void build(View view)
    {
      Assert.assertEquals("ensure the view is complete decorated view", (object) view.getView(), (object) view);
      NakedObject @object = ((ObjectContent) view.getContent()).getObject();
      TableAxis viewAxis = (TableAxis) view.getViewAxis();
      if (view.getSubviews().Length == 0)
        this.buildNew(@object, view, viewAxis);
      else
        this.buildUpdate(@object, view, viewAxis);
    }

    private void buildUpdate(NakedObject @object, View view, TableAxis viewAxis)
    {
      if (TableCellBuilder.LOG.isDebugEnabled())
        TableCellBuilder.LOG.debug((object) new StringBuffer().append("update view ").append((object) view).append(" for ").append((object) @object).ToString());
      View[] subviews = view.getSubviews();
      for (int column = 0; column < subviews.Length; ++column)
      {
        View toReplace = subviews[column];
        NakedObjectField fieldForColumn = viewAxis.getFieldForColumn(column);
        NakedObjectField fieldWithSameName = this.getFieldWithSameName(@object, fieldForColumn);
        Naked field = @object.getField(fieldWithSameName);
        if (fieldWithSameName.isValue())
        {
          bool flag1 = fieldWithSameName.isVisible((NakedReference) @object).isVetoed() ^ toReplace is BlankView;
          Naked naked = toReplace.getContent().getNaked();
          bool flag2 = field != null && !field.getObject().Equals(naked.getObject());
          if (flag1 || flag2)
          {
            View fieldView = this.createFieldView(view, @object, fieldWithSameName, field);
            view.replaceView(toReplace, this.decorateSubview(fieldView));
          }
          toReplace.refresh();
        }
        else if (fieldWithSameName.isObject())
        {
          NakedObject nakedObject = ((ObjectContent) subviews[column].getContent()).getObject();
          if (field != nakedObject)
          {
            View subview;
            try
            {
              subview = this.createFieldView(view, @object, fieldWithSameName, field);
            }
            catch (NakedObjectFieldException ex)
            {
              TableCellBuilder.LOG.error((object) "invalid field", (Throwable) ex);
              subview = (View) new FieldErrorView(((Throwable) ex).getMessage());
            }
            if (subview != null)
              view.replaceView(toReplace, this.decorateSubview(subview));
          }
        }
      }
    }

    private NakedObjectField getFieldWithSameName(
      NakedObject @object,
      NakedObjectField field)
    {
      NakedObjectField[] fields = @object.getSpecification().getFields();
      string name = field.getName();
      for (int index = 0; index < fields.Length; ++index)
      {
        if (StringImpl.equalsIgnoreCase(fields[index].getName(), name))
          return fields[index];
      }
      Assert.assertTrue(new StringBuffer().append("No field found in object ").append(@object.titleString()).append(" with name ").append(name).ToString(), false);
      return (NakedObjectField) null;
    }

    private void buildNew(NakedObject @object, View view, TableAxis viewAxis)
    {
      if (TableCellBuilder.LOG.isDebugEnabled())
        TableCellBuilder.LOG.debug((object) new StringBuffer().append("build view ").append((object) view).append(" for ").append((object) @object).ToString());
      int columnCount = viewAxis.getColumnCount();
      for (int column = 0; column < columnCount; ++column)
      {
        NakedObjectField fieldForColumn = viewAxis.getFieldForColumn(column);
        NakedObjectField fieldWithSameName = this.getFieldWithSameName(@object, fieldForColumn);
        this.addField(view, @object, fieldWithSameName);
      }
    }

    public override View createCompositeView(
      Content content,
      CompositeViewSpecification specification,
      ViewAxis axis)
    {
      return (View) new CompositeView(content, specification, axis);
    }

    private View createFieldView(
      View view,
      NakedObject @object,
      NakedObjectField field,
      Naked value)
    {
      if (field == null)
        throw new NullPointerException();
      ViewFactory viewFactory = Skylark.getViewFactory();
      if (field is OneToManyAssociation)
        throw new UnexpectedCallException("no collections allowed");
      Content content;
      ViewSpecification viewSpecification;
      if (field.isValue())
      {
        content = (Content) new ValueField(@object, (NakedValue) value, (OneToOneAssociation) field);
        if (content.getNaked() is ImageValue)
          return (View) new BlankView(content);
        if (field.isVisible((NakedReference) @object).isVetoed())
          return (View) new BlankView(content);
        viewSpecification = !(content.getNaked() is MultilineStringValue) ? viewFactory.getValueFieldSpecification((ValueContent) content) : (ViewSpecification) new UnlinedTextFieldSpecification();
      }
      else
      {
        if (!(field is OneToOneAssociation))
          throw new UnknownTypeException((object) field);
        content = (Content) new OneToOneField(@object, (NakedObject) value, (OneToOneAssociation) field);
        if (field.isVisible((NakedReference) @object).isVetoed())
          return (View) new BlankView(content);
        viewSpecification = viewFactory.getIconizedSubViewSpecification(content);
      }
      ViewAxis viewAxis = view.getViewAxis();
      return viewSpecification.createView(content, viewAxis);
    }

    public override Size getRequiredSize(View row)
    {
      int height = 0;
      int width = 0;
      TableAxis viewAxis = (TableAxis) row.getViewAxis();
      View[] subviews = row.getSubviews();
      int num1 = this.maxBaseline(subviews);
      for (int column = 0; column < subviews.Length; ++column)
      {
        width += viewAxis.getColumnWidth(column);
        Size requiredSize = subviews[column].getRequiredSize(new Size());
        int baseline = subviews[column].getBaseline();
        int num2 = Math.max(0, num1 - baseline);
        height = Math.max(height, requiredSize.getHeight() + num2);
      }
      return new Size(width, height);
    }

    public override void layout(View row, Size maximumSize)
    {
      int x = 0;
      TableAxis viewAxis = (TableAxis) row.getViewAxis();
      View[] subviews = row.getSubviews();
      int num = this.maxBaseline(subviews);
      for (int column = 0; column < subviews.Length; ++column)
      {
        View view = subviews[column];
        Size requiredSize = view.getRequiredSize(new Size(maximumSize));
        requiredSize.setWidth(viewAxis.getColumnWidth(column));
        view.setSize(requiredSize);
        int baseline = view.getBaseline();
        int y = Math.max(0, num - baseline);
        view.setLocation(new Location(x, y));
        x += requiredSize.getWidth();
      }
      Padding padding = row.getPadding();
      Size size = new Size(padding.getLeftRight(), padding.getTopBottom());
      row.setSize(size);
    }

    private int maxBaseline(View[] cells)
    {
      int num = 0;
      for (int index = 0; index < cells.Length; ++index)
      {
        View cell = cells[index];
        num = Math.max(num, cell.getBaseline());
      }
      return num;
    }

    [JavaFlags(0)]
    public TableCellBuilder()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static TableCellBuilder()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
