// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.DebugFrame
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.awt.@event;
using java.lang;
using java.net;
using java.util;
using org.nakedobjects.@object;
using System.ComponentModel;

namespace org.nakedobjects.utility
{
  public abstract class DebugFrame : Frame
  {
    private static readonly org.apache.log4j.Logger LOG;
    private static Vector frames;
    private int panel;
    private TextArea field;
    private DebugFrame.TabPane tabPane;

    public static void disposeAll()
    {
      int length = DebugFrame.frames.size();
      Frame[] frameArray = length >= 0 ? new Frame[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < frameArray.Length; ++index)
        frameArray[index] = (Frame) DebugFrame.frames.elementAt(index);
      for (int index = 0; index < frameArray.Length; ++index)
        frameArray[index].dispose();
    }

    public DebugFrame()
    {
      this.panel = 0;
      DebugFrame.frames.addElement((object) this);
      ((Window) this).addWindowListener((WindowListener) new DebugFrame.\u0031(this));
      URL resource = Class.FromType(typeof (DebugFrame)).getResource("/images/debug.png");
      if (resource != null)
      {
        Image image = Toolkit.getDefaultToolkit().getImage(resource);
        if (image != null)
          this.setIconImage(image);
      }
      ((Container) this).setLayout((LayoutManager) new BorderLayout(7, 7));
      ((Container) this).add((Component) this.createTabPane());
    }

    private Panel createTabPane()
    {
      this.tabPane = new DebugFrame.TabPane(this);
      ((Component) this.tabPane).addMouseListener((MouseListener) new DebugFrame.\u0032(this));
      ((Container) this.tabPane).setLayout((LayoutManager) new BorderLayout(7, 7));
      TextArea textArea = new TextArea("", 60, 110, 0);
      ((Component) textArea).setForeground((Color) Color.black);
      ((TextComponent) textArea).setEditable(false);
      Font font = NakedObjects.getConfiguration().getFont("nakedobjects.debug.font", new Font("Monospaced", 0, 11));
      ((Component) textArea).setFont(font);
      ((Container) this.tabPane).add("Center", (Component) textArea);
      this.field = textArea;
      Panel panel = new Panel();
      ((Container) panel).setLayout((LayoutManager) new FlowLayout());
      ((Container) this.tabPane).add((Component) panel, (object) BorderLayout.SOUTH);
      Button button1 = new Button("Refresh");
      ((Component) button1).setFont(font);
      ((Container) panel).add((Component) button1);
      button1.addActionListener((ActionListener) new DebugFrame.\u0033(this));
      Button button2 = new Button("Print...");
      ((Component) button2).setFont(font);
      ((Container) panel).add((Component) button2);
      button2.addActionListener((ActionListener) new DebugFrame.\u0034(this));
      Button button3 = new Button("Save...");
      ((Component) button3).setFont(font);
      ((Container) panel).add((Component) button3);
      button3.addActionListener((ActionListener) new DebugFrame.\u0035(this));
      Button button4 = new Button("Copy");
      ((Component) button4).setFont(font);
      ((Container) panel).add((Component) button4);
      button4.addActionListener((ActionListener) new DebugFrame.\u0036(this));
      Button button5 = new Button("Close");
      ((Component) button5).setFont(font);
      ((Container) panel).add((Component) button5);
      button5.addActionListener((ActionListener) new DebugFrame.\u0037(this));
      return (Panel) this.tabPane;
    }

    public virtual Insets getInsets()
    {
      Insets insets = ((Container) this).getInsets();
      insets.left = (__Null) (insets.left + 10);
      insets.right = (__Null) (insets.right + 10);
      insets.top = (__Null) (insets.top + 10);
      insets.bottom = (__Null) (insets.bottom + 10);
      return insets;
    }

    private void closeDialog()
    {
      this.dialogClosing();
      this.dispose();
    }

    public virtual void dialogClosing()
    {
    }

    public virtual void dispose()
    {
      if (DebugFrame.LOG.isDebugEnabled())
        DebugFrame.LOG.debug((object) "dispose...");
      ((Container) this.tabPane).removeAll();
      DebugFrame.frames.removeElement((object) this);
      base.dispose();
      if (!DebugFrame.LOG.isDebugEnabled())
        return;
      DebugFrame.LOG.debug((object) "...disposed");
    }

    [JavaFlags(1028)]
    public abstract DebugInfo[] getInfo();

    public virtual void show(int x, int y)
    {
      this.refresh();
      ((Window) this).pack();
      this.limitBounds(x, y);
      ((Window) this).show();
    }

    private void refresh()
    {
      DebugInfo debugInfo = this.getInfo()[this.panel];
      if (debugInfo == null)
        return;
      this.setTitle(debugInfo.getDebugTitle());
      DebugString debug = new DebugString();
      debugInfo.debugData(debug);
      ((TextComponent) this.field).setText(debug.ToString());
    }

    public virtual void showDebugForPane() => this.refresh();

    private void limitBounds(int x, int y)
    {
      Dimension screenSize = ((Window) this).getToolkit().getScreenSize();
      int num1 = screenSize.width - 50;
      int num2 = screenSize.height - 50;
      int num3 = (int) ((Component) this).getSize().width;
      int num4 = (int) ((Component) this).getSize().height;
      if (x + num3 > num1)
      {
        x = 0;
        if (x + num3 > num1)
          num3 = num1;
      }
      if (y + num4 > num2)
      {
        y = 0;
        if (y + num4 > num2)
          num4 = num2;
      }
      ((Component) this).setSize(num3, num4);
      ((Component) this).setLocation(x, y);
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static DebugFrame()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public virtual object MemberwiseClone()
    {
      DebugFrame debugFrame = this;
      ObjectImpl.clone((object) debugFrame);
      return ((object) debugFrame).MemberwiseClone();
    }

    [Inner]
    [JavaFlags(34)]
    private class TabPane : Panel
    {
      private const long serialVersionUID = 1;
      private Rectangle[] tabs;
      private int panel;
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private DebugFrame this\u00240;

      public virtual int select(Point point)
      {
        for (int index = 0; index < this.tabs.Length; ++index)
        {
          if (this.tabs[index] != null && this.tabs[index].contains(point))
          {
            this.panel = index;
            ((Component) this).repaint();
            break;
          }
        }
        return this.panel;
      }

      public virtual Insets getInsets()
      {
        Insets insets = ((Container) this).getInsets();
        insets.left = (__Null) (insets.left + 10);
        insets.right = (__Null) (insets.right + 10);
        insets.top = (__Null) (insets.top + 30);
        insets.bottom = (__Null) (insets.bottom + 10);
        return insets;
      }

      public virtual void paint(Graphics g)
      {
        DebugInfo[] info = this.this\u00240.getInfo();
        if (info == null)
          return;
        if (this.tabs == null)
        {
          int length = this.this\u00240.getInfo().Length;
          this.tabs = length >= 0 ? new Rectangle[length] : throw new NegativeArraySizeException();
        }
        Dimension size = ((Component) this).getSize();
        g.setColor((Color) Color.gray);
        g.drawRect(0, 20, size.width - 1, size.height - 21);
        FontMetrics fontMetrics = g.getFontMetrics();
        int num1 = 0;
        int num2 = info.Length != 0 ? size.width / info.Length - 1 : (int) size.width;
        for (int index = 0; index < info.Length; ++index)
        {
          string str = info[index].getDebugTitle() ?? ObjectImpl.getClass((object) info[index]).getName();
          int num3 = Math.min(num2, fontMetrics.stringWidth(str) + 20);
          this.tabs[index] = new Rectangle(num1, 0, num3, 20);
          g.setColor((Color) Color.gray);
          g.drawRect(num1 + 0, 0, num3, 20);
          if (index == this.panel)
          {
            g.setColor((Color) Color.white);
            g.fillRect(num1 + 1, 1, num3 - 1, 20);
            g.setColor((Color) Color.black);
          }
          else
          {
            g.setColor((Color) Color.lightGray);
            g.fillRect(num1 + 1, 1, num3 - 1, 19);
            g.setColor((Color) Color.gray);
          }
          g.drawString(str, num1 + 9, 15);
          num1 += num3;
        }
        g.setColor((Color) Color.white);
        g.fillRect(num1 + 1, 1, size.width - num1, 19);
      }

      [JavaFlags(2)]
      public TabPane(DebugFrame _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.panel = 0;
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public virtual object MemberwiseClone()
      {
        DebugFrame.TabPane tabPane = this;
        ObjectImpl.clone((object) tabPane);
        return ((object) tabPane).MemberwiseClone();
      }
    }

    [Inner]
    [JavaFlags(32)]
    public class \u0031 : WindowAdapter
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private DebugFrame this\u00240;

      public virtual void windowClosing(WindowEvent e) => this.this\u00240.closeDialog();

      public \u0031(DebugFrame _param1)
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
        DebugFrame.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public virtual string ToString() => ObjectImpl.jloToString((object) this);
    }

    [Inner]
    [JavaFlags(32)]
    public class \u0032 : MouseAdapter
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private DebugFrame this\u00240;

      public virtual void mouseClicked(MouseEvent e)
      {
        this.this\u00240.panel = this.this\u00240.tabPane.select(e.getPoint());
        this.this\u00240.showDebugForPane();
      }

      public \u0032(DebugFrame _param1)
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
        DebugFrame.\u0032 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public virtual string ToString() => ObjectImpl.jloToString((object) this);
    }

    [JavaFlags(32)]
    [Inner]
    [JavaInterfaces("1;java/awt/event/ActionListener;")]
    public class \u0033 : ActionListener
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private DebugFrame this\u00240;

      public virtual void actionPerformed(ActionEvent e) => this.this\u00240.showDebugForPane();

      public \u0033(DebugFrame _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        DebugFrame.\u0033 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [Inner]
    [JavaFlags(32)]
    [JavaInterfaces("1;java/awt/event/ActionListener;")]
    public class \u0034 : ActionListener
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private DebugFrame this\u00240;

      public virtual void actionPerformed(ActionEvent e) => DebugOutput.print(new StringBuffer().append("Debug ").append(((Component) this.this\u00240.tabPane).getName()).ToString(), ((TextComponent) this.this\u00240.field).getText());

      public \u0034(DebugFrame _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        DebugFrame.\u0034 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [JavaInterfaces("1;java/awt/event/ActionListener;")]
    [JavaFlags(32)]
    [Inner]
    public class \u0035 : ActionListener
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private DebugFrame this\u00240;

      public virtual void actionPerformed(ActionEvent e) => DebugOutput.saveToFile("Save details", new StringBuffer().append("Debug ").append(((Component) this.this\u00240.tabPane).getName()).ToString(), ((TextComponent) this.this\u00240.field).getText());

      public \u0035(DebugFrame _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        DebugFrame.\u0035 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [JavaFlags(32)]
    [JavaInterfaces("1;java/awt/event/ActionListener;")]
    [Inner]
    public class \u0036 : ActionListener
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private DebugFrame this\u00240;

      public virtual void actionPerformed(ActionEvent e) => DebugOutput.saveToClipboard(((TextComponent) this.this\u00240.field).getText());

      public \u0036(DebugFrame _param1)
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
        DebugFrame.\u0036 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [JavaInterfaces("1;java/awt/event/ActionListener;")]
    [JavaFlags(32)]
    [Inner]
    public class \u0037 : ActionListener
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private DebugFrame this\u00240;

      public virtual void actionPerformed(ActionEvent e) => this.this\u00240.closeDialog();

      public \u0037(DebugFrame _param1)
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
        DebugFrame.\u0037 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
