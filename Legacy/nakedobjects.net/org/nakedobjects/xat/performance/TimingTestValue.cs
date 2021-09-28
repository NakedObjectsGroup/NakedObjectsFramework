// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.performance.TimingTestValue
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.utility;

namespace org.nakedobjects.xat.performance
{
  public class TimingTestValue : TestValueDecorator
  {
    private readonly TimingDocumentor doc;

    public TimingTestValue(TestValue wrappedObject, TimingDocumentor documentor)
      : base(wrappedObject)
    {
      this.doc = documentor;
    }

    public override void fieldEntry(string value)
    {
      Profiler timer = new Profiler(new StringBuffer().append("Field entry ").append(value).ToString());
      Delay.userDelay(4, 8);
      timer.start();
      base.fieldEntry(value);
      timer.stop();
      this.doc.record(timer);
    }
  }
}
