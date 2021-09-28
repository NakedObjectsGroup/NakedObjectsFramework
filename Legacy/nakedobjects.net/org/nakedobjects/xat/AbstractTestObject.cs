// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.AbstractTestObject
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.reflect;
using System;

namespace org.nakedobjects.xat
{
  [JavaFlags(1056)]
  public abstract class AbstractTestObject
  {
    [JavaFlags(20)]
    public readonly TestObjectFactory factory;

    public AbstractTestObject(TestObjectFactory factory) => this.factory = factory;

    public abstract Naked getForNaked();

    public abstract string getTitle();

    public abstract void setForNaked(Naked @object);

    [Obsolete(null, false)]
    public virtual TestObject invokeAction(string name)
    {
      string name1 = name;
      int length = 0;
      TestNaked[] parameters = length >= 0 ? new TestNaked[length] : throw new NegativeArraySizeException();
      return this.invokeAction(name1, parameters);
    }

    [Obsolete(null, false)]
    public virtual TestObject invokeAction(string name, TestNaked parameter)
    {
      string name1 = name;
      int length = 1;
      TestNaked[] parameters = length >= 0 ? new TestNaked[length] : throw new NegativeArraySizeException();
      parameters[0] = parameter;
      return this.invokeAction(name1, parameters);
    }

    public virtual TestObject invokeAction(string name, TestNaked[] parameters) => (TestObject) this.newInvokeAction(name, parameters);

    private TestNaked newInvokeAction(string name, TestNaked[] parameters)
    {
      Action action = this.getAction(this.simpleName(name), parameters);
      this.assertActionUsable(name, action, parameters);
      this.assertActionVisible(name, action, parameters);
      Naked[] parameters1 = this.nakedObjects(parameters);
      Naked naked = ((NakedReference) this.getForNaked()).execute(action, parameters1);
      if (naked == null)
        return (TestNaked) null;
      return naked is NakedCollection ? (TestNaked) this.factory.createTestCollection((NakedCollection) naked) : (TestNaked) this.factory.createTestObject((NakedObject) naked);
    }

    public virtual TestCollection invokeActionReturnCollection(
      string name,
      TestNaked[] parameters)
    {
      return (TestCollection) this.newInvokeAction(name, parameters);
    }

    public virtual TestObject invokeActionReturnObject(
      string name,
      TestNaked[] parameters)
    {
      return (TestObject) this.newInvokeAction(name, parameters);
    }

    public override string ToString() => this.getForNaked() == null ? new StringBuffer().append(ObjectImpl.jloToString((object) this)).append(" ").append("null").ToString() : new StringBuffer().append(ObjectImpl.jloToString((object) this)).append(" ").append(this.getForNaked().getSpecification().getShortName()).append("/").append(this.getForNaked().ToString()).ToString();

    public virtual void assertActionExists(string name)
    {
      if (this.getAction(name) == null)
        throw new NakedAssertionFailedError(new StringBuffer().append("Action '").append(name).append("' not found in ").append((object) this.getForNaked()).ToString());
    }

    public virtual void assertActionExists(string name, TestNaked[] parameters)
    {
      if (this.getAction(name, parameters) == null)
        throw new NakedAssertionFailedError(new StringBuffer().append("Action '").append(name).append("' not found in ").append((object) this.getForNaked()).ToString());
    }

    public virtual void assertActionExists(string name, TestNaked parameter)
    {
      string name1 = name;
      int length = 1;
      TestNaked[] parameters = length >= 0 ? new TestNaked[length] : throw new NegativeArraySizeException();
      parameters[0] = parameter;
      this.assertActionExists(name1, parameters);
    }

    public virtual void assertActionInvisible(string name)
    {
      string name1 = name;
      Action action = this.getAction(name);
      int length = 0;
      TestNaked[] parameters = length >= 0 ? new TestNaked[length] : throw new NegativeArraySizeException();
      this.assertActionInvisible(name1, action, parameters);
    }

    private void assertActionInvisible(string name, Action action, TestNaked[] parameters)
    {
      if (action.isAuthorised() && this.getForNakedObject().isVisible(action).isAllowed())
        throw new NakedAssertionFailedError(new StringBuffer().append("Action '").append(name).append("' is visible").ToString());
    }

    private NakedObject getForNakedObject() => (NakedObject) this.getForNaked();

    public virtual void assertActionInvisible(string name, TestNaked[] parameters) => this.assertActionInvisible(name, this.getAction(name, parameters), parameters);

    public virtual void assertActionInvisible(string name, TestNaked parameter)
    {
      string name1 = name;
      int length = 1;
      TestNaked[] parameters = length >= 0 ? new TestNaked[length] : throw new NegativeArraySizeException();
      parameters[0] = parameter;
      this.assertActionInvisible(name1, parameters);
    }

    public virtual void assertActionUnusable(string name)
    {
      string name1 = name;
      Action action = this.getAction(name);
      int length = 0;
      TestNaked[] parameters = length >= 0 ? new TestNaked[length] : throw new NegativeArraySizeException();
      this.assertActionUnusable(name1, action, parameters);
    }

    private void assertActionUnusable(string name, Action action, TestNaked[] parameters)
    {
      this.assertTrue(new StringBuffer().append("Action cannot be seen (by this user): ").append(action.getName()).ToString(), action.isAuthorised());
      if (!this.getForNakedObject().isVisible(action).isAllowed() || !this.getForNakedObject().isAvailable(action).isAllowed())
        return;
      Naked[] parameters1 = this.nakedObjects(parameters);
      Consent consent = this.getForNakedObject().isValid(action, parameters1);
      if (consent.isAllowed())
        throw new NakedAssertionFailedError(new StringBuffer().append("Action '").append(name).append("' is usable and executable: ").append(consent.getReason()).ToString());
    }

    public virtual void assertActionUnusable(string name, TestNaked[] parameters) => this.assertActionUnusable(name, this.getAction(name, parameters), parameters);

    public virtual void assertActionUnusable(string name, TestNaked parameter)
    {
      string name1 = name;
      int length = 1;
      TestNaked[] parameters = length >= 0 ? new TestNaked[length] : throw new NegativeArraySizeException();
      parameters[0] = parameter;
      this.assertActionUnusable(name1, parameters);
    }

    public virtual void assertActionUsable(string name)
    {
      string name1 = name;
      Action action = this.getAction(name);
      int length = 0;
      TestNaked[] parameters = length >= 0 ? new TestNaked[length] : throw new NegativeArraySizeException();
      this.assertActionUsable(name1, action, parameters);
    }

    [JavaFlags(4)]
    public virtual void assertActionUsable(string name, Action action, TestNaked[] parameters)
    {
      this.assertTrue(new StringBuffer().append("Action cannot be seen (by this user): ").append(action.getName()).ToString(), action.isAuthorised());
      this.assertTrue(new StringBuffer().append("Action cannot be seen (in current state): ").append(action.getName()).ToString(), this.getForNakedObject().isVisible(action).isAllowed());
      this.assertTrue(new StringBuffer().append("Action is not usable: ").append(action.getName()).ToString(), this.getForNakedObject().isAvailable(action).isAllowed());
      Naked[] parameters1 = this.nakedObjects(parameters);
      Consent consent = this.getForNakedObject().isValid(action, parameters1);
      if (consent.isVetoed())
        throw new NakedAssertionFailedError(new StringBuffer().append("Action '").append(name).append("' is unusable: ").append(consent.getReason()).ToString());
    }

    public virtual void assertActionUsable(string name, TestNaked[] parameters) => this.assertActionUsable(name, this.getAction(name, parameters), parameters);

    public virtual void assertActionUsable(string name, TestNaked parameter)
    {
      string name1 = name;
      int length = 1;
      TestNaked[] parameters = length >= 0 ? new TestNaked[length] : throw new NegativeArraySizeException();
      parameters[0] = parameter;
      this.assertActionUsable(name1, parameters);
    }

    public virtual void assertActionVisible(string name)
    {
      string name1 = name;
      Action action = this.getAction(name);
      int length = 0;
      TestNaked[] parameters = length >= 0 ? new TestNaked[length] : throw new NegativeArraySizeException();
      this.assertActionVisible(name1, action, parameters);
    }

    [JavaFlags(4)]
    public virtual void assertActionVisible(string name, Action action, TestNaked[] parameters)
    {
      this.assertTrue(new StringBuffer().append("action '").append(name).append("' is invisible (to user)").ToString(), action.isAuthorised());
      bool condition = this.getForNakedObject().isVisible(action).isAllowed();
      this.assertTrue(new StringBuffer().append("action '").append(name).append("' is invisible (for state)").ToString(), condition);
    }

    [JavaFlags(4)]
    public virtual void assertTrue(string message, bool condition)
    {
      if (!condition)
        throw new NakedAssertionFailedError(message);
    }

    public virtual void assertActionVisible(string name, TestNaked[] parameters) => this.assertActionVisible(name, this.getAction(name, parameters), parameters);

    public virtual void assertActionVisible(string name, TestNaked parameter)
    {
      string name1 = name;
      int length = 1;
      TestNaked[] parameters = length >= 0 ? new TestNaked[length] : throw new NegativeArraySizeException();
      parameters[0] = parameter;
      this.assertActionVisible(name1, parameters);
    }

    public virtual Action getAction(string name)
    {
      Action action;
      try
      {
        string name1 = name;
        int length = 0;
        NakedObjectSpecification[] parameterClasses = length >= 0 ? new NakedObjectSpecification[length] : throw new NegativeArraySizeException();
        action = this.getAction(name1, parameterClasses);
      }
      catch (NakedObjectSpecificationException ex)
      {
        throw new NakedAssertionFailedError(((Throwable) ex).getMessage());
      }
      return action != null ? action : throw new NakedAssertionFailedError(new StringBuffer().append("Method not found: ").append(name).ToString());
    }

    public virtual Action getAction(string name, TestNaked[] parameters)
    {
      Naked[] nakedArray = this.nakedObjects(parameters);
      int length1 = parameters.Length;
      int length2 = length1;
      NakedObjectSpecification[] parameterClasses = length2 >= 0 ? new NakedObjectSpecification[length2] : throw new NegativeArraySizeException();
      for (int index = 0; index < length1; ++index)
        parameterClasses[index] = nakedArray[index] != null ? nakedArray[index].getSpecification() : ((TestNakedNullParameter) parameters[index]).getSpecification();
      try
      {
        Action action = this.getAction(name, parameterClasses);
        if (action == null)
        {
          string str = "";
          for (int index = 0; index < length1; ++index)
            str = new StringBuffer().append(str).append(index <= 0 ? "" : ", ").append(parameterClasses[index].getFullName()).ToString();
          throw new NakedAssertionFailedError(new StringBuffer().append("Method not found: ").append(name).append("(").append(str).append(") in ").append((object) this.getForNaked()).ToString());
        }
        return action;
      }
      catch (NakedObjectSpecificationException ex)
      {
        string shortName = this.getForNaked().getSpecification().getShortName();
        string str = "";
        for (int index = 0; index < length1; ++index)
          str = new StringBuffer().append(str).append(index <= 0 ? "" : ", ").append((object) parameterClasses[index]).ToString();
        throw new NakedAssertionFailedError(new StringBuffer().append("Can't find action '").append(name).append("' with parameters (").append(str).append(") on a ").append(shortName).ToString());
      }
    }

    [JavaFlags(1028)]
    public abstract Action getAction(string name, NakedObjectSpecification[] parameterClasses);

    [JavaFlags(4)]
    public virtual Naked[] nakedObjects(TestNaked[] parameters)
    {
      int length = parameters.Length;
      Naked[] nakedArray = length >= 0 ? new Naked[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < parameters.Length; ++index)
        nakedArray[index] = parameters[index].getForNaked();
      return nakedArray;
    }

    [JavaFlags(4)]
    public virtual string simpleName(string name) => NameConvertor.simpleName(name);

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AbstractTestObject abstractTestObject = this;
      ObjectImpl.clone((object) abstractTestObject);
      return ((object) abstractTestObject).MemberwiseClone();
    }
  }
}
