// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.WindowBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.special;
using org.nakedobjects.viewer.skylark.util;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.metal
{
  public class WindowBorder : AbstractWindowBorder
  {
    private static readonly IconizeViewOption iconizeOption;
    private View resizeBorder;

    public WindowBorder(View wrappedView, bool scrollable)
      : this(wrappedView, scrollable, true)
    {
    }

    public WindowBorder(View wrappedView, bool scrollable, bool resizable)
      : base(WindowBorder.addTransientBorderIfNeccessary(!scrollable ? wrappedView : (View) new ScrollBorder(wrappedView)))
    {
      if (this.isTransient())
      {
        int length = 1;
        WindowControl[] controls = length >= 0 ? new WindowControl[length] : throw new NegativeArraySizeException();
        controls[0] = (WindowControl) new CloseWindowControl((View) this);
        this.setControls(controls);
      }
      else
      {
        int length = 3;
        WindowControl[] controls = length >= 0 ? new WindowControl[length] : throw new NegativeArraySizeException();
        controls[0] = (WindowControl) new IconizeWindowControl((View) this);
        controls[1] = (WindowControl) new ResizeWindowControl((View) this);
        controls[2] = (WindowControl) new CloseWindowControl((View) this);
        this.setControls(controls);
      }
    }

    private static View addTransientBorderIfNeccessary(View view) => (View) new ObjectActionButtonBorder(view);

    public virtual View[] getButtons()
    {
      if (this.wrappedView is ButtonBorder)
        return (View[]) ((ButtonBorder) this.wrappedView).getButtons();
      int length = 0;
      return length >= 0 ? new View[length] : throw new NegativeArraySizeException();
    }

    public override void draw(Canvas canvas)
    {
      base.draw(canvas);
      if (!this.isTransient())
        return;
      int num = this.top - 5 - 2;
      int x = this.getSize().getWidth() - 50;
      Image icon = ImageFactory.getInstance().loadIcon("transient", num);
      if (icon == null)
        canvas.drawText("*", x, this.getBaseline(), Style.BLACK, Style.NORMAL);
      else
        canvas.drawIcon(icon, x, 6, num, num);
    }

    private bool isTransient()
    {
      Content content = this.getContent();
      return content.isPersistable() && content.isTransient();
    }

    public override void viewMenuOptions(UserActionSet menuOptions)
    {
      base.viewMenuOptions(menuOptions);
      menuOptions.add((UserAction) WindowBorder.iconizeOption);
    }

    public override void secondClick(Click click)
    {
      if (this.overBorder(click.getLocation()))
        this.minimize();
      else
        base.secondClick(click);
    }

    public override void minimize() => WindowBorder.iconizeOption.execute(this.getWorkspace(), this.getView(), this.getAbsoluteLocation());

    [JavaFlags(4)]
    public override string title() => this.getContent().windowTitle();

    public override string ToString() => new StringBuffer().append(this.wrappedView.ToString()).append("/WindowBorder [").append((object) this.getSpecification()).append("]").ToString();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static WindowBorder()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
