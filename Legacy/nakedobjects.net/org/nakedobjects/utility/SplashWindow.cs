// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.SplashWindow
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using com.ms.vjsharp.util;
using java.awt;
using java.awt.image;
using java.lang;
using System.ComponentModel;

namespace org.nakedobjects.utility
{
  [JavaInterfaces("1;java/lang/Runnable;")]
  public class SplashWindow : Window, Runnable
  {
    private const string IMAGE_DIRECTORY = "images";
    private static readonly org.apache.log4j.Logger LOG;
    private const string LOGO_TEXT = "Naked Objects";
    private int delay;
    private readonly Font textFont;
    private readonly int height;
    private readonly int textLineHeight;
    private readonly int titleLineHeight;
    private Image logo;
    private readonly int PADDING;
    private Frame parent;
    private readonly int width;
    private Font titleFont;
    private int left;
    private Font logoFont;

    private static Image loadAsFile(string filename)
    {
      // ISSUE: unable to decompile the method.
    }

    private static Image loadAsResource(string @ref)
    {
      // ISSUE: unable to decompile the method.
    }

    public static Image loadImage(string name) => SplashWindow.loadAsResource(name) ?? SplashWindow.loadAsFile(name);

    public SplashWindow()
      : base(new Frame())
    {
      this.PADDING = 9;
      this.parent = (Frame) ((Component) this).getParent();
      this.logo = SplashWindow.loadImage(AboutNakedObjects.getImageName());
      this.textFont = new Font("SansSerif", 0, 10);
      this.titleFont = new Font("SansSerif", 1, 11);
      this.logoFont = new Font("Serif", 0, 36);
      this.textLineHeight = Utilities.doubleToInt((double) ((Component) this).getFontMetrics(this.textFont).getHeight() * 0.85);
      this.titleLineHeight = Utilities.doubleToInt((double) ((Component) this).getFontMetrics(this.titleFont).getHeight() * 1.2);
      int num1 = 0;
      int num2;
      int num3;
      if (this.logo != null)
      {
        num2 = this.logo.getWidth((ImageObserver) this);
        num3 = num1 + this.logo.getHeight((ImageObserver) this);
      }
      else
      {
        FontMetrics fontMetrics = ((Component) this).getFontMetrics(this.logoFont);
        num2 = fontMetrics.stringWidth("Naked Objects");
        num3 = fontMetrics.getHeight();
      }
      int num4 = num3 + 9;
      Dimension dimension = this.textBounds();
      int num5 = Math.max(num2, (int) dimension.width);
      int num6 = 9 + (num4 + dimension.height) + 9;
      int num7 = 9 + num5 + 9;
      ((Component) this).setSize(num7, num6);
      this.height = num6;
      this.width = num7;
      this.left = num7 / 2 - dimension.width / 2;
      this.setupCenterLocation();
      this.show();
      this.toFront();
    }

    private void setupCenterLocation()
    {
      Dimension screenSize = this.getToolkit().getScreenSize();
      int num1 = screenSize.width / 2 - this.width / 2;
      if (screenSize.width / screenSize.height >= 2)
      {
        int num2 = screenSize.width / screenSize.height * 2;
        num1 = screenSize.width / num2 - this.width / 2;
      }
      int num3 = screenSize.height / 2 - this.width / 2;
      ((Component) this).setLocation(num1, num3);
      ((Container) this).setBackground((Color) Color.black);
    }

    private Dimension textBounds()
    {
      FontMetrics fontMetrics1 = ((Component) this).getFontMetrics(this.textFont);
      FontMetrics fontMetrics2 = ((Component) this).getFontMetrics(this.titleFont);
      int num1 = 0;
      int num2 = fontMetrics2.stringWidth(AboutNakedObjects.getFrameworkName());
      int num3 = num1 + this.titleLineHeight;
      int num4 = Math.max(num2, fontMetrics1.stringWidth(AboutNakedObjects.getFrameworkCopyrightNotice()));
      int num5 = num3 + this.textLineHeight;
      int num6 = Math.max(num4, fontMetrics1.stringWidth(this.frameworkVersion()));
      int num7 = num5 + this.textLineHeight;
      string applicationName = AboutNakedObjects.getApplicationName();
      if (applicationName != null)
      {
        num6 = Math.max(num6, fontMetrics2.stringWidth(applicationName));
        num7 += this.titleLineHeight;
      }
      string applicationCopyrightNotice = AboutNakedObjects.getApplicationCopyrightNotice();
      if (applicationCopyrightNotice != null)
      {
        num6 = Math.max(num6, fontMetrics1.stringWidth(applicationCopyrightNotice));
        num7 += this.textLineHeight;
      }
      string applicationVersion = AboutNakedObjects.getApplicationVersion();
      if (applicationVersion != null)
      {
        num6 = Math.max(num6, fontMetrics1.stringWidth(applicationVersion));
        num7 += this.textLineHeight;
      }
      return new Dimension(num6, num7);
    }

    public virtual void paint(Graphics g)
    {
      g.setColor((Color) Color.gray);
      g.drawRect(0, 0, this.width - 1, this.height - 1);
      if (this.logo != null)
      {
        g.drawImage(this.logo, 9, 9, (ImageObserver) this);
      }
      else
      {
        g.setFont(this.logoFont);
        FontMetrics fontMetrics = g.getFontMetrics();
        g.drawString("Naked Objects", 9, 9 + fontMetrics.getAscent());
      }
      int num1 = this.height - 9 - ((Component) this).getFontMetrics(this.textFont).getDescent();
      g.setFont(this.textFont);
      g.drawString(this.frameworkVersion(), this.left, num1);
      int num2 = num1 - this.textLineHeight;
      g.drawString(AboutNakedObjects.getFrameworkCopyrightNotice(), this.left, num2);
      int num3 = num2 - this.textLineHeight;
      g.setFont(this.titleFont);
      g.drawString(AboutNakedObjects.getFrameworkName(), this.left, num3);
      int num4 = num3 - this.titleLineHeight;
      g.setFont(this.textFont);
      string applicationVersion = AboutNakedObjects.getApplicationVersion();
      if (applicationVersion != null)
      {
        g.drawString(applicationVersion, this.left, num4);
        num4 -= this.textLineHeight;
      }
      string applicationCopyrightNotice = AboutNakedObjects.getApplicationCopyrightNotice();
      if (applicationCopyrightNotice != null)
      {
        g.drawString(applicationCopyrightNotice, this.left, num4);
        num4 -= this.textLineHeight;
      }
      string applicationName = AboutNakedObjects.getApplicationName();
      if (applicationName == null)
        return;
      g.setFont(this.titleFont);
      g.drawString(applicationName, this.left, num4);
    }

    private string frameworkVersion() => new StringBuffer().append(AboutNakedObjects.getFrameworkVersion()).append(" (").append(AboutNakedObjects.getFrameworkBuild()).append(")").ToString();

    public virtual void removeAfterDelay(int seconds)
    {
      this.delay = seconds * 1000;
      new Thread((Runnable) this).start();
    }

    public virtual void removeImmediately()
    {
      this.hide();
      this.dispose();
      this.parent.dispose();
    }

    public virtual void run()
    {
      try
      {
        Thread.sleep((long) this.delay);
      }
      catch (InterruptedException ex)
      {
      }
      this.removeImmediately();
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static SplashWindow()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public virtual object MemberwiseClone()
    {
      SplashWindow splashWindow = this;
      ObjectImpl.clone((object) splashWindow);
      return ((object) splashWindow).MemberwiseClone();
    }
  }
}
