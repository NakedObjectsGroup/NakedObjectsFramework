// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.special.RootWorkspaceSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.skylark.basic;

namespace org.nakedobjects.viewer.skylark.special
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/CompositeViewSpecification;")]
  public class RootWorkspaceSpecification : CompositeViewSpecification
  {
    [JavaFlags(0)]
    public WorkspaceBuilder builder;

    public virtual View createView(Content content, ViewAxis axis)
    {
      Workspace workspace;
      if (((ObjectContent) content).getObject() is UserContext)
      {
        workspace = (Workspace) new UserContextWorkspace(content, (CompositeViewSpecification) this, axis);
      }
      else
      {
        workspace = (Workspace) new DefaultWorkspace(content, (CompositeViewSpecification) this, axis);
        workspace.setFocusManager((FocusManager) new WorkspaceFocusManager());
      }
      return (View) workspace;
    }

    public virtual CompositeViewBuilder getSubviewBuilder() => (CompositeViewBuilder) this.builder;

    public virtual string getName() => "Root Workspace";

    public virtual bool isOpen() => true;

    public virtual bool isReplaceable() => false;

    public virtual bool isSubView() => false;

    public virtual bool canDisplay(Content content) => content.isObject() && content.getNaked() is UserContext;

    public RootWorkspaceSpecification() => this.builder = new WorkspaceBuilder();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      RootWorkspaceSpecification workspaceSpecification = this;
      ObjectImpl.clone((object) workspaceSpecification);
      return ((object) workspaceSpecification).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
