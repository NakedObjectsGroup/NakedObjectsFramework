// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.control.SimpleSession
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.application.control;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.reflector.java.control
{
  [JavaInterfaces("1;org/nakedobjects/object/Session;")]
  public class SimpleSession : Session
  {
    private static readonly org.apache.log4j.Logger LOG;
    private Role[] roles;
    private User user;

    public virtual void debugData(DebugString sb)
    {
      sb.append((object) "User\n");
      sb.append((object) "  Name:     ");
      if (this.user == null)
      {
        sb.append((object) "none");
      }
      else
      {
        sb.append((object) this.user.getName().stringValue());
        sb.append((object) "\n");
        sb.append((object) "  Roles:     ");
        if (this.user.getRoles() == null)
        {
          sb.append((object) "     none");
        }
        else
        {
          Enumeration enumeration = this.user.getRoles().elements();
          while (enumeration.hasMoreElements())
          {
            Role role = (Role) enumeration.nextElement();
            sb.append((object) "           ");
            sb.append((object) role);
            sb.append((object) "\n");
          }
        }
        sb.append((object) "\n");
        sb.append((object) "Root object\n");
        sb.append((object) "  Root object: ");
        object rootObject = this.user.getRootObject();
        sb.append(rootObject);
        if (rootObject is DebugInfo)
        {
          sb.append((object) "\n");
          ((DebugInfo) rootObject).debugData(sb);
        }
      }
      sb.append((object) "\n\n");
    }

    public virtual User getName() => this.user;

    public virtual string getUserName() => this.user.getName().stringValue();

    public virtual bool hasRole(Role role)
    {
      if (this.user == null)
        return true;
      string text = role.getName().stringValue();
      for (int index = 0; index < this.roles.Length; ++index)
      {
        if (this.roles[index].getName().isSameAs(text))
        {
          if (SimpleSession.LOG.isDebugEnabled())
            SimpleSession.LOG.debug((object) new StringBuffer().append("role ").append(text).append(" matches for ").append((object) this).ToString());
          return true;
        }
      }
      if (SimpleSession.LOG.isDebugEnabled())
        SimpleSession.LOG.debug((object) new StringBuffer().append("role ").append(text).append(" not matched for ").append((object) this).ToString());
      return false;
    }

    public virtual bool isCurrentUser(User user)
    {
      if (user == null)
        throw new IllegalStateException("no user set up");
      return this.user == user;
    }

    public virtual void setUser(User user)
    {
      this.user = user;
      if (user == null)
      {
        this.roles = (Role[]) null;
      }
      else
      {
        int length = user.getRoles().size();
        this.roles = length >= 0 ? new Role[length] : throw new NegativeArraySizeException();
        Enumeration enumeration = user.getRoles().elements();
        int num1 = 0;
        while (enumeration.hasMoreElements())
        {
          Role[] roles = this.roles;
          int num2;
          num1 = (num2 = num1) + 1;
          int index = num2;
          Role role = (Role) enumeration.nextElement();
          roles[index] = role;
        }
      }
    }

    public override string ToString()
    {
      string str = "";
      for (int index = 0; this.roles != null && index < this.roles.Length; ++index)
        str = new StringBuffer().append(str).append(this.roles[index].getName().stringValue()).append(" ").ToString();
      return new StringBuffer().append("Session [user=").append(this.user != null ? this.user.getName().stringValue() : "none").append(",roles=").append(str).append("]").ToString();
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static SimpleSession()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      SimpleSession simpleSession = this;
      ObjectImpl.clone((object) simpleSession);
      return ((object) simpleSession).MemberwiseClone();
    }
  }
}
