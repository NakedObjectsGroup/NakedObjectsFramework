// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.ActionFieldBuilder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.special;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class ActionFieldBuilder : AbstractViewBuilder
  {
    private static readonly org.apache.log4j.Logger LOG;
    private SubviewSpec subviewDesign;

    public ActionFieldBuilder(SubviewSpec subviewDesign) => this.subviewDesign = subviewDesign;

    public override void build(View view)
    {
      Assert.assertEquals((object) view.getView(), (object) view);
      ActionContent content = (ActionContent) view.getContent();
      if (view.getSubviews().Length == 0)
        this.newBuild(view, content);
      else
        this.updateBuild(view, content);
    }

    public override View createCompositeView(
      Content content,
      CompositeViewSpecification specification,
      ViewAxis axis)
    {
      return (View) new CompositeView(content, specification, axis);
    }

    private View createFieldView(View view, ParameterContent parameter) => this.subviewDesign.createSubview((Content) parameter, view.getViewAxis()) ?? throw new NakedObjectRuntimeException("All parameters must be shown");

    private void newBuild(View view, ActionContent actionContent)
    {
      if (ActionFieldBuilder.LOG.isDebugEnabled())
        ActionFieldBuilder.LOG.debug((object) new StringBuffer().append("build new view ").append((object) view).append(" for ").append((object) actionContent).ToString());
      int noParameters = actionContent.getNoParameters();
      View view1 = (View) null;
      for (int index = 0; index < noParameters; ++index)
      {
        ParameterContent parameterContent = actionContent.getParameterContent(index);
        View fieldView = this.createFieldView(view, parameterContent);
        View view2 = this.decorateSubview((View) ParameterLabel.createInstance(fieldView));
        view.addView(view2);
        if (view1 == null && parameterContent is ValueParameter && fieldView.canFocus())
          view1 = view2;
      }
      if (view1 == null)
        return;
      view.getViewManager().setKeyboardFocus(view1);
    }

    private void updateBuild(View view, ActionContent actionContent)
    {
      if (ActionFieldBuilder.LOG.isDebugEnabled())
        ActionFieldBuilder.LOG.debug((object) new StringBuffer().append("rebuild view ").append((object) view).append(" for ").append((object) actionContent).ToString());
      View[] subviews = view.getSubviews();
      for (int index = 0; index < subviews.Length; ++index)
      {
        View toReplace = subviews[index];
        Content content = toReplace.getContent();
        Naked naked = toReplace.getContent().getNaked();
        Naked parameterObject = ((ActionContent) view.getContent()).getParameterObject(index);
        if (content is ObjectParameter)
        {
          if (naked != parameterObject)
          {
            ObjectParameter objectParameter = new ObjectParameter((ObjectParameter) content, (NakedObject) parameterObject);
            View fieldView = this.createFieldView(view, (ParameterContent) objectParameter);
            view.replaceView(toReplace, this.decorateSubview((View) ParameterLabel.createInstance(fieldView)));
          }
        }
        else
          toReplace.refresh();
      }
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static ActionFieldBuilder()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
