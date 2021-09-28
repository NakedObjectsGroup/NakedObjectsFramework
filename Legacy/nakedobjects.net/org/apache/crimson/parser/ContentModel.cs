// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.parser.ContentModel
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.apache.crimson.parser
{
  [JavaFlags(48)]
  public sealed class ContentModel
  {
    public char type;
    public object content;
    public ContentModel next;
    private SimpleHashtable cache;

    public ContentModel(string element)
    {
      this.cache = new SimpleHashtable();
      this.type = char.MinValue;
      this.content = (object) element;
    }

    public ContentModel(char type, ContentModel content)
    {
      this.cache = new SimpleHashtable();
      this.type = type;
      this.content = (object) content;
    }

    public virtual bool empty()
    {
      switch (this.type)
      {
        case char.MinValue:
        case '+':
          return false;
        case '*':
        case '?':
          return true;
        case ',':
          if (!(this.content is ContentModel) || !((ContentModel) this.content).empty())
            return false;
          for (ContentModel next = this.next; next != null; next = next.next)
          {
            if (!next.empty())
              return false;
          }
          return true;
        case '|':
          if (this.content is ContentModel && ((ContentModel) this.content).empty())
            return true;
          for (ContentModel next = this.next; next != null; next = next.next)
          {
            if (next.empty())
              return true;
          }
          return false;
        default:
          throw new InternalError();
      }
    }

    public virtual bool first(string token)
    {
      Boolean boolean = (Boolean) this.cache.get(token);
      if (boolean != null)
        return boolean.booleanValue();
      bool flag;
      switch (this.type)
      {
        case char.MinValue:
        case '*':
        case '+':
        case '?':
          flag = !\u003CVerifierFix\u003E.isInstanceOfString(this.content) ? ((ContentModel) this.content).first(token) : this.content == (object) token;
          break;
        case ',':
          flag = !\u003CVerifierFix\u003E.isInstanceOfString(this.content) ? ((ContentModel) this.content).first(token) || ((ContentModel) this.content).empty() && this.next != null && this.next.first(token) : this.content == (object) token;
          break;
        case '|':
          flag = \u003CVerifierFix\u003E.isInstanceOfString(this.content) && this.content == (object) token || ((ContentModel) this.content).first(token) || this.next != null && this.next.first(token);
          break;
        default:
          throw new InternalError();
      }
      if (flag)
        this.cache.put((object) token, (object) Boolean.TRUE);
      else
        this.cache.put((object) token, (object) Boolean.FALSE);
      return flag;
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      ContentModel contentModel = this;
      ObjectImpl.clone((object) contentModel);
      return ((object) contentModel).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
