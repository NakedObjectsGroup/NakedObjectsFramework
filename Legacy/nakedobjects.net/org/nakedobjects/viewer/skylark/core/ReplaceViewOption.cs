// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.ReplaceViewOption
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.apache.log4j;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.core
{
  public class ReplaceViewOption : AbstractUserAction
  {
    private static readonly Logger LOG;
    private ViewSpecification specification;

    public ReplaceViewOption(ViewSpecification specification)
      : base(new StringBuffer().append("View as ").append(specification.getName()).ToString())
    {
      this.specification = specification;
    }

    public override string getDescription(View view) => new StringBuffer().append("Replace this ").append(view.getSpecification().getName()).append(" view with a ").append(this.specification.getName()).append(" view").ToString();

    public override void execute(Workspace workspace, View view, Location at)
    {
      View view1 = this.specification.createView(view.getContent(), view.getViewAxis());
      View replacement = ((CompositeViewSpecification) view.getParent().getSpecification()).getSubviewBuilder().decorateSubview(view1);
      if (ReplaceViewOption.LOG.isDebugEnabled())
        ReplaceViewOption.LOG.debug((object) new StringBuffer().append("replacement view ").append((object) replacement).ToString());
      view.getParent().replaceView(view, replacement);
    }

    public override string ToString() => new StringBuffer().append(base.ToString()).append(" [prototype=").append(this.specification.getName()).append("]").ToString();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static ReplaceViewOption()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
