// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.OpenViewOption
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.apache.log4j;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.core
{
  public class OpenViewOption : AbstractUserAction
  {
    private static readonly Logger LOG;
    private ViewSpecification specification;

    public OpenViewOption(ViewSpecification builder)
      : base(new StringBuffer().append("Open as ").append(builder.getName()).ToString())
    {
      this.specification = builder;
    }

    public override void execute(Workspace workspace, View view, Location at)
    {
      View view1 = this.specification.createView(view.getContent(), (ViewAxis) null);
      if (OpenViewOption.LOG.isDebugEnabled())
        OpenViewOption.LOG.debug((object) new StringBuffer().append("open view ").append((object) view1).ToString());
      view1.setLocation(at);
      workspace.addView(view1);
      workspace.markDamaged();
    }

    public override string getDescription(View view) => new StringBuffer().append("Open '").append((object) view).append("' in a ").append(this.specification.getName()).append(" window").ToString();

    public override string ToString() => new StringBuffer().append(base.ToString()).append(" [prototype=").append(this.specification.getName()).append("]").ToString();

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static OpenViewOption()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
