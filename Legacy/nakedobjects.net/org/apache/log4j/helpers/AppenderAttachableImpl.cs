// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.helpers.AppenderAttachableImpl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.util;
using org.apache.log4j.spi;

namespace org.apache.log4j.helpers
{
  [JavaInterfaces("1;org/apache/log4j/spi/AppenderAttachable;")]
  public class AppenderAttachableImpl : AppenderAttachable
  {
    [JavaFlags(4)]
    public Vector appenderList;

    public virtual void addAppender(Appender newAppender)
    {
      if (newAppender == null)
        return;
      if (this.appenderList == null)
        this.appenderList = new Vector(1);
      if (this.appenderList.contains((object) newAppender))
        return;
      this.appenderList.addElement((object) newAppender);
    }

    public virtual int appendLoopOnAppenders(LoggingEvent @event)
    {
      int num = 0;
      if (this.appenderList != null)
      {
        num = this.appenderList.size();
        for (int index = 0; index < num; ++index)
          ((Appender) this.appenderList.elementAt(index)).doAppend(@event);
      }
      return num;
    }

    public virtual Enumeration getAllAppenders() => this.appenderList == null ? (Enumeration) null : this.appenderList.elements();

    public virtual Appender getAppender(string name)
    {
      if (this.appenderList == null || name == null)
        return (Appender) null;
      int num = this.appenderList.size();
      for (int index = 0; index < num; ++index)
      {
        Appender appender = (Appender) this.appenderList.elementAt(index);
        if (StringImpl.equals(name, (object) appender.getName()))
          return appender;
      }
      return (Appender) null;
    }

    public virtual bool isAttached(Appender appender)
    {
      if (this.appenderList == null || appender == null)
        return false;
      int num = this.appenderList.size();
      for (int index = 0; index < num; ++index)
      {
        if ((Appender) this.appenderList.elementAt(index) == appender)
          return true;
      }
      return false;
    }

    public virtual void removeAllAppenders()
    {
      if (this.appenderList == null)
        return;
      int num = this.appenderList.size();
      for (int index = 0; index < num; ++index)
        ((Appender) this.appenderList.elementAt(index)).close();
      this.appenderList.removeAllElements();
      this.appenderList = (Vector) null;
    }

    public virtual void removeAppender(Appender appender)
    {
      if (appender == null || this.appenderList == null)
        return;
      this.appenderList.removeElement((object) appender);
    }

    public virtual void removeAppender(string name)
    {
      if (name == null || this.appenderList == null)
        return;
      int num = this.appenderList.size();
      for (int index = 0; index < num; ++index)
      {
        if (StringImpl.equals(name, (object) ((Appender) this.appenderList.elementAt(index)).getName()))
        {
          this.appenderList.removeElementAt(index);
          break;
        }
      }
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      AppenderAttachableImpl appenderAttachableImpl = this;
      ObjectImpl.clone((object) appenderAttachableImpl);
      return ((object) appenderAttachableImpl).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
