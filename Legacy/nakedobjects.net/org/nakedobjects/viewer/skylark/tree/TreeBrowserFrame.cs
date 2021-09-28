// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.tree.TreeBrowserFrame
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.util;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.special;
using org.nakedobjects.viewer.skylark.table;

namespace org.nakedobjects.viewer.skylark.tree
{
  [JavaFlags(32)]
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/ViewAxis;")]
  public class TreeBrowserFrame : AbstractView, ViewAxis
  {
    private ViewSpecification mainViewFormSpec;
    private ViewSpecification mainViewListSpec;
    private ViewSpecification mainViewTableSpec;
    private bool invalidLayout;
    private int layoutCount;
    private View left;
    private View right;
    private View selectedNode;
    private FocusManager focusManager;
    private bool nodesFocused;

    [JavaFlags(4)]
    public TreeBrowserFrame(Content content, ViewSpecification specification)
      : base(content, specification, (ViewAxis) null)
    {
      this.invalidLayout = true;
      this.layoutCount = 0;
      this.nodesFocused = false;
      this.mainViewFormSpec = (ViewSpecification) new TreeBrowserFormSpecification();
      this.mainViewTableSpec = (ViewSpecification) new InternalTableSpecification();
      this.mainViewListSpec = (ViewSpecification) new SimpleListSpecification();
    }

    public override string debugDetails()
    {
      StringBuffer stringBuffer = new StringBuffer();
      stringBuffer.append(base.debugDetails());
      stringBuffer.append(new StringBuffer().append("\nLaid out:  ").append(!this.invalidLayout ? "yes" : "no").append(", ").append(this.layoutCount).append(" layouts").ToString());
      stringBuffer.append("\nBrowser:   ");
      stringBuffer.append((object) this);
      stringBuffer.append("\n           left: ");
      stringBuffer.append((object) this.left.getBounds());
      stringBuffer.append(" ");
      stringBuffer.append((object) this.left);
      stringBuffer.append(": ");
      stringBuffer.append((object) this.left.getContent());
      stringBuffer.append("\n           right: ");
      if (this.right == null)
      {
        stringBuffer.append("nothing");
      }
      else
      {
        stringBuffer.append((object) this.right.getBounds());
        stringBuffer.append(" ");
        stringBuffer.append((object) this.right);
        stringBuffer.append(": ");
        stringBuffer.append((object) this.right.getContent());
      }
      stringBuffer.append("\n\n");
      return stringBuffer.ToString();
    }

    public override void dispose()
    {
      this.left.dispose();
      if (this.right != null)
        this.right.dispose();
      base.dispose();
    }

    public override void invalidateContent()
    {
      if (this.left != null)
        this.left.invalidateContent();
      if (this.right != null)
        this.right.invalidateContent();
      base.invalidateContent();
    }

    public override void draw(Canvas canvas)
    {
      Bounds bounds = this.left.getBounds();
      Canvas subcanvas1 = canvas.createSubcanvas(bounds);
      Color color1 = Style.background(this.getSpecification(), "left");
      if (color1 != Style.WINDOW_BACKGROUND)
        subcanvas1.clearBackground(this.left, color1);
      this.left.draw(subcanvas1);
      if (this.right != null)
      {
        bounds = this.right.getBounds();
        Canvas subcanvas2 = canvas.createSubcanvas(bounds);
        Color color2 = Style.background(this.getSpecification(), "right");
        if (color2 != Style.WINDOW_BACKGROUND)
          subcanvas2.clearBackground(this.right, color2);
        this.right.draw(subcanvas2);
      }
      canvas.drawLine(0, bounds.getHeight() - 1, this.getSize().getWidth(), bounds.getHeight() - 1, Style.SECONDARY1);
    }

    public override FocusManager getFocusManager() => this.focusManager;

    public override Size getMaximumSize()
    {
      Size size1 = this.left != null ? this.left.getMaximumSize() : new Size();
      Size size2 = this.right != null ? this.right.getMaximumSize() : new Size();
      Size size3 = new Size(size1);
      size3.extendWidth(size2.getWidth());
      size3.ensureHeight(size2.getHeight());
      return size3;
    }

    public override bool containsFocus() => this.nodesFocused || this.selectedNode.containsFocus() || base.containsFocus();

    public virtual void setNodesFocused(bool focused) => this.nodesFocused = focused;

    public override Size getRequiredSize(Size maximumSize)
    {
      Size requiredSize = this.left.getRequiredSize(new Size(maximumSize));
      Size size1 = this.right != null ? this.right.getRequiredSize(new Size(maximumSize)) : new Size();
      if (requiredSize.getWidth() + size1.getWidth() > maximumSize.getWidth())
      {
        int width1 = maximumSize.getWidth();
        int width2 = requiredSize.getWidth();
        int width3 = size1.getWidth();
        int num = width2 + width3;
        requiredSize = this.left.getRequiredSize(new Size(Utilities.doubleToInt(1.0 * (double) width2 / (double) num * (double) width1), maximumSize.getHeight()));
        size1 = this.right.getRequiredSize(new Size(width1 - requiredSize.getWidth(), maximumSize.getHeight()));
      }
      Size size2 = new Size(requiredSize);
      size2.extendWidth(size1.getWidth());
      size2.ensureHeight(size1.getHeight());
      return size2;
    }

    public virtual View getSelectedNode() => this.selectedNode;

    public override View[] getSubviews()
    {
      if (this.right == null)
      {
        int length = 1;
        View[] viewArray = length >= 0 ? new View[length] : throw new NegativeArraySizeException();
        viewArray[0] = this.left;
        return viewArray;
      }
      int length1 = 2;
      View[] viewArray1 = length1 >= 0 ? new View[length1] : throw new NegativeArraySizeException();
      viewArray1[0] = this.left;
      viewArray1[1] = this.right;
      return viewArray1;
    }

    public override void invalidateLayout()
    {
      base.invalidateLayout();
      this.invalidLayout = true;
    }

    public override void layout(Size maximumSize)
    {
      if (!this.invalidLayout)
        return;
      maximumSize.contract(this.getView().getPadding());
      Size requiredSize = this.left.getRequiredSize(new Size(maximumSize));
      Size size1 = this.right != null ? this.right.getRequiredSize(new Size(maximumSize)) : new Size();
      if (requiredSize.getWidth() + size1.getWidth() > maximumSize.getWidth())
      {
        int width1 = maximumSize.getWidth();
        int width2 = requiredSize.getWidth();
        int width3 = size1.getWidth();
        int num = width2 + width3;
        requiredSize = this.left.getRequiredSize(new Size(Utilities.doubleToInt(1.0 * (double) width2 / (double) num * (double) width1), maximumSize.getHeight()));
        size1 = this.right.getRequiredSize(new Size(width1 - requiredSize.getWidth(), maximumSize.getHeight()));
      }
      Size size2 = new Size(requiredSize);
      size2.extendWidth(size1.getWidth());
      size2.ensureHeight(size1.getHeight());
      size2.setHeight(maximumSize.getHeight());
      this.left.setSize(new Size(requiredSize.getWidth(), size2.getHeight()));
      this.left.layout(new Size(new Size(requiredSize)));
      if (this.right != null)
      {
        this.right.setLocation(new Location(this.left.getSize().getWidth(), 0));
        size1.setHeight(size2.getHeight());
        this.right.setSize(size1);
        this.right.layout(size1);
      }
      ++this.layoutCount;
      this.invalidLayout = false;
    }

    public override View subviewFor(Location location)
    {
      Location location1 = new Location(location);
      Padding padding = this.getPadding();
      location1.subtract(padding.getLeft(), padding.getTop());
      if (this.left.getBounds().contains(location))
        return this.left;
      return this.right != null && this.right.getBounds().contains(location) ? this.right : (View) null;
    }

    public override void replaceView(View toReplace, View replacement)
    {
      if (toReplace != this.left)
        throw new NakedObjectRuntimeException();
      this.initLeftPane(replacement);
      this.invalidateLayout();
    }

    public override void removeView(View view)
    {
      if (view == this.left)
      {
        this.left = (View) null;
      }
      else
      {
        if (view != this.right)
          return;
        this.right = (View) null;
      }
    }

    [JavaFlags(0)]
    public virtual void initLeftPane(View view)
    {
      this.left = (View) new TreeBrowserResizeBorder((View) new ScrollBorder(view));
      this.left.setParent(this.getView());
    }

    [JavaFlags(4)]
    public virtual void showInRightPane(View view)
    {
      this.right = view;
      this.right.setParent(this.getView());
      this.invalidateLayout();
    }

    public override void setFocusManager(FocusManager focusManager) => this.focusManager = focusManager;

    public virtual void setSelectedNode(View view)
    {
      Content content = view.getContent();
      Naked naked = content.getNaked();
      NakedObjectSpecification specification = naked.getSpecification();
      if (specification.isObject())
      {
        if (naked == null || !this.mainViewFormSpec.canDisplay(content))
          return;
        this.selectedNode = view;
        this.showInRightPane(this.mainViewFormSpec.createView(content, (ViewAxis) null));
      }
      else
      {
        if (!specification.isCollection() || ((NakedCollection) naked).size() <= 0)
          return;
        if (this.mainViewTableSpec.canDisplay(content))
        {
          this.selectedNode = view;
          this.showInRightPane(this.mainViewTableSpec.createView(content, (ViewAxis) null));
        }
        else
        {
          if (!this.mainViewListSpec.canDisplay(content))
            return;
          this.selectedNode = view;
          this.showInRightPane(this.mainViewListSpec.createView(content, (ViewAxis) null));
        }
      }
    }

    public override string ToString() => new StringBuffer().append(nameof (TreeBrowserFrame)).append(this.getId()).ToString();
  }
}
