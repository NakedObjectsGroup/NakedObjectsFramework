// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.TextMessageContent
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.skylark.metal
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/metal/MessageContent;")]
  public class TextMessageContent : MessageContent
  {
    [JavaFlags(20)]
    public readonly string message;
    [JavaFlags(20)]
    public readonly string heading;
    [JavaFlags(20)]
    public readonly string detail;
    [JavaFlags(20)]
    public readonly string title;

    public TextMessageContent(string title, string message)
    {
      int num = StringImpl.indexOf(message, 58);
      if (num > 2)
      {
        this.heading = StringImpl.trim(StringImpl.substring(message, 0, num));
        this.message = StringImpl.trim(StringImpl.substring(message, num + 1));
      }
      else
      {
        this.heading = "";
        this.message = message;
      }
      this.title = title;
      this.detail = (string) null;
    }

    public virtual string getMessage() => this.message;

    public virtual string getDetail() => this.detail;

    public virtual string getIcon() => "message";

    public virtual Consent canDrop(Content sourceContent) => (Consent) Veto.DEFAULT;

    public virtual void contentMenuOptions(UserActionSet options)
    {
    }

    public virtual void debugDetails(DebugString debug)
    {
    }

    public virtual Naked drop(Content sourceContent) => (Naked) null;

    public virtual string getDescription() => "";

    public virtual string getHelp() => "";

    public virtual string getIconName() => "";

    public virtual Image getIconPicture(int iconHeight) => (Image) null;

    public virtual string getId() => "message-exception";

    public virtual Naked getNaked() => (Naked) null;

    public virtual NakedObjectSpecification getSpecification() => (NakedObjectSpecification) null;

    public virtual bool isCollection() => false;

    public virtual bool isDerived() => false;

    public virtual bool isObject() => false;

    public virtual bool isPersistable() => false;

    public virtual bool isTransient() => false;

    public virtual bool isValue() => false;

    [JavaThrownExceptions("1;org/nakedobjects/object/InvalidEntryException;")]
    public virtual void parseTextEntry(string entryText)
    {
    }

    public virtual string title() => this.heading;

    public virtual void viewMenuOptions(UserActionSet options)
    {
    }

    public virtual string windowTitle() => this.title;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      TextMessageContent textMessageContent = this;
      ObjectImpl.clone((object) textMessageContent);
      return ((object) textMessageContent).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
