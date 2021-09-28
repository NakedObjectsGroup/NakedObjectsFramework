// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.util.TemplateImageLoader
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.lang;
using java.util;
using org.apache.log4j;
using org.nakedobjects.@object;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.viewer.skylark.util
{
  [JavaFlags(32)]
  public class TemplateImageLoader
  {
    private static readonly string[] EXTENSIONS;
    private static readonly Logger LOG;
    private bool alsoLoadAsFiles;
    private Hashtable loadedImages;
    private Vector missingImages;

    [JavaFlags(0)]
    public TemplateImageLoader()
    {
      this.loadedImages = new Hashtable();
      this.missingImages = new Vector();
      this.alsoLoadAsFiles = NakedObjects.getConfiguration().getBoolean(new StringBuffer().append("nakedobjects.viewer.skylark.").append("load-images-from-files").ToString(), true);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual TemplateImage getTemplateImage(string path)
    {
      int num = StringImpl.lastIndexOf(path, 46);
      string str = num != -1 ? StringImpl.substring(path, 0, num) : path;
      if (this.loadedImages.contains((object) str))
        return (TemplateImage) this.loadedImages.get((object) str);
      if (this.missingImages.contains((object) str))
        return (TemplateImage) null;
      if (TemplateImageLoader.LOG.isDebugEnabled())
        TemplateImageLoader.LOG.debug((object) new StringBuffer().append("searching for image ").append(path).ToString());
      if (num >= 0)
        return TemplateImage.create(this.load(path));
      for (int index = 0; index < TemplateImageLoader.EXTENSIONS.Length; ++index)
      {
        Image image = this.load(new StringBuffer().append(str).append(".").append(TemplateImageLoader.EXTENSIONS[index]).ToString());
        if (image != null)
          return TemplateImage.create(image);
      }
      if (TemplateImageLoader.LOG.isDebugEnabled())
        TemplateImageLoader.LOG.debug((object) new StringBuffer().append("failed to find image for ").append(path).ToString());
      this.missingImages.addElement((object) str);
      return (TemplateImage) null;
    }

    private Image load(string path)
    {
      Image image = this.loadAsResource(path);
      if (image == null && this.alsoLoadAsFiles)
        image = this.loadAsFile(path);
      return image;
    }

    private Image loadAsFile(string path)
    {
      // ISSUE: unable to decompile the method.
    }

    private Image loadAsResource(string path)
    {
      // ISSUE: unable to decompile the method.
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static TemplateImageLoader()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      TemplateImageLoader templateImageLoader = this;
      ObjectImpl.clone((object) templateImageLoader);
      return ((object) templateImageLoader).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
