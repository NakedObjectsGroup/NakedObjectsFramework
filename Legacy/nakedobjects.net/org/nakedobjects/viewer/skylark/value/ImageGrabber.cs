// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.value.ImageGrabber
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt.image;
using java.lang;
using java.util;
using org.nakedobjects.@object.value;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.value
{
  [JavaInterfaces("1;java/awt/image/ImageConsumer;")]
  [JavaFlags(32)]
  public class ImageGrabber : ImageConsumer
  {
    private static readonly org.apache.log4j.Logger LOG;
    private int[][] pix;
    private ImageValue value;

    public ImageGrabber(ImageValue value) => this.value = value;

    public virtual void imageComplete(int status)
    {
      if (ImageGrabber.LOG.isDebugEnabled())
        ImageGrabber.LOG.debug((object) new StringBuffer().append("complete ").append(status).ToString());
      if (status == 1)
        throw new NakedObjectRuntimeException("Failed to load image");
      if (status != 2 && status != 3)
        return;
      this.value.setImage(this.pix);
    }

    public virtual void setHints(int hintflags)
    {
      if (!ImageGrabber.LOG.isDebugEnabled())
        return;
      ImageGrabber.LOG.debug((object) new StringBuffer().append("hint ").append(hintflags).ToString());
    }

    public virtual void setDimensions(int width, int height)
    {
      if (ImageGrabber.LOG.isDebugEnabled())
        ImageGrabber.LOG.debug((object) new StringBuffer().append("dimensions ").append(width).append(" x ").append(height).ToString());
      int length = height;
      this.pix = length >= 0 ? new int[length][] : throw new NegativeArraySizeException();
    }

    public virtual void setPixels(
      int x,
      int y,
      int w,
      int h,
      ColorModel model,
      sbyte[] pixels,
      int off,
      int scansize)
    {
      if (ImageGrabber.LOG.isDebugEnabled())
        ImageGrabber.LOG.debug((object) new StringBuffer().append("pixels ").append(x).append(",").append(y).append(" ").append(w).append(nameof (x)).append(h).append(" ").append(pixels.Length).append(" ").append(off).append(" ").append(scansize).ToString());
      int[][] pix = this.pix;
      int index1 = y;
      int length = w;
      int[] numArray = length >= 0 ? new int[length] : throw new NegativeArraySizeException();
      pix[index1] = numArray;
      for (int index2 = 0; index2 < pixels.Length; ++index2)
        this.pix[y][index2] = model.getRGB((int) pixels[index2]);
    }

    public virtual void setPixels(
      int x,
      int y,
      int w,
      int h,
      ColorModel model,
      int[] pixels,
      int off,
      int scansize)
    {
      if (ImageGrabber.LOG.isDebugEnabled())
        ImageGrabber.LOG.debug((object) new StringBuffer().append("pixels (ints) ").append(pixels.Length).append(" ").append(off).ToString());
      this.pix[y] = pixels;
    }

    public virtual void setColorModel(ColorModel model)
    {
      if (!ImageGrabber.LOG.isDebugEnabled())
        return;
      ImageGrabber.LOG.debug((object) new StringBuffer().append("model ").append((object) model).ToString());
    }

    public virtual void setProperties(Hashtable props)
    {
      if (!ImageGrabber.LOG.isDebugEnabled())
        return;
      ImageGrabber.LOG.debug((object) new StringBuffer().append("properties ").append((object) props).ToString());
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static ImageGrabber()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      ImageGrabber imageGrabber = this;
      ObjectImpl.clone((object) imageGrabber);
      return ((object) imageGrabber).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
