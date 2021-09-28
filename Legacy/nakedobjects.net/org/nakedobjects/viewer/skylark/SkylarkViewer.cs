// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.SkylarkViewer
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.awt.@event;
using java.lang;
using java.util;
using org.apache.log4j;
using org.nakedobjects.@event;
using org.nakedobjects.@object;
using org.nakedobjects.utility.configuration;
using org.nakedobjects.viewer.skylark.@event;
using org.nakedobjects.viewer.skylark.basic;
using org.nakedobjects.viewer.skylark.metal;
using org.nakedobjects.viewer.skylark.special;
using org.nakedobjects.viewer.skylark.table;
using org.nakedobjects.viewer.skylark.util;
using org.nakedobjects.viewer.skylark.value;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.viewer.skylark
{
  public class SkylarkViewer : NakedObjectsViewer
  {
    private static readonly Logger LOG;
    public const string PROPERTY_BASE = "nakedobjects.viewer.skylark.";
    private static readonly string SPECIFICATION_BASE;
    private ViewUpdateNotifier updateNotifier;
    private ViewerFrame frame;
    private Viewer viewer;
    private ObjectViewingMechanismListener shutdownListener;
    private bool inExplorationMode;
    private UserContext applicationContext;
    private Bounds bounds;
    private string title;
    private HelpViewer helpViewer;
    private bool focused;
    private MouseWheelListener mouseWheelListener;

    public override void init()
    {
      if (this.updateNotifier == null)
        throw new NullPointerException(new StringBuffer().append("No update notifier set for ").append((object) this).ToString());
      if (this.shutdownListener == null)
        throw new NullPointerException(new StringBuffer().append("No shutdown listener set for ").append((object) this).ToString());
      if (this.applicationContext == null)
        throw new NullPointerException(new StringBuffer().append("No application context set for ").append((object) this).ToString());
      this.setupViewFactory();
      this.frame = new ViewerFrame();
      if (this.bounds == null)
        this.bounds = this.calculateBounds(((Window) this.frame).getToolkit().getScreenSize());
      ((Window) this.frame).pack();
      ((Component) this.frame).setBounds(this.bounds.getX(), this.bounds.getY(), this.bounds.getWidth(), this.bounds.getHeight());
      this.viewer = new Viewer();
      this.viewer.setRenderingArea((RenderingArea) this.frame);
      this.viewer.setUpdateNotifier(this.updateNotifier);
      this.viewer.setListener(this.shutdownListener);
      this.viewer.setExploration(this.inExplorationMode);
      if (this.mouseWheelListener != null)
        this.viewer.setMouseWheelListener(this.mouseWheelListener);
      ((Component) this.frame).addFocusListener((FocusListener) new SkylarkViewer.\u0031(this));
      if (this.helpViewer == null)
        this.helpViewer = (HelpViewer) new InternalHelpViewer(this.viewer);
      this.viewer.setHelpViewer(this.helpViewer);
      this.frame.setViewer(this.viewer);
      NakedObjects.getObjectPersistor().addObjectChangedListener((DirtyObjectSet) this.updateNotifier);
      this.viewer.setRootView(new RootWorkspaceSpecification().createView((Content) new RootObject(NakedObjects.getObjectLoader().createAdapterForTransient((object) this.applicationContext)), (ViewAxis) null));
      this.viewer.init();
      this.frame.setTitle(this.title != null ? this.title : this.applicationContext.getName());
      this.frame.init();
      this.viewer.initSize();
      this.viewer.repaint();
      ((Window) this.frame).show();
      ((Window) this.frame).toFront();
    }

    public virtual bool isFocused() => this.focused;

    private Bounds calculateBounds(Dimension screenSize)
    {
      int num1 = (int) screenSize.width;
      int height1 = (int) screenSize.height;
      if (screenSize.width / screenSize.height >= 2)
      {
        int num2 = (int) (screenSize.width / screenSize.height);
        num1 = screenSize.width / num2;
      }
      int width = num1 - 80;
      int height2 = height1 - 80;
      int x = 40;
      int y = 40;
      string str1 = NakedObjects.getConfiguration().getString(new StringBuffer().append("nakedobjects.viewer.skylark.").append("initial.size").ToString());
      if (str1 != null)
      {
        StringTokenizer stringTokenizer = new StringTokenizer(str1, "x");
        if (stringTokenizer.countTokens() == 2)
        {
          width = Integer.valueOf(StringImpl.trim(stringTokenizer.nextToken())).intValue();
          height2 = Integer.valueOf(StringImpl.trim(stringTokenizer.nextToken())).intValue();
        }
      }
      string str2 = NakedObjects.getConfiguration().getString(new StringBuffer().append("nakedobjects.viewer.skylark.").append("initial.location").ToString());
      if (str2 != null)
      {
        StringTokenizer stringTokenizer = new StringTokenizer(str2, ",");
        if (stringTokenizer.countTokens() == 2)
        {
          x = Integer.valueOf(StringImpl.trim(stringTokenizer.nextToken())).intValue();
          y = Integer.valueOf(StringImpl.trim(stringTokenizer.nextToken())).intValue();
        }
      }
      return new Bounds(x, y, width, height2);
    }

    [JavaThrownExceptions("2;org/nakedobjects/utility/configuration/ConfigurationException;org/nakedobjects/utility/configuration/ComponentException;")]
    private void setupViewFactory()
    {
      ViewFactory viewFactory = Skylark.getViewFactory();
      if (SkylarkViewer.LOG.isDebugEnabled())
        SkylarkViewer.LOG.debug((object) "setting up default views (provided by the framework)");
      viewFactory.addValueFieldSpecification(this.loadSpecification("field.image", Class.FromType(typeof (ImageField.Specification))));
      viewFactory.addValueFieldSpecification(this.loadSpecification("field.color", Class.FromType(typeof (ColorField.Specification))));
      viewFactory.addValueFieldSpecification(this.loadSpecification("field.password", Class.FromType(typeof (PasswordFieldSpecification))));
      viewFactory.addValueFieldSpecification(this.loadSpecification("field.wrappedtext", Class.FromType(typeof (WrappedTextFieldSpecification))));
      viewFactory.addValueFieldSpecification(this.loadSpecification("field.checkbox", Class.FromType(typeof (CheckboxField.Specification))));
      viewFactory.addValueFieldSpecification(this.loadSpecification("field.text", Class.FromType(typeof (TextFieldSpecification))));
      viewFactory.addRootWorkspaceSpecification((ViewSpecification) new RootWorkspaceSpecification());
      viewFactory.addWorkspaceSpecification((ViewSpecification) new InnerWorkspaceSpecification());
      if (NakedObjects.getConfiguration().getBoolean(new StringBuffer().append(SkylarkViewer.SPECIFICATION_BASE).append("defaults").ToString(), true))
      {
        viewFactory.addCompositeRootViewSpecification((ViewSpecification) new org.nakedobjects.viewer.skylark.metal.ListSpecification());
        viewFactory.addCompositeRootViewSpecification((ViewSpecification) new WindowTableSpecification());
        viewFactory.addCompositeRootViewSpecification((ViewSpecification) new TreeBrowserSpecification());
        viewFactory.addCompositeRootViewSpecification((ViewSpecification) new org.nakedobjects.viewer.skylark.metal.FormSpecification());
        viewFactory.addCompositeRootViewSpecification((ViewSpecification) new DataFormSpecification());
      }
      viewFactory.addCompositeRootViewSpecification((ViewSpecification) new MessageDialogSpecification());
      viewFactory.addCompositeRootViewSpecification((ViewSpecification) new DetailedMessageViewSpecification());
      viewFactory.addEmptyFieldSpecification(this.loadSpecification("field.empty", Class.FromType(typeof (EmptyField.Specification))));
      viewFactory.addSubviewIconSpecification(this.loadSpecification("icon.subview", Class.FromType(typeof (SubviewIconSpecification))));
      viewFactory.addObjectIconSpecification(this.loadSpecification("icon.object", Class.FromType(typeof (RootIconSpecification))));
      viewFactory.addClassIconSpecification(this.loadSpecification("icon.class", Class.FromType(typeof (ClassIcon.Specification))));
      string str1 = NakedObjects.getConfiguration().getString(new StringBuffer().append(SkylarkViewer.SPECIFICATION_BASE).append("view").ToString());
      if (str1 == null)
        return;
      StringTokenizer stringTokenizer = new StringTokenizer(str1, ",");
      bool flag = SkylarkViewer.LOG.isInfoEnabled();
      while (stringTokenizer.hasMoreTokens())
      {
        string str2 = stringTokenizer.nextToken();
        if (str2 != null)
        {
          if (!StringImpl.equals(StringImpl.trim(str2), (object) ""))
          {
            try
            {
              ViewSpecification spec = (ViewSpecification) Class.forName(str2).newInstance();
              if (flag)
                SkylarkViewer.LOG.info((object) new StringBuffer().append("adding view specification: ").append((object) spec).ToString());
              viewFactory.addCompositeRootViewSpecification(spec);
            }
            catch (ClassNotFoundException ex)
            {
              SkylarkViewer.LOG.error((object) new StringBuffer().append("failed to find view specification class ").append(str2).ToString());
            }
            catch (InstantiationException ex)
            {
              SkylarkViewer.LOG.error((object) new StringBuffer().append("failed to instantiate view specification ").append(str2).ToString());
            }
            catch (IllegalAccessException ex)
            {
              ((Throwable) ex).printStackTrace();
            }
          }
        }
      }
    }

    private ViewSpecification loadSpecification(string name, Class cls)
    {
      string className = NakedObjects.getConfiguration().getString(new StringBuffer().append(SkylarkViewer.SPECIFICATION_BASE).append(name).ToString());
      return className == null ? (ViewSpecification) ComponentLoader.loadComponent(cls.getName(), Class.FromType(typeof (ViewSpecification))) : (ViewSpecification) ComponentLoader.loadComponent(className, Class.FromType(typeof (ViewSpecification)));
    }

    public override void setApplication(UserContext applicationContext) => this.applicationContext = applicationContext;

    public virtual void setBounds(Bounds bounds) => this.bounds = bounds;

    public override void setShutdownListener(ObjectViewingMechanismListener shutdownListener) => this.shutdownListener = shutdownListener;

    public override void setExploration(bool inExplorationMode) => this.inExplorationMode = inExplorationMode;

    public override void setUpdateNotifier(ViewUpdateNotifier updateNotifier) => this.updateNotifier = updateNotifier;

    public override void setHelpViewer(HelpViewer helpViewer) => this.helpViewer = helpViewer;

    public override void setTitle(string title) => this.title = title;

    public virtual void setMouseWheelListener(MouseWheelListener mouseWheelListener) => this.mouseWheelListener = mouseWheelListener;

    public virtual Frame getViewerFrame() => (Frame) this.frame;

    public override void shutdown()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static SkylarkViewer()
    {
      // ISSUE: unable to decompile the method.
    }

    [Inner]
    [JavaFlags(32)]
    [JavaInterfaces("1;java/awt/event/FocusListener;")]
    public class \u0031 : FocusListener
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private SkylarkViewer this\u00240;

      [MethodImpl(MethodImplOptions.Synchronized)]
      public virtual void focusGained(FocusEvent evt) => this.this\u00240.focused = true;

      [MethodImpl(MethodImplOptions.Synchronized)]
      public virtual void focusLost(FocusEvent evt) => this.this\u00240.focused = false;

      public \u0031(SkylarkViewer _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        SkylarkViewer.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
