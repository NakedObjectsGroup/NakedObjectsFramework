// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.tree.TreeNodeBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.skylark.basic;
using org.nakedobjects.viewer.skylark.core;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.tree
{
  public class TreeNodeBorder : AbstractBorder
  {
    private const int BORDER = 13;
    private const int BOX_PADDING = 2;
    private const int BOX_SIZE = 13;
    private const int BOX_X_OFFSET = 5;
    private static readonly Text LABEL_STYLE;
    private static readonly Logger LOG;
    private int baseline;
    private IconGraphic icon;
    private ViewSpecification replaceWithSpecification;
    private TitleText text;
    private bool isFocused;

    public TreeNodeBorder(View wrappedView, ViewSpecification replaceWith)
      : base(wrappedView)
    {
      this.isFocused = false;
      this.replaceWithSpecification = replaceWith;
      this.icon = new IconGraphic((View) this, TreeNodeBorder.LABEL_STYLE);
      this.text = (TitleText) new ObjectTitleText((View) this, TreeNodeBorder.LABEL_STYLE);
      int height = this.icon.getSize().getHeight();
      this.baseline = this.icon.getBaseline() + 1;
      this.left = 22;
      this.right = 13;
      this.top = height + 2;
      this.bottom = 0;
    }

    private int canOpen() => ((NodeSpecification) this.getSpecification()).canOpen(this.getContent());

    [JavaFlags(4)]
    public override Bounds contentArea() => new Bounds(this.getLeft(), this.getTop(), this.wrappedView.getSize().getWidth(), this.wrappedView.getSize().getHeight());

    public override void debugDetails(StringBuffer b)
    {
      b.append(new StringBuffer().append("TreeNodeBorder ").append(this.left).append(" pixels\n").ToString());
      b.append(new StringBuffer().append("           titlebar ").append(this.top).append(" pixels\n").ToString());
      b.append(new StringBuffer().append("           replace with  ").append((object) this.replaceWithSpecification).ToString());
      b.append(new StringBuffer().append("           text ").append((object) this.text).ToString());
      b.append(new StringBuffer().append("           icon ").append((object) this.icon).ToString());
      base.debugDetails(b);
    }

    public override Drag dragStart(DragStart drag)
    {
      if (drag.getLocation().getX() > this.getSize().getWidth() - this.right)
      {
        View dragView = (View) new DragViewOutline(this.getView());
        return (Drag) new ViewDrag((View) this, new Offset(drag.getLocation()), dragView);
      }
      if (!this.overBorder(drag.getLocation()))
        return base.dragStart(drag);
      View dragView1 = (View) new DragContentIcon(this.getContent());
      return (Drag) new ContentDrag((View) this, drag.getLocation(), dragView1);
    }

    public override void draw(Canvas canvas)
    {
      Color color = Style.SECONDARY2;
      if (((TreeBrowserFrame) this.getViewAxis()).getSelectedNode() == this.getView())
      {
        canvas.drawSolidRectangle(this.left, 0, this.getSize().getWidth() - this.left, this.top, Style.PRIMARY2);
        color = Style.SECONDARY1;
      }
      if (this.getState().isObjectIdentified())
      {
        canvas.drawRectangle(this.left, 0, this.getSize().getWidth() - this.left, this.top, color);
        int width = this.getSize().getWidth();
        canvas.drawSolidRectangle(width - 13 + 1, 1, 11, this.top - 2, Style.SECONDARY3);
        canvas.drawLine(width - 13, 0, width - 13, this.top - 2, color);
      }
      int x1 = 0;
      int num1 = this.top / 2;
      canvas.drawLine(x1, num1, x1 + this.left, num1, Style.SECONDARY2);
      bool flag = this.getSpecification().isOpen();
      int num2 = this.canOpen();
      if (flag || num2 != 2)
      {
        int x2 = x1 + 5;
        canvas.drawLine(x2, num1, x2 + 13 - 1, num1, Style.SECONDARY3);
        canvas.drawSolidRectangle(x2, num1 - 6, 13, 13, Style.WHITE);
        canvas.drawRectangle(x2, num1 - 6, 13, 13, Style.SECONDARY1);
        if (num2 != 0)
        {
          canvas.drawLine(x2 + 2, num1, x2 + 13 - 1 - 2, num1, Style.BLACK);
          if (!flag)
          {
            int num3 = x2 + 6;
            canvas.drawLine(num3, num1 - 6 + 2, num3, num1 + 6 - 2, Style.BLACK);
          }
        }
      }
      View[] subviews = this.getSubviews();
      if (subviews.Length > 0)
      {
        int y = this.top / 2;
        int y2 = this.top + subviews[subviews.Length - 1].getLocation().getY() + this.top / 2;
        canvas.drawLine(this.left - 1, y, this.left - 1, y2, Style.SECONDARY2);
      }
      int x3 = this.left + 1;
      this.icon.draw(canvas, x3, this.baseline);
      int x4 = x3 + this.icon.getSize().getWidth();
      this.text.draw(canvas, x4, this.baseline);
      if (AbstractView.debug)
        canvas.drawRectangleAround((View) this, Color.DEBUG_BASELINE);
      base.draw(canvas);
    }

    public override void entered()
    {
      this.getState().setObjectIdentified();
      this.getState().setViewIdentified();
      this.wrappedView.entered();
      this.markDamaged();
    }

    public override void exited()
    {
      this.getState().clearObjectIdentified();
      this.getState().clearViewIdentified();
      this.wrappedView.exited();
      this.markDamaged();
    }

    public override void firstClick(Click click)
    {
      int x = click.getLocation().getX();
      int y = click.getLocation().getY();
      if (this.withinBox(x, y))
      {
        if (this.canOpen() == 0)
        {
          this.resolveContent();
          this.markDamaged();
        }
        if (TreeNodeBorder.LOG.isDebugEnabled())
          TreeNodeBorder.LOG.debug((object) new StringBuffer().append(!this.getSpecification().isOpen() ? "open" : "close").append(" node ").append((object) this.getContent().getNaked()).ToString());
        if (this.canOpen() != 1)
          return;
        View view = this.replaceWithSpecification.createView(this.getContent(), this.getViewAxis());
        this.getParent().replaceView(this.getView(), view);
      }
      else if (y < this.top && x > this.left)
        this.selectNode();
      else
        base.firstClick(click);
    }

    private bool isSubviewIdentified(View v) => true;

    public override int getBaseline() => this.wrappedView.getBaseline() + this.baseline;

    public override Size getRequiredSize(Size maximumSize)
    {
      Size requiredSize = base.getRequiredSize(maximumSize);
      requiredSize.ensureWidth(this.left + View.HPADDING + this.icon.getSize().getWidth() + this.text.getSize().getWidth() + View.HPADDING + this.right);
      return requiredSize;
    }

    public override void objectActionResult(Naked result, Location at)
    {
      if (this.getContent() is OneToManyField)
      {
        OneToManyAssociation toManyAssociation = ((OneToManyField) this.getContent()).getOneToManyAssociation();
        NakedObject nakedObject = ((ObjectContent) this.getParent().getContent()).getObject();
        if (nakedObject.canAdd(toManyAssociation, (NakedObject) result).isAllowed())
          nakedObject.setAssociation((NakedObjectField) toManyAssociation, (NakedObject) result);
      }
      base.objectActionResult(result, at);
    }

    private void resolveContent()
    {
      Naked naked = this.getParent().getContent().getNaked();
      if (!(naked is NakedObject))
        naked = this.getParent().getParent().getContent().getNaked();
      if (this.getContent() is FieldContent)
      {
        NakedObjectField field = ((FieldContent) this.getContent()).getField();
        NakedObjects.getObjectPersistor().resolveField((NakedObject) naked, field);
      }
      else if (this.getContent() is CollectionContent)
      {
        NakedObjects.getObjectPersistor().resolveImmediately((NakedObject) naked);
      }
      else
      {
        if (!(this.getContent() is CollectionElement))
          return;
        NakedObjects.getObjectPersistor().resolveImmediately((NakedObject) this.getContent().getNaked());
      }
    }

    public override void secondClick(Click click)
    {
      int x = click.getLocation().getX();
      if (click.getLocation().getY() < this.top && x > this.left)
      {
        Location absoluteLocation = this.getAbsoluteLocation();
        absoluteLocation.translate(click.getLocation());
        View window = Skylark.getViewFactory().createWindow(this.getContent());
        window.setLocation(absoluteLocation);
        this.getWorkspace().addView(window);
      }
      else
        base.secondClick(click);
    }

    private void selectNode() => this.selectNode(this.getView());

    private void selectNode(View node)
    {
      TreeBrowserFrame viewAxis = (TreeBrowserFrame) this.getViewAxis();
      if (viewAxis.getSelectedNode() != node)
      {
        if (TreeNodeBorder.LOG.isDebugEnabled())
          TreeNodeBorder.LOG.debug((object) new StringBuffer().append("node selected ").append((object) this.getContent().getNaked()).ToString());
        viewAxis.setSelectedNode(node);
      }
      this.invalidateLayout();
    }

    public override void focusLost() => ((TreeBrowserFrame) this.getViewAxis()).setNodesFocused(false);

    public override void focusReceived() => ((TreeBrowserFrame) this.getViewAxis()).setNodesFocused(true);

    public override string ToString() => new StringBuffer().append(this.wrappedView.ToString()).append("/TreeNodeBorder").ToString();

    public override ViewAreaType viewAreaType(Location mouseLocation) => new Bounds(this.left + 1, 0, this.getSize().getWidth() - this.left - 13, this.top).contains(mouseLocation) ? ViewAreaType.CONTENT : base.viewAreaType(mouseLocation);

    public override void viewMenuOptions(UserActionSet options)
    {
      base.viewMenuOptions(options);
      TreeDisplayRules.menuOptions(options);
      options.add((UserAction) new TreeNodeBorder.\u0031(this, "Select node"));
      Naked naked = this.getView().getContent().getNaked();
      ResolveState resolveState = ((NakedReference) naked).getResolveState();
      if (!(naked is NakedReference) || !resolveState.isGhost() && !resolveState.isPartlyResolved())
        return;
      options.add((UserAction) new TreeNodeBorder.\u0032(this, "Load object"));
    }

    private bool withinBox(int x, int y) => x >= 5 && x <= 18 && y >= (this.top - 13) / 2 && y <= (this.top + 13) / 2;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static TreeNodeBorder()
    {
      // ISSUE: unable to decompile the method.
    }

    [Inner]
    [JavaFlags(32)]
    public class \u0031 : AbstractUserAction
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private TreeNodeBorder this\u00240;

      public override void execute(Workspace workspace, View view, Location at) => this.this\u00240.selectNode();

      public override string getDescription(View view) => "Show this node in the right-hand pane";

      public \u0031(TreeNodeBorder _param1, string dummy0)
        : base(dummy0)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }

    [Inner]
    [JavaFlags(32)]
    public class \u0032 : AbstractUserAction
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private TreeNodeBorder this\u00240;

      public override void execute(Workspace workspace, View view, Location at) => this.this\u00240.resolveContent();

      public \u0032(TreeNodeBorder _param1, string dummy0)
        : base(dummy0)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }
  }
}
