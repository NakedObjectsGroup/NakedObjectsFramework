// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.AwtImage
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.awt.image;
using java.lang;

namespace org.nakedobjects.viewer.skylark.core
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/Image;")]
  public class AwtImage : Image
  {
    [JavaFlags(0)]
    public Image iconImage;

    public AwtImage(Image iconImage) => this.iconImage = iconImage != null ? iconImage : throw new NullPointerException();

    public virtual int getHeight() => this.iconImage.getHeight((ImageObserver) null);

    public virtual int getWidth() => this.iconImage.getWidth((ImageObserver) null);

    public virtual Size getSize() => new Size(this.getWidth(), this.getHeight());

    [JavaFlags(0)]
    public virtual Image getAwtImage() => this.iconImage;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AwtImage awtImage = this;
      ObjectImpl.clone((object) awtImage);
      return ((object) awtImage).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
