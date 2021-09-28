// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.Skylark
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.viewer.skylark.util;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark
{
  public class Skylark
  {
    private static Skylark instance;
    [JavaFlags(4)]
    public ContentFactory contentFactory;
    [JavaFlags(4)]
    public ViewFactory viewFactory;

    public static ContentFactory getContentFactory() => Skylark.getInstance().contentFactory;

    private static Skylark getInstance() => Skylark.instance;

    public static ViewFactory getViewFactory() => Skylark.getInstance().viewFactory;

    public Skylark()
    {
      this.contentFactory = new ContentFactory();
      this.viewFactory = new ViewFactory();
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static Skylark()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      Skylark skylark = this;
      ObjectImpl.clone((object) skylark);
      return ((object) skylark).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
