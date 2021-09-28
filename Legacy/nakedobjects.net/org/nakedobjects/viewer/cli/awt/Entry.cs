// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.awt.Entry
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.viewer.cli.awt
{
  public class Entry
  {
    private string text;

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void set(string text)
    {
      while (this.text != null)
      {
        try
        {
          ObjectImpl.wait((object) this);
        }
        catch (InterruptedException ex)
        {
        }
      }
      this.text = text;
      ObjectImpl.notify((object) this);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual string get()
    {
      while (this.text == null)
      {
        try
        {
          ObjectImpl.wait((object) this);
        }
        catch (InterruptedException ex)
        {
        }
      }
      string text = this.text;
      this.text = (string) null;
      ObjectImpl.notify((object) this);
      return text;
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Entry entry = this;
      ObjectImpl.clone((object) entry);
      return ((object) entry).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
