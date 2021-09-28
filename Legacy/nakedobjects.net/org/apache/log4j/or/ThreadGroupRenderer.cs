// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.or.ThreadGroupRenderer
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.apache.log4j.or
{
  [JavaInterfaces("1;org/apache/log4j/or/ObjectRenderer;")]
  public class ThreadGroupRenderer : ObjectRenderer
  {
    public virtual string doRender(object o)
    {
      if (!(o is ThreadGroup))
        return o.ToString();
      StringBuffer stringBuffer = new StringBuffer();
      ThreadGroup threadGroup = (ThreadGroup) o;
      stringBuffer.append("java.lang.ThreadGroup[name=");
      stringBuffer.append(threadGroup.getName());
      stringBuffer.append(", maxpri=");
      stringBuffer.append(threadGroup.getMaxPriority());
      stringBuffer.append("]");
      int length = threadGroup.activeCount();
      Thread[] threadArray = length >= 0 ? new Thread[length] : throw new NegativeArraySizeException();
      threadGroup.enumerate(threadArray);
      for (int index = 0; index < threadArray.Length; ++index)
      {
        stringBuffer.append(Layout.LINE_SEP);
        stringBuffer.append("   Thread=[");
        stringBuffer.append(threadArray[index].getName());
        stringBuffer.append(",");
        stringBuffer.append(threadArray[index].getPriority());
        stringBuffer.append(",");
        stringBuffer.append(threadArray[index].isDaemon());
        stringBuffer.append("]");
      }
      return stringBuffer.ToString();
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ThreadGroupRenderer threadGroupRenderer = this;
      ObjectImpl.clone((object) threadGroupRenderer);
      return ((object) threadGroupRenderer).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
