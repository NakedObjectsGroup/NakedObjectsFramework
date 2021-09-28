// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.util.ImageFactory
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.viewer.skylark.util
{
  public class ImageFactory
  {
    private const string FALLBACK_IMAGE = "Unknown.gif";
    private static readonly string FALLBACK_PARAM;
    private const string IMAGE_DIRECTORY = "images/";
    private static readonly string IMAGE_DIRECTORY_PARAM;
    private static ImageFactory instance;
    private string directory;
    private TemplateImageLoader loader;
    private Hashtable templateImages;

    public static ImageFactory getInstance()
    {
      if (ImageFactory.instance == null)
        ImageFactory.instance = new ImageFactory();
      return ImageFactory.instance;
    }

    private ImageFactory()
    {
      this.templateImages = new Hashtable();
      this.loader = new TemplateImageLoader();
    }

    [JavaFlags(18)]
    private Image createIcon(string fullName, string shortName, int height, Color tint)
    {
      if (fullName == null)
        return (Image) null;
      string str1 = new StringBuffer().append(fullName).append("/").append(height).append("/").append((object) tint).ToString();
      Image mage;
      if (this.templateImages.containsKey((object) str1))
      {
        mage = (Image) this.templateImages.get((object) str1);
      }
      else
      {
        string str2 = this.directory();
        TemplateImage templateImage = this.loader.getTemplateImage(new StringBuffer().append(str2).append(fullName).ToString());
        if (templateImage == null && shortName != null)
          templateImage = this.loader.getTemplateImage(new StringBuffer().append(str2).append(shortName).ToString());
        if (templateImage == null)
        {
          mage = (Image) null;
        }
        else
        {
          mage = templateImage.getIcon(height, tint);
          this.templateImages.put((object) str1, (object) mage);
        }
      }
      return mage;
    }

    public virtual Image createImage(string path) => this.loader.getTemplateImage(new StringBuffer().append(this.directory()).append(path).ToString())?.getFullSizeImage();

    private string directory()
    {
      if (this.directory == null)
        this.directory = NakedObjects.getConfiguration().getString(ImageFactory.IMAGE_DIRECTORY_PARAM, "images/");
      return this.directory;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual bool isIconAvailable(string name, int height, Color tint)
    {
      string str = new StringBuffer().append(name).append("/").append(height).append("/").append((object) tint).ToString();
      if (this.templateImages.containsKey((object) str))
        return true;
      TemplateImage templateImage = this.loader.getTemplateImage(new StringBuffer().append(this.directory()).append(name).ToString());
      if (templateImage == null)
        return false;
      Image icon = templateImage.getIcon(height, tint);
      this.templateImages.put((object) str, (object) icon);
      return true;
    }

    public virtual Image loadObjectIcon(
      NakedObjectSpecification specification,
      string type,
      int iconHeight)
    {
      return this.loadIcon(specification, type, iconHeight) ?? this.loadFallbackIcon(iconHeight);
    }

    public virtual Image loadClassIcon(
      NakedObjectSpecification specification,
      string type,
      int iconHeight)
    {
      return this.loadIcon(specification, new StringBuffer().append(type).append("_class").ToString(), iconHeight) ?? this.loadObjectIcon(specification, type, iconHeight);
    }

    private Image loadIcon(
      NakedObjectSpecification specification,
      string extension,
      int iconHeight)
    {
      return this.loadIcon(new StringBuffer().append(StringImpl.replace(specification.getFullName(), '.', '_')).append(extension).ToString(), new StringBuffer().append(StringImpl.replace(specification.getShortName(), '.', '_')).append(extension).ToString(), iconHeight) ?? this.loadIconForSuperClass(specification, extension, iconHeight);
    }

    private Image loadIconForSuperClass(
      NakedObjectSpecification specification,
      string extension,
      int iconHeight)
    {
      NakedObjectSpecification specification1 = specification.superclass();
      return specification1 != null ? this.loadIcon(specification1, extension, iconHeight) : (Image) null;
    }

    public virtual Image loadIcon(string iconName, int iconHeight) => ImageFactory.getInstance().createIcon(iconName, (string) null, iconHeight, (Color) null);

    public virtual Image loadIcon(string fullIconName, string shortIconName, int iconHeight) => this.createIcon(fullIconName, shortIconName, iconHeight, (Color) null);

    public virtual Image loadFallbackIcon(int iconHeight) => this.loadIcon(NakedObjects.getConfiguration().getString(ImageFactory.FALLBACK_PARAM, "Unknown.gif"), iconHeight);

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static ImageFactory()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ImageFactory imageFactory = this;
      ObjectImpl.clone((object) imageFactory);
      return ((object) imageFactory).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
