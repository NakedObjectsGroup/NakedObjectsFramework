// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.DebugContent
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.skylark.core
{
  [JavaInterfaces("1;org/nakedobjects/utility/DebugInfo;")]
  public class DebugContent : DebugInfo
  {
    private readonly View view;

    public DebugContent(View display) => this.view = display;

    public virtual void debugData(DebugString debug)
    {
      Content content = this.view.getContent();
      if (content != null)
      {
        string name = ObjectImpl.getClass((object) content).getName();
        string str = StringImpl.substring(name, StringImpl.lastIndexOf(name, 46) + 1);
        debug.appendln("Content", (object) str);
        debug.indent();
        content.debugDetails(debug);
        debug.appendln("Icon name", (object) content.getIconName());
        debug.appendln("Icon ", (object) content.getIconPicture(32));
        debug.appendln("Window title", (object) content.windowTitle());
        debug.appendln("Object", content.isObject());
        debug.appendln("Collection", content.isCollection());
        debug.appendln("Value", content.isValue());
        debug.unindent();
      }
      else
        debug.appendln("Content", (object) "none");
      debug.blankLine();
    }

    public virtual string getDebugTitle() => "Content";

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      DebugContent debugContent = this;
      ObjectImpl.clone((object) debugContent);
      return ((object) debugContent).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
