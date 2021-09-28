// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.logging.SubmitDialog
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.awt.@event;
using java.lang;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.utility.logging
{
  [JavaFlags(32)]
  public class SubmitDialog : Frame
  {
    public SubmitDialog(string title)
      : base(title)
    {
      ((Container) this).setLayout((LayoutManager) new BorderLayout());
      ((Component) this).setBounds(0, 200, 800, 400);
    }

    public virtual void show(string message, string text)
    {
      TextArea textArea = new TextArea();
      ((TextComponent) textArea).setText(new StringBuffer().append(message).append("\n\n").append(text).ToString());
      ((Component) textArea).setForeground((Color) Color.black);
      ((TextComponent) textArea).setEditable(false);
      ((Component) textArea).setFont(new Font("Dialog", 0, 9));
      ((Container) this).add((Component) textArea, (object) BorderLayout.CENTER);
      Panel panel = new Panel();
      ((Container) panel).setLayout((LayoutManager) new FlowLayout(1, 20, 0));
      ((Container) this).add((Component) panel, (object) BorderLayout.SOUTH);
      Button button = new Button("Close");
      button.addActionListener((ActionListener) new SubmitDialog.\u0031(this));
      ((Container) panel).add((Component) button);
      ((Window) this).addWindowListener((WindowListener) new SubmitDialog.\u0032(this));
      ((Window) this).show();
    }

    [JavaFlags(36)]
    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void ok(bool b) => this.dispose();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public virtual object MemberwiseClone()
    {
      SubmitDialog submitDialog = this;
      ObjectImpl.clone((object) submitDialog);
      return ((object) submitDialog).MemberwiseClone();
    }

    [JavaFlags(32)]
    [JavaInterfaces("1;java/awt/event/ActionListener;")]
    [Inner]
    public class \u0031 : ActionListener
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private SubmitDialog this\u00240;

      public virtual void actionPerformed(ActionEvent e) => this.this\u00240.ok(true);

      public \u0031(SubmitDialog _param1)
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
        SubmitDialog.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [Inner]
    [JavaFlags(32)]
    public class \u0032 : WindowAdapter
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private SubmitDialog this\u00240;

      public virtual void windowClosing(WindowEvent e) => this.this\u00240.ok(false);

      public \u0032(SubmitDialog _param1)
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
        SubmitDialog.\u0032 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public virtual string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
