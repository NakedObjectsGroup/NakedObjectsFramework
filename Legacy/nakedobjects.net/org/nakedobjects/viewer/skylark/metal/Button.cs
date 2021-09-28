// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.Button
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

namespace org.nakedobjects.viewer.skylark.metal
{
  public class Button : AbstractControlView
  {
    private const int TEXT_PADDING = 12;
    private readonly int buttonHeight;
    private bool over;
    private bool pressed;

    public Button(ButtonAction action, View target)
      : base((UserAction) action, target)
    {
      this.buttonHeight = 4 + Style.CONTROL.getTextHeight() + 4;
    }

    public override bool containsFocus() => this.hasFocus();

    public override void draw(Canvas canvas)
    {
      int x = 0;
      int y = 0;
      View parent = this.getParent();
      string name = this.action.getName(parent);
      bool flag = this.action.disabled(parent).isVetoed();
      Color color1 = !flag ? Style.BLACK : Style.DISABLED_MENU;
      Color color2 = !flag ? Style.SECONDARY2 : Style.SECONDARY3;
      Text control = Style.CONTROL;
      int width = 12 + control.stringWidth(name) + 12;
      canvas.clearBackground((View) this, Style.SECONDARY3);
      canvas.drawRectangle(x, y, width, this.buttonHeight, ((this.over ? 1 : 0) & ((flag ? 1 : 0) ^ 1)) == 0 ? Style.BLACK : Style.PRIMARY1);
      canvas.draw3DRectangle(x + 1, y + 1, width - 2, this.buttonHeight - 2, color2, ((this.pressed ? 1 : 0) ^ 1) != 0);
      canvas.draw3DRectangle(x + 2, y + 2, width - 4, this.buttonHeight - 4, color2, ((this.pressed ? 1 : 0) ^ 1) != 0);
      if (((ButtonAction) this.action).isDefault())
        canvas.drawRectangle(x + 3, y + 3, width - 6, this.buttonHeight - 6, color2);
      if (this.hasFocus())
        canvas.drawRectangle(x + 3, y + 3, width - 6, this.buttonHeight - 6, Style.WHITE);
      canvas.drawText(name, x + 12, y + this.buttonHeight / 2 + control.getMidPoint(), color1, control);
    }

    public override void entered()
    {
      this.over = true;
      this.pressed = false;
      this.markDamaged();
      base.entered();
    }

    public override void exited()
    {
      this.over = false;
      this.pressed = false;
      this.markDamaged();
      base.exited();
    }

    public override Size getMaximumSize()
    {
      string name = this.action.getName(this.getView());
      return new Size(12 + Style.CONTROL.stringWidth(name) + 12, this.buttonHeight);
    }

    public override void mouseDown(Click click)
    {
      if (this.action.disabled(this.getParent()).isVetoed())
        return;
      this.pressed = true;
      this.markDamaged();
    }

    public override void mouseUp(Click click)
    {
      if (this.action.disabled(this.getParent()).isVetoed())
        return;
      this.pressed = false;
      this.markDamaged();
    }
  }
}
