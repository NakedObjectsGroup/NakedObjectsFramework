// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.html.HtmlTestClass
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.xat.html
{
  public class HtmlTestClass : TestClassDecorator
  {
    private HtmlDocumentor doc;

    public HtmlTestClass(TestClass wrappedObject, HtmlDocumentor documentor)
      : base(wrappedObject)
    {
      this.doc = documentor;
    }

    public override TestObject findInstance(string title)
    {
      NakedObject forNaked = (NakedObject) this.getForNaked();
      string singularName = forNaked.getSpecification().getSingularName();
      TestObject instance = base.findInstance(title);
      this.doc.doc(new StringBuffer().append("Find the <b>").append(singularName).append("</b> instance ").append(this.doc.simpleObjectString((Naked) forNaked)).append(". ").ToString());
      return instance;
    }

    public override TestCollection instances()
    {
      string fullName = ((NakedClass) this.getForNaked()).getFullName();
      string str = StringImpl.substring(fullName, StringImpl.lastIndexOf(fullName, ".") + 1);
      this.doc.doc(new StringBuffer().append("Get the instances of the ").append(str).append(" (<img width=\"16\" height=\"16\" align=\"Center\" src=\"images/").append(str).append("16.gif\">) ").append(" class").ToString());
      TestCollection testCollection = base.instances();
      this.doc.docln(new StringBuffer().append(", which returns ").append(this.doc.objectString(testCollection.getForNaked())).append(". ").ToString());
      return testCollection;
    }

    public override TestObject newInstance()
    {
      string pluralName = ((NakedClass) this.getForNaked()).getPluralName();
      this.doc.doc(new StringBuffer().append("Create a new ").append(pluralName).append(" instance: ").ToString());
      TestObject testObject = base.newInstance();
      NakedObject forNaked = (NakedObject) testObject.getForNaked();
      this.doc.doc(this.doc.simpleObjectString((Naked) forNaked));
      this.doc.docln(!StringImpl.equals(forNaked.titleString(), (object) "") ? ". " : ", which is untitled. ");
      return testObject;
    }
  }
}
