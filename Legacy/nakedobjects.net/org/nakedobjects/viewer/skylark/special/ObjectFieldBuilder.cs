// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.special.ObjectFieldBuilder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.util;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.special
{
  public class ObjectFieldBuilder : AbstractViewBuilder
  {
    private static readonly org.apache.log4j.Logger LOG;
    private SubviewSpec subviewDesign;
    private readonly bool useFieldType;

    public ObjectFieldBuilder(SubviewSpec subviewDesign)
      : this(subviewDesign, false)
    {
    }

    public ObjectFieldBuilder(SubviewSpec subviewDesign, bool useFieldType)
    {
      this.subviewDesign = subviewDesign;
      this.useFieldType = useFieldType;
    }

    public override void build(View view)
    {
      Assert.assertEquals("ensure the view is the complete decorated view", (object) view.getView(), (object) view);
      Content content = view.getContent();
      NakedObject @object = ((ObjectContent) content).getObject();
      if (ObjectFieldBuilder.LOG.isDebugEnabled())
        ObjectFieldBuilder.LOG.debug((object) new StringBuffer().append("build view ").append((object) view).append(" for ").append((object) @object).ToString());
      NakedObjectSpecification objectSpecification = (NakedObjectSpecification) null;
      if (this.useFieldType)
        objectSpecification = content.getSpecification();
      if (objectSpecification == null)
        objectSpecification = @object.getSpecification();
      NakedObjectField[] visibleFields = objectSpecification.getVisibleFields(@object);
      if (view.getSubviews().Length == 0)
        this.newBuild(view, @object, visibleFields);
      else
        this.updateBuild(view, @object, visibleFields);
    }

    public override View createCompositeView(
      Content content,
      CompositeViewSpecification specification,
      ViewAxis axis)
    {
      return (View) new CompositeView(content, specification, axis);
    }

    public override View decorateSubview(View subview) => this.subviewDesign.decorateSubview(subview);

    private void newBuild(View view, NakedObject @object, NakedObjectField[] flds)
    {
      if (ObjectFieldBuilder.LOG.isDebugEnabled())
        ObjectFieldBuilder.LOG.debug((object) "  as new build");
      for (int index = 0; index < flds.Length; ++index)
      {
        NakedObjectField fld = flds[index];
        this.addField(view, @object, fld);
      }
    }

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
        ObjectFieldBuilder.LOG.error((object) "invalid field", (Throwable) ex);
        view.addView((View) new FieldErrorView(((Throwable) ex).getMessage()));
      }
    }

    private void updateBuild(View view, NakedObject @object, NakedObjectField[] flds)
    {
      if (ObjectFieldBuilder.LOG.isDebugEnabled())
        ObjectFieldBuilder.LOG.debug((object) "  as update build");
      View[] subviews1 = view.getSubviews();
label_10:
      for (int index1 = 0; index1 < subviews1.Length; ++index1)
      {
        FieldContent content = (FieldContent) subviews1[index1].getContent();
        for (int index2 = 0; index2 < flds.Length; ++index2)
        {
          NakedObjectField fld = flds[index2];
          if (content.getField() == fld)
            goto label_10;
        }
        view.removeView(subviews1[index1]);
      }
      View[] subviews2 = view.getSubviews();
      for (int index = 0; index < subviews2.Length; ++index)
      {
        View toReplace = subviews2[index];
        NakedObjectField field1 = ((FieldContent) toReplace.getContent()).getField();
        Naked field2 = @object.getField(field1);
        if (field1.isValue())
        {
          Naked naked = toReplace.getContent().getNaked();
          if (!toReplace.canChangeValue() && field2 != null && !field2.getObject().Equals(naked.getObject()))
          {
            View fieldView = this.createFieldView(view, @object, field1, field2);
            view.replaceView(toReplace, this.decorateSubview(fieldView));
          }
          toReplace.refresh();
        }
        else if (field2 is NakedCollection)
        {
          toReplace.update(field2);
        }
        else
        {
          NakedObject nakedObject = ((ObjectContent) subviews2[index].getContent()).getObject();
          if (field2 != nakedObject)
          {
            View subview;
            try
            {
              subview = this.createFieldView(view, @object, field1, field2);
            }
            catch (NakedObjectFieldException ex)
            {
              ObjectFieldBuilder.LOG.error((object) "invalid field", (Throwable) ex);
              subview = (View) new FieldErrorView(((Throwable) ex).getMessage());
            }
            if (subview != null)
              view.replaceView(toReplace, this.decorateSubview(subview));
          }
        }
      }
label_33:
      for (int index3 = 0; index3 < flds.Length; ++index3)
      {
        NakedObjectField fld = flds[index3];
        for (int index4 = 0; index4 < subviews2.Length; ++index4)
        {
          if (((FieldContent) subviews2[index4].getContent()).getField() == fld)
            goto label_33;
        }
        this.addField(view, @object, fld);
      }
      if (!ObjectFieldBuilder.LOG.isDebugEnabled())
        return;
      ObjectFieldBuilder.LOG.debug((object) "fields:-");
      View[] subviews3 = view.getSubviews();
      for (int index = 0; index < flds.Length; ++index)
      {
        org.apache.log4j.Logger log = ObjectFieldBuilder.LOG;
        StringBuffer stringBuffer = new StringBuffer().append("  - ").append(index).append(") ").append(flds[index].getId()).append(" ");
        NakedObjectField fld = flds[index];
        int num = !(fld is string) ? ObjectImpl.hashCode((object) fld) : StringImpl.hashCode((string) fld);
        string str = stringBuffer.append(num).ToString();
        log.debug((object) str);
      }
      ObjectFieldBuilder.LOG.debug((object) "subviews:-");
      for (int index = 0; index < subviews3.Length; ++index)
      {
        FieldContent content = (FieldContent) subviews3[index].getContent();
        org.apache.log4j.Logger log = ObjectFieldBuilder.LOG;
        StringBuffer stringBuffer = new StringBuffer().append("  - ").append(index).append(") ").append(content.getFieldName()).append(" ");
        NakedObjectField field = content.getField();
        int num = !(field is string) ? ObjectImpl.hashCode((object) field) : StringImpl.hashCode((string) field);
        string str = stringBuffer.append(num).ToString();
        log.debug((object) str);
      }
    }

    private View createFieldView(
      View view,
      NakedObject @object,
      NakedObjectField field,
      Naked value)
    {
      if (field == null)
        throw new NullPointerException();
      Content content;
      if (field is OneToManyAssociation)
        content = (Content) new OneToManyField(@object, (InternalCollection) value, (OneToManyAssociation) field);
      else if (field.isValue())
      {
        content = (Content) new ValueField(@object, (NakedValue) value, (OneToOneAssociation) field);
      }
      else
      {
        if (!(field is OneToOneAssociation))
          throw new NakedObjectRuntimeException();
        content = (Content) new OneToOneField(@object, (NakedObject) value, (OneToOneAssociation) field);
      }
      return this.subviewDesign.createSubview(content, view.getViewAxis());
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static ObjectFieldBuilder()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
