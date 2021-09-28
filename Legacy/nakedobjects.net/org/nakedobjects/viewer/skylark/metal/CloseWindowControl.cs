// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.CloseWindowControl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.metal
{
  public class CloseWindowControl : WindowControl
  {
    public CloseWindowControl(View target)
      : base((UserAction) new CloseViewOption(), target)
    {
    }

    public override void draw(Canvas canvas)
    {
      int x = 0;
      int y = 0;
      canvas.drawRectangle(x + 1, y + 1, 14, 12, Style.WHITE);
      canvas.drawRectangle(x, y, 14, 12, Style.SECONDARY1);
      Color black = Style.BLACK;
      canvas.drawLine(x + 4, y + 3, x + 10, y + 9, black);
      canvas.drawLine(x + 5, y + 3, x + 11, y + 9, black);
      canvas.drawLine(x + 10, y + 3, x + 4, y + 9, black);
      canvas.drawLine(x + 11, y + 3, x + 5, y + 9, black);
    }
  }
}
