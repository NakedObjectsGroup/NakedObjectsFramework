// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.java.JavaResult
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.util;

namespace org.nakedobjects.distribution.java
{
  [JavaInterfaces("1;org/nakedobjects/distribution/ServerActionResultData;")]
  public class JavaResult : ServerActionResultData
  {
    private readonly Data result;
    private readonly ObjectData[] updatesData;
    private readonly ObjectData persistedTarget;
    private readonly ObjectData[] persistedParameters;
    private string[] warnings;
    private string[] messages;

    public JavaResult(
      Data result,
      ObjectData[] updatesData,
      ObjectData persistedTarget,
      ObjectData[] persistedParameters,
      string[] messages,
      string[] warnings)
    {
      this.result = result;
      this.updatesData = updatesData;
      this.persistedTarget = persistedTarget;
      this.persistedParameters = persistedParameters;
      this.messages = messages;
      this.warnings = warnings;
    }

    public virtual Data getReturn() => this.result;

    public virtual ObjectData getPersistedTarget() => this.persistedTarget;

    public virtual ObjectData[] getPersistedParameters() => this.persistedParameters;

    public virtual ObjectData[] getUpdates() => this.updatesData;

    public virtual string[] getMessages() => this.messages;

    public virtual string[] getWarnings() => this.warnings;

    public virtual Hashtable getResolvedOids() => (Hashtable) null;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      JavaResult javaResult = this;
      ObjectImpl.clone((object) javaResult);
      return ((object) javaResult).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
