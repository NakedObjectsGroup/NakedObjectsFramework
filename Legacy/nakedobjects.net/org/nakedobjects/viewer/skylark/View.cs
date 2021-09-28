// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.View
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.skylark.@event;
using System;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark
{
  [JavaInterfaces("1;System/ICloneable;")]
  [JavaInterface]
  public interface View : ICloneable
  {
    static readonly int HPADDING;
    static readonly int VPADDING;

    void addView(View view);

    bool canChangeValue();

    bool canFocus();

    bool contains(View view);

    bool containsFocus();

    void contentMenuOptions(UserActionSet menuOptions);

    string debugDetails();

    void dispose();

    void drag(InternalDrag drag);

    void dragCancel(InternalDrag drag);

    View dragFrom(Location location);

    void dragIn(ContentDrag drag);

    void dragOut(ContentDrag drag);

    Drag dragStart(DragStart drag);

    void dragTo(InternalDrag drag);

    void draw(Canvas canvas);

    void drop(ContentDrag drag);

    void drop(ViewDrag drag);

    void editComplete();

    void entered();

    void exited();

    void firstClick(Click click);

    void focusLost();

    void focusReceived();

    Location getAbsoluteLocation();

    int getBaseline();

    Bounds getBounds();

    Content getContent();

    FocusManager getFocusManager();

    int getId();

    Location getLocation();

    Size getMaximumSize();

    Padding getPadding();

    View getParent();

    Size getRequiredSize(Size maximumSize);

    Size getSize();

    ViewSpecification getSpecification();

    ViewState getState();

    View[] getSubviews();

    View getView();

    ViewAxis getViewAxis();

    Viewer getViewManager();

    Workspace getWorkspace();

    bool hasFocus();

    View identify(Location mouseLocation);

    void invalidateContent();

    void invalidateLayout();

    void keyPressed(KeyboardAction key);

    void keyReleased(int keyCode, int modifiers);

    void keyTyped(char keyCode);

    void layout(Size maximumSize);

    void limitBoundsWithin(Size size);

    void markDamaged();

    void markDamaged(Bounds bounds);

    void mouseDown(Click click);

    void mouseMoved(Location location);

    void mouseUp(Click click);

    void mouseWheelMoved(MouseWheelEvent evt);

    void objectActionResult(Naked result, Location at);

    View pickupContent(Location location);

    View pickupView(Location location);

    void print(Canvas canvas);

    void refresh();

    void removeView(View view);

    void replaceView(View toReplace, View replacement);

    void secondClick(Click click);

    void setBounds(Bounds bounds);

    void setFocusManager(FocusManager focusManager);

    void setLocation(Location point);

    void setParent(View view);

    void setMaximumSize(Size size);

    void setSize(Size size);

    void setView(View view);

    View subviewFor(Location location);

    void thirdClick(Click click);

    void update(Naked @object);

    void updateView();

    ViewAreaType viewAreaType(Location mouseLocation);

    void viewMenuOptions(UserActionSet menuOptions);

    void minimize();

    void restore();

    View getActiveSubview();

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static View()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
