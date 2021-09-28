// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.field.ClearFieldDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.cli.@object;

namespace org.nakedobjects.viewer.cli.field
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class ClearFieldDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view)
    {
      NakedObject @object = ((ObjectAgent) context.getAgent()).getObject();
      string parameterAsLowerCase = command.getParameterAsLowerCase(0);
      string name = StringImpl.substring(parameterAsLowerCase, StringImpl.lastIndexOf(parameterAsLowerCase, 46) + 1);
      NakedObjectField field = this.findField(@object, name);
      if (command.getNumberOfParameters() == 0)
        throw new IllegalDispatchException("No field specified");
      if (field == null || field.isVisible((NakedReference) @object).isVetoed())
        throw new IllegalDispatchException("No such field");
      if (!field.isAuthorised())
        throw new IllegalDispatchException("Not authorised to clear this field");
      if (field.isAvailable((NakedReference) @object).isVetoed())
        throw new IllegalDispatchException(new StringBuffer().append("Field not available: ").append(field.isAvailable((NakedReference) @object).getReason()).ToString());
      if (field.isCollection())
        throw new IllegalDispatchException("Can't clear a collection field");
      this.clearField(command, context, view, @object, name, field);
    }

    private void clearField(
      Command command,
      Context context,
      View view,
      NakedObject @object,
      string name,
      NakedObjectField field)
    {
      if (field.isValue())
      {
        this.clearValue(view, @object, (OneToOneAssociation) field);
      }
      else
      {
        if (!field.isObject())
          throw new UnknownTypeException((object) field);
        this.clearReference(view, @object, (OneToOneAssociation) field);
      }
    }

    private void clearReference(
      View view,
      NakedObject @object,
      OneToOneAssociation oneToOneAssociation)
    {
      Consent consent = oneToOneAssociation.isAssociationValid(@object, (NakedObject) null);
      if (consent.isVetoed())
        view.error(new StringBuffer().append("Can't clear reference: ").append(consent.getReason()).ToString());
      else
        oneToOneAssociation.clearAssociation(@object, (NakedObject) oneToOneAssociation.get(@object));
    }

    private void clearValue(View view, NakedObject @object, OneToOneAssociation valueField) => valueField.clearValue(@object);

    private NakedObjectField findField(NakedObject @object, string name)
    {
      NakedObjectField[] visibleFields = @object.getSpecification().getVisibleFields(@object);
      return (NakedObjectField) MatchAlgorithm.match(name, (Matcher) new FieldMatcher(visibleFields));
    }

    public virtual string getHelp() => "Clear the specified field";

    public virtual string getNames() => "clear clr";

    public virtual bool isAvailable(Context context) => context.currentAgentIs(Class.FromType(typeof (ObjectAgent)));

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ClearFieldDispatcher clearFieldDispatcher = this;
      ObjectImpl.clone((object) clearFieldDispatcher);
      return ((object) clearFieldDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
