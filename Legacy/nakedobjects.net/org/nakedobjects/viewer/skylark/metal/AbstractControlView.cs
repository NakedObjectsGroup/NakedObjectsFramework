// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.AbstractControlView
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.viewer.skylark.@event;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.metal
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/View;")]
  public abstract class AbstractControlView : View
  {
    [JavaFlags(20)]
    public readonly UserAction action;
    private readonly View parent;
    private int width;
    private int x;
    private int y;
    private int height;

    public AbstractControlView(UserAction action, View target)
    {
      this.action = action;
      this.parent = target;
    }

    public virtual void addView(View view)
    {
    }

    public virtual bool canChangeValue() => false;

    public virtual bool canFocus() => this.action.disabled(this.parent).isAllowed();

    public virtual bool contains(View view) => false;

    public virtual bool containsFocus() => false;

    public virtual void contentMenuOptions(UserActionSet menuOptions)
    {
    }

    public virtual string debugDetails() => (string) null;

    public virtual void dispose()
    {
    }

    public virtual void drag(InternalDrag drag)
    {
    }

    public virtual void dragCancel(InternalDrag drag)
    {
    }

    public virtual View dragFrom(Location location) => (View) null;

    public virtual void dragIn(ContentDrag drag)
    {
    }

    public virtual void dragOut(ContentDrag drag)
    {
    }

    public virtual Drag dragStart(DragStart drag) => (Drag) null;

    public virtual void dragTo(InternalDrag drag)
    {
    }

    public virtual void draw(Canvas canvas)
    {
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
      View parent = this.getParent();
      Consent consent = this.action.disabled(parent);
      if (consent.isVetoed())
        this.getViewManager().setStatus(new StringBuffer().append(this.action.getName(parent)).append(" - ").append(consent.getReason()).ToString());
      else
        this.getViewManager().setStatus(new StringBuffer().append(this.action.getName(parent)).append(" - ").append(this.action.getDescription(parent)).ToString());
    }

    public virtual void enteredSubview()
    {
    }

    public virtual void exited() => this.getViewManager().clearStatus();

    public virtual void exitedSubview()
    {
    }

    public virtual void firstClick(Click click) => this.executeAction();

    private void executeAction()
    {
      View view = this.getParent().getView();
      if (!this.action.disabled(view).isAllowed())
        return;
      this.markDamaged();
      this.getViewManager().saveCurrentFieldEntry();
      this.action.execute(view.getWorkspace(), view, this.getLocation());
    }

    public virtual void focusLost()
    {
    }

    public virtual void focusReceived()
    {
    }

    public virtual Location getAbsoluteLocation()
    {
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

    public virtual Content getContent() => (Content) null;

    public virtual int getId() => 0;

    public virtual FocusManager getFocusManager() => this.getParent() == null ? (FocusManager) null : this.getParent().getFocusManager();

    public virtual Location getLocation() => new Location(this.x, this.y);

    public virtual Padding getPadding() => (Padding) null;

    public virtual View getParent() => this.parent;

    public virtual Size getRequiredSize(Size maximumSize) => this.getMaximumSize();

    public virtual Size getSize() => new Size(this.width, this.height);

    public virtual ViewSpecification getSpecification() => (ViewSpecification) null;

    public virtual ViewState getState() => (ViewState) null;

    public virtual View[] getSubviews()
    {
      int length = 0;
      return length >= 0 ? new View[length] : throw new NegativeArraySizeException();
    }

    public virtual View getView() => (View) this;

    public virtual ViewAxis getViewAxis() => (ViewAxis) null;

    public virtual Viewer getViewManager() => Viewer.getInstance();

    public virtual Workspace getWorkspace() => (Workspace) null;

    public virtual bool hasFocus() => this.getViewManager().hasFocus(this.getView());

    public virtual View identify(Location location) => (View) this;

    public virtual void invalidateContent()
    {
    }

    public virtual void invalidateLayout()
    {
    }

    public virtual void keyPressed(KeyboardAction key)
    {
      if (key.getKeyCode() != 10)
        return;
      this.executeAction();
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

    public virtual void limitBoundsWithin(Bounds bounds)
    {
    }

    public virtual void limitBoundsWithin(Size size)
    {
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
      View view = this.getParent().getView();
      if (!this.action.disabled(view).isAllowed())
        return;
      this.markDamaged();
      this.getViewManager().saveCurrentFieldEntry();
      this.action.execute(view.getWorkspace(), view, this.getLocation());
    }

    public virtual void mouseMoved(Location location)
    {
    }

    public virtual void mouseWheelMoved(MouseWheelEvent evt)
    {
    }

    public virtual void mouseUp(Click click)
    {
    }

    public virtual void objectActionResult(Naked result, Location at)
    {
    }

    public virtual View pickupContent(Location location) => (View) null;

    public virtual View pickupView(Location location) => (View) null;

    public virtual void print(Canvas canvas)
    {
    }

    public virtual void refresh()
    {
    }

    public virtual void removeView(View view)
    {
    }

    public virtual void replaceView(View toReplace, View replacement)
    {
    }

    public virtual void secondClick(Click click)
    {
    }

    public virtual void setBounds(Bounds bounds)
    {
    }

    public virtual void setFocusManager(FocusManager focusManager)
    {
    }

    public virtual void setLocation(Location point)
    {
      this.x = point.getX();
      this.y = point.getY();
    }

    public virtual void setParent(View view)
    {
    }

    public virtual void setMaximumSize(Size size)
    {
    }

    public virtual void setSize(Size size)
    {
      this.width = size.getWidth();
      this.height = size.getHeight();
    }

    public virtual void setView(View view)
    {
    }

    public virtual View subviewFor(Location location) => (View) null;

    public virtual void thirdClick(Click click)
    {
    }

    public virtual void update(Naked @object)
    {
    }

    public virtual void updateView()
    {
    }

    public virtual ViewAreaType viewAreaType(Location mouseLocation) => (ViewAreaType) null;

    public virtual void viewMenuOptions(UserActionSet menuOptions)
    {
    }

    public virtual void minimize()
    {
    }

    public virtual void restore()
    {
    }

    public virtual View getActiveSubview() => (View) null;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(4227073)]
    public virtual object Clone()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AbstractControlView abstractControlView = this;
      ObjectImpl.clone((object) abstractControlView);
      return ((object) abstractControlView).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    public abstract Size getMaximumSize();
  }
}
