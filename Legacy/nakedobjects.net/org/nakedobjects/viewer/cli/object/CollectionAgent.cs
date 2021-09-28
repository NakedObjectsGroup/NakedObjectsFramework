// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.object.CollectionAgent
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.cli.@object;

namespace org.nakedobjects.viewer.cli.@object
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Agent;")]
  public class CollectionAgent : Agent
  {
    private readonly NakedCollection collection;
    private readonly string fieldName;

    public CollectionAgent(NakedCollection collection, string fieldName)
    {
      this.collection = collection;
      this.fieldName = fieldName;
    }

    public virtual string debug() => new StringBuffer().append("Collection '").append(this.fieldName).append("', ").append(this.collection.size()).append(" elements").ToString();

    public virtual void list(View view, string[] layout)
    {
      if (layout != null && layout.Length > 0)
        this.displayTable(view, layout);
      else
        this.displayList(view);
    }

    private void displayList(View view)
    {
      Enumeration enumeration = this.collection.elements();
      while (enumeration.hasMoreElements())
      {
        NakedObject @object = (NakedObject) enumeration.nextElement();
        view.display(@object);
      }
    }

    private void displayTable(View view, string[] fieldNames)
    {
      NakedObjectField[] accessibleFields = ((TypedNakedCollection) this.collection).getElementSpecification().getAccessibleFields();
      int length = fieldNames.Length;
      NakedObjectField[] nakedObjectFieldArray = length >= 0 ? new NakedObjectField[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < nakedObjectFieldArray.Length; ++index)
      {
        nakedObjectFieldArray[index] = (NakedObjectField) MatchAlgorithm.match(fieldNames[index], (Matcher) new FieldMatcher(accessibleFields));
        if (nakedObjectFieldArray[index] == null)
          throw new IllegalDispatchException(new StringBuffer().append("No field with name ").append(fieldNames[index]).ToString());
      }
      Enumeration enumeration = this.collection.elements();
      while (enumeration.hasMoreElements())
      {
        NakedObject fromObject = (NakedObject) enumeration.nextElement();
        StringBuffer stringBuffer = new StringBuffer();
        for (int index = 0; index < nakedObjectFieldArray.Length; ++index)
        {
          if (index > 0)
            stringBuffer.append("; ");
          Naked naked = nakedObjectFieldArray[index].get(fromObject);
          stringBuffer.append(naked != null ? naked.titleString() : "null");
        }
        view.display(stringBuffer.ToString());
      }
    }

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      if (!(obj is CollectionAgent))
        return false;
      CollectionAgent collectionAgent = (CollectionAgent) obj;
      return collectionAgent.collection.Equals((object) this.collection) && StringImpl.equals(collectionAgent.fieldName, (object) this.fieldName);
    }

    public override int GetHashCode()
    {
      int num1 = 37 * 17;
      NakedCollection collection = this.collection;
      int num2 = !(collection is string) ? ObjectImpl.hashCode((object) collection) : StringImpl.hashCode((string) collection);
      int num3 = 37 * (num1 + num2) + StringImpl.hashCode(this.fieldName);
      return base.GetHashCode();
    }

    public virtual bool isReplaceable() => true;

    public virtual string getName() => "Collection";

    public virtual NakedCollection getObject() => this.collection;

    public virtual Agent findAgent(string criteria) => StringImpl.startsWith(criteria, "#") ? this.getElementAt(criteria) : this.getElementWithTitle(criteria);

    private Agent getElementWithTitle(string criteria)
    {
      Enumeration enumeration = this.collection.elements();
      while (enumeration.hasMoreElements())
      {
        NakedObject @object = (NakedObject) enumeration.nextElement();
        string lowerCase = StringImpl.toLowerCase(@object.titleString());
        if (lowerCase != null && StringImpl.indexOf(lowerCase, criteria) >= 0)
          return (Agent) new ObjectAgent(@object);
      }
      return (Agent) null;
    }

    private Agent getElementAt(string criteria)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual string getPrompt() => this.fieldName;

    public virtual bool isValueEntry() => false;

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("object", (object) this.collection);
      return toString.ToString();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      CollectionAgent collectionAgent = this;
      ObjectImpl.clone((object) collectionAgent);
      return ((object) collectionAgent).MemberwiseClone();
    }
  }
}
