// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.ExpectationObject
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using junit.framework;
using org.nakedobjects.@object;

namespace org.nakedobjects.xat
{
  public class ExpectationObject
  {
    private Class type;
    private Hashtable expectedReferences;
    private Hashtable expectedValues;
    private TestObject actual;

    public ExpectationObject(Class type)
    {
      this.expectedReferences = new Hashtable();
      this.expectedValues = new Hashtable();
      this.type = type;
    }

    public virtual void addActual(TestObject viewer)
    {
      NakedObject forNaked = (NakedObject) viewer.getForNaked();
      if (!ObjectImpl.getClass((object) forNaked).isAssignableFrom(this.type))
        Assert.fail(new StringBuffer().append("Expected an object of type ").append(this.type.getName()).append(" but got a ").append(ObjectImpl.getClass((object) forNaked).getName()).ToString());
      this.actual = viewer;
    }

    public virtual void addExpectedReference(string name, NakedObject reference)
    {
      this.checkFieldUse(name);
      this.expectedReferences.put((object) name, (object) new ExpectationValue(reference));
      this.setHasExpectations();
    }

    private void setHasExpectations()
    {
    }

    public virtual void addExpectedReference(string name, TestObject view)
    {
      this.checkFieldUse(name);
      this.expectedReferences.put((object) name, (object) new ExpectationValue((NakedObject) view.getForNaked()));
      this.setHasExpectations();
    }

    public virtual void addExpectedText(string name, string value)
    {
      this.checkFieldUse(name);
      this.expectedValues.put((object) name, (object) value);
      this.setHasExpectations();
    }

    private void checkFieldUse(string name)
    {
      if (this.expectedValues.containsKey((object) name) || this.expectedReferences.containsKey((object) name))
        throw new RuntimeException(new StringBuffer().append("Duplicate field: ").append(name).ToString());
    }

    public virtual void clearActual() => this.actual = (TestObject) null;

    [JavaFlags(4)]
    public virtual void clearExpectation()
    {
      this.expectedValues.clear();
      this.expectedReferences.clear();
    }

    public virtual void setExpectNothing()
    {
      this.clearExpectation();
      this.setHasExpectations();
    }

    public virtual void verify()
    {
      Enumeration enumeration1 = this.expectedValues.keys();
      while (enumeration1.hasMoreElements())
      {
        string fieldName = \u003CVerifierFix\u003E.genCastToString(enumeration1.nextElement());
        string title = this.actual.getField(fieldName).getTitle();
        Assert.assertEquals(new StringBuffer().append("Wrong value for ").append(fieldName).ToString(), this.expectedValues.get((object) fieldName).ToString(), title);
      }
      Enumeration enumeration2 = this.expectedReferences.keys();
      while (enumeration2.hasMoreElements())
      {
        string fieldName = \u003CVerifierFix\u003E.genCastToString(enumeration2.nextElement());
        NakedObject forNaked = (NakedObject) this.actual.getField(fieldName).getForNaked();
        NakedObject reference = ((ExpectationValue) this.expectedReferences.get((object) fieldName)).getReference();
        Assert.assertEquals(new StringBuffer().append("Wrong reference for ").append(fieldName).ToString(), (object) reference, (object) forNaked);
      }
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      ExpectationObject expectationObject = this;
      ObjectImpl.clone((object) expectationObject);
      return ((object) expectationObject).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
