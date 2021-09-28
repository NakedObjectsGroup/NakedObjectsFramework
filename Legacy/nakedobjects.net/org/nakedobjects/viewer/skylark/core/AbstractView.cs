// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.AbstractView
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.undo;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.@event;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace org.nakedobjects.viewer.skylark.core
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/View;")]
  public abstract class AbstractView : View
  {
    private static readonly UserAction CLOSE_ALL_OPTION;
    private static readonly UserAction CLOSE_OPTION;
    private static readonly UserAction CLOSE_VIEWS_FOR_OBJECT;
    public static bool debug;
    private static readonly org.apache.log4j.Logger LOG;
    private static int nextId;
    private Content content;
    private int height;
    private int id;
    private View parent;
    private ViewSpecification specification;
    private ViewState state;
    private View view;
    private ViewAxis viewAxis;
    private int width;
    private int x;
    private int y;

    public static string name(View view)
    {
      ViewSpecification specification = view.getSpecification();
      if (specification != null)
        return specification.getName();
      string name = ObjectImpl.getClass((object) view).getName();
      return StringImpl.substring(name, StringImpl.lastIndexOf(name, 46) + 1);
    }

    [JavaFlags(4)]
    public AbstractView()
      : this((Content) null, (ViewSpecification) null, (ViewAxis) null)
    {
    }

    [JavaFlags(4)]
    public AbstractView(Content content, ViewSpecification specification, ViewAxis axis)
    {
      this.id = 0;
      this.assignId();
      this.content = content;
      this.specification = specification;
      this.viewAxis = axis;
      this.state = new ViewState();
      this.view = (View) this;
    }

    public virtual void addView(View view) => throw new NakedObjectRuntimeException(new StringBuffer().append("Can't add views to ").append((object) this).ToString());

    [JavaFlags(4)]
    public virtual void assignId()
    {
      int nextId;
      AbstractView.nextId = (nextId = AbstractView.nextId) + 1;
      this.id = nextId;
    }

    public virtual bool canChangeValue() => false;

    public virtual bool canFocus() => true;

    public virtual bool contains(View view)
    {
      View[] subviews = this.getSubviews();
      for (int index = 0; index < subviews.Length; ++index)
      {
        if (subviews[index] == view || subviews[index].contains(view))
          return true;
      }
      return false;
    }

    public virtual bool containsFocus()
    {
      if (this.hasFocus())
        return true;
      foreach (View subview in this.getSubviews())
      {
        if (subview.containsFocus())
          return true;
      }
      return false;
    }

    public virtual void contentMenuOptions(UserActionSet options)
    {
      options.setColor(Style.CONTENT_MENU);
      this.getContent()?.contentMenuOptions(options);
    }

    public virtual string debugDetails()
    {
      DebugString debugString = new DebugString();
      string name = ObjectImpl.getClass((object) this).getName();
      debugString.append((object) new StringBuffer().append("Root:      ").append(StringImpl.substring(name, StringImpl.lastIndexOf(name, 46) + 1)).append(this.getId()).ToString());
      debugString.append((object) new StringBuffer().append("\n           set size ").append((object) this.getSize()).ToString());
      debugString.append((object) new StringBuffer().append("\n           maximum ").append((object) this.getMaximumSize()).ToString());
      debugString.append((object) new StringBuffer().append("\n           required ").append((object) this.getRequiredSize(new Size())).ToString());
      debugString.append((object) new StringBuffer().append("\n           w/in parent ").append((object) this.getRequiredSize(this.getParent() != null ? this.getParent().getSize() : new Size())).ToString());
      debugString.append((object) new StringBuffer().append("\n           (parent's ").append(this.getParent() != null ? (object) this.getParent().getSize() : (object) new Size()).append(")").ToString());
      debugString.append((object) new StringBuffer().append("\n           padding ").append((object) this.getPadding()).ToString());
      debugString.append((object) new StringBuffer().append("\n           baseline ").append(this.getBaseline()).ToString());
      debugString.appendln();
      debugString.appendln();
      debugString.appendTitle("Specification");
      if (this.specification == null)
      {
        debugString.append((object) "\nnone");
      }
      else
      {
        debugString.appendln(this.specification.getName());
        debugString.appendln(new StringBuffer().append("  ").append(ObjectImpl.getClass((object) this.specification).getName()).ToString());
        debugString.appendln(new StringBuffer().append("  ").append(!this.specification.isOpen() ? "closed" : "open").ToString());
        debugString.appendln(new StringBuffer().append("  ").append(!this.specification.isReplaceable() ? "non-replaceable" : "replaceable").ToString());
        debugString.appendln(new StringBuffer().append("  ").append(!this.specification.isSubView() ? "main view" : "subview").ToString());
      }
      debugString.append((object) "\n");
      debugString.appendTitle("View");
      debugString.append((object) new StringBuffer().append("Changable: ").append(this.canChangeValue()).ToString());
      debugString.append((object) "\n");
      debugString.append((object) new StringBuffer().append("\nFocus:     ").append(!this.canFocus() ? "non-focusable" : "focusable").ToString());
      debugString.append((object) new StringBuffer().append("\nHas focus:     ").append(this.hasFocus()).ToString());
      debugString.append((object) new StringBuffer().append("\nContains focus:     ").append(this.containsFocus()).ToString());
      debugString.append((object) new StringBuffer().append("\nFocus manager:     ").append((object) this.getFocusManager()).ToString());
      debugString.append((object) "\n");
      debugString.append((object) new StringBuffer().append("\nSelf:      ").append((object) this.getView()).ToString());
      debugString.append((object) new StringBuffer().append("\nAxis:      ").append((object) this.getViewAxis()).ToString());
      debugString.append((object) new StringBuffer().append("\nState:     ").append((object) this.getState()).ToString());
      debugString.append((object) new StringBuffer().append("\nLocation:  ").append((object) this.getLocation()).ToString());
      debugString.append((object) "\nParent:    ");
      View parent = this.getParent();
      string str = parent != null ? new StringBuffer().append("").append((object) parent).ToString() : "none";
      debugString.append((object) str);
      while (parent != null)
      {
        parent = parent.getParent();
        debugString.append((object) new StringBuffer().append("\n           ").append((object) parent).ToString());
      }
      debugString.append((object) new StringBuffer().append("\nWorkspace: ").append((object) this.getWorkspace()).ToString());
      debugString.append((object) "\n\n\n");
      return debugString.ToString();
    }

    public virtual void dispose()
    {
      if (this.parent != null)
        this.parent.removeView(this.getView());
      else
        this.getViewManager().clearOverlayView((View) this);
    }

    public virtual void drag(InternalDrag drag)
    {
    }

    public virtual void dragCancel(InternalDrag drag) => this.getViewManager().showArrowCursor();

    public virtual View dragFrom(Location location)
    {
      View view = this.subviewFor(location);
      if (view == null)
        return (View) null;
      location.subtract(view.getLocation());
      return view.dragFrom(location);
    }

    public virtual void dragIn(ContentDrag drag)
    {
    }

    public virtual void dragOut(ContentDrag drag)
    {
    }

    public virtual Drag dragStart(DragStart drag)
    {
      View view = this.subviewFor(drag.getLocation());
      if (view == null)
        return (Drag) null;
      drag.subtract(view.getLocation());
      return view.dragStart(drag);
    }

    public virtual void dragTo(InternalDrag drag)
    {
    }

    public virtual void draw(Canvas canvas)
    {
      if (!AbstractView.debug)
        return;
      canvas.drawDebugOutline(new Bounds(this.getSize()), this.getBaseline(), Color.DEBUG_VIEW_BOUNDS);
    }

    public virtual void drop(ContentDrag drag)
    {
    }

    public virtual void drop(ViewDrag drag)
    {
    }

    public virtual void editComplete()
    {
    }

    public virtual void entered()
    {
    }

    public virtual void exited()
    {
    }

    public virtual void firstClick(Click click)
    {
      View view = this.subviewFor(click.getLocation());
      if (view == null)
        return;
      click.subtract(view.getLocation());
      view.firstClick(click);
    }

    public virtual void focusLost() => this.getViewManager().clearStatus();

    public virtual void focusReceived() => this.getViewManager().clearStatus();

    public virtual Location getAbsoluteLocation()
    {
      if (this.parent == null)
        return this.getLocation();
      Location absoluteLocation = this.parent.getAbsoluteLocation();
      this.getViewManager().getSpy().addTrace((View) this, "parent location", (object) absoluteLocation);
      absoluteLocation.add(this.x, this.y);
      this.getViewManager().getSpy().addTrace((View) this, "plus view's location", (object) absoluteLocation);
      Padding padding = this.parent.getPadding();
      absoluteLocation.add(padding.getLeft(), padding.getTop());
      this.getViewManager().getSpy().addTrace((View) this, "plus view's padding", (object) absoluteLocation);
      return absoluteLocation;
    }

    public virtual int getBaseline() => 0;

    public virtual Bounds getBounds() => new Bounds(this.x, this.y, this.width, this.height);

    public virtual Content getContent() => this.content;

    public virtual FocusManager getFocusManager() => this.getParent() == null ? (FocusManager) null : this.getParent().getFocusManager();

    public virtual int getId() => this.id;

    public virtual Location getLocation() => new Location(this.x, this.y);

    public virtual Padding getPadding() => new Padding(0, 0, 0, 0);

    [JavaFlags(17)]
    public virtual View getParent()
    {
      Assert.assertEquals(this.parent != null ? (object) this.parent.getView() : (object) (View) null, (object) this.parent);
      return this.parent;
    }

    public virtual Size getRequiredSize(Size maximumSize) => this.getMaximumSize();

    public virtual Size getMaximumSize() => new Size();

    public virtual Size getSize() => new Size(this.width, this.height);

    public virtual ViewSpecification getSpecification()
    {
      if (this.specification == null)
        this.specification = (ViewSpecification) new NonBuildingSpecification((View) this);
      return this.specification;
    }

    public virtual ViewState getState() => this.state;

    public virtual View[] getSubviews()
    {
      int length = 0;
      return length >= 0 ? new View[length] : throw new NegativeArraySizeException();
    }

    [JavaFlags(17)]
    public virtual View getView() => this.view;

    [JavaFlags(17)]
    public virtual ViewAxis getViewAxis() => this.viewAxis;

    public virtual Viewer getViewManager() => Viewer.getInstance();

    public virtual Workspace getWorkspace() => this.getParent() == null ? (Workspace) null : this.getParent().getWorkspace();

    public virtual bool hasFocus() => this.getViewManager().hasFocus(this.getView());

    public virtual View identify(Location location)
    {
      View view = this.subviewFor(location);
      if (view == null)
      {
        this.getViewManager().getSpy().addTrace((View) this, "mouse location within node view", (object) location);
        this.getViewManager().getSpy().addTrace("----");
        return this.getView();
      }
      location.subtract(view.getLocation());
      return view.identify(location);
    }

    public virtual void invalidateContent()
    {
    }

    public virtual void invalidateLayout()
    {
      if (this.parent == null)
        return;
      this.parent.invalidateLayout();
    }

    public virtual void keyPressed(KeyboardAction key)
    {
    }

    public virtual void keyReleased(int keyCode, int modifiers)
    {
    }

    public virtual void keyTyped(char keyCode)
    {
    }

    public virtual void layout(Size maximumSize)
    {
    }

    public virtual void limitBoundsWithin(Bounds containerBounds)
    {
      Bounds bounds = this.getView().getBounds();
      if (!containerBounds.limitBounds(bounds))
        return;
      this.getView().setBounds(bounds);
    }

    public virtual void limitBoundsWithin(Size size)
    {
      int width = this.getView().getSize().getWidth();
      int height = this.getView().getSize().getHeight();
      int x = this.getView().getLocation().getX();
      int y = this.getView().getLocation().getY();
      if (x + width > size.getWidth())
        x = size.getWidth() - width;
      if (x < 0)
        x = 0;
      if (y + height > size.getHeight())
        y = size.getHeight() - height;
      if (y < 0)
        y = 0;
      this.getView().setLocation(new Location(x, y));
    }

    public virtual void markDamaged() => this.markDamaged(this.getView().getBounds());

    public virtual void markDamaged(Bounds bounds)
    {
      if (this.parent == null)
      {
        this.getViewManager().markDamaged(bounds);
      }
      else
      {
        Location location = this.parent.getLocation();
        bounds.translate(location.getX(), location.getY());
        Padding padding = this.parent.getPadding();
        bounds.translate(padding.getLeft(), padding.getTop());
        this.parent.markDamaged(bounds);
      }
    }

    public virtual void mouseDown(Click click)
    {
      View view = this.subviewFor(click.getLocation());
      if (view == null)
        return;
      click.subtract(view.getLocation());
      view.mouseDown(click);
    }

    public virtual void mouseMoved(Location location)
    {
      View view = this.subviewFor(location);
      if (view == null)
        return;
      location.subtract(view.getLocation());
      view.mouseMoved(location);
    }

    public virtual void mouseUp(Click click)
    {
      View view = this.subviewFor(click.getLocation());
      if (view == null)
        return;
      click.subtract(view.getLocation());
      view.mouseUp(click);
    }

    public virtual void mouseWheelMoved(MouseWheelEvent evt)
    {
      if (this.parent == null)
        return;
      this.parent.mouseWheelMoved(evt);
    }

    public virtual void objectActionResult(Naked result, Location at) => this.getWorkspace().addOpenViewFor(result, at);

    public virtual View pickupContent(Location location)
    {
      View view = this.subviewFor(location);
      if (view == null)
        return (View) new DragViewOutline(this.getView());
      location.subtract(view.getLocation());
      return view.pickupView(location);
    }

    public virtual View pickupView(Location location)
    {
      View view = this.subviewFor(location);
      if (view == null)
        return (View) null;
      location.subtract(view.getLocation());
      return view.pickupView(location);
    }

    public virtual void print(Canvas canvas) => this.draw(canvas);

    public virtual void refresh()
    {
    }

    public virtual void removeView(View view) => throw new NakedObjectRuntimeException();

    [JavaFlags(4)]
    public virtual void replaceOptions(Enumeration possibleViews, UserActionSet options)
    {
      while (possibleViews.hasMoreElements())
      {
        ViewSpecification specification = (ViewSpecification) possibleViews.nextElement();
        if (specification != this.getSpecification() && ObjectImpl.getClass((object) this.view) != ObjectImpl.getClass((object) this))
        {
          AbstractUserAction abstractUserAction = (AbstractUserAction) new ReplaceViewOption(specification);
          options.add((UserAction) abstractUserAction);
        }
      }
    }

    public virtual void replaceView(View toReplace, View replacement) => throw new NakedObjectRuntimeException();

    public virtual void secondClick(Click click)
    {
      View view = this.subviewFor(click.getLocation());
      if (view == null)
        return;
      click.subtract(view.getLocation());
      view.secondClick(click);
    }

    public virtual void setBounds(Bounds bounds)
    {
      this.x = bounds.getX();
      this.y = bounds.getY();
      this.width = bounds.getWidth();
      this.height = bounds.getHeight();
    }

    public virtual void setFocusManager(FocusManager focusManager) => throw new UnexpectedCallException();

    [JavaFlags(4)]
    public virtual void setContent(Content content) => this.content = content;

    public virtual void setLocation(Location location)
    {
      this.x = location.getX();
      this.y = location.getY();
    }

    [JavaFlags(17)]
    public virtual void setParent(View view)
    {
      this.parent = view.getView();
      if (!AbstractView.LOG.isDebugEnabled())
        return;
      AbstractView.LOG.debug((object) new StringBuffer().append("set parent ").append((object) this.parent).append(" for ").append((object) this).ToString());
    }

    public virtual void setMaximumSize(Size size)
    {
    }

    public virtual void setSize(Size size)
    {
      this.width = size.getWidth();
      this.height = size.getHeight();
    }

    [JavaFlags(4)]
    public virtual void setSpecification(ViewSpecification specification) => this.specification = specification;

    [JavaFlags(17)]
    public virtual void setView(View view) => this.view = view;

    [JavaFlags(4)]
    public virtual void setViewAxis(ViewAxis viewAxis) => this.viewAxis = viewAxis;

    public virtual View subviewFor(Location location) => (View) null;

    public virtual void thirdClick(Click click)
    {
      View view = this.subviewFor(click.getLocation());
      if (view == null)
        return;
      click.subtract(view.getLocation());
      view.thirdClick(click);
    }

    public override string ToString()
    {
      string name = ObjectImpl.getClass((object) this).getName();
      return new StringBuffer().append(StringImpl.substring(name, StringImpl.lastIndexOf(name, 46) + 1)).append(this.getId()).append(":").append((object) this.getState()).append(":").append((object) this.getContent()).ToString();
    }

    public virtual void update(Naked @object)
    {
    }

    public virtual void updateView()
    {
    }

    public virtual ViewAreaType viewAreaType(Location location)
    {
      View view = this.subviewFor(location);
      if (view == null)
        return ViewAreaType.CONTENT;
      location.subtract(view.getLocation());
      return view.viewAreaType(location);
    }

    public virtual void viewMenuOptions(UserActionSet options)
    {
      options.setColor(Style.VIEW_MENU);
      Content content = this.getContent();
      content?.viewMenuOptions(options);
      if (this.getParent() != null)
      {
        Enumeration enumeration = Skylark.getViewFactory().openRootViews(content, (View) null);
        while (enumeration.hasMoreElements())
        {
          AbstractUserAction abstractUserAction = (AbstractUserAction) new OpenViewOption((ViewSpecification) enumeration.nextElement());
          options.add((UserAction) abstractUserAction);
        }
      }
      if (this.view.getSpecification() != null && this.view.getSpecification().isSubView())
      {
        if (this.view.getSpecification().isReplaceable())
        {
          this.replaceOptions(Skylark.getViewFactory().openSubviews(content, (View) this), options);
          this.replaceOptions(Skylark.getViewFactory().closedSubviews(content, (View) this), options);
        }
      }
      else
      {
        if (this.view.getSpecification() != null && this.view.getSpecification().isReplaceable())
          this.replaceOptions(Skylark.getViewFactory().openRootViews(content, (View) this), options);
        options.add((UserAction) new PrintOption());
      }
      options.add((UserAction) new AbstractView.\u0031(this, "Refresh view", UserAction.DEBUG));
      options.add((UserAction) new AbstractView.\u0032(this, "Invalidate content", UserAction.DEBUG));
      options.add((UserAction) new AbstractView.\u0033(this, "Invalidate layout", UserAction.DEBUG));
      UndoStack undoStack = this.getViewManager().getUndoStack();
      if (undoStack.isEmpty())
        return;
      options.add((UserAction) new AbstractView.\u0034(this, new StringBuffer().append("Undo ").append(undoStack.getNameOfUndo()).ToString(), undoStack));
    }

    public virtual void minimize()
    {
    }

    public virtual void restore()
    {
    }

    public virtual View getActiveSubview() => (View) null;

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static AbstractView()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227073)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual object Clone()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      AbstractView abstractView = this;
      ObjectImpl.clone((object) abstractView);
      return ((object) abstractView).MemberwiseClone();
    }

    [Inner]
    [JavaFlags(32)]
    public class \u0031 : AbstractUserAction
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private AbstractView this\u00240;

      public override void execute(Workspace workspace, View view, Location at) => this.this\u00240.refresh();

      public \u0031(AbstractView _param1, string dummy0, Action.Type dummy1)
        : base(dummy0, dummy1)
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
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private AbstractView this\u00240;

      public override void execute(Workspace workspace, View view, Location at) => this.this\u00240.invalidateContent();

      public \u0032(AbstractView _param1, string dummy0, Action.Type dummy1)
        : base(dummy0, dummy1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }

    [Inner]
    [JavaFlags(32)]
    public class \u0033 : AbstractUserAction
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private AbstractView this\u00240;

      public override void execute(Workspace workspace, View view, Location at) => this.this\u00240.invalidateLayout();

      public \u0033(AbstractView _param1, string dummy0, Action.Type dummy1)
        : base(dummy0, dummy1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }

    [JavaFlags(32)]
    [Inner]
    public class \u0034 : AbstractUserAction
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private AbstractView this\u00240;
      [JavaFlags(16)]
      public readonly UndoStack undoStack_\u003E;

      public override Consent disabled(View component) => (Consent) new Allow(this.undoStack_\u003E.descriptionOfUndo());

      public override void execute(Workspace workspace, View view, Location at) => this.undoStack_\u003E.undoLastCommand();

      public \u0034(AbstractView _param1, string dummy0, [In] UndoStack obj2)
        : base(dummy0)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.undoStack_\u003E = obj2;
      }
    }
  }
}
