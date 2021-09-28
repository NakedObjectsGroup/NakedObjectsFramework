// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.help.HelpManagerAssist
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object.help;
using org.nakedobjects.@object.reflect;

namespace org.nakedobjects.@object.help
{
  [JavaInterfaces("1;org/nakedobjects/object/help/HelpManager;")]
  public class HelpManagerAssist : HelpManager
  {
    private HelpManager decorated;
    private bool showIdentifier;

    public virtual void setShowIdentifier(bool showIdentifier) => this.showIdentifier = showIdentifier;

    public virtual void setDecorated(HelpManager decorated) => this.decorated = decorated;

    public virtual string help(MemberIdentifier identifier)
    {
      string str = "";
      if (this.decorated != null)
        str = this.decorated.help(identifier);
      return this.showIdentifier ? new StringBuffer().append(identifier.ToString()).append("\n").ToString() : new StringBuffer().append("").append(str).ToString();
    }

    public virtual void init()
    {
    }

    public virtual void shutdown()
    {
    }

    public HelpManagerAssist() => this.showIdentifier = false;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      HelpManagerAssist helpManagerAssist = this;
      ObjectImpl.clone((object) helpManagerAssist);
      return ((object) helpManagerAssist).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
