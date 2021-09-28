// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.debug.FileInput
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;

namespace org.nakedobjects.viewer.cli.debug
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Input;")]
  public class FileInput : Input
  {
    private readonly Input input;
    private readonly BufferedReader reader;

    public FileInput(BufferedReader reader, Input input)
    {
      this.reader = reader;
      this.input = input;
    }

    public virtual void connect() => this.input.connect();

    public virtual void disconnect()
    {
      this.input.disconnect();
      try
      {
        this.reader.close();
      }
      catch (IOException ex)
      {
        ((Throwable) ex).printStackTrace();
      }
    }

    [JavaThrownExceptions("1;org/nakedobjects/viewer/cli/ConnectionException;")]
    public virtual string accept()
    {
      string str = (string) null;
      try
      {
        do
        {
          str = this.reader.readLine();
          if (str == null)
            break;
        }
        while (StringImpl.startsWith(str, "#"));
      }
      catch (IOException ex)
      {
        ((Throwable) ex).printStackTrace();
      }
      return str ?? this.input.accept();
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      FileInput fileInput = this;
      ObjectImpl.clone((object) fileInput);
      return ((object) fileInput).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
