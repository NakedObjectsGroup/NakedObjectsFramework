// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.repository.SimpleMessageBroker
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.repository;

namespace org.nakedobjects.@object.repository
{
  [JavaInterfaces("1;org/nakedobjects/object/MessageBroker;")]
  public class SimpleMessageBroker : MessageBroker
  {
    private Vector messages;
    private Vector warnings;

    public virtual string[] getMessages() => this.convert(this.messages);

    private string[] convert(Vector items)
    {
      int length = items.size();
      string[] strArray1 = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
      int num1 = 0;
      Enumeration enumeration = items.elements();
      while (enumeration.hasMoreElements())
      {
        string[] strArray2 = strArray1;
        int num2;
        num1 = (num2 = num1) + 1;
        int index = num2;
        string str = enumeration.nextElement().ToString();
        strArray2[index] = str;
      }
      items.removeAllElements();
      return strArray1;
    }

    public virtual string[] getWarnings() => this.convert(this.warnings);

    public virtual void addWarning(string message) => this.warnings.addElement((object) message);

    public virtual void addMessage(string message) => this.messages.addElement((object) message);

    public SimpleMessageBroker()
    {
      this.messages = new Vector();
      this.warnings = new Vector();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      SimpleMessageBroker simpleMessageBroker = this;
      ObjectImpl.clone((object) simpleMessageBroker);
      return ((object) simpleMessageBroker).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
