// Decompiled with JetBrains decompiler
// Type: junit.awtui.ProgressBar
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.lang;

namespace junit.awtui
{
  public class ProgressBar : Canvas
  {
    public bool fError;
    public int fTotal;
    public int fProgress;
    public int fProgressX;

    public ProgressBar()
    {
      this.fError = false;
      this.fTotal = 0;
      this.fProgress = 0;
      this.fProgressX = 0;
      ((Component) this).setSize(20, 30);
    }

    private Color getStatusColor() => this.fError ? (Color) Color.red : (Color) Color.green;

    public virtual void paint(Graphics g)
    {
      this.paintBackground(g);
      this.paintStatus(g);
    }

    public virtual void paintBackground(Graphics g)
    {
      g.setColor((Color) SystemColor.control);
      Rectangle bounds = ((Component) this).getBounds();
      g.fillRect(0, 0, (int) bounds.width, (int) bounds.height);
      g.setColor((Color) Color.darkGray);
      g.drawLine(0, 0, bounds.width - 1, 0);
      g.drawLine(0, 0, 0, bounds.height - 1);
      g.setColor((Color) Color.white);
      g.drawLine(bounds.width - 1, 0, bounds.width - 1, bounds.height - 1);
      g.drawLine(0, bounds.height - 1, bounds.width - 1, bounds.height - 1);
    }

    public virtual void paintStatus(Graphics g)
    {
      g.setColor(this.getStatusColor());
      Rectangle rectangle = new Rectangle(0, 0, this.fProgressX, (int) ((Component) this).getBounds().height);
      g.fillRect(1, 1, rectangle.width - 1, rectangle.height - 2);
    }

    private void paintStep(int startX, int endX) => ((Component) this).repaint(startX, 1, endX - startX, ((Component) this).getBounds().height - 2);

    public virtual void reset()
    {
      this.fProgressX = 1;
      this.fProgress = 0;
      this.fError = false;
      this.paint(((Component) this).getGraphics());
    }

    public virtual int scale(int value) => this.fTotal > 0 ? Math.max(1, value * (((Component) this).getBounds().width - 1) / this.fTotal) : value;

    public virtual void setBounds(int x, int y, int w, int h)
    {
      ((Component) this).setBounds(x, y, w, h);
      this.fProgressX = this.scale(this.fProgress);
    }

    public virtual void start(int total)
    {
      this.fTotal = total;
      this.reset();
    }

    public virtual void step(bool successful)
    {
      ++this.fProgress;
      int startX = this.fProgressX;
      this.fProgressX = this.scale(this.fProgress);
      if (!this.fError && !successful)
      {
        this.fError = true;
        startX = 1;
      }
      this.paintStep(startX, this.fProgressX);
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public virtual object MemberwiseClone()
    {
      ProgressBar progressBar = this;
      ObjectImpl.clone((object) progressBar);
      return ((object) progressBar).MemberwiseClone();
    }
  }
}
