// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.Color
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.lang;
using org.nakedobjects.@object;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark
{
  public class Color
  {
    public static readonly Color DEBUG_BASELINE;
    public static readonly Color DEBUG_DRAW_BOUNDS;
    public static readonly Color DEBUG_VIEW_BOUNDS;
    public static readonly Color DEBUG_REPAINT_BOUNDS;
    public static readonly Color DEBUG_BORDER_BOUNDS;
    public static readonly Color RED;
    public static readonly Color GREEN;
    public static readonly Color BLUE;
    public static readonly Color BLACK;
    public static readonly Color WHITE;
    public static readonly Color GRAY;
    public static readonly Color LIGHT_GRAY;
    public static readonly Color ORANGE;
    public static readonly Color YELLOW;
    [JavaFlags(28)]
    public static readonly Color NULL;
    private static readonly string PROPERTY_STEM;
    private Color color;
    private string name;

    public Color(int rgbColor)
      : this(new Color(rgbColor))
    {
    }

    private Color(Color color) => this.color = color;

    [JavaFlags(0)]
    public Color(string propertyName, string defaultColor)
    {
      this.name = propertyName;
      this.color = NakedObjects.getConfiguration().getColor(new StringBuffer().append(Color.PROPERTY_STEM).append(propertyName).ToString(), Color.decode(defaultColor));
    }

    [JavaFlags(0)]
    public Color(string propertyName, Color defaultColor)
    {
      this.name = propertyName;
      this.color = NakedObjects.getConfiguration().getColor(new StringBuffer().append(Color.PROPERTY_STEM).append(propertyName).ToString(), defaultColor.getAwtColor());
    }

    public virtual Color brighter() => new Color(this.color.brighter());

    public virtual Color darker() => new Color(this.color.darker());

    public virtual Color getAwtColor() => this.color;

    public override string ToString() => new StringBuffer().append(this.name).append(" (").append("#").append(Integer.toHexString(this.color.getRGB())).append(")").ToString();

    public virtual string getName() => this.name;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static Color()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      Color color = this;
      ObjectImpl.clone((object) color);
      return ((object) color).MemberwiseClone();
    }
  }
}
