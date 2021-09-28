// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.action.ActionAgent
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.viewer.cli.@object;
using org.nakedobjects.viewer.cli.util;

namespace org.nakedobjects.viewer.cli.action
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Agent;")]
  public class ActionAgent : Agent
  {
    private readonly Action action;
    private readonly bool[] isParameterSet;
    private readonly bool[] promptForParameter;
    private int parameterIndex;
    private readonly Naked[] @params;
    private readonly NakedObject target;
    private readonly ActionHelper helper;

    public static bool create(
      Action[] actions,
      Command command,
      NakedObject @object,
      Context context,
      View view)
    {
      if (!ActionAgent.methodExists(actions, command))
        throw new IllegalDispatchException(new StringBuffer().append("No such action ").append(command.getParameter(0)).append(" on ").append(@object.titleString()).ToString());
      ActionAgent context1 = ActionAgent.createContext(actions, command, @object, context, view);
      if (context1 == null)
        return false;
      context.addSubject((Agent) context1);
      if (context1.canExecute() && !context1.hasNextParameter())
        context1.execute(context, view);
      return true;
    }

    private static ActionAgent createContext(
      Action[] actions,
      Command command,
      NakedObject target,
      Context context,
      View view)
    {
      string parameterAsLowerCase = command.getParameterAsLowerCase(0);
      string actionName = StringImpl.substring(parameterAsLowerCase, StringImpl.lastIndexOf(parameterAsLowerCase, 46) + 1);
      int parametersLength = command.getNumberOfParameters() - 1;
      Action action = ActionAgent.findAction(actions, parametersLength, actionName);
      if (action == null || target != null && action.isVisible((NakedReference) target).isVetoed())
        view.error("No such action");
      else if (!action.isAuthorised())
        view.error("Not authorised to use this method");
      else if (target != null && action.isAvailable((NakedReference) target).isVetoed())
      {
        view.error(new StringBuffer().append("Action not available: ").append(action.isAvailable((NakedReference) target).getReason()).ToString());
      }
      else
      {
        int parameterCount = action.getParameterCount();
        int length1 = parameterCount;
        string[] strArray = length1 >= 0 ? new string[length1] : throw new NegativeArraySizeException();
        if (parametersLength > 0)
        {
          for (int index = 0; index < parametersLength; ++index)
            strArray[index] = command.getParameter(index + 1);
        }
        NakedObjectSpecification[] parameterTypes = action.getParameterTypes();
        ActionHelper instance = ActionHelper.createInstance((NakedReference) target, action);
        object[] defaultParameterValues = instance.getDefaultParameterValues();
        bool[] requiredParameters = instance.getRequiredParameters();
        int length2 = requiredParameters.Length;
        bool[] promptForParameter = length2 >= 0 ? new bool[length2] : throw new NegativeArraySizeException();
        int length3 = strArray.Length;
        Naked[] @params = length3 >= 0 ? new Naked[length3] : throw new NegativeArraySizeException();
        int length4 = strArray.Length;
        bool[] isParameterSet = length4 >= 0 ? new bool[length4] : throw new NegativeArraySizeException();
        for (int index = 0; index < parametersLength; ++index)
        {
          string entry = strArray[index];
          if (StringImpl.equals(entry, (object) "-"))
          {
            @params[index] = (Naked) null;
            if (requiredParameters[index])
              throw new IllegalDispatchException(new StringBuffer().append("Parameter ").append(index + 1).append(" required, cannot be set to null").ToString());
            isParameterSet[index] = true;
          }
          else if (StringImpl.equals(entry, (object) "!"))
          {
            @params[index] = ActionAgent.parameterDefault(parameterTypes[index], defaultParameterValues[index]);
            promptForParameter[index] = requiredParameters[index] && @params[index] == null;
            isParameterSet[index] = !requiredParameters[index] || @params[index] != null;
          }
          else if (StringImpl.equals(entry, (object) "?"))
          {
            @params[index] = (Naked) null;
            promptForParameter[index] = true;
            isParameterSet[index] = false;
          }
          else
          {
            @params[index] = ActionAgent.parameterText(entry, parameterTypes[index], instance.getOptions()[index], context);
            promptForParameter[index] = !requiredParameters[index] || @params[index] != null;
            isParameterSet[index] = !requiredParameters[index] || @params[index] != null;
          }
        }
        for (int index = parametersLength; index < parameterCount; ++index)
        {
          if (defaultParameterValues[index] != null)
            @params[index] = ActionAgent.parameterDefault(parameterTypes[index], defaultParameterValues[index]);
          bool flag = false;
          if (@params[index] is NakedValue)
          {
            isParameterSet[index] = !requiredParameters[index] ? flag : ((((NakedValue) @params[index]).isEmpty() ? 1 : 0) ^ 1) != 0;
          }
          else
          {
            isParameterSet[index] = !requiredParameters[index] ? flag : @params[index] != null;
            promptForParameter[index] = ((isParameterSet[index] ? 1 : 0) ^ 1) != 0;
          }
        }
        return new ActionAgent(action, target, @params, isParameterSet, promptForParameter, instance);
      }
      return (ActionAgent) null;
    }

    private static Naked parameterText(
      string entry,
      NakedObjectSpecification type,
      object[] options,
      Context context)
    {
      if (type.isValue())
      {
        NakedValue valueInstance = NakedObjects.getObjectLoader().createValueInstance(type);
        valueInstance.parseTextEntry(entry);
        return (Naked) valueInstance;
      }
      if (!type.isObject())
        throw new IllegalDispatchException(new StringBuffer().append("Can't handle entry for ").append(type.getSingularName()).append(" parameter").ToString());
      if (options.Length > 0)
      {
        Matcher matcher = (Matcher) new ActionAgent.ObjectMatcher(options);
        Naked naked = (Naked) MatchAlgorithm.match(entry, matcher);
        if (naked != null)
          return naked;
      }
      else if (type.isLookup())
      {
        Naked lookup = ActionAgent.findLookup(entry, type);
        if (lookup != null)
          return lookup;
      }
      Agent agent = context.findAgent(entry);
      return agent is ObjectAgent ? (Naked) ((ObjectAgent) agent).getObject() : throw new IllegalDispatchException(new StringBuffer().append("No object matching ").append(entry).ToString());
    }

    private static Naked findLookup(string entry, NakedObjectSpecification type)
    {
      Matcher matcher = (Matcher) new ActionAgent.ElementMatcher(NakedObjects.getObjectPersistor().allInstances(type, false).elements());
      return (Naked) MatchAlgorithm.match(entry, matcher);
    }

    private static Naked parameterDefault(
      NakedObjectSpecification parameterType,
      object defaultParameterValue)
    {
      if (parameterType.isValue())
        return (Naked) NakedObjects.getObjectLoader().createAdapterForValue(defaultParameterValue);
      if (!parameterType.isObject())
        return (Naked) null;
      return defaultParameterValue == null ? (Naked) null : (Naked) NakedObjects.getObjectLoader().getAdapterForElseCreateAdapterForTransient(defaultParameterValue);
    }

    private static Action findAction(
      Action[] actions,
      int parametersLength,
      string actionName)
    {
      Matcher matcher = (Matcher) new ActionMatcher(actions);
      Action action = (Action) MatchAlgorithm.match(actionName, matcher);
      return action != null && parametersLength <= action.getParameterCount() ? action : (Action) null;
    }

    public static bool methodExists(Action[] actions, Command command)
    {
      string parameter = command.getParameter(0);
      string actionName = StringImpl.substring(parameter, StringImpl.lastIndexOf(parameter, 46) + 1);
      int parametersLength = command.getNumberOfParameters() - 1;
      return ActionAgent.findAction(actions, parametersLength, actionName) != null;
    }

    private ActionAgent(
      Action action,
      NakedObject target,
      Naked[] @params,
      bool[] isParameterSet,
      bool[] promptForParameter,
      ActionHelper helper)
    {
      this.@params = @params;
      this.target = target;
      this.action = action;
      this.isParameterSet = isParameterSet;
      this.promptForParameter = promptForParameter;
      this.helper = helper;
      this.findNextEmptyParameter();
    }

    private bool canSetReferenceParameter(NakedObject @object) => @object.getSpecification().isOfType(this.action.getParameterTypes()[this.parameterIndex]);

    public virtual bool canSetValueParameter(string value)
    {
      NakedObjectSpecification parameterType = this.action.getParameterTypes()[this.parameterIndex];
      NakedValue valueInstance = NakedObjects.getObjectLoader().createValueInstance(parameterType);
      try
      {
        valueInstance.parseTextEntry(value);
      }
      catch (TextEntryParseException ex)
      {
        return false;
      }
      return true;
    }

    public virtual string debug()
    {
      string str = this.target != null ? this.target.titleString() : "null";
      return new StringBuffer().append("Action '").append(this.action.getName()).append("' on ").append(str).ToString();
    }

    public virtual void list(View view, string[] layout)
    {
      string str1 = this.target.titleString() ?? new StringBuffer().append("(").append(this.target.getSpecification().getSingularName()).append(")").ToString();
      view.display(new StringBuffer().append(str1).append(": ").append(this.action.getName()).append(!this.canExecute() ? "" : " (ready)").ToString());
      string[] parameterLabels = this.helper.getParameterLabels();
      int maxWidth = 0;
      for (int index = 0; index < parameterLabels.Length; ++index)
        maxWidth = Math.max(maxWidth, StringImpl.length(parameterLabels[index]));
      for (int i = 0; i < this.@params.Length; ++i)
      {
        string str2 = i != this.parameterIndex ? "  " : " *";
        view.display(new StringBuffer().append(str2).append(this.parameterDetails(i, maxWidth)).ToString());
      }
    }

    public virtual void showParameter(View view) => view.display(this.parameterDetails(this.parameterIndex, 0));

    private string parameterDetails(int i, int maxWidth)
    {
      string str1 = this.@params[i] != null ? this.@params[i].titleString() : "null";
      string str2 = !this.helper.getRequiredParameters()[i] ? "" : " (required)";
      string parameterLabel = this.helper.getParameterLabels()[i];
      return new StringBuffer().append(i + 1).append(". ").append(parameterLabel).append(": ").append(Util.padding(maxWidth, parameterLabel)).append(str1).append(" ").append(str2).ToString();
    }

    public virtual void execute(Context context, View view)
    {
      Consent consent = this.action.isParameterSetValid((NakedReference) this.target, this.@params);
      if (consent.isAllowed())
      {
        context.removeAgent();
        Naked naked = this.action.execute((NakedReference) this.target, this.@params);
        switch (naked)
        {
          case NakedObject _:
            context.addObject((Agent) new ObjectAgent((NakedObject) naked));
            break;
          case NakedCollection _:
            int num = ((NakedCollection) naked).size();
            if (num <= 0)
              break;
            string str = !(naked is TypedNakedCollection) ? "Objects " : ((TypedNakedCollection) naked).getElementSpecification().getPluralName();
            context.addObject((Agent) new CollectionAgent((NakedCollection) naked, new StringBuffer().append("Collection of ").append(num).append(" ").append(str).ToString()));
            break;
        }
      }
      else
        view.error(new StringBuffer().append("Can't invoke action: ").append(consent.getReason()).ToString());
    }

    private void findNextEmptyParameter()
    {
      int index = 0;
      while (index <= this.isParameterSet.Length && index != this.@params.Length && this.isParameterSet[index])
        ++index;
      this.parameterIndex = index;
    }

    public virtual string getName() => "Action";

    public virtual Agent findAgent(string lowecaseTitle) => (Agent) null;

    public virtual string getPrompt()
    {
      if (!this.isCollectingParameters())
        return new StringBuffer().append(this.action.getName()).append(!this.canExecute() ? "" : " (ready)").ToString();
      string parameterLabel = this.helper.getParameterLabels()[this.parameterIndex];
      return new StringBuffer().append(this.parameterIndex + 1).append(". ").append(parameterLabel).append(!this.isValueEntry() ? "" : ":").ToString();
    }

    public virtual bool canExecute()
    {
      bool[] requiredParameters = this.helper.getRequiredParameters();
      for (int index = 0; index < this.isParameterSet.Length; ++index)
      {
        if ((this.promptForParameter[index] || requiredParameters[index]) && !this.isParameterSet[index])
          return false;
      }
      return true;
    }

    public virtual bool isValueEntry() => this.isCollectingParameters() && this.action.getParameterTypes()[this.parameterIndex].isValue();

    public virtual bool isCollectingParameters() => this.parameterIndex < this.@params.Length;

    public virtual void options(View view)
    {
      object[] option = this.helper.getOptions()[this.parameterIndex];
      if (option.Length > 0)
      {
        for (int index = 0; option != null && index < option.Length; ++index)
        {
          NakedObject adapterFor = NakedObjects.getObjectLoader().getAdapterFor(option[index]);
          view.display(adapterFor.titleString());
        }
      }
      else
      {
        if (!this.getParameterType().isLookup())
          return;
        Enumeration enumeration = NakedObjects.getObjectPersistor().allInstances(this.getParameterType(), false).elements();
        while (enumeration.hasMoreElements())
        {
          NakedObject nakedObject = (NakedObject) enumeration.nextElement();
          view.display(nakedObject.titleString());
        }
      }
    }

    public virtual void setReferenceParameter(string optionTitle)
    {
      object[] option = this.helper.getOptions()[this.parameterIndex];
      NakedObject @object;
      if (option.Length > 0)
      {
        Matcher matcher = (Matcher) new ActionAgent.OptionMatcher(option);
        @object = (NakedObject) MatchAlgorithm.match(optionTitle, matcher);
      }
      else
        @object = (NakedObject) ActionAgent.findLookup(optionTitle, this.getParameterType());
      if (@object == null)
        throw new IllegalDispatchException(new StringBuffer().append("No option with title ").append(optionTitle).ToString());
      this.setReferenceParameter(@object);
    }

    public virtual void setReferenceParameter(NakedObject @object)
    {
      this.@params[this.parameterIndex] = this.canSetReferenceParameter(@object) ? (Naked) @object : throw new IllegalDispatchException("Can't set parameter; type is wrong");
      this.isParameterSet[this.parameterIndex] = @object != null || this.helper.getRequiredParameters()[this.parameterIndex];
      this.findNextEmptyParameter();
    }

    public virtual void setParameterToDefault()
    {
      object defaultParameterValue = this.helper.getDefaultParameterValues()[this.parameterIndex];
      if (this.action.getParameterTypes()[this.parameterIndex].isValue())
        this.setValueParameter(\u003CVerifierFix\u003E.genCastToString(defaultParameterValue));
      else
        this.setReferenceParameter(NakedObjects.getObjectLoader().getAdapterFor(defaultParameterValue));
    }

    public virtual void clearParameter() => this.@params[this.parameterIndex] = (Naked) null;

    public virtual void setValueParameter(string value)
    {
      NakedObjectSpecification parameterType = this.action.getParameterTypes()[this.parameterIndex];
      this.@params[this.parameterIndex] = (Naked) NakedObjects.getObjectLoader().createValueInstance(parameterType);
      ((NakedValue) this.@params[this.parameterIndex]).parseTextEntry(value);
      this.isParameterSet[this.parameterIndex] = true;
      this.findNextEmptyParameter();
    }

    public virtual void type(View view)
    {
      NakedObjectSpecification[] parameterTypes = this.action.getParameterTypes();
      view.display(parameterTypes[this.parameterIndex].getSingularName());
    }

    public virtual NakedObjectSpecification getParameterType() => this.action.getParameterTypes()[this.parameterIndex];

    public virtual bool hasNextParameter() => this.parameterIndex + 1 < this.@params.Length;

    public virtual bool hasPreviousParameter() => this.parameterIndex > 0;

    public virtual bool isReplaceable() => false;

    [JavaFlags(0)]
    public virtual void nextParameter()
    {
      if (!this.hasNextParameter())
        throw new IllegalDispatchException("No previous parameter");
      ++this.parameterIndex;
    }

    [JavaFlags(0)]
    public virtual void previousParameter()
    {
      if (!this.hasPreviousParameter())
        throw new IllegalDispatchException("No previous parameter");
      this.parameterIndex += -1;
    }

    public virtual bool hasOptions()
    {
      object[][] options = this.helper.getOptions();
      return options[this.parameterIndex] != null && options[this.parameterIndex].Length > 0;
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ActionAgent actionAgent = this;
      ObjectImpl.clone((object) actionAgent);
      return ((object) actionAgent).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaInterfaces("1;org/nakedobjects/viewer/cli/Matcher;")]
    [JavaFlags(58)]
    private sealed class OptionMatcher : Matcher
    {
      private readonly object[] options;
      private int i;
      private NakedObject adapter;

      [JavaFlags(2)]
      public OptionMatcher(object[] options)
      {
        this.i = 0;
        this.options = options;
      }

      public virtual bool hasMoreElements() => this.options != null && this.i < this.options.Length;

      public virtual object getElement() => (object) this.adapter;

      public virtual string nextElement()
      {
        NakedObjectLoader objectLoader = NakedObjects.getObjectLoader();
        object[] options = this.options;
        int i;
        this.i = (i = this.i) + 1;
        int index = i;
        object @object = options[index];
        this.adapter = objectLoader.getAdapterForElseCreateAdapterForTransient(@object);
        return this.adapter.titleString();
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        ActionAgent.OptionMatcher optionMatcher = this;
        ObjectImpl.clone((object) optionMatcher);
        return ((object) optionMatcher).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [JavaInterfaces("1;org/nakedobjects/viewer/cli/Matcher;")]
    [JavaFlags(58)]
    private sealed class ElementMatcher : Matcher
    {
      private readonly Enumeration enumeration;
      private NakedObject adapter;

      [JavaFlags(2)]
      public ElementMatcher(Enumeration enumeration) => this.enumeration = enumeration;

      public virtual bool hasMoreElements() => this.enumeration.hasMoreElements();

      public virtual object getElement() => (object) this.adapter;

      public virtual string nextElement()
      {
        this.adapter = (NakedObject) this.enumeration.nextElement();
        return this.adapter.titleString();
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        ActionAgent.ElementMatcher elementMatcher = this;
        ObjectImpl.clone((object) elementMatcher);
        return ((object) elementMatcher).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [JavaInterfaces("1;org/nakedobjects/viewer/cli/Matcher;")]
    [JavaFlags(58)]
    private sealed class ObjectMatcher : Matcher
    {
      private readonly object[] options;
      private int i;
      private NakedObject adapter;

      [JavaFlags(2)]
      public ObjectMatcher(object[] options)
      {
        this.i = 0;
        this.options = options;
      }

      public virtual bool hasMoreElements() => this.i < this.options.Length;

      public virtual object getElement() => (object) this.adapter;

      public virtual string nextElement()
      {
        NakedObjectLoader objectLoader = NakedObjects.getObjectLoader();
        object[] options = this.options;
        int i;
        this.i = (i = this.i) + 1;
        int index = i;
        object @object = options[index];
        this.adapter = objectLoader.getAdapterFor(@object);
        return this.adapter.titleString();
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        ActionAgent.ObjectMatcher objectMatcher = this;
        ObjectImpl.clone((object) objectMatcher);
        return ((object) objectMatcher).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
