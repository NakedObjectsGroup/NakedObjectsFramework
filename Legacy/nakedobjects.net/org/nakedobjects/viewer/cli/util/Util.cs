// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.util.Util
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using System.ComponentModel;

namespace org.nakedobjects.viewer.cli.util
{
  public class Util
  {
    private static readonly int MAX_LENGTH;
    private const string SPACES = "                               ";

    public static string padding(int max, string label) => max == 0 ? "" : Util.padding(max - StringImpl.length(label) + 1);

    private static string padding(int length) => StringImpl.substring("                               ", 0, Math.min(Util.MAX_LENGTH, length));

    public static string titleString(NakedObject @object) => @object.titleString() ?? new StringBuffer().append("(").append(@object.getSpecification().getSingularName()).append(")").ToString();

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static Util()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Util util = this;
      ObjectImpl.clone((object) util);
      return ((object) util).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
