// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.html.HtmlTestObject
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.xat.html
{
  public class HtmlTestObject : TestObjectDecorator
  {
    private HtmlDocumentor doc;

    public HtmlTestObject(TestObject wrappedObject, HtmlDocumentor documentor)
      : base(wrappedObject)
    {
      this.doc = documentor;
    }

    public override void assertActionUnusable(string name)
    {
      base.assertActionUnusable(name);
      this.doc(new StringBuffer().append("Right clicking on the ").append(this.doc.objectString(this.getForNaked())).ToString());
      this.doc(new StringBuffer().append(" shows that <strong>").append(name).ToString());
      this.doc(!this.getAction(name).hasReturn() ? "" : "...");
      this.doc("</strong> is not currently available. ");
    }

    public override void assertActionUsable(string name, TestNaked parameter)
    {
      base.assertActionUsable(name, parameter);
      this.doc("note that it can't be dropped onto the ");
      this.doc(new StringBuffer().append(this.doc.objectString(this.getForNaked())).append(". ").ToString());
    }

    public override void assertFieldContains(string fieldName, string expectedValue)
    {
      base.assertFieldContains(fieldName, expectedValue);
      if (this.getField(fieldName) is TestValue)
        this.doc(new StringBuffer().append("<p>Note that the field <em>").append(fieldName).append("</em> in the ").append(this.doc.objectString(this.getForNaked())).append(" is now set to '").append(this.getField(fieldName).getForNaked().titleString()).append("'.").ToString());
      else
        this.doc(new StringBuffer().append("<p>Note that object in the field <em>").append(fieldName).append("</em> of ").append(this.doc.objectString(this.getForNaked())).append(" now has a title of '").append(this.getField(fieldName).getForNaked().titleString()).append("'.").ToString());
    }

    public override void assertFieldContains(
      string message,
      string fieldName,
      object expectedValue)
    {
      base.assertFieldContains(message, fieldName, expectedValue);
      this.doc(new StringBuffer().append("<p>Note that the field <em>").append(fieldName).append("</em> in the ").append(this.doc.objectString(this.getForNaked())).append(" is now set to '").append(this.getField(fieldName).getForNaked().titleString()).append("'.").ToString());
    }

    public override void assertFieldContains(string fieldName, TestObject expectedView)
    {
      base.assertFieldContains(fieldName, expectedView);
      NakedObject forNaked = (NakedObject) this.getForNaked();
      if (forNaked is NakedCollection)
        this.doc(new StringBuffer().append("<em>").append(fieldName).append("</em> contains the ").append(this.doc.objectString(expectedView.getForNaked())).append("; ").ToString());
      else
        this.doc(new StringBuffer().append("<em>").append(fieldName).append("</em> contains the ").append(this.doc.objectString((Naked) forNaked)).append("; ").ToString());
    }

    public override void assertFieldDoesNotContain(string fieldName, string testValue)
    {
      base.assertFieldDoesNotContain(fieldName, testValue);
      if (this.getField(fieldName) is TestValue)
        this.doc(new StringBuffer().append("<p>Note that the field <em>").append(fieldName).append("</em> in the ").append(this.doc.objectString(this.getForNaked())).append(" does not contain a value of '").append(testValue).append("'.").ToString());
      else
        this.doc(new StringBuffer().append("<p>Note that object in the field <em>").append(fieldName).append("</em> of ").append(this.doc.objectString(this.getForNaked())).append(" does not contains an object titled of '").append(this.getField(fieldName).getForNaked().titleString()).append("'.").ToString());
    }

    public override void assertFieldDoesNotContain(
      string message,
      string fieldName,
      NakedObject expectedValue)
    {
      base.assertFieldDoesNotContain(message, fieldName, expectedValue);
      this.doc(new StringBuffer().append("<p>Note that the field <em>").append(fieldName).append("</em> in the ").append(this.doc.objectString(this.getForNaked())).append(" does not contain '").append(this.getField(fieldName).getForNaked().titleString()).append("'.").ToString());
    }

    public override void assertFieldDoesNotContain(string fieldName, TestObject testView)
    {
      base.assertFieldDoesNotContain(fieldName, testView);
      NakedObject forNaked = (NakedObject) this.getForNaked();
      if (forNaked is NakedCollection)
        this.doc(new StringBuffer().append("<em>").append(fieldName).append("</em> does not contain an instance of ").append(this.doc.objectString(testView.getForNaked())).append("; ").ToString());
      else
        this.doc(new StringBuffer().append("<em>").append(fieldName).append("</em> does not contains ").append(this.doc.objectString((Naked) forNaked)).append("; ").ToString());
    }

    public override void assertNoOfElements(string collectionName, int noOfElements)
    {
      base.assertNoOfElements(collectionName, noOfElements);
      this.doc(new StringBuffer().append("<em>").append(collectionName).append("</em> contains ").append(noOfElements).append(" elements; ").ToString());
    }

    public override void assertNoOfElementsNotEqual(string collectionName, int noOfElements)
    {
      base.assertNoOfElementsNotEqual(collectionName, noOfElements);
      this.doc(new StringBuffer().append("<em>").append(collectionName).append("</em> does not contain ").append(noOfElements).append(" elements; ").ToString());
    }

    public override void assertTitleEquals(string expectedTitle)
    {
      base.assertTitleEquals(expectedTitle);
      this.doc(new StringBuffer().append("Note the new  title: ").append(this.doc.objectString(this.getForNaked())).append(" .").ToString());
    }

    public override void associate(string fieldName, TestObject draggedView)
    {
      base.associate(fieldName, draggedView);
      this.doc("Drop it into the ");
      this.docln(new StringBuffer().append("<em>").append(fieldName).append("</em> field within the ").append(this.doc.objectString(this.getForNaked())).append(". ").ToString());
    }

    private void doc(string text) => this.doc.doc(text);

    private void docln(string text) => this.doc.docln(text);

    public override void fieldEntry(string name, string value)
    {
      this.doc(new StringBuffer().append("Set the <em>").append(name).append("</em> field within the ").append(this.doc.objectString(this.getForNaked())).ToString());
      this.docln(new StringBuffer().append(" to <code>").append(value).append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</code>. ").ToString());
      base.fieldEntry(name, value);
    }

    public override TestObject getAssociation(string title)
    {
      NakedCollection forNaked = (NakedCollection) this.getForNaked();
      this.doc(new StringBuffer().append("Select the instance within ").append(this.doc.objectString((Naked) forNaked)).append(" (<img width=\"16\" height=\"16\" align=\"Center\" src=\"images/Collection16.gif\">) ").append(" class whose title matches <strong>").append(title).append("</strong>").ToString());
      TestObject association = base.getAssociation(title);
      this.docln(new StringBuffer().append(", which returns ").append(this.doc.objectString(association.getForNaked())).append(". ").ToString());
      return association;
    }

    public override TestObject invokeAction(string name, TestNaked[] parameters)
    {
      TestObject testObject = base.invokeAction(name, parameters);
      if (parameters.Length == 1)
      {
        this.doc("drop it onto the ");
        this.doc(this.doc.objectString(this.getForNaked()));
      }
      else
      {
        this.doc(new StringBuffer().append("Right click on the ").append(this.doc.objectString(this.getForNaked())).ToString());
        this.doc(new StringBuffer().append(" and select the <strong>").append(name).ToString());
        this.doc(!this.getAction(name).hasReturn() ? "" : "...");
        this.doc("</strong> action");
      }
      this.doc(testObject != null ? new StringBuffer().append(", which returns ").append(this.objectString(testObject.getForNaked())).append(". ").ToString() : ".");
      return testObject;
    }

    private string objectString(Naked @object) => this.doc.objectString(@object);
  }
}
