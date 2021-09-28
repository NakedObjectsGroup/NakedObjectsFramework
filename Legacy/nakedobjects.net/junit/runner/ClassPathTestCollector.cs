// Decompiled with JetBrains decompiler
// Type: junit.runner.ClassPathTestCollector
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.util;
using System.ComponentModel;

namespace junit.runner
{
  [JavaInterfaces("1;junit/runner/TestCollector;")]
  public abstract class ClassPathTestCollector : TestCollector
  {
    [JavaFlags(24)]
    public static readonly int SUFFIX_LENGTH;

    public virtual Enumeration collectTests() => this.collectFilesInPath(java.lang.System.getProperty("java.class.path")).elements();

    public virtual Hashtable collectFilesInPath(string classPath) => this.collectFilesInRoots(this.splitClassPath(classPath));

    [JavaFlags(0)]
    public virtual Hashtable collectFilesInRoots(Vector roots)
    {
      Hashtable result = new Hashtable(100);
      Enumeration enumeration = roots.elements();
      while (enumeration.hasMoreElements())
        this.gatherFiles(new File(\u003CVerifierFix\u003E.genCastToString(enumeration.nextElement())), "", result);
      return result;
    }

    [JavaFlags(0)]
    public virtual void gatherFiles(File classRoot, string classFileName, Hashtable result)
    {
      File file = new File(classRoot, classFileName);
      if (file.isFile())
      {
        if (!this.isTestClass(classFileName))
          return;
        string str = this.classNameFromFile(classFileName);
        result.put((object) str, (object) str);
      }
      else
      {
        string[] strArray = file.list();
        if (strArray == null)
          return;
        for (int index = 0; index < strArray.Length; ++index)
          this.gatherFiles(classRoot, new StringBuffer().append(classFileName).append((char) File.separatorChar).append(strArray[index]).ToString(), result);
      }
    }

    [JavaFlags(0)]
    public virtual Vector splitClassPath(string classPath)
    {
      Vector vector = new Vector();
      string property = java.lang.System.getProperty("path.separator");
      StringTokenizer stringTokenizer = new StringTokenizer(classPath, property);
      while (stringTokenizer.hasMoreTokens())
        vector.addElement((object) stringTokenizer.nextToken());
      return vector;
    }

    [JavaFlags(4)]
    public virtual bool isTestClass(string classFileName) => StringImpl.endsWith(classFileName, ".class") && StringImpl.indexOf(classFileName, 36) < 0 && StringImpl.indexOf(classFileName, "Test") > 0;

    [JavaFlags(4)]
    public virtual string classNameFromFile(string classFileName)
    {
      string str = StringImpl.replace(StringImpl.substring(classFileName, 0, StringImpl.length(classFileName) - ClassPathTestCollector.SUFFIX_LENGTH), (char) File.separatorChar, '.');
      return StringImpl.startsWith(str, ".") ? StringImpl.substring(str, 1) : str;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static ClassPathTestCollector()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ClassPathTestCollector pathTestCollector = this;
      ObjectImpl.clone((object) pathTestCollector);
      return ((object) pathTestCollector).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
