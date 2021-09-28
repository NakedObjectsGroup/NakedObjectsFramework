// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.util.LogoBackground
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.util
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/Background;")]
  public class LogoBackground : Background
  {
    private static readonly org.apache.log4j.Logger LOG;
    private static readonly string PARAMETER_BASE;
    private Location location;
    private Image logo;
    private Size logoSize;

    public LogoBackground()
    {
      NakedObjectConfiguration configuration = NakedObjects.getConfiguration();
      string path = configuration.getString(new StringBuffer().append(LogoBackground.PARAMETER_BASE).append("image").ToString(), "logo.gif");
      this.logo = ImageFactory.getInstance().createImage(path);
      if (this.logo == null)
      {
        if (!LogoBackground.LOG.isWarnEnabled())
          return;
        LogoBackground.LOG.warn((object) "logo image not found");
      }
      else
      {
        this.location = new Location();
        this.location.setX(configuration.getInteger(new StringBuffer().append(LogoBackground.PARAMETER_BASE).append("position.x").ToString(), 0));
        this.location.setY(configuration.getInteger(new StringBuffer().append(LogoBackground.PARAMETER_BASE).append("position.y").ToString(), 0));
        this.logoSize = new Size();
        this.logoSize.setWidth(configuration.getInteger(new StringBuffer().append(LogoBackground.PARAMETER_BASE).append("size.width").ToString(), this.logo.getWidth()));
        this.logoSize.setHeight(configuration.getInteger(new StringBuffer().append(LogoBackground.PARAMETER_BASE).append("size.height").ToString(), this.logo.getHeight()));
      }
    }

    public virtual void draw(Canvas canvas, Size viewSize)
    {
      if (this.logo == null)
        return;
      int x;
      int y;
      if (this.location.getX() == 0 && this.location.getY() == 0)
      {
        x = viewSize.getWidth() / 2 - this.logoSize.getWidth() / 2;
        y = viewSize.getHeight() / 2 - this.logoSize.getHeight() / 2;
      }
      else
      {
        x = this.location.getX() < 0 ? viewSize.getWidth() + this.location.getX() - this.logoSize.getWidth() : this.location.getX();
        y = this.location.getY() < 0 ? viewSize.getHeight() + this.location.getY() - this.logoSize.getHeight() : this.location.getY();
      }
      canvas.drawIcon(this.logo, x, y, this.logoSize.getWidth(), this.logoSize.getHeight());
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static LogoBackground()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      LogoBackground logoBackground = this;
      ObjectImpl.clone((object) logoBackground);
      return ((object) logoBackground).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
