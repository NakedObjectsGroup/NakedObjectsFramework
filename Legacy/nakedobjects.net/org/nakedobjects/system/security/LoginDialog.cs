// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.system.security.LoginDialog
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

namespace org.nakedobjects.system.security
{
  [JavaInterfaces("2;java/awt/event/ActionListener;java/awt/event/KeyListener;")]
  public class LoginDialog : Frame, ActionListener, KeyListener
  {
    private static readonly Logger LOG;
    private const int BORDER = 10;
    private TextField user;
    private TextField password;
    private Button cancel;
    private Button login;
    private static string CANCEL_LABEL;
    private static string LOGIN_LABEL;
    private bool logIn;

    public LoginDialog()
      : base("Naked Objects Login")
    {
      this.logIn = true;
      ((Container) this).setLayout((LayoutManager) new GridLayout(3, 2, 10, 10));
      ((Container) this).add((Component) new Label("User name:", 0));
      ((Container) this).add((Component) (this.user = new TextField()));
      ((Component) this.user).addKeyListener((KeyListener) this);
      ((Container) this).add((Component) new Label("Password:", 0));
      ((Container) this).add((Component) (this.password = new TextField()));
      ((Component) this.password).addKeyListener((KeyListener) this);
      this.password.setEchoChar('*');
      ((Container) this).add((Component) (this.cancel = new Button(LoginDialog.CANCEL_LABEL)));
      this.cancel.addActionListener((ActionListener) this);
      ((Component) this.cancel).addKeyListener((KeyListener) this);
      ((Container) this).add((Component) (this.login = new Button(LoginDialog.LOGIN_LABEL)));
      this.login.addActionListener((ActionListener) this);
      ((Component) this.login).addKeyListener((KeyListener) this);
      ((Window) this).pack();
      int width = (int) ((Component) this).getSize().width;
      int height = (int) ((Component) this).getSize().height;
      Dimension screenSize = ((Window) this).getToolkit().getScreenSize();
      int num1 = screenSize.width / 2 - width / 2;
      if (screenSize.width / screenSize.height >= 2)
        num1 = screenSize.width / 4 - width / 2;
      int num2 = screenSize.height / 2 - height / 2;
      ((Component) this).setLocation(num1, num2);
      ((Window) this).show();
      ((Component) this.user).requestFocus();
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
    private void cancel(object widget)
    {
      this.logIn = false;
      ObjectImpl.notify((object) this);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private void action(object widget)
    {
      if (widget == this.cancel)
        this.cancel(widget);
      else if (widget == this.login || widget == this.password)
      {
        this.logIn = true;
        ObjectImpl.notify((object) this);
      }
      else
      {
        if (widget != this.user)
          return;
        ((Component) this.password).requestFocus();
      }
    }

    public virtual void dispose()
    {
      if (LoginDialog.LOG.isDebugEnabled())
        LoginDialog.LOG.debug((object) "dispose...");
      base.dispose();
      if (!LoginDialog.LOG.isDebugEnabled())
        return;
      LoginDialog.LOG.debug((object) "...disposed");
    }

    public virtual string getUser() => ((TextComponent) this.user).getText();

    public virtual string getPassword() => ((TextComponent) this.password).getText();

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual bool login()
    {
      try
      {
        ObjectImpl.wait((object) this);
      }
      catch (InterruptedException ex)
      {
      }
      return this.logIn;
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static LoginDialog()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public virtual object MemberwiseClone()
    {
      LoginDialog loginDialog = this;
      ObjectImpl.clone((object) loginDialog);
      return ((object) loginDialog).MemberwiseClone();
    }
  }
}
