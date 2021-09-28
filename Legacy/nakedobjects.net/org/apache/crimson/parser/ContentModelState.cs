// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.parser.ContentModelState
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.apache.crimson.parser
{
  [JavaFlags(32)]
  public class ContentModelState
  {
    private ContentModel model;
    private bool sawOne;
    private ContentModelState next;

    [JavaFlags(0)]
    public ContentModelState(ContentModel model)
      : this((object) model, (ContentModelState) null)
    {
    }

    private ContentModelState(object content, ContentModelState next)
    {
      this.model = (ContentModel) content;
      this.next = next;
      this.sawOne = false;
    }

    [JavaFlags(0)]
    public virtual bool terminate()
    {
      switch (this.model.type)
      {
        case char.MinValue:
          return false;
        case '*':
        case '?':
          return this.next == null || this.next.terminate();
        case '+':
          if (!this.sawOne && !this.model.empty())
            return false;
          goto case '*';
        case ',':
          ContentModel contentModel = this.model;
          while (contentModel != null && contentModel.empty())
            contentModel = contentModel.next;
          return contentModel == null && (this.next == null || this.next.terminate());
        case '|':
          return this.model.empty() && (this.next == null || this.next.terminate());
        default:
          throw new InternalError();
      }
    }

    [JavaFlags(0)]
    [JavaThrownExceptions("1;org/apache/crimson/parser/EndOfInputException;")]
    public virtual ContentModelState advance(string token)
    {
      switch (this.model.type)
      {
        case char.MinValue:
          if (this.model.content == (object) token)
            return this.next;
          break;
        case '*':
        case '+':
          if (this.model.first(token))
          {
            this.sawOne = true;
            return \u003CVerifierFix\u003E.isInstanceOfString(this.model.content) ? this : new ContentModelState(this.model.content, this).advance(token);
          }
          if ((this.model.type == '*' || this.sawOne) && this.next != null)
            return this.next.advance(token);
          break;
        case ',':
          if (this.model.first(token))
          {
            if (this.model.type == char.MinValue)
              return this.next;
            ContentModelState contentModelState;
            if (this.model.next == null)
            {
              contentModelState = new ContentModelState(this.model.content, this.next);
            }
            else
            {
              contentModelState = new ContentModelState(this.model.content, this);
              this.model = this.model.next;
            }
            return contentModelState.advance(token);
          }
          if (this.model.empty() && this.next != null)
            return this.next.advance(token);
          break;
        case '?':
          if (this.model.first(token))
            return \u003CVerifierFix\u003E.isInstanceOfString(this.model.content) ? this.next : new ContentModelState(this.model.content, this.next).advance(token);
          if (this.next != null)
            return this.next.advance(token);
          break;
        case '|':
          for (ContentModel contentModel = this.model; contentModel != null; contentModel = contentModel.next)
          {
            if (\u003CVerifierFix\u003E.isInstanceOfString(contentModel.content))
            {
              if ((object) token == contentModel.content)
                return this.next;
            }
            else if (((ContentModel) contentModel.content).first(token))
              return new ContentModelState(contentModel.content, this.next).advance(token);
          }
          if (this.model.empty() && this.next != null)
            return this.next.advance(token);
          break;
      }
      throw new EndOfInputException();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      ContentModelState contentModelState = this;
      ObjectImpl.clone((object) contentModelState);
      return ((object) contentModelState).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
