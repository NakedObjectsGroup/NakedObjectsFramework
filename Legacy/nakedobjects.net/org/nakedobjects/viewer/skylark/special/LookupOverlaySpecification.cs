// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.special.LookupOverlaySpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.defaults;

namespace org.nakedobjects.viewer.skylark.special
{
  [JavaFlags(32)]
  public class LookupOverlaySpecification : AbstractDropDownSpecification
  {
    public override bool canDisplay(Content content) => content.isCollection();

    [JavaFlags(4)]
    public override Content getOptionsContent(Content content)
    {
      ObjectContent objectContent = (ObjectContent) content;
      NakedObjectSpecification specification = objectContent.getSpecification();
      TypedNakedCollection typedNakedCollection1;
      if (objectContent is OneToOneField && ((OneToOneField) objectContent).isObject())
      {
        TypedNakedCollection typedNakedCollection2 = ((OneToOneField) objectContent).proposedOptions();
        int length1 = typedNakedCollection2.size();
        object[] objArray = length1 >= 0 ? new object[length1] : throw new NegativeArraySizeException();
        for (int index = 0; index < typedNakedCollection2.size(); ++index)
          objArray[index] = typedNakedCollection2.elementAt(index).getObject();
        int length2 = objArray.Length;
        NakedObject[] instances = length2 >= 0 ? new NakedObject[length2] : throw new NegativeArraySizeException();
        for (int index = 0; index < instances.Length; ++index)
          instances[index] = NakedObjects.getObjectLoader().getAdapterForElseCreateAdapterForTransient(objArray[index]);
        typedNakedCollection1 = (TypedNakedCollection) new InstanceCollectionVector(specification, instances);
      }
      else
        typedNakedCollection1 = NakedObjects.getObjectPersistor().allInstances(specification, true);
      RootCollection rootCollection = new RootCollection((NakedCollection) typedNakedCollection1);
      rootCollection.setOrderByElement();
      return (Content) rootCollection;
    }

    public override string getName() => "Lookup Overlay";

    [JavaFlags(0)]
    public LookupOverlaySpecification()
    {
    }
  }
}
