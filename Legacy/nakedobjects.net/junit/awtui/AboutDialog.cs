// Decompiled with JetBrains decompiler
// Type: junit.awtui.AboutDialog
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.awt.@event;
using java.lang;
using junit.runner;
using System.ComponentModel;

namespace junit.awtui
{
  [JavaFlags(32)]
  public class AboutDialog : Dialog
  {
    public AboutDialog(Frame parent)
      : base(parent)
    {
      this.setResizable(false);
      ((Container) this).setLayout((LayoutManager) new GridBagLayout());
      ((Component) this).setSize(330, 138);
      this.setTitle("About");
      Button button = new Button("Close");
      button.addActionListener((ActionListener) new AboutDialog.\u0031(this));
      Label label1 = new Label("JUnit");
      ((Component) label1).setFont(new Font("dialog", 0, 36));
      Label label2 = new Label(new StringBuffer().append("JUnit ").append(Version.id()).append(" by Kent Beck and Erich Gamma").ToString());
      ((Component) label2).setFont(new Font("dialog", 0, 14));
      Logo logo = new Logo();
      ((Container) this).add((Component) label1, (object) new GridBagConstraints()
      {
        gridx = (__Null) 3,
        gridy = (__Null) 0,
        gridwidth = (__Null) 1,
        gridheight = (__Null) 1,
        anchor = (__Null) 10
      });
      ((Container) this).add((Component) label2, (object) new GridBagConstraints()
      {
        gridx = (__Null) 2,
        gridy = (__Null) 1,
        gridwidth = (__Null) 2,
        gridheight = (__Null) 1,
        anchor = (__Null) 10
      });
      ((Container) this).add((Component) button, (object) new GridBagConstraints()
      {
        gridx = (__Null) 2,
        gridy = (__Null) 2,
        gridwidth = (__Null) 2,
        gridheight = (__Null) 1,
        anchor = (__Null) 10,
        insets = (__Null) new Insets(8, 0, 8, 0)
      });
      ((Container) this).add((Component) logo, (object) new GridBagConstraints()
      {
        gridx = (__Null) 2,
        gridy = (__Null) 0,
        gridwidth = (__Null) 1,
        gridheight = (__Null) 1,
        anchor = (__Null) 10
      });
      ((Window) this).addWindowListener((WindowListener) new AboutDialog.\u0032(this));
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public virtual object MemberwiseClone()
    {
      AboutDialog aboutDialog = this;
      ObjectImpl.clone((object) aboutDialog);
      return ((object) aboutDialog).MemberwiseClone();
    }

    [Inner]
    [JavaInterfaces("1;java/awt/event/ActionListener;")]
    [JavaFlags(32)]
    public class \u0031 : ActionListener
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private AboutDialog this\u00240;

      public virtual void actionPerformed(ActionEvent e) => this.this\u00240.dispose();

      public \u0031(AboutDialog _param1)
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
        AboutDialog.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [JavaFlags(32)]
    [Inner]
    public class \u0032 : WindowAdapter
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private AboutDialog this\u00240;

      public virtual void windowClosing(WindowEvent e) => this.this\u00240.dispose();

      public \u0032(AboutDialog _param1)
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
        AboutDialog.\u0032 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public virtual string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
