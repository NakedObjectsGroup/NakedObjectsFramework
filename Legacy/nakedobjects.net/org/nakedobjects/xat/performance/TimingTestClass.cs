// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.performance.TimingTestClass
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.utility;

namespace org.nakedobjects.xat.performance
{
  public class TimingTestClass : TestClassDecorator
  {
    private readonly TimingDocumentor doc;

    public TimingTestClass(TestClass wrappedObject, TimingDocumentor documentor)
      : base(wrappedObject)
    {
      this.doc = documentor;
    }

    public override TestObject findInstance(string title)
    {
      Profiler timer = new Profiler(new StringBuffer().append("Find instance of ").append(this.getTitle()).ToString());
      Delay.userDelay(4, 8);
      timer.start();
      TestObject instance = base.findInstance(title);
      timer.stop();
      this.doc.record(timer);
      return instance;
    }

    public override TestObject newInstance()
    {
      Profiler timer = new Profiler(new StringBuffer().append("New instance of ").append(this.getTitle()).ToString());
      Delay.userDelay(4, 8);
      timer.start();
      TestObject testObject = base.newInstance();
      testObject.getForNaked();
      timer.stop();
      this.doc.record(timer);
      return testObject;
    }

    public override TestObject invokeAction(string name, TestNaked[] parameters)
    {
      StringBuffer stringBuffer = new StringBuffer();
      for (int index = 0; index < parameters.Length; ++index)
      {
        if (index > 0)
          stringBuffer.append(", ");
        stringBuffer.append(parameters[index].getTitle());
      }
      Profiler timer = new Profiler(new StringBuffer().append("class action '").append(name).append("' with ").append((object) stringBuffer).ToString());
      Delay.userDelay(4, 8);
      timer.start();
      TestObject testObject = base.invokeAction(name, parameters);
      timer.stop();
      this.doc.record(timer);
      return testObject;
    }

    public override TestCollection instances()
    {
      Profiler timer = new Profiler(new StringBuffer().append("Instances of ").append(this.getTitle()).ToString());
      Delay.userDelay(4, 8);
      timer.start();
      TestCollection testCollection = base.instances();
      timer.stop();
      this.doc.record(timer);
      return testCollection;
    }
  }
}
