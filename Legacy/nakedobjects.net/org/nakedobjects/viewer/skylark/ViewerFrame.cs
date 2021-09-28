// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.ViewerFrame
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.awt.@event;
using java.lang;
using org.nakedobjects.utility;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace org.nakedobjects.viewer.skylark
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/RenderingArea;")]
  public class ViewerFrame : Frame, RenderingArea
  {
    private const string DEFAULT_TITLE = "Naked Objects";
    private const long serialVersionUID = 1;
    private Viewer viewer;

    public ViewerFrame()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(17)]
    public virtual void paint(Graphics g) => this.update(g);

    public virtual void quit() => this.viewer.close();

    public virtual void update(Graphics g) => this.viewer.paint(g);

    public virtual void setViewer(Viewer viewer) => this.viewer = viewer;

    public virtual void init()
    {
      ((Window) this).addWindowListener((WindowListener) new ViewerFrame.\u0031(this, this.getTitle()));
      ((Component) this).addComponentListener((ComponentListener) new ViewerFrame.\u0032(this));
    }

    public virtual Viewer Viewer
    {
      set => this.setViewer(value);
    }

    public virtual string Title
    {
      set => this.setTitle(value);
    }

    public virtual void setTitle(string title)
    {
      string applicationName = AboutNakedObjects.getApplicationName();
      base.setTitle(title != null ? title : applicationName ?? "Naked Objects");
    }

    public virtual string selectFilePath(string title, string directory)
    {
      FileDialog fileDialog = new FileDialog((Frame) this, title);
      ((Dialog) fileDialog).show();
      return new StringBuffer().append(fileDialog.getDirectory()).append(fileDialog.getFile()).ToString();
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public virtual object MemberwiseClone()
    {
      ViewerFrame viewerFrame = this;
      ObjectImpl.clone((object) viewerFrame);
      return ((object) viewerFrame).MemberwiseClone();
    }

    [JavaFlags(32)]
    [Inner]
    public class \u0031 : WindowAdapter
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private ViewerFrame this\u00240;
      [JavaFlags(16)]
      public readonly string title_\u003E;

      public virtual void windowClosing(WindowEvent e)
      {
        ShutdownDialog shutdownDialog = new ShutdownDialog(this.this\u00240, this.title_\u003E);
      }

      public \u0031(ViewerFrame _param1, [In] string obj1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.title_\u003E = obj1;
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public virtual object MemberwiseClone()
      {
        ViewerFrame.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public virtual string ToString() => ObjectImpl.jloToString((object) this);
    }

    [Inner]
    [JavaFlags(32)]
    public class \u0032 : ComponentAdapter
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private ViewerFrame this\u00240;

      public virtual void componentResized(ComponentEvent e) => this.this\u00240.viewer.sizeChange();

      public \u0032(ViewerFrame _param1)
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
        ViewerFrame.\u0032 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public virtual string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
