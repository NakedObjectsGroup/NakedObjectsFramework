// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.value.ColorFieldOverlay
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.viewer.skylark.core;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.value
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/View;")]
  [JavaFlags(32)]
  public class ColorFieldOverlay : AbstractView, View
  {
    private static readonly int[] colors;
    private const int COLUMNS = 4;
    private const int ROWS = 4;
    private const int ROW_HEIGHT = 18;
    private const int COLUMN_WIDTH = 23;
    private ColorField field;

    public ColorFieldOverlay(ColorField field)
      : base(field.getContent(), (ViewSpecification) null, (ViewAxis) null)
    {
      this.field = field;
    }

    public override Size getMaximumSize() => new Size(92, 72);

    public override void draw(Canvas canvas)
    {
      canvas.drawSolidRectangle(0, 0, 91, 71, Style.SECONDARY3);
      for (int index = 0; index < ColorFieldOverlay.colors.Length; ++index)
      {
        Color color = new Color(ColorFieldOverlay.colors[index]);
        int y = index / 4 * 18;
        int x = index % 4 * 23;
        canvas.drawSolidRectangle(x, y, 22, 17, color);
      }
      canvas.drawRectangle(0, 0, 91, 71, Style.PRIMARY2);
    }

    public override void firstClick(Click click)
    {
      int x = click.getLocation().getX();
      int y = click.getLocation().getY();
      this.field.setColor(ColorFieldOverlay.colors[y / 18 * 4 + x / 23]);
      this.dispose();
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static ColorFieldOverlay()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
