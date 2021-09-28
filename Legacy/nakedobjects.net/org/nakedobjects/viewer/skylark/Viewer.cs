// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.Viewer
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.awt.@event;
using java.awt.image;
using java.lang;
using java.util;
using org.apache.log4j;
using org.nakedobjects.@event;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.undo;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.@event;
using org.nakedobjects.viewer.skylark.basic;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.metal;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace org.nakedobjects.viewer.skylark
{
  public class Viewer
  {
    private static readonly NullView CLEAR_OVERLAY;
    private static readonly AbstractUserAction DEBUG_OPTION;
    private static Viewer instance;
    private static readonly org.apache.log4j.Logger LOG;
    public const string PROPERTY_BASE = "nakedobjects.viewer.skylark.";
    private static readonly Bounds NO_REDRAW;
    private static readonly org.apache.log4j.Logger UI_LOG;
    private Graphics bufferGraphics;
    private Vector busy;
    private Image doubleBuffer;
    private bool doubleBuffering;
    private Insets insets;
    private Size internalDisplaySize;
    private ObjectViewingMechanismListener listener;
    private View overlayView;
    private readonly Bounds redrawArea;
    private int redrawCount;
    private RenderingArea renderingArea;
    private View rootView;
    private bool runningAsExploration;
    private bool showExplorationMenuByDefault;
    private bool showRepaintArea;
    private InteractionSpy spy;
    private Bounds statusBarArea;
    private int statusBarHeight;
    private readonly UndoStack undoStack;
    [JavaFlags(4)]
    public ViewUpdateNotifier updateNotifier;
    private InteractionHandler interactionHandler;
    private string userStatus;
    private KeyboardManager keyboardManager;
    private HelpViewer helpViewer;
    private MouseWheelListener mouseWheelListener;

    public static Viewer getInstance() => Viewer.instance;

    public Viewer()
    {
      this.busy = new Vector();
      this.doubleBuffering = false;
      this.internalDisplaySize = new Size(1, 1);
      this.redrawCount = 100000;
      this.statusBarArea = new Bounds();
      this.undoStack = new UndoStack();
      Viewer.instance = this;
      this.doubleBuffering = NakedObjects.getConfiguration().getBoolean(new StringBuffer().append("nakedobjects.viewer.skylark.").append("doublebuffering").ToString(), true);
      this.showExplorationMenuByDefault = NakedObjects.getConfiguration().getBoolean(new StringBuffer().append("nakedobjects.viewer.skylark.").append("show-exploration").ToString(), true);
      this.overlayView = (View) Viewer.CLEAR_OVERLAY;
      this.redrawArea = new Bounds();
      this.spy = new InteractionSpy();
    }

    public virtual void addSpyAction(string actionMessage)
    {
      if (this.spy == null)
        return;
      this.spy.addAction(actionMessage);
    }

    public virtual void addToNotificationList(View view) => this.updateNotifier.add(view);

    public virtual string selectFilePath(string title, string directory) => this.renderingArea.selectFilePath(title, directory);

    public virtual void clearBusy(View view)
    {
      this.showDefaultCursor();
      this.busy.removeElement((object) view);
    }

    public virtual void setKeyboardFocus(View view)
    {
      if (view == null)
        throw new NullPointerException("Cannot set keyboard focus on a null view");
      FocusManager focusManager1 = this.keyboardManager.getFocusManager();
      if (focusManager1 != null && focusManager1.getFocus() != null && focusManager1.getFocus().getParent() != null)
        focusManager1.getFocus().getParent().markDamaged();
      if (focusManager1 != null)
      {
        View focus = focusManager1.getFocus();
        if (focus != null && focus != view)
          focus.focusLost();
      }
      FocusManager focusManager2 = view.getFocusManager();
      if (focusManager2 != null)
      {
        focusManager2.setFocus(view);
        if (view.getParent() != null)
          view.getParent().markDamaged();
      }
      if (focusManager2 == null)
      {
        if (!Viewer.LOG.isWarnEnabled())
          return;
        Viewer.LOG.warn((object) new StringBuffer().append("No focus manager for ").append((object) view).ToString());
      }
      else
        this.keyboardManager.setFocusManager(focusManager2);
    }

    public virtual void clearOverlayView()
    {
      this.overlayView.markDamaged();
      this.overlayView = (View) Viewer.CLEAR_OVERLAY;
    }

    public virtual void clearOverlayView(View view)
    {
      if (this.getOverlayView() != view && Viewer.LOG.isWarnEnabled())
        Viewer.LOG.warn((object) new StringBuffer().append("no such view to remove: ").append((object) view).ToString());
      this.clearOverlayView();
    }

    public virtual void clearStatus() => this.setStatus("");

    public virtual void close()
    {
      if (this.spy != null)
        this.spy.close();
      DebugFrame.disposeAll();
      this.renderingArea.dispose();
      if (this.listener == null)
        return;
      this.listener.viewerClosing();
    }

    public virtual void disposeOverlayView() => this.clearOverlayView();

    public virtual View dragFrom(Location location)
    {
      if (!this.onOverlay(location))
        return this.rootView.dragFrom(location);
      location.subtract(this.overlayView.getLocation());
      return this.overlayView.dragFrom(location);
    }

    public virtual Drag dragStart(DragStart start)
    {
      if (!this.onOverlay(start.getLocation()))
        return this.rootView.dragStart(start);
      start.subtract(this.overlayView.getLocation());
      return this.overlayView.dragStart(start);
    }

    public virtual void firstClick(Click click)
    {
      if (this.onOverlay(click.getLocation()))
      {
        click.subtract(this.overlayView.getLocation());
        this.overlayView.firstClick(click);
      }
      else
        this.rootView.firstClick(click);
    }

    private FocusManager getFocusManager() => this.overlayView == Viewer.CLEAR_OVERLAY ? this.keyboardManager.getFocusManager() : this.overlayView.getFocusManager();

    public virtual KeyboardManager getKeyboardManager() => this.keyboardManager;

    public virtual Bounds getOverlayBounds()
    {
      Bounds bounds = new Bounds(new Size(this.renderingArea.getSize()));
      Insets insets = this.renderingArea.getInsets();
      bounds.contract((int) (insets.left + insets.right), (int) (insets.top + insets.bottom));
      bounds.contract(0, this.statusBarHeight);
      return bounds;
    }

    public virtual View getOverlayView() => this.overlayView;

    public virtual InteractionSpy getSpy() => this.spy;

    public virtual UndoStack getUndoStack() => this.undoStack;

    public virtual bool hasFocus(View view)
    {
      FocusManager focusManager = this.keyboardManager.getFocusManager();
      return focusManager != null && focusManager.getFocus() == view;
    }

    public virtual View identifyView(Location location, bool includeOverlay)
    {
      if (!includeOverlay || !this.onOverlay(location))
        return this.rootView.identify(location);
      location.subtract(this.overlayView.getLocation());
      return this.overlayView.identify(location);
    }

    public virtual void init()
    {
      if (this.updateNotifier == null)
        throw new NullPointerException(new StringBuffer().append("No update notifier set for ").append((object) this).ToString());
      if (this.rootView == null)
        throw new NullPointerException(new StringBuffer().append("No root view set for ").append((object) this).ToString());
      this.insets = new Insets(0, 0, 0, 0);
      this.keyboardManager = new KeyboardManager(this);
      this.interactionHandler = new InteractionHandler(this, this.keyboardManager, this.spy);
      this.renderingArea.addMouseMotionListener((MouseMotionListener) this.interactionHandler);
      this.renderingArea.addMouseListener((MouseListener) this.interactionHandler);
      this.renderingArea.addKeyListener((KeyListener) this.interactionHandler);
      if (this.mouseWheelListener != null)
        this.interactionHandler.setMouseWheelListener(this.mouseWheelListener);
      if (NakedObjects.getConfiguration().getBoolean(new StringBuffer().append("nakedobjects.viewer.skylark.").append("show-mouse-spy").ToString(), false))
        this.spy.open();
      this.setKeyboardFocus(this.rootView);
    }

    public virtual bool isBusy(View view) => this.busy.size() > 0;

    public virtual bool isRunningAsExploration() => this.runningAsExploration;

    public virtual bool isShowingMouseSpy() => this.spy.isVisible();

    private AbstractUserAction loggingOption(string name, Level level) => (AbstractUserAction) new Viewer.\u0031(this, new StringBuffer().append("Log level ").append((object) level).ToString(), UserAction.DEBUG, level);

    public virtual void markDamaged(Bounds bounds)
    {
      if (this.spy != null)
        this.spy.addDamagedArea(bounds);
      object redrawArea = (object) this.redrawArea;
      \u003CCorArrayWrapper\u003E.Enter(redrawArea);
      try
      {
        if (this.redrawArea.Equals((object) Viewer.NO_REDRAW))
        {
          this.redrawArea.setBounds(bounds);
          if (!Viewer.UI_LOG.isDebugEnabled())
            return;
          Viewer.UI_LOG.debug((object) new StringBuffer().append("damage - new area ").append((object) this.redrawArea).ToString());
        }
        else
        {
          this.redrawArea.union(bounds);
          if (!Viewer.UI_LOG.isDebugEnabled())
            return;
          Viewer.UI_LOG.debug((object) new StringBuffer().append("damage - extend area ").append((object) this.redrawArea).append(" - to include ").append((object) bounds).ToString());
        }
      }
      finally
      {
        Monitor.Exit(redrawArea);
      }
    }

    public virtual void menuOptions(UserActionSet options)
    {
      options.add((UserAction) new Viewer.\u0032(this, "Quit"));
      options.add((UserAction) this.loggingOption("Off", Level.OFF));
      options.add((UserAction) this.loggingOption("Error", Level.ERROR));
      options.add((UserAction) this.loggingOption("Warn", Level.WARN));
      options.add((UserAction) this.loggingOption("Info", Level.INFO));
      options.add((UserAction) this.loggingOption("Debug", Level.DEBUG));
      string dummy0_1 = new StringBuffer().append("Always show exploration menu ").append(!this.showExplorationMenuByDefault ? "on" : "off").ToString();
      options.add((UserAction) new Viewer.\u0033(this, dummy0_1, UserAction.DEBUG));
      string dummy0_2 = new StringBuffer().append("Show painting area  ").append(!this.showRepaintArea ? "on" : "off").ToString();
      options.add((UserAction) new Viewer.\u0034(this, dummy0_2, UserAction.DEBUG));
      string dummy0_3 = new StringBuffer().append("Debug graphics ").append(!AbstractView.debug ? "on" : "off").ToString();
      options.add((UserAction) new Viewer.\u0035(this, dummy0_3, UserAction.DEBUG));
      string str = !this.isShowingMouseSpy() ? "Show" : "Hide";
      options.add((UserAction) new Viewer.\u0036(this, new StringBuffer().append(str).append(" mouse spy").ToString(), UserAction.DEBUG));
      options.add((UserAction) new Viewer.\u0037(this, "Restart object loader/persistor", UserAction.DEBUG));
      options.add((UserAction) new Viewer.\u0038(this, "Debug system...", UserAction.DEBUG));
      options.add((UserAction) new Viewer.\u0039(this, "Debug viewer...", UserAction.DEBUG));
      options.add((UserAction) new Viewer.\u00310(this, "Debug overlay...", UserAction.DEBUG));
      options.add((UserAction) new DebugDumpSnapshotOption());
    }

    public virtual void mouseDown(Click click)
    {
      if (this.onOverlay(click.getLocation()))
      {
        click.subtract(this.overlayView.getLocation());
        this.overlayView.mouseDown(click);
      }
      else
        this.rootView.mouseDown(click);
    }

    public virtual void mouseMoved(Location location)
    {
      if (this.onOverlay(location))
      {
        location.subtract(this.overlayView.getLocation());
        this.overlayView.mouseMoved(location);
      }
      else
        this.rootView.mouseMoved(location);
    }

    public virtual void mouseUp(Click click)
    {
      if (this.onOverlay(click.getLocation()))
      {
        click.subtract(this.overlayView.getLocation());
        this.overlayView.mouseUp(click);
      }
      else
        this.rootView.mouseUp(click);
    }

    private bool onOverlay(Location mouse) => this.overlayView.getBounds().contains(mouse);

    public virtual void paint(Graphics g)
    {
      ++this.redrawCount;
      g.translate((int) this.insets.left, (int) this.insets.top);
      int width = this.internalDisplaySize.getWidth();
      int height = this.internalDisplaySize.getHeight();
      if (this.doubleBuffering)
      {
        if (this.doubleBuffer == null || this.bufferGraphics == null || this.doubleBuffer.getWidth((ImageObserver) null) < width || this.doubleBuffer.getHeight((ImageObserver) null) < height)
        {
          this.doubleBuffer = this.renderingArea.createImage(width, height);
          if (Viewer.LOG.isDebugEnabled())
            Viewer.LOG.debug((object) new StringBuffer().append("buffer sized to ").append(this.doubleBuffer.getWidth((ImageObserver) null)).append("x").append(this.doubleBuffer.getHeight((ImageObserver) null)).ToString());
        }
        this.bufferGraphics = this.doubleBuffer.getGraphics().create();
      }
      else
        this.bufferGraphics = g;
      Rectangle clipBounds = g.getClipBounds();
      this.bufferGraphics.clearRect((int) clipBounds.x, (int) clipBounds.y, (int) clipBounds.width, (int) clipBounds.height);
      this.bufferGraphics.clearRect(0, 0, width, height);
      this.bufferGraphics.setClip((int) clipBounds.x, (int) clipBounds.y, (int) clipBounds.width, (int) clipBounds.height);
      Canvas canvas = (Canvas) new DrawingCanvas(this.bufferGraphics, (int) clipBounds.x, (int) clipBounds.y, (int) clipBounds.width, (int) clipBounds.height);
      if (this.spy != null)
        this.spy.redraw(clipBounds, this.redrawCount);
      if (AbstractView.debug && Viewer.LOG.isDebugEnabled())
        Viewer.LOG.debug((object) new StringBuffer().append("------ repaint viewer #").append(this.redrawCount).append(" ").append((int) clipBounds.x).append(",").append((int) clipBounds.y).append(" ").append((int) clipBounds.width).append("x").append((int) clipBounds.height).ToString());
      if (this.rootView != null)
        this.rootView.draw(canvas.createSubcanvas());
      this.overlayView.draw(canvas.createSubcanvas(this.overlayView.getBounds()));
      if (this.doubleBuffering)
        g.drawImage(this.doubleBuffer, 0, 0, (ImageObserver) null);
      if (this.showRepaintArea)
      {
        g.setColor(Color.DEBUG_REPAINT_BOUNDS.getAwtColor());
        g.drawRect((int) clipBounds.x, (int) clipBounds.y, clipBounds.width - 1, clipBounds.height - 1);
        g.drawString(new StringBuffer().append("#").append(this.redrawCount).ToString(), clipBounds.x + 3, clipBounds.y + 15);
      }
      this.paintUserStatus(g);
    }

    private void paintStatus(Graphics bufferCanvas, int top, string text)
    {
      bufferCanvas.setFont(Style.STATUS.getAwtFont());
      int num = top + Style.STATUS.getAscent();
      bufferCanvas.fillRect(0, top, this.internalDisplaySize.getWidth(), this.statusBarHeight);
      bufferCanvas.setColor(Style.SECONDARY1.getAwtColor());
      bufferCanvas.drawLine(0, top, this.internalDisplaySize.getWidth(), top);
      bufferCanvas.setColor(Style.BLACK.getAwtColor());
      if (text == null)
        return;
      bufferCanvas.drawString(text, 5, num + View.VPADDING);
    }

    private void paintUserStatus(Graphics bufferCanvas)
    {
      int top = this.internalDisplaySize.getHeight() - this.statusBarHeight;
      bufferCanvas.setColor(Style.SECONDARY3.getAwtColor());
      this.paintStatus(bufferCanvas, top, this.userStatus);
    }

    public virtual View pickupContent(Location location)
    {
      if (!this.onOverlay(location))
        return this.rootView.pickupContent(location);
      location.subtract(this.overlayView.getLocation());
      return this.overlayView.pickupContent(location);
    }

    public virtual View pickupView(Location location)
    {
      if (!this.onOverlay(location))
        return this.rootView.pickupView(location);
      location.subtract(this.overlayView.getLocation());
      return this.overlayView.pickupView(location);
    }

    [JavaFlags(4)]
    public virtual void popupMenu(
      View over,
      Location at,
      bool forView,
      bool includeExploration,
      bool includeDebug)
    {
      this.saveCurrentFieldEntry();
      includeExploration = this.runningAsExploration && (includeExploration || this.showExplorationMenuByDefault);
      this.popupStatus(over, forView, includeExploration, includeDebug);
      UserActionSet menuOptions = new UserActionSet(includeExploration, includeDebug, UserAction.USER);
      if (forView)
        over.viewMenuOptions(menuOptions);
      else
        over.contentMenuOptions(menuOptions);
      menuOptions.add((UserAction) Viewer.DEBUG_OPTION);
      PopupMenu popupMenu = new PopupMenu();
      Location location = new Location(at);
      location.move(-14, -10);
      popupMenu.setLocation(location);
      popupMenu.init(over, menuOptions.getMenuOptions(), menuOptions.getColor());
      this.setOverlayView((View) popupMenu);
    }

    private void popupStatus(View over, bool forView, bool includeExploration, bool includeDebug)
    {
      StringBuffer stringBuffer = new StringBuffer("Menu for ");
      if (forView)
      {
        stringBuffer.append("view ");
        stringBuffer.append(AbstractView.name(over));
      }
      else
      {
        stringBuffer.append("object: ");
        Content content = over.getContent();
        if (content != null)
          stringBuffer.append(content.title());
      }
      if (includeDebug || includeExploration)
      {
        stringBuffer.append(" (includes ");
        if (includeExploration)
          stringBuffer.append("exploration");
        if (includeDebug)
        {
          if (includeExploration)
            stringBuffer.append(" & ");
          stringBuffer.append("debug");
        }
        stringBuffer.append(" options)");
      }
      this.setStatus(stringBuffer.ToString());
    }

    public virtual void removeFromNotificationList(View view) => this.updateNotifier.remove(view);

    public virtual void repaint()
    {
      this.updateNotifier.invalidateViewsForChangedObjects();
      object redrawArea = (object) this.redrawArea;
      \u003CCorArrayWrapper\u003E.Enter(redrawArea);
      try
      {
        this.overlayView.layout(new Size(this.rootView.getSize()));
        this.rootView.layout(new Size(this.rootView.getSize()));
        if (this.redrawArea.Equals((object) Viewer.NO_REDRAW))
          return;
        if (Viewer.UI_LOG.isDebugEnabled())
          Viewer.UI_LOG.debug((object) new StringBuffer().append("repaint viewer ").append((object) this.redrawArea).ToString());
        Bounds bounds = new Bounds(this.redrawArea);
        bounds.translate((int) this.insets.left, (int) this.insets.top);
        this.renderingArea.repaint(bounds.x, bounds.y, bounds.width, bounds.height);
        this.redrawArea.setBounds(Viewer.NO_REDRAW);
      }
      finally
      {
        Monitor.Exit(redrawArea);
      }
    }

    public virtual void saveCurrentFieldEntry() => this.getFocusManager()?.getFocus()?.editComplete();

    public virtual void secondClick(Click click)
    {
      if (this.onOverlay(click.getLocation()))
      {
        click.subtract(this.overlayView.getLocation());
        this.overlayView.secondClick(click);
      }
      else
        this.rootView.secondClick(click);
    }

    public virtual void setBusy(View view)
    {
      this.showWaitCursor();
      this.busy.addElement((object) view);
    }

    public virtual void setCursor(Cursor cursor) => this.renderingArea.setCursor(cursor);

    public virtual void setExploration(bool asExploration) => this.runningAsExploration = asExploration;

    public virtual void setListener(ObjectViewingMechanismListener listener) => this.listener = listener;

    public virtual void setOverlayView(View view)
    {
      this.disposeOverlayView();
      this.overlayView = view;
      Size requiredSize = view.getRequiredSize(this.rootView.getSize());
      view.setSize(requiredSize);
      view.layout(this.rootView.getSize());
      view.limitBoundsWithin(this.rootView.getSize());
      this.overlayView.markDamaged();
    }

    public virtual void setRenderingArea(RenderingArea renderingArea) => this.renderingArea = renderingArea;

    public virtual void setRootView(View rootView)
    {
      this.rootView = rootView;
      rootView.invalidateContent();
    }

    public virtual void setHelpViewer(HelpViewer helpViewer) => this.helpViewer = helpViewer;

    public virtual void setShowMouseSpy(bool showDeveloperStatus)
    {
      if (this.spy.isVisible())
        this.spy.close();
      else
        this.spy.open();
    }

    public virtual void setStatus(string status)
    {
      if (StringImpl.equals(status, (object) this.userStatus))
        return;
      this.userStatus = status;
      if (Viewer.UI_LOG.isDebugEnabled())
        Viewer.UI_LOG.debug((object) new StringBuffer().append("changed user status ").append(status).append(" ").append((object) this.statusBarArea).ToString());
      Graphics graphics = ((Component) this.renderingArea).getGraphics();
      graphics.setClip(this.statusBarArea.x, this.statusBarArea.y, this.statusBarArea.width, this.statusBarArea.height);
      this.paint(graphics);
    }

    public virtual void setUpdateNotifier(ViewUpdateNotifier updateNotifier) => this.updateNotifier = updateNotifier;

    public virtual void setMouseWheelListener(MouseWheelListener mouseWheelListener) => this.mouseWheelListener = mouseWheelListener;

    public virtual void showArrowCursor() => this.setCursor(Cursor.getPredefinedCursor(0));

    public virtual void showCrosshairCursor() => this.setCursor(Cursor.getPredefinedCursor(1));

    public virtual void showDefaultCursor() => this.setCursor(Cursor.getPredefinedCursor(0));

    public virtual void showException(Throwable e)
    {
      ExceptionMessageContent exceptionMessageContent = new ExceptionMessageContent(e);
      View window = Skylark.getViewFactory().createWindow((Content) exceptionMessageContent);
      this.locateInCentre(window);
      this.rootView.getWorkspace().addView(window);
      this.repaint();
    }

    private void locateInCentre(View view)
    {
      Size size = this.rootView.getSize();
      Location point = new Location(size.getWidth() / 2, size.getHeight() / 2);
      Size requiredSize = view.getRequiredSize(new Size());
      point.subtract(requiredSize.getWidth() / 2, requiredSize.getHeight() / 2);
      view.setLocation(point);
    }

    public virtual void showHandCursor() => this.setCursor(Cursor.getPredefinedCursor(12));

    public virtual void showMessages()
    {
      string[] messages = NakedObjects.getMessageBroker().getMessages();
      StringBuffer stringBuffer = new StringBuffer();
      for (int index = 0; index < messages.Length; ++index)
      {
        if (index > 0)
          stringBuffer.append("; ");
        stringBuffer.append(messages[index]);
      }
      this.setStatus(stringBuffer.ToString());
      foreach (string warning in NakedObjects.getMessageBroker().getWarnings())
      {
        TextMessageContent textMessageContent = new TextMessageContent("Warning", warning);
        View window = Skylark.getViewFactory().createWindow((Content) textMessageContent);
        this.locateInCentre(window);
        this.rootView.getWorkspace().addView(window);
      }
    }

    public virtual void showMoveCursor() => this.setCursor(Cursor.getPredefinedCursor(13));

    public virtual void showResizeDownCursor() => this.setCursor(Cursor.getPredefinedCursor(9));

    public virtual void showResizeDownLeftCursor() => this.setCursor(Cursor.getPredefinedCursor(4));

    public virtual void showResizeDownRightCursor() => this.setCursor(Cursor.getPredefinedCursor(5));

    public virtual void showResizeLeftCursor() => this.setCursor(Cursor.getPredefinedCursor(10));

    public virtual void showResizeRightCursor() => this.setCursor(Cursor.getPredefinedCursor(11));

    public virtual void showResizeUpCursor() => this.setCursor(Cursor.getPredefinedCursor(8));

    public virtual void showResizeUpLeftCursor() => this.setCursor(Cursor.getPredefinedCursor(6));

    public virtual void showResizeUpRightCursor() => this.setCursor(Cursor.getPredefinedCursor(7));

    public virtual void showSpy() => this.spy.open();

    public virtual void showTextCursor() => this.setCursor(Cursor.getPredefinedCursor(2));

    public virtual void showWaitCursor() => this.setCursor(Cursor.getPredefinedCursor(3));

    public virtual void sizeChange()
    {
      this.initSize();
      foreach (View subview in this.rootView.getSubviews())
        subview.invalidateLayout();
      this.markDamaged(new Bounds(this.internalDisplaySize));
      this.repaint();
    }

    [JavaFlags(0)]
    public virtual void initSize()
    {
      this.internalDisplaySize = new Size(this.renderingArea.getSize());
      this.insets = this.renderingArea.getInsets();
      if (Viewer.LOG.isDebugEnabled())
        Viewer.LOG.debug((object) new StringBuffer().append("  insets ").append((object) this.insets).ToString());
      this.internalDisplaySize.contract((int) (this.insets.left + this.insets.right), (int) (this.insets.top + this.insets.bottom));
      if (Viewer.LOG.isDebugEnabled())
        Viewer.LOG.debug((object) new StringBuffer().append("  internal ").append((object) this.internalDisplaySize).ToString());
      Size size = new Size(this.internalDisplaySize);
      this.statusBarHeight = Style.STATUS.getLineHeight() + Style.STATUS.getDescent();
      size.contractHeight(this.statusBarHeight);
      this.statusBarArea = new Bounds((int) this.insets.left, this.insets.top + size.height, size.width, this.statusBarHeight);
      this.rootView.setSize(size);
    }

    public virtual void thirdClick(Click click)
    {
      if (this.onOverlay(click.getLocation()))
      {
        click.subtract(this.overlayView.getLocation());
        this.overlayView.thirdClick(click);
      }
      else
        this.rootView.thirdClick(click);
    }

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("renderingArea", (object) this.renderingArea);
      toString.append("redrawArea", (object) this.redrawArea);
      toString.append("rootView", (object) this.rootView);
      return toString.ToString();
    }

    public virtual void translate(MouseEvent me) => me.translatePoint((int) -this.insets.left, (int) -this.insets.top);

    public virtual ViewAreaType viewAreaType(Location location)
    {
      if (!this.onOverlay(location))
        return this.rootView.viewAreaType(location);
      location.subtract(this.overlayView.getLocation());
      return this.overlayView.viewAreaType(location);
    }

    public virtual bool isOverlayAvailable() => this.overlayView != Viewer.CLEAR_OVERLAY;

    public virtual void makeRootFocus()
    {
    }

    public virtual void openHelp(View forView)
    {
      if (forView == null)
        return;
      string description = (string) null;
      string help = (string) null;
      string name = (string) null;
      if (forView != null && forView.getContent() != null)
      {
        Content content = forView.getContent();
        description = content.getDescription();
        help = content.getHelp();
        name = content.getId() ?? content.title();
      }
      this.helpViewer.open(forView.getAbsoluteLocation(), name, description, help);
    }

    public virtual void filterKeyShortcuts(KeyboardAction keyboardAction) => this.rootView.getWorkspace().filterKeyShortcuts(keyboardAction);

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static Viewer()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      Viewer viewer = this;
      ObjectImpl.clone((object) viewer);
      return ((object) viewer).MemberwiseClone();
    }

    [JavaFlags(32)]
    [Inner]
    public class \u0031 : AbstractUserAction
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private Viewer this\u00240;
      [JavaFlags(16)]
      public readonly Level level_\u003E;

      public override Consent disabled(View component) => AbstractConsent.allow(LogManager.getRootLogger().getLevel() != this.level_\u003E);

      public override void execute(Workspace workspace, View view, Location at) => LogManager.getRootLogger().setLevel(this.level_\u003E);

      public \u0031(Viewer _param1, string dummy0, Action.Type dummy1, [In] Level obj3)
        : base(dummy0, dummy1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.level_\u003E = obj3;
      }
    }

    [Inner]
    [JavaFlags(32)]
    public class \u0032 : AbstractUserAction
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private Viewer this\u00240;

      public override void execute(Workspace workspace, View view, Location at) => this.this\u00240.close();

      public \u0032(Viewer _param1, string dummy0)
        : base(dummy0)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }

    [JavaFlags(32)]
    [Inner]
    public class \u0033 : AbstractUserAction
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private Viewer this\u00240;

      public override void execute(Workspace workspace, View view, Location at)
      {
        this.this\u00240.showExplorationMenuByDefault = ((this.this\u00240.showExplorationMenuByDefault ? 1 : 0) ^ 1) != 0;
        view.markDamaged();
      }

      public \u0033(Viewer _param1, string dummy0, Action.Type dummy1)
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
    public class \u0034 : AbstractUserAction
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private Viewer this\u00240;

      public override void execute(Workspace workspace, View view, Location at)
      {
        this.this\u00240.showRepaintArea = ((this.this\u00240.showRepaintArea ? 1 : 0) ^ 1) != 0;
        view.markDamaged();
      }

      public \u0034(Viewer _param1, string dummy0, Action.Type dummy1)
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
    public class \u0035 : AbstractUserAction
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private Viewer this\u00240;

      public override void execute(Workspace workspace, View view, Location at)
      {
        AbstractView.debug = ((AbstractView.debug ? 1 : 0) ^ 1) != 0;
        view.markDamaged();
      }

      public \u0035(Viewer _param1, string dummy0, Action.Type dummy1)
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
    public class \u0036 : AbstractUserAction
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private Viewer this\u00240;

      public override void execute(Workspace workspace, View view, Location at) => this.this\u00240.setShowMouseSpy(((this.this\u00240.isShowingMouseSpy() ? 1 : 0) ^ 1) != 0);

      public \u0036(Viewer _param1, string dummy0, Action.Type dummy1)
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
    public class \u0037 : AbstractUserAction
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private Viewer this\u00240;

      public override void execute(Workspace workspace, View view, Location at)
      {
        NakedObjects.getObjectPersistor().reset();
        NakedObjects.getObjectLoader().reset();
      }

      public \u0037(Viewer _param1, string dummy0, Action.Type dummy1)
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
    public class \u0038 : AbstractUserAction
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private Viewer this\u00240;

      public override void execute(Workspace workspace, View view, Location at)
      {
        InfoDebugFrame infoDebugFrame1 = new InfoDebugFrame();
        InfoDebugFrame infoDebugFrame2 = infoDebugFrame1;
        int length = 6;
        DebugInfo[] info = length >= 0 ? new DebugInfo[length] : throw new NegativeArraySizeException();
        info[0] = NakedObjects.debug();
        info[1] = (DebugInfo) NakedObjects.getObjectPersistor();
        info[2] = (DebugInfo) NakedObjects.getObjectLoader();
        info[3] = (DebugInfo) NakedObjects.getConfiguration();
        info[4] = (DebugInfo) NakedObjects.getSpecificationLoader();
        info[5] = (DebugInfo) this.this\u00240.updateNotifier;
        infoDebugFrame2.setInfo(info);
        infoDebugFrame1.show(at.x + 50, workspace.getBounds().y + 6);
      }

      public \u0038(Viewer _param1, string dummy0, Action.Type dummy1)
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
    public class \u0039 : AbstractUserAction
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private Viewer this\u00240;

      public override void execute(Workspace workspace, View view, Location at)
      {
        InfoDebugFrame infoDebugFrame1 = new InfoDebugFrame();
        InfoDebugFrame infoDebugFrame2 = infoDebugFrame1;
        int length = 2;
        DebugInfo[] info = length >= 0 ? new DebugInfo[length] : throw new NegativeArraySizeException();
        info[0] = (DebugInfo) Skylark.getViewFactory();
        info[1] = (DebugInfo) this.this\u00240.updateNotifier;
        infoDebugFrame2.setInfo(info);
        infoDebugFrame1.show(at.x + 50, workspace.getBounds().y + 6);
      }

      public \u0039(Viewer _param1, string dummy0, Action.Type dummy1)
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
    public class \u00310 : AbstractUserAction
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private Viewer this\u00240;

      public override void execute(Workspace workspace, View view, Location at) => new OverlayDebugFrame(this.this\u00240).show(at.x + 50, workspace.getBounds().y + 6);

      public \u00310(Viewer _param1, string dummy0, Action.Type dummy1)
        : base(dummy0, dummy1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }
  }
}
