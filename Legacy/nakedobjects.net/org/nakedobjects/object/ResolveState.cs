// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.ResolveState
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.utility;
using System;
using System.ComponentModel;

namespace org.nakedobjects.@object
{
  public sealed class ResolveState
  {
    private static readonly Hashtable states;
    public static readonly ResolveState GHOST;
    public static readonly ResolveState NEW;
    public static readonly ResolveState PART_RESOLVED;
    public static readonly ResolveState RESOLVED;
    public static readonly ResolveState RESOLVING;
    public static readonly ResolveState RESOLVING_PART;
    public static readonly ResolveState SERIALIZING_GHOST;
    public static readonly ResolveState SERIALIZING_PART_RESOLVED;
    public static readonly ResolveState SERIALIZING_RESOLVED;
    public static readonly ResolveState TRANSIENT;
    public static readonly ResolveState SERIALIZING_TRANSIENT;
    public static readonly ResolveState UPDATING;
    private readonly string code;
    private readonly ResolveState endState;
    private readonly string name;
    private static bool updatesContainCompleteState;
    [JavaFlags(130)]
    [NonSerialized]
    private string cachedToString;

    public static ResolveState getResolveState(string name) => (ResolveState) ResolveState.states.get((object) name);

    private ResolveState(string name, string code)
      : this(name, code, (ResolveState) null)
    {
    }

    private ResolveState(string name, string code, ResolveState endState)
    {
      this.name = name;
      this.code = code;
      this.endState = endState;
      ResolveState.states.put((object) name, (object) this);
    }

    public virtual string code() => this.code;

    public virtual ResolveState getEndState() => this.endState;

    public virtual bool isGhost() => this == ResolveState.GHOST;

    public virtual bool respondToChangesInPersistentObjects() => ((this == ResolveState.TRANSIENT || this.isResolving() || this.isUpdating() || this.isSerializing() ? 1 : 0) ^ 1) != 0;

    private bool isUpdating() => this == ResolveState.UPDATING;

    public virtual bool isPartlyResolved() => this == ResolveState.PART_RESOLVED;

    public virtual bool isPersistent() => this == ResolveState.GHOST || this == ResolveState.PART_RESOLVED || this == ResolveState.RESOLVED || this.isResolving() || this.isUpdating() || this.isSerializing() && this != ResolveState.SERIALIZING_TRANSIENT;

    public virtual bool isResolvable(ResolveState newState)
    {
      Assert.assertTrue("state must be one of RESOLVING_PART; RESOLVING; or UPDATING", newState == ResolveState.RESOLVING || newState == ResolveState.RESOLVING_PART || newState == ResolveState.UPDATING);
      return (this == ResolveState.RESOLVED || this == ResolveState.PART_RESOLVED || this == ResolveState.GHOST) && this.isValidToChangeTo(newState);
    }

    public virtual bool isResolved() => this == ResolveState.RESOLVED;

    public virtual bool isResolving() => this == ResolveState.RESOLVING || this == ResolveState.RESOLVING_PART;

    public virtual bool isSerializing() => this == ResolveState.SERIALIZING_GHOST || this == ResolveState.SERIALIZING_PART_RESOLVED || this == ResolveState.SERIALIZING_RESOLVED || this == ResolveState.SERIALIZING_TRANSIENT;

    public virtual bool isTransient() => this == ResolveState.TRANSIENT || this == ResolveState.SERIALIZING_TRANSIENT;

    public virtual bool isValidToChangeTo(ResolveState nextState) => this == ResolveState.PART_RESOLVED ? nextState == ResolveState.RESOLVING_PART || nextState == ResolveState.RESOLVING || nextState == ResolveState.SERIALIZING_PART_RESOLVED || nextState == ResolveState.UPDATING && ResolveState.updatesContainCompleteState : (this == ResolveState.RESOLVED ? nextState == ResolveState.UPDATING || nextState == ResolveState.SERIALIZING_RESOLVED || nextState == ResolveState.GHOST : (this == ResolveState.NEW ? nextState == ResolveState.TRANSIENT || nextState == ResolveState.GHOST : (this == ResolveState.TRANSIENT ? nextState == ResolveState.RESOLVED || nextState == ResolveState.SERIALIZING_TRANSIENT : (this == ResolveState.GHOST ? nextState == ResolveState.RESOLVING_PART || nextState == ResolveState.RESOLVING || nextState == ResolveState.UPDATING || nextState == ResolveState.SERIALIZING_GHOST : (this == ResolveState.RESOLVING_PART ? nextState == ResolveState.PART_RESOLVED || nextState == ResolveState.RESOLVED : (this == ResolveState.RESOLVING ? nextState == ResolveState.RESOLVED : (this == ResolveState.UPDATING ? nextState == ResolveState.RESOLVED : (this == ResolveState.SERIALIZING_TRANSIENT ? nextState == ResolveState.TRANSIENT : (this == ResolveState.SERIALIZING_GHOST ? nextState == ResolveState.GHOST : (this == ResolveState.SERIALIZING_PART_RESOLVED ? nextState == ResolveState.PART_RESOLVED : this == ResolveState.SERIALIZING_RESOLVED && nextState == ResolveState.RESOLVED))))))))));

    public virtual string name() => this.name;

    public virtual ResolveState serializeFrom()
    {
      if (this == ResolveState.RESOLVED)
        return ResolveState.SERIALIZING_RESOLVED;
      if (this == ResolveState.PART_RESOLVED)
        return ResolveState.SERIALIZING_PART_RESOLVED;
      if (this == ResolveState.GHOST)
        return ResolveState.SERIALIZING_GHOST;
      return this == ResolveState.TRANSIENT ? ResolveState.SERIALIZING_TRANSIENT : (ResolveState) null;
    }

    public override string ToString()
    {
      if (this.cachedToString == null)
      {
        org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
        toString.append("name", this.name);
        toString.append("code", this.code);
        if (this.endState != null)
          toString.append("endstate", this.endState.name());
        this.cachedToString = toString.ToString();
      }
      return this.cachedToString;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static ResolveState()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      ResolveState resolveState = this;
      ObjectImpl.clone((object) resolveState);
      return ((object) resolveState).MemberwiseClone();
    }
  }
}
