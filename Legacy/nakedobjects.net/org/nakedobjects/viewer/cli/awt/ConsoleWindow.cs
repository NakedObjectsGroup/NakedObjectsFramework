// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.awt.ConsoleWindow
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.awt.@event;
using java.util;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace org.nakedobjects.viewer.cli.awt
{
  public class ConsoleWindow : Frame
  {
    private static readonly Font FONT;
    private TextFieldInput input;
    private TextArea outputPane;
    private Vector scrollback;
    private int scrollbackPosition;
    private TextAreaView view;

    public ConsoleWindow()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4)]
    public virtual void append(string text) => this.outputPane.append(text);

    [JavaFlags(4)]
    public virtual void clear() => ((TextComponent) this.outputPane).setText("");

    public virtual TextFieldInput getInput() => this.input;

    public virtual Insets getInsets()
    {
      Insets insets = ((Container) this).getInsets();
      insets.top = (__Null) (insets.top + 6);
      insets.bottom = (__Null) (insets.bottom + 6);
      insets.left = (__Null) (insets.left + 6);
      insets.right = (__Null) (insets.right + 6);
      return insets;
    }

    public virtual TextAreaView getView() => this.view;

    private void setFocusable(Component component, bool flag)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual void show()
    {
      ((Window) this).show();
      ((TextComponent) this.outputPane).setCaretPosition(StringImpl.length(((TextComponent) this.outputPane).getText()));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static ConsoleWindow()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public virtual object MemberwiseClone()
    {
      ConsoleWindow consoleWindow = this;
      ObjectImpl.clone((object) consoleWindow);
      return ((object) consoleWindow).MemberwiseClone();
    }

    [Inner]
    [JavaFlags(32)]
    [JavaInterfaces("1;java/awt/event/ActionListener;")]
    public class \u0031 : ActionListener
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private ConsoleWindow this\u00240;
      [JavaFlags(16)]
      public readonly Entry entry_\u003E;
      [JavaFlags(16)]
      public readonly TextField inputField_\u003E;

      public virtual void actionPerformed(ActionEvent e)
      {
        string text = ((TextComponent) this.inputField_\u003E).getText();
        if (this.this\u00240.scrollback.contains((object) text))
          this.this\u00240.scrollback.removeElement((object) text);
        this.this\u00240.scrollback.addElement((object) text);
        this.this\u00240.scrollbackPosition = this.this\u00240.scrollback.size();
        this.entry_\u003E.set(text);
        ((TextComponent) this.inputField_\u003E).setText("");
      }

      public \u0031(ConsoleWindow _param1, [In] TextField obj1, [In] Entry obj2)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.inputField_\u003E = obj1;
        this.entry_\u003E = obj2;
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        ConsoleWindow.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [Inner]
    [JavaFlags(32)]
    public class \u0032 : KeyAdapter
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private ConsoleWindow this\u00240;
      [JavaFlags(16)]
      public readonly TextField inputField_\u003E;

      public virtual void keyPressed(KeyEvent e)
      {
        if (e.getKeyCode() == 40 && this.this\u00240.scrollbackPosition < this.this\u00240.scrollback.size())
          ++this.this\u00240.scrollbackPosition;
        else if (e.getKeyCode() == 38 && this.this\u00240.scrollbackPosition > 0)
        {
          this.this\u00240.scrollbackPosition += -1;
        }
        else
        {
          if (e.getKeyCode() != 27)
            return;
          this.this\u00240.scrollbackPosition = this.this\u00240.scrollback.size();
        }
        string str = this.this\u00240.scrollbackPosition != this.this\u00240.scrollback.size() ? \u003CVerifierFix\u003E.genCastToString(this.this\u00240.scrollback.elementAt(this.this\u00240.scrollbackPosition)) : "";
        ((TextComponent) this.inputField_\u003E).setText(str);
        ((TextComponent) this.inputField_\u003E).setCaretPosition(StringImpl.length(str));
      }

      public \u0032(ConsoleWindow _param1, [In] TextField obj1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.inputField_\u003E = obj1;
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public virtual object MemberwiseClone()
      {
        ConsoleWindow.\u0032 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public virtual string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
