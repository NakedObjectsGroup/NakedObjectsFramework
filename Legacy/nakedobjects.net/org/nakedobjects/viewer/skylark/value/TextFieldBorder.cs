// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.value.TextFieldBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.value
{
  public class TextFieldBorder : AbstractBorder
  {
    public TextFieldBorder(View view)
      : base(view)
    {
      this.top = this.bottom = this.left = this.right = 2;
    }

    public override void draw(Canvas canvas)
    {
      int num = this.getSize().getHeight() - 2;
      int width = this.getSize().getWidth();
      canvas.drawSolidRectangle(0, 1, width - 1, num - 2, Style.WHITE);
      canvas.drawRectangle(0, 1, width - 3, num - 2, Style.SECONDARY1);
      canvas.drawRectangle(1, 2, width - 1, num - 2, Style.WHITE);
      base.draw(canvas);
    }
  }
}
