// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.object.ObjectAgent
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.cli.@object;
using org.nakedobjects.viewer.cli.util;

namespace org.nakedobjects.viewer.cli.@object
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Agent;")]
  public class ObjectAgent : Agent
  {
    private readonly NakedObject @object;

    public ObjectAgent(NakedObject @object)
    {
      Assert.assertNotNull((object) @object);
      this.@object = @object;
    }

    public virtual string debug()
    {
      string str = this.@object != null ? this.@object.titleString() : "null";
      return new StringBuffer().append("Object, '").append(str).append("'").ToString();
    }

    public virtual void list(View view, string[] layout)
    {
      NakedObjectSpecification specification = this.@object.getSpecification();
      if (specification.isObject() && layout != null && layout.Length > 0)
        this.displayObject(view, this.@object, specification, layout);
      else if (specification.isObject())
      {
        this.displayObject(view, this.@object, specification);
      }
      else
      {
        if (!specification.isCollection())
          return;
        this.displayCollection(view, (NakedCollection) this.@object);
      }
    }

    private void displayCollection(View view, NakedCollection collection)
    {
      Enumeration enumeration = collection.elements();
      int num1 = 1;
      while (enumeration.hasMoreElements())
      {
        NakedObject nakedObject = (NakedObject) enumeration.nextElement();
        View view1 = view;
        StringBuffer stringBuffer = new StringBuffer();
        int num2;
        num1 = (num2 = num1) + 1;
        int num3 = num2;
        string message = stringBuffer.append(num3).append(". ").append(nakedObject.titleString()).ToString();
        view1.display(message);
      }
    }

    private void displayObject(
      View view,
      NakedObject @object,
      NakedObjectSpecification specification)
    {
      NakedObjectField[] visibleFields = specification.getVisibleFields(@object);
      this.displayObject(view, @object, visibleFields);
    }

    private void displayObject(
      View view,
      NakedObject @object,
      NakedObjectSpecification specification,
      string[] fieldNames)
    {
      NakedObjectField[] visibleFields = specification.getVisibleFields(@object);
      int length = fieldNames.Length;
      NakedObjectField[] fields = length >= 0 ? new NakedObjectField[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < fields.Length; ++index)
      {
        fields[index] = (NakedObjectField) MatchAlgorithm.match(fieldNames[index], (Matcher) new FieldMatcher(visibleFields));
        if (fields[index] == null)
          throw new IllegalDispatchException(new StringBuffer().append("No field with name ").append(fieldNames[index]).ToString());
      }
      this.displayObject(view, @object, fields);
    }

    private void displayObject(View view, NakedObject @object, NakedObjectField[] fields)
    {
      string str1 = Util.titleString(@object);
      string str2 = !@object.getResolveState().isTransient() ? "" : " (unsaved)";
      view.display(new StringBuffer().append(str1).append(str2).ToString());
      int max = 0;
      for (int index = 0; index < fields.Length; ++index)
        max = Math.max(max, StringImpl.length(fields[index].getName()));
      for (int index = 0; index < fields.Length; ++index)
      {
        Naked naked = fields[index].get(@object);
        string name = fields[index].getName();
        string str3;
        if (fields[index].isCollection())
        {
          int num = ((NakedCollection) naked).size();
          str3 = num != 0 ? new StringBuffer().append(num).append(" elements").ToString() : "empty";
        }
        else
          str3 = naked != null ? naked.titleString() : "";
        view.display(new StringBuffer().append(name).append(":  ").append(Util.padding(max, name)).append(str3).ToString());
      }
    }

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      return obj is ObjectAgent && ((ObjectAgent) obj).@object.Equals((object) this.@object);
    }

    public override int GetHashCode()
    {
      NakedObject nakedObject = this.@object;
      return nakedObject is string ? StringImpl.hashCode((string) nakedObject) : ObjectImpl.hashCode((object) nakedObject);
    }

    public virtual bool isReplaceable() => true;

    public virtual string getName() => "Object";

    public virtual NakedObject getObject() => this.@object;

    public virtual Agent findAgent(string criteria)
    {
      StringImpl.toLowerCase(this.@object.titleString());
      NakedObjectField[] visibleFields = this.@object.getSpecification().getVisibleFields(this.@object);
      FieldMatcher fieldMatcher = new FieldMatcher(visibleFields);
      NakedObjectField nakedObjectField = (NakedObjectField) MatchAlgorithm.match(criteria, (Matcher) fieldMatcher);
      if (nakedObjectField != null)
      {
        if (nakedObjectField.isObject())
        {
          NakedObject @object = (NakedObject) nakedObjectField.get(this.@object);
          if (@object != null)
            return (Agent) new ObjectAgent(@object);
        }
        else if (nakedObjectField.isCollection())
          return (Agent) new CollectionAgent((NakedCollection) nakedObjectField.get(this.@object), new StringBuffer().append(this.@object.titleString()).append(" - ").append(nakedObjectField.getName()).ToString());
      }
      Matcher matcher = (Matcher) new ObjectMatcher(this.@object, visibleFields);
      NakedObject object1 = (NakedObject) MatchAlgorithm.match(criteria, matcher);
      return object1 != null ? (Agent) new ObjectAgent(object1) : (Agent) null;
    }

    public virtual string getPrompt()
    {
      string str1 = this.@object.titleString() ?? new StringBuffer().append("(").append(this.@object.getSpecification().getSingularName()).append(")").ToString();
      string str2 = !this.@object.getResolveState().isTransient() ? "" : " (unsaved)";
      return new StringBuffer().append(str1).append(str2).ToString();
    }

    public virtual bool isValueEntry() => false;

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("object", (object) this.@object);
      return toString.ToString();
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ObjectAgent objectAgent = this;
      ObjectImpl.clone((object) objectAgent);
      return ((object) objectAgent).MemberwiseClone();
    }
  }
}
