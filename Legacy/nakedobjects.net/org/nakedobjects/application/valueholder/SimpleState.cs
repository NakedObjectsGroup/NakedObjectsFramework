// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.valueholder.SimpleState
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.application.control;

namespace org.nakedobjects.application.valueholder
{
  [JavaInterfaces("1;org/nakedobjects/application/control/State;")]
  public class SimpleState : BusinessValueHolder, State
  {
    private const long serialVersionUID = 1;
    private string name;
    private int id;
    private State[] states;

    public SimpleState(int id, string name)
    {
      this.id = id >= 0 ? id : throw new IllegalArgumentException("Id must be 0 or greater");
      this.name = name;
    }

    public override object getValue() => (object) this;

    public SimpleState(State[] states) => this.states = states;

    public override bool userChangeable() => false;

    public override void clear()
    {
      this.id = -1;
      this.name = (string) null;
    }

    [JavaThrownExceptions("1;org/nakedobjects/application/ValueParseException;")]
    public override void parseUserEntry(string text) => throw new ApplicationException();

    public virtual void reset()
    {
      this.id = -1;
      this.name = (string) null;
    }

    public override void restoreFromEncodedString(string data)
    {
      this.id = Integer.valueOf(data).intValue();
      this.name = "unmatched state";
      for (int index = 0; index < this.states.Length; ++index)
      {
        if (this.id == ((SimpleState) this.states[index]).id)
        {
          this.name = ((SimpleState) this.states[index]).name;
          break;
        }
      }
    }

    public override string asEncodedString() => StringImpl.valueOf(this.id);

    public override void copyObject(BusinessValueHolder @object) => throw new ApplicationException();

    public override bool isEmpty() => this.id == -1;

    public override bool Equals(object @object) => @object is SimpleState && ((SimpleState) @object).id == this.id;

    public override bool isSameAs(BusinessValueHolder @object) => @object is SimpleState && ((SimpleState) @object).id == this.id;

    public virtual void changeTo(State state)
    {
      this.id = ((SimpleState) state).id;
      this.name = ((SimpleState) state).name;
    }

    public override string titleString() => this.name;

    public override Title title() => new Title(this.name);
  }
}
