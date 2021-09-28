// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.ButtonBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.metal
{
  public class ButtonBorder : AbstractBorder
  {
    private const int BUTTON_SPACING = 5;
    private readonly Button[] buttons;
    private ButtonAction defaultAction;

    public ButtonBorder(ButtonAction[] actions, View view)
      : base(view)
    {
      if (actions.Length > 0)
      {
        int length = actions.Length;
        this.buttons = length >= 0 ? new Button[length] : throw new NegativeArraySizeException();
        for (int index = 0; index < actions.Length; ++index)
        {
          ButtonAction action = actions[index];
          this.buttons[index] = new Button(action, view);
          if (action.isDefault())
            this.defaultAction = action;
        }
        this.bottom = 1 + View.VPADDING + this.buttons[0].getRequiredSize(new Size()).getHeight() + View.VPADDING;
      }
      else
      {
        int length = 0;
        this.buttons = length >= 0 ? new Button[length] : throw new NegativeArraySizeException();
      }
    }

    public override void draw(Canvas canvas)
    {
      for (int index = 0; index < this.buttons.Length; ++index)
      {
        Canvas subcanvas = canvas.createSubcanvas(this.buttons[index].getBounds());
        this.buttons[index].draw(subcanvas);
        int width = this.buttons[index].getSize().getWidth();
        subcanvas.offset(5 + width, 0);
      }
      base.draw(canvas);
    }

    public override void firstClick(Click click)
    {
      View view = this.overButton(click.getLocation());
      if (view == null)
        base.firstClick(click);
      else
        view.firstClick(click);
    }

    public virtual Button[] getButtons() => this.buttons;

    public override Size getRequiredSize(Size maximumSize)
    {
      Size requiredSize = base.getRequiredSize(maximumSize);
      requiredSize.ensureWidth(this.totalButtonWidth());
      return requiredSize;
    }

    public override View identify(Location location)
    {
      for (int index = 0; index < this.buttons.Length; ++index)
      {
        Button button = this.buttons[index];
        if (button.getBounds().contains(location))
          return (View) button;
      }
      return base.identify(location);
    }

    public override void keyPressed(KeyboardAction key)
    {
      if (key.getKeyCode() == 10 && this.defaultAction != null && this.defaultAction.disabled(this.getView()).isAllowed())
      {
        key.consume();
        this.defaultAction.execute(this.getWorkspace(), this.getView(), this.getLocation());
      }
      base.keyPressed(key);
    }

    public virtual void layout(int width)
    {
      int x = width / 2 - this.totalButtonWidth() / 2;
      int y = 0;
      if (this.buttons.Length > 0)
        y = this.getSize().getHeight() - View.VPADDING - this.buttons[0].getRequiredSize(new Size()).getHeight();
      for (int index = 0; index < this.buttons.Length; ++index)
      {
        this.buttons[index] = this.buttons[index];
        this.buttons[index].setSize(this.buttons[index].getRequiredSize(new Size()));
        this.buttons[index].setLocation(new Location(x, y));
        x = x + this.buttons[index].getSize().getWidth() + 5;
      }
    }

    public override bool containsFocus()
    {
      for (int index = 0; index < this.buttons.Length; ++index)
      {
        if (this.buttons[index].containsFocus())
          return true;
      }
      return base.containsFocus();
    }

    public override void mouseDown(Click click)
    {
      View view = this.overButton(click.getLocation());
      if (view == null)
        base.mouseDown(click);
      else
        view.mouseDown(click);
    }

    public override void mouseUp(Click click)
    {
      View view = this.overButton(click.getLocation());
      if (view == null)
        base.mouseUp(click);
      else
        view.mouseUp(click);
    }

    private View overButton(Location location)
    {
      for (int index = 0; index < this.buttons.Length; ++index)
      {
        Button button = this.buttons[index];
        if (button.getBounds().contains(location))
          return (View) button;
      }
      return (View) null;
    }

    public override void secondClick(Click click)
    {
      if (this.overButton(click.getLocation()) != null)
        return;
      base.secondClick(click);
    }

    public override void setBounds(Bounds bounds)
    {
      base.setBounds(bounds);
      this.layout(bounds.getWidth());
    }

    public override void setSize(Size size)
    {
      base.setSize(size);
      this.layout(size.getWidth());
    }

    public override void thirdClick(Click click)
    {
      if (this.overButton(click.getLocation()) != null)
        return;
      base.thirdClick(click);
    }

    private int totalButtonWidth()
    {
      int num = 0;
      for (int index = 0; index < this.buttons.Length; ++index)
      {
        int width = this.buttons[index].getRequiredSize(new Size()).getWidth();
        num = num + (index <= 0 ? 0 : 5) + width;
      }
      return num;
    }
  }
}
