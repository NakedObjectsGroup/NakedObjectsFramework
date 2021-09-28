// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.tree.FullTreeBrowserSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.viewer.skylark.metal;

namespace org.nakedobjects.viewer.skylark.tree
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/ViewSpecification;")]
  public class FullTreeBrowserSpecification : ViewSpecification
  {
    private readonly OpenCollectionNodeSpecification openCollection;
    private readonly OpenObjectNodeSpecification openObject;

    public FullTreeBrowserSpecification()
    {
      ClosedObjectNodeSpecification nodeSpecification1 = new ClosedObjectNodeSpecification(true);
      ClosedCollectionNodeSpecification nodeSpecification2 = new ClosedCollectionNodeSpecification();
      this.openObject = new OpenObjectNodeSpecification();
      this.openObject.setCollectionSubNodeSpecification((NodeSpecification) nodeSpecification2);
      this.openObject.setObjectSubNodeSpecification((NodeSpecification) nodeSpecification1);
      this.openObject.setReplacementNodeSpecification((ViewSpecification) nodeSpecification1);
      nodeSpecification1.setReplacementNodeSpecification((ViewSpecification) this.openObject);
      this.openCollection = new OpenCollectionNodeSpecification();
      this.openCollection.setCollectionSubNodeSpecification((NodeSpecification) nodeSpecification2);
      this.openCollection.setObjectSubNodeSpecification((NodeSpecification) nodeSpecification1);
      this.openCollection.setReplacementNodeSpecification((ViewSpecification) nodeSpecification2);
      nodeSpecification2.setReplacementNodeSpecification((ViewSpecification) this.openCollection);
    }

    public virtual bool canDisplay(Content content) => this.openCollection.canDisplay(content) || this.openObject.canDisplay(content);

    public virtual View createView(Content content, ViewAxis axis)
    {
      TreeBrowserFrame treeBrowserFrame = new TreeBrowserFrame(content, (ViewSpecification) this);
      View view1 = this.addBorder((View) treeBrowserFrame);
      axis = (ViewAxis) treeBrowserFrame;
      View view2;
      if (this.openCollection.canDisplay(content))
      {
        view2 = this.openCollection.createView(content, axis);
      }
      else
      {
        view2 = this.openObject.createView(content, axis);
        treeBrowserFrame.setSelectedNode(view2);
      }
      View view3 = view2;
      treeBrowserFrame.initLeftPane(view3);
      Size requiredSize = view3.getRequiredSize(new Size());
      requiredSize.setWidth(220);
      view3.setMaximumSize(requiredSize);
      return view1;
    }

    [JavaFlags(4)]
    public virtual View addBorder(View frame) => (View) new WindowBorder(frame, true);

    public virtual string getName() => "Long Tree Browser";

    public virtual bool isOpen() => true;

    public virtual bool isReplaceable() => true;

    public virtual bool isSubView() => false;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      FullTreeBrowserSpecification browserSpecification = this;
      ObjectImpl.clone((object) browserSpecification);
      return ((object) browserSpecification).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
