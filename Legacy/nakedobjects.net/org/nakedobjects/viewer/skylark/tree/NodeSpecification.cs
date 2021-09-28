// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.tree.NodeSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.tree
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/ViewSpecification;")]
  [JavaFlags(1056)]
  public abstract class NodeSpecification : ViewSpecification
  {
    public const int CAN_OPEN = 1;
    public const int CANT_OPEN = 2;
    public const int UNKNOWN = 0;
    private ViewSpecification replacementNodeSpecification;

    public abstract int canOpen(Content content);

    [JavaFlags(1028)]
    public abstract View createNodeView(Content content, ViewAxis axis);

    [JavaFlags(17)]
    public virtual View createView(Content content, ViewAxis axis)
    {
      TreeNodeBorder treeNodeBorder = new TreeNodeBorder(this.createNodeView(content, axis), this.replacementNodeSpecification);
      treeNodeBorder.setFocusManager((FocusManager) new NodeSpecification.\u0031(this));
      return (View) treeNodeBorder;
    }

    public virtual bool isOpen() => false;

    public virtual bool isReplaceable() => false;

    public virtual bool isSubView() => true;

    [JavaFlags(16)]
    public void setReplacementNodeSpecification(ViewSpecification replacementNodeSpecification) => this.replacementNodeSpecification = replacementNodeSpecification;

    [JavaFlags(0)]
    public NodeSpecification()
    {
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      NodeSpecification nodeSpecification = this;
      ObjectImpl.clone((object) nodeSpecification);
      return ((object) nodeSpecification).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    public abstract bool canDisplay(Content content);

    public abstract string getName();

    [Inner]
    [JavaFlags(32)]
    public class \u0031 : NullFocusManager
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private NodeSpecification this\u00240;

      public override void setFocus(View view)
      {
        view?.focusReceived();
        base.setFocus(view);
      }

      public \u0031(NodeSpecification _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }
  }
}
