// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.ShutdownDialog
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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.viewer.skylark
{
  [JavaFlags(32)]
  [JavaInterfaces("2;java/awt/event/ActionListener;java/awt/event/KeyListener;")]
  public class ShutdownDialog : Dialog, ActionListener, KeyListener
  {
    private static readonly Logger LOG;
    private const int BORDER = 10;
    private Button cancel;
    private Button quit;
    private static string CANCEL_LABEL;
    private static string QUIT_LABEL;

    public ShutdownDialog(ViewerFrame owner)
      : this(owner, "Naked Objects")
    {
    }

    public ShutdownDialog(ViewerFrame owner, string title)
      : base((Frame) owner, title, true)
    {
      ((Container) this).setLayout((LayoutManager) new GridLayout(2, 3, 10, 10));
      ((Container) this).add((Component) new Label(new StringBuffer().append("Exit ").append(title).append(" Application?").ToString(), 0));
      ((Container) this).add((Component) new Panel());
      ((Container) this).add((Component) new Panel());
      ((Container) this).add((Component) new Panel());
      ((Container) this).add((Component) (this.quit = new Button(ShutdownDialog.QUIT_LABEL)));
      this.quit.addActionListener((ActionListener) this);
      ((Component) this.quit).addKeyListener((KeyListener) this);
      ((Container) this).add((Component) (this.cancel = new Button(ShutdownDialog.CANCEL_LABEL)));
      this.cancel.addActionListener((ActionListener) this);
      ((Component) this.cancel).addKeyListener((KeyListener) this);
      ((Window) this).pack();
      int width = (int) ((Component) this).getSize().width;
      int height = (int) ((Component) this).getSize().height;
      Dimension size = ((Component) owner).getSize();
      Point location = ((Component) owner).getLocation();
      ((Component) this).setLocation(location.x + size.width / 2 - width / 2, location.y + size.height / 2 - height / 2);
      ((Window) this).addWindowListener((WindowListener) new ShutdownDialog.\u0031(this));
      ((Window) this).addWindowListener((WindowListener) new ShutdownDialog.\u0032(this));
      this.setModal(true);
      ((Component) this).setVisible(true);
      ((Window) this).toFront();
    }

    public virtual Insets getInsets()
    {
      Insets insets = ((Container) this).getInsets();
      insets.top = (__Null) (insets.top + 10);
      insets.bottom = (__Null) (insets.bottom + 10);
      insets.left = (__Null) (insets.left + 10);
      insets.right = (__Null) (insets.right + 10);
      return insets;
    }

    public virtual void actionPerformed(ActionEvent evt) => this.action(((EventObject) evt).getSource());

    public virtual void keyPressed(KeyEvent e)
    {
    }

    public virtual void keyReleased(KeyEvent e)
    {
      if (e.getKeyCode() == 10)
        this.action((object) ((ComponentEvent) e).getComponent());
      if (e.getKeyCode() != 27)
        return;
      this.cancel((object) ((ComponentEvent) e).getComponent());
    }

    public virtual void keyTyped(KeyEvent e)
    {
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private void cancel(object widget) => this.dispose();

    [MethodImpl(MethodImplOptions.Synchronized)]
    private void action(object widget)
    {
      if (widget == this.cancel)
      {
        this.cancel(widget);
      }
      else
      {
        if (widget != this.quit)
          return;
        this.quit();
      }
    }

    private void quit()
    {
      this.dispose();
      ((ViewerFrame) ((Component) this).getParent()).quit();
    }

    public virtual void dispose()
    {
      if (ShutdownDialog.LOG.isDebugEnabled())
        ShutdownDialog.LOG.debug((object) "dispose...");
      base.dispose();
      if (!ShutdownDialog.LOG.isDebugEnabled())
        return;
      ShutdownDialog.LOG.debug((object) "...disposed");
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static ShutdownDialog()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public virtual object MemberwiseClone()
    {
      ShutdownDialog shutdownDialog = this;
      ObjectImpl.clone((object) shutdownDialog);
      return ((object) shutdownDialog).MemberwiseClone();
    }

    [Inner]
    [JavaFlags(32)]
    public class \u0031 : WindowAdapter
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private ShutdownDialog this\u00240;

      public virtual void windowOpened(WindowEvent e) => ((Component) this.this\u00240.quit).requestFocus();

      public \u0031(ShutdownDialog _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public virtual object MemberwiseClone()
      {
        ShutdownDialog.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public virtual string ToString() => ObjectImpl.jloToString((object) this);
    }

    [JavaFlags(32)]
    [Inner]
    public class \u0032 : WindowAdapter
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private ShutdownDialog this\u00240;

      public virtual void windowClosing(WindowEvent e) => this.this\u00240.dispose();

      public \u0032(ShutdownDialog _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public virtual object MemberwiseClone()
      {
        ShutdownDialog.\u0032 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public virtual string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
