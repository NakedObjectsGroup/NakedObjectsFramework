// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.field.ShowFieldDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.cli.@object;

namespace org.nakedobjects.viewer.cli.field
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class ShowFieldDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view)
    {
      if (!command.hasParameters())
        throw new IllegalDispatchException("No field name specified");
      NakedObject @object = ((ObjectAgent) context.getAgent()).getObject();
      NakedObjectField[] visibleFields = @object.getSpecification().getVisibleFields(@object);
      for (int index = 0; index < command.getNumberOfParameters(); ++index)
      {
        string parameter = command.getParameter(index);
        NakedObjectField nakedObjectField = (NakedObjectField) MatchAlgorithm.match(parameter, (Matcher) new FieldMatcher(visibleFields));
        if (nakedObjectField == null)
          view.error(new StringBuffer().append("No such field ").append(parameter).ToString());
        else
          this.showField(view, @object, nakedObjectField);
      }
    }

    public virtual string getHelp() => "Show the named field from the current object";

    public virtual string getNames() => "field fld";

    public virtual bool isAvailable(Context context) => context.currentAgentIs(Class.FromType(typeof (ObjectAgent)));

    private void showField(View view, NakedObject @object, NakedObjectField nakedObjectField)
    {
      string str;
      if (nakedObjectField.isObject())
      {
        NakedObject nakedObject = (NakedObject) nakedObjectField.get(@object);
        str = nakedObject != null ? nakedObject.titleString() : "";
      }
      else if (nakedObjectField.isValue())
      {
        NakedValue nakedValue = (NakedValue) nakedObjectField.get(@object);
        str = nakedValue != null ? nakedValue.getObject().ToString() : "";
      }
      else
      {
        int num = nakedObjectField.isCollection() ? ((NakedCollection) nakedObjectField.get(@object)).size() : throw new UnknownTypeException((object) nakedObjectField);
        switch (num)
        {
          case 0:
            str = "collection empty";
            break;
          case 1:
            str = "1 element";
            break;
          default:
            str = new StringBuffer().append(num).append(" elements").ToString();
            break;
        }
      }
      view.display(new StringBuffer().append(nakedObjectField.getName()).append(": ").append(str).ToString());
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      ShowFieldDispatcher showFieldDispatcher = this;
      ObjectImpl.clone((object) showFieldDispatcher);
      return ((object) showFieldDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
