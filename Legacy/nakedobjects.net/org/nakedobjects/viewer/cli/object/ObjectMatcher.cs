// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.object.ObjectMatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.cli.@object;

namespace org.nakedobjects.viewer.cli.@object
{
  [JavaFlags(48)]
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Matcher;")]
  public sealed class ObjectMatcher : Matcher
  {
    private readonly NakedObjectField[] fields;
    private readonly NakedObject @object;
    private int fieldIndex;
    private NakedObject element;

    [JavaFlags(0)]
    public ObjectMatcher(NakedObject @object, NakedObjectField[] fields)
    {
      this.fieldIndex = 0;
      this.@object = @object;
      this.fields = fields;
    }

    public virtual object getElement() => (object) this.element;

    public virtual bool hasMoreElements() => this.fieldIndex < this.fields.Length;

    public virtual string nextElement()
    {
      if (this.fields[this.fieldIndex].isObject())
        this.element = (NakedObject) this.fields[this.fieldIndex].get(this.@object);
      ++this.fieldIndex;
      return this.element == null ? "" : this.element.titleString();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      ObjectMatcher objectMatcher = this;
      ObjectImpl.clone((object) objectMatcher);
      return ((object) objectMatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
