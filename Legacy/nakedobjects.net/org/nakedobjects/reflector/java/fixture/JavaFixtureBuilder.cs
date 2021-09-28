// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.fixture.JavaFixtureBuilder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using java.util;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.@object.defaults;
using org.nakedobjects.@object.fixture;
using org.nakedobjects.application.system;
using System.ComponentModel;

namespace org.nakedobjects.reflector.java.fixture
{
  public sealed class JavaFixtureBuilder : FixtureBuilder
  {
    private static readonly Logger LOG;
    private ExplorationClock clock;
    private Vector newInstances;

    [JavaFlags(4)]
    public override void postInstallFixtures(NakedObjectPersistor objectManager)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4)]
    public override void installFixture(NakedObjectPersistor objectManager, Fixture fixture)
    {
      objectManager.startTransaction();
      fixture.install();
      objectManager.saveChanges();
      objectManager.endTransaction();
    }

    public JavaFixtureBuilder()
    {
      this.newInstances = new Vector();
      this.clock = new ExplorationClock();
    }

    public virtual void resetClock() => this.clock.reset();

    public virtual void setDate(int year, int month, int day) => this.clock.setDate(year, month, day);

    public virtual void setTime(int hour, int minute) => this.clock.setTime(hour, minute);

    [JavaFlags(17)]
    public object createInstance(string className)
    {
      NakedObjectSpecification specification = NakedObjects.getSpecificationLoader().loadSpecification(className);
      if (specification == null)
        return (object) new Error(new StringBuffer().append("Could not create an object of class ").append(className).ToString(), (Throwable) null);
      NakedObject transientInstance = NakedObjects.getObjectPersistor().createTransientInstance(specification);
      if (JavaFixtureBuilder.LOG.isDebugEnabled())
        JavaFixtureBuilder.LOG.debug((object) new StringBuffer().append("adding ").append((object) transientInstance).ToString());
      this.newInstances.addElement((object) transientInstance);
      return transientInstance.getObject();
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static JavaFixtureBuilder()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
