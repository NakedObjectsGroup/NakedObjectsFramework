// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.InteractionHandler
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.awt.@event;
using java.lang;
using org.apache.log4j;
using org.nakedobjects.viewer.skylark.@event;
using org.nakedobjects.viewer.skylark.core;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.viewer.skylark
{
  [JavaInterfaces("4;java/awt/event/MouseMotionListener;java/awt/event/MouseListener;java/awt/event/KeyListener;org/nakedobjects/viewer/skylark/event/MouseWheelHandler;")]
  public class InteractionHandler : 
    MouseMotionListener,
    MouseListener,
    KeyListener,
    MouseWheelHandler
  {
    private static readonly Logger LOG;
    private const int THRESHOLD = 7;
    private bool canDrag;
    private Location downAt;
    private Drag drag;
    private readonly KeyboardManager keyboardManager;
    private View identifiedView;
    private InteractionSpy spy;
    private readonly Viewer viewer;
    private KeyEvent lastTyped;
    private View draggedView;
    private MouseWheelListener mouseWheelListener;

    [JavaFlags(0)]
    public InteractionHandler(Viewer viewer, KeyboardManager keyboardManager, InteractionSpy spy)
    {
      this.viewer = viewer;
      this.spy = spy;
      this.keyboardManager = keyboardManager;
    }

    private void drag(MouseEvent me)
    {
      Location location = new Location(me.getPoint());
      this.spy.addAction(new StringBuffer().append("Mouse dragged ").append((object) location).ToString());
      this.drag.drag(this.viewer, location, ((InputEvent) me).getModifiers());
    }

    private void dragStart(MouseEvent me)
    {
      if (!this.isOverThreshold(this.downAt, me.getPoint()))
        return;
      this.spy.addAction(new StringBuffer().append("Drag start  at ").append((object) this.downAt).ToString());
      this.drag = this.viewer.dragStart(new DragStart(this.downAt, ((InputEvent) me).getModifiers()));
      if (this.drag == null)
      {
        this.spy.addAction("drag start  ignored");
        this.canDrag = false;
      }
      else
      {
        this.spy.addAction(new StringBuffer().append("drag start ").append((object) this.drag).ToString());
        this.drag.start(this.viewer);
        View overlay = this.drag.getOverlay();
        if (overlay != null)
          this.viewer.setOverlayView(overlay);
        this.drag.drag(this.viewer, new Location(me.getPoint()), ((InputEvent) me).getModifiers());
      }
      this.identifiedView = (View) null;
    }

    private bool isOverThreshold(Location pressed, Point dragged)
    {
      int x1 = pressed.x;
      int y1 = pressed.y;
      int x2 = (int) dragged.x;
      int y2 = (int) dragged.y;
      return x2 > x1 + 7 || x2 < x1 - 7 || y2 > y1 + 7 || y2 < y1 - 7;
    }

    public virtual void keyPressed(KeyEvent ke)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual void keyReleased(KeyEvent ke)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual void keyTyped(KeyEvent ke)
    {
      if (this.isBusy((View) null))
        return;
      char keyChar = ke.getKeyChar();
      if (Character.isISOControl(keyChar))
        return;
      this.keyboardManager.typed(keyChar);
      ((InputEvent) ke).consume();
      this.lastTyped = ke;
      this.redraw();
    }

    private void interactionException(string action, Exception e)
    {
      InteractionHandler.LOG.error((object) new StringBuffer().append("error during user interaction: ").append(action).ToString(), (Throwable) e);
      this.viewer.showException((Throwable) e);
    }

    public virtual void mouseClicked(MouseEvent me)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual void mouseDragged(MouseEvent me)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual void mouseEntered(MouseEvent me)
    {
    }

    public virtual void mouseExited(MouseEvent me)
    {
    }

    public virtual void mouseMoved(MouseEvent me)
    {
      // ISSUE: unable to decompile the method.
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void mouseWheelMoved(MouseWheelEvent me)
    {
      // ISSUE: unable to decompile the method.
    }

    private bool isBusy(View view) => this.viewer.isBusy(view);

    public virtual void mousePressed(MouseEvent me)
    {
      // ISSUE: unable to decompile the method.
    }

    private void fireMenuPopup(Click click)
    {
      this.spy.addAction(new StringBuffer().append(" popup ").append((object) this.downAt).append(" over ").append((object) this.identifiedView).ToString());
      Location location = click.getLocation();
      bool flag = this.viewer.viewAreaType(new Location(click.getLocation())) == ViewAreaType.VIEW;
      bool forView = click.isAlt() ^ flag;
      bool includeExploration = click.isCtrl();
      bool includeDebug = click.isShift();
      this.viewer.popupMenu(this.identifiedView, location, forView, includeExploration, includeDebug);
    }

    public virtual void mouseReleased(MouseEvent me)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual void setMouseWheelListener(MouseWheelListener mouseWheelListener)
    {
      this.mouseWheelListener = mouseWheelListener;
      if (this.mouseWheelListener == null)
        return;
      this.mouseWheelListener.startListening((MouseWheelHandler) this);
    }

    private void redraw() => this.viewer.repaint();

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static InteractionHandler()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      InteractionHandler interactionHandler = this;
      ObjectImpl.clone((object) interactionHandler);
      return ((object) interactionHandler).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaFlags(32)]
    [Inner]
    public class \u0031 : AbstractView
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private InteractionHandler this\u00240;

      public \u0031(InteractionHandler _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }
  }
}
