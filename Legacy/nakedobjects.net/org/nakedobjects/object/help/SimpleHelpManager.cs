// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.help.SimpleHelpManager
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using org.apache.log4j;
using org.nakedobjects.@object.help;
using org.nakedobjects.@object.reflect;
using System.ComponentModel;

namespace org.nakedobjects.@object.help
{
  [JavaInterfaces("1;org/nakedobjects/object/help/HelpManager;")]
  public class SimpleHelpManager : HelpManager
  {
    private static readonly Logger LOG;
    private const string CLASS_PREFIX = "c:";
    private const string NAME_PREFIX = "m:";
    private string fileName;

    public virtual string help(MemberIdentifier identifier)
    {
      BufferedReader bufferedReader = (BufferedReader) null;
      try
      {
        bufferedReader = this.getReader();
        if (bufferedReader == null)
          return "No help available (no file found)";
        string str1 = new StringBuffer().append("c:").append(StringImpl.toLowerCase(identifier.getClassName())).ToString();
        string str2 = new StringBuffer().append("m:").append(StringImpl.toLowerCase(identifier.getName())).ToString();
        StringBuffer stringBuffer = new StringBuffer();
        bool flag1 = true;
        bool flag2 = StringImpl.length(identifier.getName()) > 0;
        string str3;
        while ((str3 = bufferedReader.readLine()) != null)
        {
          if (StringImpl.length(str3) <= 0 || StringImpl.charAt(str3, 0) != '#')
          {
            if (StringImpl.equals(StringImpl.toLowerCase(str3), (object) str1))
              flag1 = false;
            else if (!flag1)
            {
              if (!StringImpl.startsWith(StringImpl.toLowerCase(str3), "c:"))
              {
                if (StringImpl.equals(StringImpl.toLowerCase(str3), (object) str2))
                  flag2 = false;
                else if (!flag2)
                {
                  if (!StringImpl.startsWith(StringImpl.toLowerCase(str3), "m:"))
                  {
                    stringBuffer.append(str3);
                    stringBuffer.append('\n');
                  }
                  else
                    break;
                }
              }
              else
                break;
            }
          }
        }
        return stringBuffer.ToString();
      }
      catch (FileNotFoundException ex)
      {
        SimpleHelpManager.LOG.error((object) "opening help file", (Throwable) ex);
        return new StringBuffer().append("Failure opening help file: ").append(((Throwable) ex).getMessage()).ToString();
      }
      catch (IOException ex)
      {
        SimpleHelpManager.LOG.error((object) "reading help file", (Throwable) ex);
        return new StringBuffer().append("Failure reading help file: ").append(((Throwable) ex).getMessage()).ToString();
      }
      finally
      {
        if (bufferedReader != null)
        {
          try
          {
            bufferedReader.close();
          }
          catch (IOException ex)
          {
          }
        }
      }
    }

    [JavaFlags(4)]
    [JavaThrownExceptions("1;java/io/FileNotFoundException;")]
    public virtual BufferedReader getReader()
    {
      File file = new File(this.fileName);
      if (file.exists())
        return new BufferedReader((Reader) new FileReader(file));
      if (SimpleHelpManager.LOG.isWarnEnabled())
        SimpleHelpManager.LOG.warn((object) new StringBuffer().append("No help file found: ").append(file.getAbsolutePath()).ToString());
      return (BufferedReader) null;
    }

    public virtual void setFileName(string fileName) => this.fileName = fileName;

    public virtual void init()
    {
    }

    public virtual void shutdown()
    {
    }

    public SimpleHelpManager() => this.fileName = "help.txt";

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static SimpleHelpManager()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      SimpleHelpManager simpleHelpManager = this;
      ObjectImpl.clone((object) simpleHelpManager);
      return ((object) simpleHelpManager).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
