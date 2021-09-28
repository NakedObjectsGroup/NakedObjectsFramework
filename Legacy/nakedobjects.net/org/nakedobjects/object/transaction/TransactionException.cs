// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.transaction.TransactionException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object.transaction;

namespace org.nakedobjects.@object.transaction
{
  public class TransactionException : RuntimeException
  {
    public TransactionException()
    {
    }

    public TransactionException(string s)
      : base(s)
    {
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public virtual object MemberwiseClone()
    {
      TransactionException transactionException = this;
      ObjectImpl.clone((object) transactionException);
      return ((object) transactionException).MemberwiseClone();
    }
  }
}
