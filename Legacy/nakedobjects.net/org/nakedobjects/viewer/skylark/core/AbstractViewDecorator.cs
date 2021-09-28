// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.AbstractViewDecorator
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.skylark.@event;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.core
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/View;")]
  public abstract class AbstractViewDecorator : View
  {
    [JavaFlags(4)]
    public View wrappedView;

    [JavaFlags(4)]
    public AbstractViewDecorator(View wrappedView)
    {
      this.wrappedView = wrappedView;
      wrappedView.setView((View) this);
    }

    public virtual void addView(View view) => this.wrappedView.addView(view);

    public virtual bool canChangeValue() => this.wrappedView.canChangeValue();

    public virtual bool canFocus() => this.wrappedView.canFocus();

    public virtual bool contains(View view) => this.wrappedView.contains(view);

    public virtual bool containsFocus() => this.wrappedView.containsFocus();

    public virtual void contentMenuOptions(UserActionSet menuOptions) => this.wrappedView.contentMenuOptions(menuOptions);

    [JavaFlags(17)]
    public virtual string debugDetails()
    {
      StringBuffer b = new StringBuffer();
      b.append("Decorator: ");
      this.debugDetails(b);
      b.append(new StringBuffer().append("\n           set size ").append((object) this.getSize()).append("\n").ToString());
      b.append(new StringBuffer().append("           maximum ").append((object) this.getMaximumSize()).append("\n").ToString());
      b.append(new StringBuffer().append("           required ").append((object) this.getRequiredSize(Size.ALL)).append("\n").ToString());
      b.append(new StringBuffer().append("           padding ").append((object) this.getPadding()).append("\n").ToString());
      b.append(new StringBuffer().append("           baseline ").append(this.getBaseline()).append("\n").ToString());
      b.append("\n");
      b.append(this.wrappedView.debugDetails());
      return b.ToString();
    }

    [JavaFlags(4)]
    public virtual void debugDetails(StringBuffer b)
    {
      string name = ObjectImpl.getClass((object) this).getName();
      b.append(StringImpl.substring(name, StringImpl.lastIndexOf(name, 46) + 1));
    }

    public virtual void dispose() => this.wrappedView.dispose();

    public virtual void drag(InternalDrag drag) => this.wrappedView.drag(drag);

    public virtual void dragCancel(InternalDrag drag) => this.wrappedView.dragCancel(drag);

    public virtual View dragFrom(Location location) => this.wrappedView.dragFrom(location);

    public virtual void dragIn(ContentDrag drag) => this.wrappedView.dragIn(drag);

    public virtual void dragOut(ContentDrag drag) => this.wrappedView.dragOut(drag);

    public virtual Drag dragStart(DragStart drag) => this.wrappedView.dragStart(drag);

    public virtual void dragTo(InternalDrag drag) => this.wrappedView.dragTo(drag);

    public virtual void draw(Canvas canvas) => this.wrappedView.draw(canvas);

    public virtual void drop(ContentDrag drag) => this.wrappedView.drop(drag);

    public virtual void drop(ViewDrag drag) => this.wrappedView.drop(drag);

    public virtual void editComplete() => this.wrappedView.editComplete();

    public virtual void entered() => this.wrappedView.entered();

    public virtual void exited() => this.wrappedView.exited();

    public virtual void firstClick(Click click) => this.wrappedView.firstClick(click);

    public virtual void focusLost() => this.wrappedView.focusLost();

    public virtual void focusReceived() => this.wrappedView.focusReceived();

    public virtual Location getAbsoluteLocation() => this.wrappedView.getAbsoluteLocation();

    public virtual int getBaseline() => this.wrappedView.getBaseline();

    public virtual Bounds getBounds() => new Bounds(this.getLocation(), this.getSize());

    public virtual Content getContent() => this.wrappedView.getContent();

    public virtual FocusManager getFocusManager() => this.wrappedView.getFocusManager();

    public virtual int getId() => this.wrappedView.getId();

    public virtual Location getLocation() => this.wrappedView.getLocation();

    public virtual Padding getPadding() => this.wrappedView.getPadding();

    public virtual View getParent() => this.wrappedView.getParent();

    public virtual Size getRequiredSize(Size maximumSize) => this.wrappedView.getRequiredSize(maximumSize);

    public virtual Size getMaximumSize() => this.wrappedView.getMaximumSize();

    public virtual Size getSize() => this.wrappedView.getSize();

    public virtual ViewSpecification getSpecification() => this.wrappedView.getSpecification();

    public virtual ViewState getState() => this.wrappedView.getState();

    public virtual View[] getSubviews() => this.wrappedView.getSubviews();

    public virtual View getView() => this.wrappedView.getView();

    public virtual ViewAxis getViewAxis() => this.wrappedView.getViewAxis();

    public virtual Viewer getViewManager() => this.wrappedView.getViewManager();

    public virtual Workspace getWorkspace() => this.wrappedView.getWorkspace();

    public virtual bool hasFocus() => this.wrappedView.hasFocus();

    public virtual View identify(Location mouseLocation) => this.wrappedView.identify(mouseLocation);

    public virtual void invalidateContent() => this.wrappedView.invalidateContent();

    public virtual void invalidateLayout() => this.wrappedView.invalidateLayout();

    public virtual void keyPressed(KeyboardAction key) => this.wrappedView.keyPressed(key);

    public virtual void keyReleased(int keyCode, int modifiers) => this.wrappedView.keyReleased(keyCode, modifiers);

    public virtual void keyTyped(char keyCode) => this.wrappedView.keyTyped(keyCode);

    public virtual void layout(Size maximumSize) => this.wrappedView.layout(maximumSize);

    public virtual void limitBoundsWithin(Size size) => this.wrappedView.limitBoundsWithin(size);

    public virtual void markDamaged() => this.wrappedView.markDamaged();

    public virtual void markDamaged(Bounds bounds) => this.wrappedView.markDamaged(bounds);

    public virtual void mouseDown(Click click) => this.wrappedView.mouseDown(click);

    public virtual void mouseMoved(Location at) => this.wrappedView.mouseMoved(at);

    public virtual void mouseUp(Click click) => this.wrappedView.mouseUp(click);

    public virtual void mouseWheelMoved(MouseWheelEvent evt) => this.wrappedView.mouseWheelMoved(evt);

    public virtual void objectActionResult(Naked result, Location at) => this.wrappedView.objectActionResult(result, at);

    public virtual View pickupContent(Location location) => this.wrappedView.pickupContent(location);

    public virtual View pickupView(Location location) => this.wrappedView.pickupView(location);

    public virtual void print(Canvas canvas) => this.wrappedView.print(canvas);

    public virtual void refresh() => this.wrappedView.refresh();

    public virtual void removeView(View view) => this.wrappedView.removeView(view);

    public virtual void replaceView(View toReplace, View replacement) => this.wrappedView.replaceView(toReplace, replacement);

    public virtual void secondClick(Click click) => this.wrappedView.secondClick(click);

    public virtual void setBounds(Bounds bounds) => this.wrappedView.setBounds(bounds);

    public virtual void setFocusManager(FocusManager focusManager) => this.wrappedView.setFocusManager(focusManager);

    public virtual void setLocation(Location point) => this.wrappedView.setLocation(point);

    public virtual void setParent(View view) => this.wrappedView.setParent(view);

    public virtual void setMaximumSize(Size size) => this.wrappedView.setMaximumSize(size);

    public virtual void setSize(Size size) => this.wrappedView.setSize(size);

    public virtual void setView(View view) => this.wrappedView.setView(view);

    public virtual View subviewFor(Location location) => this.wrappedView.subviewFor(location);

    public virtual void thirdClick(Click click) => this.wrappedView.thirdClick(click);

    public override string ToString()
    {
      string name = ObjectImpl.getClass((object) this).getName();
      return new StringBuffer().append((object) this.wrappedView).append("/").append(StringImpl.substring(name, StringImpl.lastIndexOf(name, 46) + 1)).ToString();
    }

    public virtual void update(Naked @object) => this.wrappedView.update(@object);

    public virtual void updateView() => this.wrappedView.updateView();

    public virtual ViewAreaType viewAreaType(Location mouseLocation) => this.wrappedView.viewAreaType(mouseLocation);

    public virtual void viewMenuOptions(UserActionSet menuOptions) => this.wrappedView.viewMenuOptions(menuOptions);

    public virtual void minimize()
    {
    }

    public virtual void restore()
    {
    }

    public virtual View getActiveSubview() => this.wrappedView.getActiveSubview();

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
      AbstractViewDecorator abstractViewDecorator = this;
      ObjectImpl.clone((object) abstractViewDecorator);
      return ((object) abstractViewDecorator).MemberwiseClone();
    }
  }
}
