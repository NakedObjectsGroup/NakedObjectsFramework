// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.Dump
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object
{
  public class Dump
  {
    private static void collectionGraph(
      NakedCollection collection,
      int level,
      Vector recursiveElements,
      DebugString s)
    {
      if (recursiveElements.contains((object) collection))
      {
        s.append((object) "*\n");
      }
      else
      {
        recursiveElements.addElement((object) collection);
        Enumeration enumeration = collection.elements();
        while (enumeration.hasMoreElements())
        {
          Dump.graphIndent(s, level);
          NakedObject nakedObject = (NakedObject) enumeration.nextElement();
          s.append((object) nakedObject);
          s.indent();
          Dump.graph((Naked) nakedObject, level + 1, recursiveElements, s);
          s.unindent();
        }
      }
    }

    public static string graph(Naked @object)
    {
      DebugString s = new DebugString();
      Dump.graph(@object, s);
      return s.ToString();
    }

    public static void graph(Naked @object, DebugString s)
    {
      Dump.simpleObject(@object, s);
      s.appendln();
      s.append((object) @object);
      Dump.graph(@object, 0, new Vector(25, 10), s);
    }

    private static void simpleObject(Naked @object, DebugString s)
    {
      // ISSUE: unable to decompile the method.
    }

    private static void graph(Naked @object, int level, Vector ignoreObjects, DebugString info)
    {
      if (level > 3)
      {
        info.appendln("...");
      }
      else
      {
        info.blankLine();
        switch (@object)
        {
          case NakedCollection _:
            Dump.collectionGraph((NakedCollection) @object, level, ignoreObjects, info);
            break;
          case NakedObject _:
            Dump.objectGraph((NakedObject) @object, level, ignoreObjects, info);
            break;
        }
      }
    }

    public static string graph(Naked @object, Vector excludedObjects)
    {
      DebugString info = new DebugString();
      info.append((object) @object);
      Dump.graph(@object, 0, excludedObjects, info);
      return info.ToString();
    }

    private static void graphIndent(DebugString s, int level)
    {
      for (int index = 0; index < level; ++index)
        s.append((object) new StringBuffer().append(Debug.indentString(4)).append("|").ToString());
      s.append((object) new StringBuffer().append(Debug.indentString(4)).append("+--").ToString());
    }

    public static string adapter(Naked @object)
    {
      DebugString @string = new DebugString();
      Dump.adapter(@object, @string);
      return @string.ToString();
    }

    public static void adapter(Naked @object, DebugString @string)
    {
      // ISSUE: unable to decompile the method.
    }

    private static void objectGraph(
      NakedObject @object,
      int level,
      Vector ignoreObjects,
      DebugString s)
    {
      // ISSUE: unable to decompile the method.
    }

    public static string specification(Naked @object)
    {
      DebugString debug = new DebugString();
      Dump.specification(@object, debug);
      return debug.ToString();
    }

    public static void specification(Naked naked, DebugString debug)
    {
      // ISSUE: unable to decompile the method.
    }

    private static void specificationActionMethods(
      NakedObjectSpecification specification,
      DebugString debug)
    {
      // ISSUE: unable to decompile the method.
    }

    private static void specificationClassMethods(
      NakedObjectSpecification specification,
      DebugString debug)
    {
      // ISSUE: unable to decompile the method.
    }

    private static void specificationFields(
      NakedObjectSpecification specification,
      DebugString debug)
    {
      // ISSUE: unable to decompile the method.
    }

    private static void specificationMethods(
      Action[] userActions,
      Action[] explActions,
      Action[] debActions,
      DebugString debug)
    {
      if (userActions.Length == 0 && explActions.Length == 0 && debActions.Length == 0)
      {
        debug.appendln("no actions...");
      }
      else
      {
        debug.appendln("User actions");
        debug.indent();
        for (int count = 0; count < userActions.Length; ++count)
          Dump.actionDetails(debug, userActions[count], 8, count);
        debug.unindent();
        debug.appendln("Exploration actions");
        debug.indent();
        for (int count = 0; count < explActions.Length; ++count)
          Dump.actionDetails(debug, explActions[count], 8, count);
        debug.unindent();
        debug.appendln("Debug actions");
        debug.indent();
        for (int count = 0; count < debActions.Length; ++count)
          Dump.actionDetails(debug, debActions[count], 8, count);
        debug.unindent();
      }
    }

    private static void actionDetails(DebugString debug, Action a, int indent, int count)
    {
      // ISSUE: unable to decompile the method.
    }

    private static string[] specificationNames(NakedObjectSpecification[] specifications)
    {
      int length = specifications.Length;
      string[] strArray = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < strArray.Length; ++index)
        strArray[index] = specifications[index].getFullName();
      return strArray;
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      Dump dump = this;
      ObjectImpl.clone((object) dump);
      return ((object) dump).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
