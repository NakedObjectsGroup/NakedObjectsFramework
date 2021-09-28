// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.persistence.file.utility.CopyXmlObjectStore
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using com.ms.vjsharp.util;
using java.io;
using java.lang;
using org.nakedobjects.utility;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.persistence.file.utility
{
  public class CopyXmlObjectStore
  {
    public static void main(string[] args)
    {
      // ISSUE: type reference
      RuntimeHelpers.RunClassConstructor(__typeref (ObjectImpl));
      string workingDirectory = args[0];
      CopyXmlObjectStore.copyAllFiles(args[1], workingDirectory);
      Utilities.cleanupAfterMainReturns();
    }

    private static void copyAllFiles(string testDirectory, string workingDirectory)
    {
      File file1 = new File(testDirectory);
      File file2 = new File(workingDirectory);
      if (!file2.exists())
        file2.mkdirs();
      if (file2.isFile())
        throw new NakedObjectRuntimeException(new StringBuffer().append("To directory is actually a file ").append(file2.getAbsolutePath()).ToString());
      string[] strArray = file1.list();
      for (int index = 0; index < strArray.Length; ++index)
        CopyXmlObjectStore.copyFile(new File(file1, strArray[index]), new File(file2, strArray[index]));
    }

    private static void copyFile(File from, File to)
    {
      BufferedInputStream bufferedInputStream = (BufferedInputStream) null;
      BufferedOutputStream bufferedOutputStream = (BufferedOutputStream) null;
      try
      {
        bufferedInputStream = new BufferedInputStream((InputStream) new FileInputStream(from));
        bufferedOutputStream = new BufferedOutputStream((OutputStream) new FileOutputStream(to));
        int length = 2048;
        sbyte[] numArray = length >= 0 ? new sbyte[length] : throw new NegativeArraySizeException();
        int num;
        while ((num = ((FilterInputStream) bufferedInputStream).read(numArray)) > 0)
          bufferedOutputStream.write(numArray, 0, num);
      }
      catch (IOException ex)
      {
        throw new NakedObjectRuntimeException(new StringBuffer().append("Error copying file ").append(from.getAbsolutePath()).append(" to ").append(to.getAbsolutePath()).ToString(), (Throwable) ex);
      }
      finally
      {
        if (bufferedInputStream != null)
        {
          try
          {
            bufferedInputStream.close();
          }
          catch (IOException ex)
          {
          }
        }
        if (bufferedOutputStream != null)
        {
          try
          {
            bufferedOutputStream.close();
          }
          catch (IOException ex)
          {
          }
        }
      }
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      CopyXmlObjectStore copyXmlObjectStore = this;
      ObjectImpl.clone((object) copyXmlObjectStore);
      return ((object) copyXmlObjectStore).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
