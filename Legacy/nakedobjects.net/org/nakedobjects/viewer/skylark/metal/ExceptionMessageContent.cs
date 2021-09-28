// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.ExceptionMessageContent
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.skylark.metal
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/metal/MessageContent;")]
  public class ExceptionMessageContent : MessageContent
  {
    [JavaFlags(4)]
    public string message;
    [JavaFlags(4)]
    public string name;
    [JavaFlags(4)]
    public string trace;
    [JavaFlags(4)]
    public string title;
    private readonly string icon;

    public ExceptionMessageContent(Throwable error)
    {
      string name = ObjectImpl.getClass((object) error).getName();
      this.name = NameConvertor.naturalName(StringImpl.substring(name, StringImpl.lastIndexOf(name, 46) + 1));
      this.message = error.getMessage();
      this.trace = ExceptionHelper.exceptionTraceAsString(error);
      if (this.name == null)
        this.name = "";
      if (this.message == null)
        this.message = "";
      if (this.trace == null)
        this.trace = "";
      else if (!StringImpl.equals(this.message, (object) ""))
        this.trace = this.trace.Replace(this.message, "");
      switch (error)
      {
        case NakedObjectApplicationException _:
          this.title = "Application Exception";
          this.icon = "application-exception";
          break;
        case ConcurrencyException _:
          this.title = "Concurrency Exception";
          this.icon = "concurrency-exception";
          break;
        default:
          this.title = "System Error";
          this.icon = "system-error";
          break;
      }
    }

    public virtual string getMessage() => this.message;

    public virtual string getDetail() => this.trace;

    public virtual string getIconName() => this.icon;

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

    public virtual string title() => this.name;

    public virtual void viewMenuOptions(UserActionSet options)
    {
    }

    public virtual string windowTitle() => this.title;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ExceptionMessageContent exceptionMessageContent = this;
      ObjectImpl.clone((object) exceptionMessageContent);
      return ((object) exceptionMessageContent).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
