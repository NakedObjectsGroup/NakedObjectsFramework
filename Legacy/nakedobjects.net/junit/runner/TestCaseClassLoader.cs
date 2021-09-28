// Decompiled with JetBrains decompiler
// Type: junit.runner.TestCaseClassLoader
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.net;
using java.util;
using java.util.zip;
using System.Runtime.CompilerServices;

namespace junit.runner
{
  public class TestCaseClassLoader : ClassLoader
  {
    private Vector fPathItems;
    private string[] defaultExclusions;
    [JavaFlags(24)]
    public const string EXCLUDED_FILE = "excluded.properties";
    private Vector fExcluded;

    public TestCaseClassLoader()
      : this(java.lang.System.getProperty("java.class.path"))
    {
    }

    public TestCaseClassLoader(string classPath)
    {
      int length = 3;
      string[] strArray = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
      strArray[0] = "junit.framework.";
      strArray[1] = "junit.extensions.";
      strArray[2] = "junit.runner.";
      this.defaultExclusions = strArray;
      this.scanPath(classPath);
      this.readExcludedPackages();
    }

    private void scanPath(string classPath)
    {
      string property = java.lang.System.getProperty("path.separator");
      this.fPathItems = new Vector(10);
      StringTokenizer stringTokenizer = new StringTokenizer(classPath, property);
      while (stringTokenizer.hasMoreTokens())
        this.fPathItems.addElement((object) stringTokenizer.nextToken());
    }

    public virtual URL getResource(string name) => ClassLoader.getSystemResource(name);

    public virtual InputStream getResourceAsStream(string name) => ClassLoader.getSystemResourceAsStream(name);

    public virtual bool isExcluded(string name)
    {
      for (int index = 0; index < this.fExcluded.size(); ++index)
      {
        if (StringImpl.startsWith(name, \u003CVerifierFix\u003E.genCastToString(this.fExcluded.elementAt(index))))
          return true;
      }
      return false;
    }

    [JavaThrownExceptions("1;java/lang/ClassNotFoundException;")]
    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual Class loadClass(string name, bool resolve)
    {
      Class @class = this.findLoadedClass(name);
      if (@class != null)
        return @class;
      if (this.isExcluded(name))
      {
        try
        {
          return this.findSystemClass(name);
        }
        catch (ClassNotFoundException ex)
        {
        }
      }
      if (@class == null)
      {
        sbyte[] numArray = this.lookupClassData(name);
        if (numArray == null)
          throw new ClassNotFoundException();
        @class = this.defineClass(name, numArray, 0, numArray.Length);
      }
      if (resolve)
        this.resolveClass(@class);
      return @class;
    }

    [JavaThrownExceptions("1;java/lang/ClassNotFoundException;")]
    private sbyte[] lookupClassData(string className)
    {
      for (int index = 0; index < this.fPathItems.size(); ++index)
      {
        string str = \u003CVerifierFix\u003E.genCastToString(this.fPathItems.elementAt(index));
        string fileName = new StringBuffer().append(StringImpl.replace(className, '.', '/')).append(".class").ToString();
        sbyte[] numArray = !this.isJar(str) ? this.loadFileData(str, fileName) : this.loadJarData(str, fileName);
        if (numArray != null)
          return numArray;
      }
      throw new ClassNotFoundException(className);
    }

    [JavaFlags(0)]
    public virtual bool isJar(string pathEntry) => StringImpl.endsWith(pathEntry, ".jar") || StringImpl.endsWith(pathEntry, ".zip");

    private sbyte[] loadFileData(string path, string fileName)
    {
      File f = new File(path, fileName);
      return f.exists() ? this.getClassData(f) : (sbyte[]) null;
    }

    private sbyte[] getClassData(File f)
    {
      try
      {
        FileInputStream fileInputStream = new FileInputStream(f);
        ByteArrayOutputStream arrayOutputStream = new ByteArrayOutputStream(1000);
        int length = 1000;
        sbyte[] numArray = length >= 0 ? new sbyte[length] : throw new NegativeArraySizeException();
        int num;
        while ((num = fileInputStream.read(numArray)) != -1)
          arrayOutputStream.write(numArray, 0, num);
        fileInputStream.close();
        ((OutputStream) arrayOutputStream).close();
        return arrayOutputStream.toByteArray();
      }
      catch (IOException ex)
      {
      }
      return (sbyte[]) null;
    }

    private sbyte[] loadJarData(string path, string fileName)
    {
      InputStream inputStream = (InputStream) null;
      File file = new File(path);
      if (!file.exists())
        return (sbyte[]) null;
      ZipFile zipFile;
      try
      {
        zipFile = new ZipFile(file);
      }
      catch (IOException ex)
      {
        return (sbyte[]) null;
      }
      ZipEntry entry = zipFile.getEntry(fileName);
      if (entry == null)
        return (sbyte[]) null;
      int size = (int) entry.getSize();
      try
      {
        inputStream = zipFile.getInputStream(entry);
        int length = size;
        sbyte[] numArray = length >= 0 ? new sbyte[length] : throw new NegativeArraySizeException();
        int num;
        for (int index = 0; index < size; index += num)
          num = inputStream.read(numArray, index, numArray.Length - index);
        zipFile.close();
        return numArray;
      }
      catch (IOException ex)
      {
      }
      finally
      {
        try
        {
          inputStream?.close();
        }
        catch (IOException ex)
        {
        }
      }
      return (sbyte[]) null;
    }

    private void readExcludedPackages()
    {
      this.fExcluded = new Vector(10);
      for (int index = 0; index < this.defaultExclusions.Length; ++index)
        this.fExcluded.addElement((object) this.defaultExclusions[index]);
      InputStream resourceAsStream = ObjectImpl.getClass((object) this).getResourceAsStream("excluded.properties");
      if (resourceAsStream == null)
        return;
      Properties properties = new Properties();
      try
      {
        properties.load(resourceAsStream);
      }
      catch (IOException ex)
      {
        return;
      }
      finally
      {
        try
        {
          resourceAsStream.close();
        }
        catch (IOException ex)
        {
        }
      }
      Enumeration enumeration = properties.propertyNames();
      while (enumeration.hasMoreElements())
      {
        string str1 = \u003CVerifierFix\u003E.genCastToString(enumeration.nextElement());
        if (StringImpl.startsWith(str1, "excluded."))
        {
          string str2 = StringImpl.trim(properties.getProperty(str1));
          if (StringImpl.endsWith(str2, "*"))
            str2 = StringImpl.substring(str2, 0, StringImpl.length(str2) - 1);
          if (StringImpl.length(str2) > 0)
            this.fExcluded.addElement((object) str2);
        }
      }
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public virtual object MemberwiseClone()
    {
      TestCaseClassLoader testCaseClassLoader = this;
      ObjectImpl.clone((object) testCaseClassLoader);
      return ((object) testCaseClassLoader).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public virtual string ToString() => ObjectImpl.jloToString((object) this);
  }
}
