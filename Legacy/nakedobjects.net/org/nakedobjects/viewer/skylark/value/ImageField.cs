// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.value.ImageField
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.awt.datatransfer;
using java.awt.image;
using java.lang;
using java.net;
using org.nakedobjects.@object.value;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.core;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace org.nakedobjects.viewer.skylark.value
{
  public class ImageField : AbstractField
  {
    private static readonly org.apache.log4j.Logger LOG;
    private Image image;

    public override bool canFocus() => true;

    public ImageField(Content content, ViewSpecification specification, ViewAxis axis)
      : base(content, specification, axis)
    {
    }

    public override void keyPressed(KeyboardAction key)
    {
      if (!this.canChangeValue())
        return;
      int keyCode = key.getKeyCode();
      switch (keyCode)
      {
        case 16:
          break;
        case 17:
          break;
        case 18:
          break;
        default:
          bool flag = (key.getModifiers() & 2) > 0;
          switch (keyCode)
          {
            case 67:
              if (!flag)
                return;
              key.consume();
              this.copy();
              return;
            case 86:
              if (!flag)
                return;
              key.consume();
              this.paste();
              return;
            default:
              return;
          }
      }
    }

    private void paste()
    {
      Transferable contents = Toolkit.getDefaultToolkit().getSystemClipboard().getContents((object) this);
      try
      {
        if (contents.isDataFlavorSupported((DataFlavor) DataFlavor.stringFlavor))
        {
          string filename = \u003CVerifierFix\u003E.genCastToString(contents.getTransferData((DataFlavor) DataFlavor.stringFlavor));
          if (ImageField.LOG.isDebugEnabled())
            ImageField.LOG.debug((object) new StringBuffer().append("pasted image from ").append(filename).ToString());
          this.loadImageFromFile(filename);
        }
        else
        {
          if (!ImageField.LOG.isInfoEnabled())
            return;
          ImageField.LOG.info((object) new StringBuffer().append("unsupported paste operation ").append((object) contents).ToString());
        }
      }
      catch (Exception ex)
      {
        Throwable throwable = ThrowableWrapper.wrapThrowable(ex);
        ImageField.LOG.error((object) new StringBuffer().append("invalid paste operation ").append((object) throwable).ToString());
      }
    }

    private void loadImageFromFile(string filename)
    {
      try
      {
        Image image = Toolkit.getDefaultToolkit().getImage(new URL(filename));
        ImageValue naked = (ImageValue) this.getContent().getNaked();
        ImageProducer source = image.getSource();
        if (ImageField.LOG.isDebugEnabled())
          ImageField.LOG.debug((object) new StringBuffer().append("producer ").append((object) source).ToString());
        source.startProduction((ImageConsumer) new ImageGrabber(naked));
        this.image = (Image) null;
      }
      catch (MalformedURLException ex)
      {
        throw new NakedObjectRuntimeException(new StringBuffer().append("Failed to load image from ").append(filename).ToString());
      }
    }

    private void createImage()
    {
      if (this.image != null)
        return;
      ImageProducer imageProducer = (ImageProducer) new ImageField.\u0031(this, ((ImageValue) this.getContent().getNaked()).getImage());
      this.image = (Image) new AwtImage(Toolkit.getDefaultToolkit().createImage(imageProducer));
    }

    private void copy()
    {
    }

    public override void contentMenuOptions(UserActionSet options)
    {
      base.contentMenuOptions(options);
      options.add((UserAction) new ImageField.\u0032(this, "Load image from file..."));
    }

    public override void draw(Canvas canvas)
    {
      Color color = !this.hasFocus() ? (!this.getParent().getState().isObjectIdentified() ? (!this.getParent().getState().isRootViewIdentified() ? Style.SECONDARY1 : Style.PRIMARY2) : Style.IDENTIFIED) : Style.PRIMARY1;
      int y1 = 0;
      int x1 = 0;
      Size size1 = this.getSize();
      int width1 = size1.getWidth() - 1;
      int height1 = size1.getHeight() - 1;
      canvas.drawRectangle(x1, y1, width1, height1, color);
      int x2 = x1 + 1;
      int y2 = y1 + 1;
      int width2 = width1 - 1;
      int height2 = height1 - 1;
      this.createImage();
      if (this.image == null)
        return;
      Size size2 = this.image.getSize();
      if (size2.getWidth() <= width2 && size2.getHeight() <= height2)
        canvas.drawIcon(this.image, x2, y2);
      else
        canvas.drawIcon(this.image, x2, y2, width2, height2);
    }

    public override int getBaseline() => View.VPADDING + Style.NORMAL.getAscent();

    public override Size getMaximumSize() => new Size(120, 100);

    [JavaFlags(4)]
    public override void save()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static ImageField()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(41)]
    public class Specification : AbstractFieldSpecification
    {
      public override bool canDisplay(Content content) => content.isValue() && content.getNaked() is ImageValue;

      public override View createView(Content content, ViewAxis axis) => (View) new ImageField(content, (ViewSpecification) this, axis);

      public override string getName() => "Image";
    }

    [JavaFlags(32)]
    [Inner]
    [JavaInterfaces("1;java/awt/image/ImageProducer;")]
    public new class \u0031 : ImageProducer
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private ImageField this\u00240;
      [JavaFlags(16)]
      public readonly int[][] pixels_\u003E;

      public virtual void addConsumer(ImageConsumer ic)
      {
        if (!ImageField.LOG.isDebugEnabled())
          return;
        ImageField.LOG.debug((object) new StringBuffer().append("add ").append((object) ic).ToString());
      }

      public virtual void removeConsumer(ImageConsumer ic)
      {
        if (!ImageField.LOG.isDebugEnabled())
          return;
        ImageField.LOG.debug((object) new StringBuffer().append("remove ").append((object) ic).ToString());
      }

      public virtual void requestTopDownLeftRightResend(ImageConsumer ic)
      {
        if (!ImageField.LOG.isDebugEnabled())
          return;
        ImageField.LOG.debug((object) new StringBuffer().append("request top down ").append((object) ic).ToString());
      }

      public virtual void startProduction(ImageConsumer ic)
      {
        int length1 = this.pixels_\u003E.Length;
        if (length1 <= 0)
          return;
        if (ImageField.LOG.isDebugEnabled())
          ImageField.LOG.debug((object) new StringBuffer().append("start ").append((object) ic).ToString());
        int length2 = this.pixels_\u003E[0].Length;
        ic.setDimensions(length1, length2);
        ColorModel rgBdefault = ColorModel.getRGBdefault();
        ic.setColorModel(rgBdefault);
        ic.setHints(24);
        for (int index = 0; index < this.pixels_\u003E.Length; ++index)
          ic.setPixels(0, index, length1, 1, rgBdefault, this.pixels_\u003E[index], 0, 0);
        ic.imageComplete(3);
      }

      public virtual bool isConsumer(ImageConsumer ic)
      {
        if (ImageField.LOG.isDebugEnabled())
          ImageField.LOG.debug((object) new StringBuffer().append("is consumer ").append((object) ic).ToString());
        return false;
      }

      public \u0031(ImageField _param1, [In] int[][] obj1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.pixels_\u003E = obj1;
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        ImageField.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [Inner]
    [JavaFlags(32)]
    public new class \u0032 : AbstractUserAction
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private ImageField this\u00240;

      public override void execute(Workspace workspace, View view, Location at)
      {
        string str = this.this\u00240.getViewManager().selectFilePath("Load image", ".");
        this.this\u00240.loadImageFromFile(new StringBuffer().append("file://").append(str).ToString());
      }

      public \u0032(ImageField _param1, string dummy0)
        : base(dummy0)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }
  }
}
