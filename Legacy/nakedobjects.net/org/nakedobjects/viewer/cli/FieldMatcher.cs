// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.FieldMatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.cli
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Matcher;")]
  public class FieldMatcher : Matcher
  {
    private int index;
    private readonly NakedObjectField[] members;

    public FieldMatcher(NakedObjectField[] member)
    {
      this.index = 0;
      this.members = member;
    }

    public virtual object getElement() => (object) this.members[this.index - 1];

    public virtual bool hasMoreElements() => this.index < this.members.Length;

    public virtual string nextElement()
    {
      NakedObjectField[] members = this.members;
      int index1;
      this.index = (index1 = this.index) + 1;
      int index2 = index1;
      return members[index2].getName();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      FieldMatcher fieldMatcher = this;
      ObjectImpl.clone((object) fieldMatcher);
      return ((object) fieldMatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
