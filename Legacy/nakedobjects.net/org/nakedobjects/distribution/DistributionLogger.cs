// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.DistributionLogger
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.distribution
{
  [JavaInterfaces("1;org/nakedobjects/distribution/Distribution;")]
  public class DistributionLogger : Logger, Distribution
  {
    private static string padding;
    private static DataStructure dataStructure;
    private readonly Distribution decorated;

    public static string dump(Data data)
    {
      StringBuffer str = new StringBuffer();
      DistributionLogger.dump(str, data, 1, new Vector());
      return str.ToString();
    }

    public static string dump(Data[] data)
    {
      StringBuffer str = new StringBuffer();
      for (int index = 0; index < data.Length; ++index)
      {
        str.append("\n    [");
        str.append(index + 1);
        str.append("] ");
        DistributionLogger.dump(str, data[index], 3, new Vector());
      }
      return str.ToString();
    }

    private static void dump(StringBuffer str, Data data, int indent, Vector complete)
    {
      switch (data)
      {
        case null:
          str.append("null");
          break;
        case NullData _:
          str.append("NULL (NullData object)");
          break;
        case ValueData _:
          ValueData valueData1 = (ValueData) data;
          StringBuffer stringBuffer1 = str;
          StringBuffer stringBuffer2 = new StringBuffer().append("ValueData@");
          ValueData valueData2 = valueData1;
          string hexString1 = Integer.toHexString(!(valueData2 is string) ? ObjectImpl.hashCode((object) valueData2) : StringImpl.hashCode((string) valueData2));
          string str1 = stringBuffer2.append(hexString1).append(" ").append(valueData1.getType()).append(":").append(valueData1.getValue()).ToString();
          stringBuffer1.append(str1);
          break;
        case IdentityData _:
          IdentityData dentityData1 = (IdentityData) data;
          StringBuffer stringBuffer3 = str;
          StringBuffer stringBuffer4 = new StringBuffer().append("ReferenceData@");
          IdentityData dentityData2 = dentityData1;
          string hexString2 = Integer.toHexString(!(dentityData2 is string) ? ObjectImpl.hashCode((object) dentityData2) : StringImpl.hashCode((string) dentityData2));
          string str2 = stringBuffer4.append(hexString2).append(" ").append(dentityData1.getType()).append(":").append((object) dentityData1.getOid()).append(":").append((object) dentityData1.getVersion()).ToString();
          stringBuffer3.append(str2);
          break;
        case ObjectData _:
          DistributionLogger.dumpObjectData(str, data, indent, complete);
          break;
        case CollectionData _:
          DistributionLogger.dumpCollectionData(str, data, indent, complete);
          break;
        default:
          str.append(new StringBuffer().append("Unknown: ").append((object) data).ToString());
          break;
      }
    }

    private static void dumpCollectionData(
      StringBuffer str,
      Data data,
      int indent,
      Vector complete)
    {
      CollectionData collectionData1 = (CollectionData) data;
      StringBuffer stringBuffer1 = str;
      StringBuffer stringBuffer2 = new StringBuffer().append("CollectionData@");
      CollectionData collectionData2 = collectionData1;
      string hexString = Integer.toHexString(!(collectionData2 is string) ? ObjectImpl.hashCode((object) collectionData2) : StringImpl.hashCode((string) collectionData2));
      string str1 = stringBuffer2.append(hexString).append(" ").append(collectionData1.getType()).append(":").append((object) collectionData1.getOid()).append(":").append(!collectionData1.hasAllElements() ? "-" : "A").append(":").append((object) collectionData1.getVersion()).ToString();
      stringBuffer1.append(str1);
      object[] elements = (object[]) collectionData1.getElements();
      for (int index = 0; elements != null && index < elements.Length; ++index)
      {
        str.append("\n");
        str.append(DistributionLogger.padding(indent));
        str.append(index + 1);
        str.append(") ");
        DistributionLogger.dump(str, (Data) elements[index], indent + 1, complete);
      }
    }

    private static void dumpObjectData(StringBuffer str, Data data, int indent, Vector complete)
    {
      ObjectData objectData1 = (ObjectData) data;
      StringBuffer stringBuffer1 = str;
      StringBuffer stringBuffer2 = new StringBuffer().append("ObjectData@");
      ObjectData objectData2 = objectData1;
      string hexString = Integer.toHexString(!(objectData2 is string) ? ObjectImpl.hashCode((object) objectData2) : StringImpl.hashCode((string) objectData2));
      string str1 = stringBuffer2.append(hexString).append(" ").append(objectData1.getType()).append(":").append((object) objectData1.getOid()).append(":").append(!objectData1.hasCompleteData() ? "-" : "C").append(":").append((object) objectData1.getVersion()).ToString();
      stringBuffer1.append(str1);
      if (complete.contains((object) objectData1))
      {
        str.append(" (already detailed)");
      }
      else
      {
        complete.addElement((object) objectData1);
        NakedObjectSpecification specification = NakedObjects.getSpecificationLoader().loadSpecification(objectData1.getType());
        NakedObjectField[] fields = DistributionLogger.dataStructure.getFields(specification);
        object[] fieldContent = (object[]) objectData1.getFieldContent();
        for (int index = 0; fieldContent != null && index < fieldContent.Length; ++index)
        {
          str.append("\n");
          str.append(DistributionLogger.padding(indent));
          str.append(index + 1);
          str.append(") ");
          str.append(fields[index].getId());
          str.append(": ");
          DistributionLogger.dump(str, (Data) fieldContent[index], indent + 1, complete);
        }
      }
    }

    private static string indentedNewLine() => new StringBuffer().append("\n").append(DistributionLogger.padding(2)).ToString();

    private static string padding(int indent)
    {
      int num = indent * 3;
      if (num > StringImpl.length(DistributionLogger.padding))
        DistributionLogger.padding = new StringBuffer().append(DistributionLogger.padding).append(DistributionLogger.padding).ToString();
      return StringImpl.substring(DistributionLogger.padding, 0, num);
    }

    public DistributionLogger(Distribution decorated, string fileName)
      : base(fileName, false)
    {
      this.decorated = decorated;
    }

    public DistributionLogger(Distribution decorated)
      : base((string) null, true)
    {
      this.decorated = decorated;
    }

    public virtual ObjectData[] allInstances(
      Session session,
      string fullName,
      bool includeSubclasses)
    {
      this.log(new StringBuffer().append("all instances: ").append(fullName).append(!includeSubclasses ? "" : "with subclasses").ToString());
      ObjectData[] objectDataArray = this.decorated.allInstances(session, fullName, includeSubclasses);
      this.log(new StringBuffer().append("  <-- instances: ").append(DistributionLogger.dump((Data[]) objectDataArray)).ToString());
      return objectDataArray;
    }

    public virtual ObjectData[] clearAssociation(
      Session session,
      string fieldIdentifier,
      IdentityData target,
      IdentityData associate)
    {
      this.log(new StringBuffer().append("clear association ").append(fieldIdentifier).append(DistributionLogger.indentedNewLine()).append("target: ").append(DistributionLogger.dump((Data) target)).append(DistributionLogger.indentedNewLine()).append("associate: ").append(DistributionLogger.dump((Data) associate)).ToString());
      ObjectData[] objectDataArray = this.decorated.clearAssociation(session, fieldIdentifier, target, associate);
      this.log(new StringBuffer().append("  <-- changes: ").append(DistributionLogger.dump((Data[]) objectDataArray)).ToString());
      return objectDataArray;
    }

    public virtual ServerActionResultData executeServerAction(
      Session session,
      string actionType,
      string actionIdentifier,
      ReferenceData target,
      Data[] parameters,
      int actionGraphDepth)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual ClientActionResultData executeClientAction(
      Session session,
      ReferenceData[] data,
      int[] types)
    {
      Vector complete1 = new Vector();
      StringBuffer str1 = new StringBuffer();
      for (int index = 0; index < data.Length; ++index)
      {
        str1.append(DistributionLogger.indentedNewLine());
        str1.append("[");
        str1.append(index + 1);
        str1.append("] ");
        switch (types[index])
        {
          case 1:
            str1.append("persisted: ");
            break;
          case 2:
            str1.append("changed: ");
            break;
          case 3:
            str1.append("deleted: ");
            break;
        }
        DistributionLogger.dump(str1, (Data) data[index], 3, complete1);
      }
      this.log(new StringBuffer().append("execute client action ").append((object) str1).ToString());
      ClientActionResultData actionResultData = this.decorated.executeClientAction(session, data, types);
      Vector complete2 = new Vector();
      StringBuffer str2 = new StringBuffer();
      ObjectData[] persisted = actionResultData.getPersisted();
      Version[] changed = actionResultData.getChanged();
      for (int index = 0; index < persisted.Length; ++index)
      {
        str2.append(DistributionLogger.indentedNewLine());
        str2.append("[");
        str2.append(index + 1);
        str2.append("] ");
        switch (types[index])
        {
          case 1:
            str2.append("persisted: ");
            DistributionLogger.dump(str2, (Data) persisted[index], 3, complete2);
            break;
          case 2:
            str2.append("changed: ");
            str2.append((object) changed[index]);
            break;
        }
      }
      this.log(new StringBuffer().append(" <--- execute client action results").append((object) str2).ToString());
      return actionResultData;
    }

    public virtual ObjectData[] findInstances(Session session, InstancesCriteria criteria)
    {
      this.log(new StringBuffer().append("find instances ").append((object) criteria).ToString());
      ObjectData[] instances = this.decorated.findInstances(session, criteria);
      this.log(new StringBuffer().append(" <-- instances: ").append(DistributionLogger.dump((Data[]) instances)).ToString());
      return instances;
    }

    [JavaFlags(4)]
    public override Class getDecoratedClass() => ObjectImpl.getClass((object) this.decorated);

    public virtual bool hasInstances(Session session, string fullName)
    {
      this.log(new StringBuffer().append("has instances ").append(fullName).ToString());
      bool flag = this.decorated.hasInstances(session, fullName);
      this.log(new StringBuffer().append(" <-- instances: ").append(!flag ? "no" : "yes").ToString());
      return flag;
    }

    public virtual int numberOfInstances(Session sessionId, string fullName)
    {
      this.log(new StringBuffer().append("number of instances of ").append(fullName).ToString());
      int num = this.decorated.numberOfInstances(sessionId, fullName);
      this.log(new StringBuffer().append("  <-- instances: ").append(num).ToString());
      return num;
    }

    public virtual Data resolveField(Session session, IdentityData data, string name)
    {
      this.log(new StringBuffer().append("resolve field ").append(name).append(" - ").append(DistributionLogger.dump((Data) data)).ToString());
      Data data1 = this.decorated.resolveField(session, data, name);
      this.log(new StringBuffer().append(" <-- data: ").append(DistributionLogger.dump(data1)).ToString());
      return data1;
    }

    public virtual ObjectData resolveImmediately(Session session, IdentityData target)
    {
      this.log(new StringBuffer().append("resolve immediately").append(DistributionLogger.dump((Data) target)).ToString());
      ObjectData objectData = this.decorated.resolveImmediately(session, target);
      this.log(new StringBuffer().append("  <-- data: ").append(DistributionLogger.dump((Data) objectData)).ToString());
      return objectData;
    }

    public virtual ObjectData[] setAssociation(
      Session session,
      string fieldIdentifier,
      IdentityData target,
      IdentityData associate)
    {
      this.log(new StringBuffer().append("set association ").append(fieldIdentifier).append(DistributionLogger.indentedNewLine()).append("target: ").append(DistributionLogger.dump((Data) target)).append(DistributionLogger.indentedNewLine()).append("associate: ").append(DistributionLogger.dump((Data) associate)).ToString());
      ObjectData[] objectDataArray = this.decorated.setAssociation(session, fieldIdentifier, target, associate);
      this.log(new StringBuffer().append("  <-- changes: ").append(DistributionLogger.dump((Data[]) objectDataArray)).ToString());
      return objectDataArray;
    }

    public virtual ObjectData[] setValue(
      Session session,
      string fieldIdentifier,
      IdentityData target,
      object value)
    {
      this.log(new StringBuffer().append("set value ").append(fieldIdentifier).append(DistributionLogger.indentedNewLine()).append("target: ").append(DistributionLogger.dump((Data) target)).append(DistributionLogger.indentedNewLine()).append("value: ").append(value).ToString());
      ObjectData[] objectDataArray = this.decorated.setValue(session, fieldIdentifier, target, value);
      this.log(new StringBuffer().append("  <-- changes: ").append(DistributionLogger.dump((Data[]) objectDataArray)).ToString());
      return objectDataArray;
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static DistributionLogger()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
