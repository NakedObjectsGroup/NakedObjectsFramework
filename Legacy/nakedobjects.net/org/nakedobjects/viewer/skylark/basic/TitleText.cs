// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.TitleText
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.basic
{
  public abstract class TitleText
  {
    private const int NO_MAX_WIDTH = -1;
    private readonly int ellipsisWidth;
    private readonly Text style;
    private readonly View view;
    private bool resolveFailure;

    public TitleText(View view, Text style)
    {
      this.view = view;
      this.style = style;
      this.ellipsisWidth = style.stringWidth("...");
    }

    public virtual void draw(Canvas canvas, int x, int baseline) => this.draw(canvas, x, baseline, -1);

    public virtual void draw(Canvas canvas, int x, int baseline, int maxWidth)
    {
      ViewState state = this.view.getState();
      Color color = !this.resolveFailure ? (!state.canDrop() ? (!state.cantDrop() ? (!state.isObjectIdentified() ? Style.BLACK : Style.PRIMARY1) : Style.INVALID) : Style.VALID) : Style.ERROR;
      int x1 = x;
      int y = baseline;
      string title = this.getTitle();
      if (maxWidth > 0 && this.style.stringWidth(title) > maxWidth)
      {
        int num1 = 0;
        int ellipsisWidth = this.ellipsisWidth;
        while (ellipsisWidth <= maxWidth)
        {
          char c = StringImpl.charAt(title, num1);
          ellipsisWidth += this.style.charWidth(c);
          ++num1;
        }
        int num2 = StringImpl.lastIndexOf(title, 32, num1 - 1);
        string str;
        if (num2 > 0)
        {
          while (num2 >= 0 && !Character.isLetterOrDigit(StringImpl.charAt(title, num2 - 1)))
            num2 += -1;
          str = StringImpl.substring(title, 0, num2);
        }
        else
          str = StringImpl.substring(title, 0, num1 - 1);
        title = new StringBuffer().append(str).append("...").ToString();
      }
      if (AbstractView.debug)
      {
        int width = this.style.stringWidth(title);
        canvas.drawDebugOutline(new Bounds(x1, y - this.style.getAscent(), width, this.style.getTextHeight()), baseline, Color.DEBUG_DRAW_BOUNDS);
      }
      canvas.drawText(title, x1, y, color, this.style);
    }

    public virtual Size getSize()
    {
      int textHeight = this.style.getTextHeight();
      return new Size(this.style.stringWidth(this.getTitle()), textHeight);
    }

    private string getTitle()
    {
      if (this.resolveFailure)
        return "Resolve Failure!";
      try
      {
        return this.title();
      }
      catch (ResolveException ex)
      {
        this.resolveFailure = true;
        return "Resolve Failure!";
      }
    }

    [JavaFlags(1028)]
    public abstract string title();

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("style", (object) this.style);
      return toString.ToString();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      TitleText titleText = this;
      ObjectImpl.clone((object) titleText);
      return ((object) titleText).MemberwiseClone();
    }
  }
}
