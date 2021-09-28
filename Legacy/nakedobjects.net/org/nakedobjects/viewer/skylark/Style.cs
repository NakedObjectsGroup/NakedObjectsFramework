// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.Style
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark
{
  public class Style
  {
    public static readonly Color APPLICATION_BACKGROUND;
    public static readonly Color WINDOW_BACKGROUND;
    public static readonly Color BLACK;
    public static readonly Color CONTENT_MENU;
    public static readonly Color DISABLED_MENU;
    public static readonly Color IDENTIFIED;
    public static readonly Color ERROR;
    public static readonly Color INVALID;
    public static readonly Color NORMAL_MENU;
    public static readonly Color PRIMARY1;
    public static readonly Color PRIMARY2;
    public static readonly Color PRIMARY3;
    public static readonly Color ACTIVE;
    public static readonly Color TEXT_EDIT;
    public static readonly Color REVERSE_MENU;
    public static readonly Color SECONDARY1;
    public static readonly Color SECONDARY2;
    public static readonly Color SECONDARY3;
    public static readonly Color RESIZE;
    public static readonly Color VALID;
    public static readonly Color VALUE_MENU;
    public static readonly Color VIEW_MENU;
    public static readonly Color WHITE;
    public static readonly Color WORKSPACE_MENU;
    public static readonly Color OUT_OF_SYNCH;
    public static readonly Text CONTROL;
    public static readonly Text SMALL;
    public static readonly Text SYSTEM;
    public static readonly Text USER;
    public static readonly Text TITLE;
    public static readonly Text STATUS;
    public static readonly Text CLASS;
    public static readonly Text DEBUG;
    public static readonly Text LABEL;
    public static readonly Text MENU;
    public static readonly Text NORMAL;
    private static int defaultBaseline;
    private static int defaultFieldHeight;
    private static Hashtable backgrounds;

    public static int defaultBaseline()
    {
      if (Style.defaultBaseline == 0)
      {
        int ascent = Style.NORMAL.getAscent();
        Style.defaultBaseline = View.VPADDING + ascent;
      }
      return Style.defaultBaseline;
    }

    public static int defaultFieldHeight()
    {
      if (Style.defaultFieldHeight == 0)
        Style.defaultFieldHeight = Style.NORMAL.getTextHeight();
      return Style.defaultFieldHeight;
    }

    public static Color background(ViewSpecification specification) => Style.background(specification, "");

    public static Color background(ViewSpecification specification, string part)
    {
      string propertyName = new StringBuffer().append(ObjectImpl.getClass((object) specification).getName()).append(".background").append(part == null || StringImpl.length(part) == 0 ? "" : new StringBuffer().append(".").append(part).ToString()).ToString();
      Color color = (Color) Style.backgrounds.get((object) propertyName);
      if (color == null)
      {
        color = new Color(propertyName, Style.WINDOW_BACKGROUND);
        Style.backgrounds.put((object) propertyName, (object) color);
      }
      return color;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static Style()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Style style = this;
      ObjectImpl.clone((object) style);
      return ((object) style).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
