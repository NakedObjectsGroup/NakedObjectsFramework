// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.DebugDumpSnapshotOption
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.util;
using org.apache.log4j;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility.logging;

namespace org.nakedobjects.viewer.skylark.core
{
  public class DebugDumpSnapshotOption : AbstractUserAction
  {
    public DebugDumpSnapshotOption()
      : base("Dump log snapshot", UserAction.DEBUG)
    {
    }

    public override Consent disabled(View component)
    {
      Enumeration allAppenders = Logger.getRootLogger().getAllAppenders();
      while (allAppenders.hasMoreElements())
      {
        if ((Appender) allAppenders.nextElement() is SnapshotAppender)
          return (Consent) Allow.DEFAULT;
      }
      return (Consent) new Veto("No available snapshot appender");
    }

    public override void execute(Workspace workspace, View view, Location at)
    {
      Enumeration allAppenders = Logger.getRootLogger().getAllAppenders();
      while (allAppenders.hasMoreElements())
      {
        Appender appender = (Appender) allAppenders.nextElement();
        if (appender is SnapshotAppender)
          ((SnapshotAppender) appender).forceSnapshot();
      }
    }

    public override string getDescription(View view) => "Force a snapshot of the log";
  }
}
