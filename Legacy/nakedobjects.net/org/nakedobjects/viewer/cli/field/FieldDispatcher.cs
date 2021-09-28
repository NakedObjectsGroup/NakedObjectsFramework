// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.field.FieldDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.cli.@object;

namespace org.nakedobjects.viewer.cli.field
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public abstract class FieldDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view)
    {
      if (!command.hasParameters())
        throw new IllegalDispatchException("Need field specified");
      NakedObject @object = ((ObjectAgent) context.getAgent()).getObject();
      string parameterAsLowerCase = command.getParameterAsLowerCase(0);
      NakedObjectField field = this.findField(@object, parameterAsLowerCase);
      if (field == null || field.isVisible((NakedReference) @object).isVetoed())
        view.error("No such field");
      else if (!field.isAuthorised())
        view.error("Not authorised to set this field");
      else if (field.isAvailable((NakedReference) @object).isVetoed())
        view.error(new StringBuffer().append("Field not available: ").append(field.isAvailable((NakedReference) @object).getReason()).ToString());
      else
        this.execute(command, context, view, field, @object, parameterAsLowerCase);
    }

    [JavaFlags(1028)]
    public abstract void execute(
      Command command,
      Context context,
      View view,
      NakedObjectField field,
      NakedObject @object,
      string name);

    private NakedObjectField findField(NakedObject @object, string name)
    {
      Matcher matcher = (Matcher) new FieldMatcher(@object.getSpecification().getVisibleFields(@object));
      return (NakedObjectField) MatchAlgorithm.match(name, matcher);
    }

    public virtual bool isAvailable(Context context) => context.currentAgentIs(Class.FromType(typeof (ObjectAgent)));

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      FieldDispatcher fieldDispatcher = this;
      ObjectImpl.clone((object) fieldDispatcher);
      return ((object) fieldDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    public abstract string getHelp();

    public abstract string getNames();
  }
}
