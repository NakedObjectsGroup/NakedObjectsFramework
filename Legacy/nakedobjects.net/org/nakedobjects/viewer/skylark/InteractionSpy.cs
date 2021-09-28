// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.InteractionSpy
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.awt.@event;
using java.lang;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark
{
  public class InteractionSpy
  {
    private int actionCount;
    private string damagedArea;
    private int @event;
    private string[][] label;
    private InteractionSpy.SpyFrame frame;
    private string[] trace;
    private int traceIndex;
    private bool isVisible;

    public InteractionSpy()
    {
      int length1 = 20;
      int length2 = 2;
      this.label = length1 >= 0 && length2 >= 0 ? new string[length2, length1][] : throw new NegativeArraySizeException();
      int length3 = 60;
      this.trace = length3 >= 0 ? new string[length3] : throw new NegativeArraySizeException();
    }

    public virtual void addAction(string action)
    {
      if (!this.isVisible)
        return;
      int actionCount;
      this.actionCount = (actionCount = this.actionCount) + 1;
      this.set(actionCount, "Action", (object) action);
    }

    public virtual void addDamagedArea(Bounds bounds)
    {
      if (!this.isVisible)
        return;
      this.damagedArea = new StringBuffer().append(this.damagedArea).append((object) bounds).append("; ").ToString();
      this.set(7, "Damaged areas", (object) this.damagedArea);
    }

    public virtual void addTrace(string message)
    {
      if (!this.isVisible || this.traceIndex >= this.trace.Length)
        return;
      this.trace[this.traceIndex] = message;
      ++this.traceIndex;
    }

    public virtual void addTrace(View view, string message, object @object)
    {
      if (!this.isVisible || this.traceIndex >= this.trace.Length)
        return;
      this.trace[this.traceIndex] = new StringBuffer().append(ObjectImpl.getClass((object) view).getName()).append(" ").append(message).append(": ").append(@object).ToString();
      ++this.traceIndex;
    }

    public virtual void close()
    {
      if (!this.isVisible)
        return;
      ((Window) this.frame).hide();
      this.frame.dispose();
      this.isVisible = false;
    }

    public virtual void reset()
    {
      if (!this.isVisible)
        return;
      ++this.@event;
      this.traceIndex = 0;
      this.actionCount = 8;
      this.damagedArea = "";
      this.setDownAt((Location) null);
      for (int actionCount = this.actionCount; actionCount < this.label[0].Length; ++actionCount)
      {
        this.label[0][actionCount] = (string) null;
        this.label[1][actionCount] = (string) null;
      }
    }

    private void set(int index, string label, object debug)
    {
      if (this.frame == null)
        return;
      this.label[0][index] = debug != null ? new StringBuffer().append(label).append(":").ToString() : (string) null;
      this.label[1][index] = debug?.ToString();
      ((Component) this.frame).repaint();
    }

    public virtual void setAbsoluteLocation(Location absoluteLocation)
    {
      if (!this.isVisible)
        return;
      this.set(6, "Absolute view location", (object) absoluteLocation);
    }

    public virtual void setDownAt(Location downAt)
    {
      if (!this.isVisible)
        return;
      this.set(0, "Down at", (object) downAt);
    }

    public virtual void setLocationInView(Location internalLocation)
    {
      if (!this.isVisible)
        return;
      this.set(3, "Relative mouse location", (object) internalLocation);
    }

    public virtual void setLocationInViewer(Location mouseLocation)
    {
      if (!this.isVisible)
        return;
      this.set(1, "Mouse location", (object) mouseLocation);
    }

    public virtual void setOver(object data)
    {
      if (!this.isVisible)
        return;
      this.set(2, "Mouse over", data);
    }

    public virtual void setType(ViewAreaType type)
    {
      if (!this.isVisible)
        return;
      this.set(4, "Area type", (object) type);
    }

    public virtual void setViewLocation(Location locationWithinViewer)
    {
      if (!this.isVisible)
        return;
      this.set(5, "View location", (object) locationWithinViewer);
    }

    public virtual void open()
    {
      if (this.isVisible)
        return;
      this.frame = new InteractionSpy.SpyFrame(this);
      ((Component) this.frame).setBounds(10, 10, 800, 500);
      ((Window) this.frame).show();
      this.isVisible = true;
    }

    public virtual bool isVisible() => this.isVisible;

    public virtual void redraw(Rectangle redrawArea, int redrawCount)
    {
      this.set(8, "Redraw", (object) new StringBuffer().append("#").append(redrawCount).append("  ").append((object) redrawArea).ToString());
      this.damagedArea = "";
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      InteractionSpy interactionSpy = this;
      ObjectImpl.clone((object) interactionSpy);
      return ((object) interactionSpy).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [Inner]
    [JavaFlags(34)]
    private class SpyFrame : Frame
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private InteractionSpy this\u00240;

      public SpyFrame(InteractionSpy _param1)
        : base("View/Interaction Spy")
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        ((Window) this).addWindowListener((WindowListener) new InteractionSpy.SpyFrame.\u0031(this));
      }

      public virtual void paint(Graphics g)
      {
        int num1 = ((Container) this).getInsets().top + 15;
        g.drawString(new StringBuffer().append("Event ").append(this.this\u00240.@event).ToString(), 10, num1);
        int num2 = num1 + 18;
        for (int index = 0; index < this.this\u00240.label[0].Length; ++index)
        {
          if (this.this\u00240.label[0][index] != null)
          {
            g.drawString(this.this\u00240.label[0][index], 10, num2);
            g.drawString(this.this\u00240.label[1][index], 150, num2);
          }
          num2 += 12;
        }
        int num3 = num2 + 6;
        for (int index = 0; index < this.this\u00240.traceIndex; ++index)
        {
          if (this.this\u00240.trace[index] != null)
            g.drawString(this.this\u00240.trace[index], 10, num3);
          num3 += 12;
        }
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public virtual object MemberwiseClone()
      {
        InteractionSpy.SpyFrame spyFrame = this;
        ObjectImpl.clone((object) spyFrame);
        return ((object) spyFrame).MemberwiseClone();
      }

      [JavaFlags(32)]
      [Inner]
      public class \u0031 : WindowAdapter
      {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JavaFlags(32770)]
        private InteractionSpy.SpyFrame this\u00240;

        public virtual void windowClosing(WindowEvent e) => this.this\u00240.this\u00240.close();

        public \u0031(InteractionSpy.SpyFrame _param1)
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
          InteractionSpy.SpyFrame.\u0031 obj = this;
          ObjectImpl.clone((object) obj);
          return ((object) obj).MemberwiseClone();
        }

        [JavaFlags(4227073)]
        public virtual string ToString() => ObjectImpl.jloToString((object) this);
      }
    }
  }
}
