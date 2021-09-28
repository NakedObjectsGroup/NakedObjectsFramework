// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.PrintOption
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.util;
using org.nakedobjects.@object.control;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.core
{
  public class PrintOption : AbstractUserAction
  {
    private readonly int HEIGHT;
    private readonly int LEFT;

    public PrintOption()
      : base("Print...")
    {
      this.HEIGHT = 60;
      this.LEFT = 60;
    }

    public override Consent disabled(View component) => (Consent) Allow.DEFAULT;

    public override void execute(Workspace workspace, View view, Location at)
    {
      Frame frame = new Frame();
      PrintJob printJob = Toolkit.getDefaultToolkit().getPrintJob(frame, "Print object", (Properties) null);
      if (printJob != null)
      {
        Graphics graphics = printJob.getGraphics();
        Dimension pageDimension = printJob.getPageDimension();
        if (graphics != null)
        {
          graphics.translate(60, 60);
          graphics.drawRect(0, 0, pageDimension.width - 60 - 1, pageDimension.height - 60 - 1);
          view.print((Canvas) new PrintOption.PrintCanvas(this, graphics, view));
          graphics.dispose();
        }
        printJob.end();
      }
      frame.dispose();
    }

    [JavaFlags(34)]
    [Inner]
    private class PrintCanvas : DrawingCanvas
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private PrintOption this\u00240;

      [JavaFlags(0)]
      public PrintCanvas(PrintOption _param1, Graphics g, View view)
        : base(g, 0, 0, view.getSize().getWidth(), view.getSize().getHeight())
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }
  }
}
