// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.TestClassDecorator
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.xat
{
  [JavaInterfaces("1;org/nakedobjects/xat/TestClass;")]
  public class TestClassDecorator : TestClass
  {
    private readonly TestClass wrappedObject;

    public TestClassDecorator(TestClass wrappedObject) => this.wrappedObject = wrappedObject;

    public virtual TestObject findInstance(string title) => this.wrappedObject.findInstance(title);

    public virtual Naked getForNaked() => this.wrappedObject.getForNaked();

    public virtual string getTitle() => this.wrappedObject.getTitle();

    public virtual TestCollection instances() => this.wrappedObject.instances();

    public virtual TestObject invokeAction(string name, TestNaked[] parameters) => this.wrappedObject.invokeAction(name, parameters);

    public virtual TestCollection invokeActionReturnCollection(
      string name,
      TestNaked[] parameters)
    {
      return this.wrappedObject.invokeActionReturnCollection(name, parameters);
    }

    public virtual TestObject invokeActionReturnObject(
      string name,
      TestNaked[] parameters)
    {
      return this.wrappedObject.invokeActionReturnObject(name, parameters);
    }

    public virtual TestObject newInstance() => this.wrappedObject.newInstance();

    public virtual void setForNaked(Naked @object) => this.wrappedObject.setForNaked(@object);

    public virtual void assertActionExists(string name) => this.wrappedObject.assertActionExists(name);

    public virtual void assertActionExists(string name, TestNaked[] parameters) => this.wrappedObject.assertActionExists(name, parameters);

    public virtual void assertActionExists(string name, TestNaked parameter) => this.wrappedObject.assertActionExists(name, parameter);

    public virtual void assertActionInvisible(string name) => this.wrappedObject.assertActionInvisible(name);

    public virtual void assertActionInvisible(string name, TestNaked[] parameters) => this.wrappedObject.assertActionInvisible(name, parameters);

    public virtual void assertActionInvisible(string name, TestNaked parameter) => this.wrappedObject.assertActionInvisible(name, parameter);

    public virtual void assertActionUnusable(string name) => this.wrappedObject.assertActionUnusable(name);

    public virtual void assertActionUnusable(string name, TestNaked[] parameters) => this.wrappedObject.assertActionUnusable(name, parameters);

    public virtual void assertActionUnusable(string name, TestNaked parameter) => this.wrappedObject.assertActionUnusable(name, parameter);

    public virtual void assertActionUsable(string name) => this.wrappedObject.assertActionUsable(name);

    public virtual void assertActionUsable(string name, TestNaked[] parameters) => this.wrappedObject.assertActionUsable(name, parameters);

    public virtual void assertActionUsable(string name, TestNaked parameter) => this.wrappedObject.assertActionUsable(name, parameter);

    public virtual void assertActionVisible(string name) => this.wrappedObject.assertActionVisible(name);

    public virtual void assertActionVisible(string name, TestNaked[] parameters) => this.wrappedObject.assertActionVisible(name, parameters);

    public virtual void assertActionVisible(string name, TestNaked parameter) => this.wrappedObject.assertActionVisible(name, parameter);

    public virtual TestObject invokeAction(string name) => this.wrappedObject.invokeAction(name);

    public virtual TestObject invokeAction(string name, TestNaked parameter) => this.wrappedObject.invokeAction(name, parameter);

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      TestClassDecorator testClassDecorator = this;
      ObjectImpl.clone((object) testClassDecorator);
      return ((object) testClassDecorator).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
