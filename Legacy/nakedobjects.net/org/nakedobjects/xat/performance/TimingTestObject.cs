// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.performance.TimingTestObject
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.utility;

namespace org.nakedobjects.xat.performance
{
  public class TimingTestObject : TestObjectDecorator
  {
    private readonly TimingDocumentor doc;

    public TimingTestObject(TestObject wrappedObject, TimingDocumentor documentor)
      : base(wrappedObject)
    {
      this.doc = documentor;
    }

    public override void associate(string fieldName, TestObject draggedView)
    {
      Profiler profiler = this.start(new StringBuffer().append("associate ").append(draggedView.getTitle()).append(" in ").append(fieldName).ToString());
      base.associate(fieldName, draggedView);
      this.stop(profiler);
    }

    public override void clearAssociation(string fieldName)
    {
      Profiler profiler = this.start(new StringBuffer().append("clear ").append(fieldName).ToString());
      base.clearAssociation(fieldName);
      this.stop(profiler);
    }

    public override void clearAssociation(string fieldName, string title)
    {
      Profiler profiler = this.start(new StringBuffer().append("clear ").append(fieldName).ToString());
      base.clearAssociation(fieldName, title);
      this.stop(profiler);
    }

    public override void fieldEntry(string fieldName, string value)
    {
      Profiler profiler = this.start(new StringBuffer().append("field entry '").append(value).append("' into ").append(fieldName).ToString());
      base.fieldEntry(fieldName, value);
      this.stop(profiler);
    }

    public override TestObject invokeAction(string name, TestNaked[] parameters)
    {
      string str = "";
      int index = 0;
      for (int length = parameters.Length; index < length; ++index)
        str = new StringBuffer().append(str).append(index <= 0 ? "" : ",").append(parameters[index].getTitle()).ToString();
      Profiler profiler = this.start(new StringBuffer().append("action '").append(name).append("' with ").append((object) parameters).ToString());
      TestObject testObject = base.invokeAction(name, parameters);
      this.stop(profiler);
      return testObject;
    }

    private Profiler start(string name)
    {
      Profiler profiler = new Profiler(name);
      Delay.userDelay(4, 8);
      profiler.start();
      return profiler;
    }

    private void stop(Profiler profiler)
    {
      profiler.stop();
      this.doc.record(profiler);
    }
  }
}
