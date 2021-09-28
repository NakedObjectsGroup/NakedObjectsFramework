// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.util.TemplateImage
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.awt.image;
using java.lang;
using org.apache.log4j;
using org.nakedobjects.viewer.skylark.core;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.util
{
  [JavaFlags(32)]
  public class TemplateImage
  {
    private static readonly Logger LOG;
    private Image image;

    [JavaFlags(8)]
    public static TemplateImage create(Image image) => image == null ? (TemplateImage) null : new TemplateImage(image);

    private TemplateImage(Image image) => this.image = image != null ? image : throw new NullPointerException();

    public virtual Image getFullSizeImage() => (Image) new AwtImage(this.image);

    public virtual Image getIcon(int height, Color tint)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static TemplateImage()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      TemplateImage templateImage = this;
      ObjectImpl.clone((object) templateImage);
      return ((object) templateImage).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [Inner]
    [JavaFlags(34)]
    private class Filter : RGBImageFilter
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private TemplateImage this\u00240;

      public virtual int filterRGB(int x, int y, int rgb) => 16777215 - rgb;

      [JavaFlags(2)]
      public Filter(TemplateImage _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaFlags(4227073)]
      public virtual string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
