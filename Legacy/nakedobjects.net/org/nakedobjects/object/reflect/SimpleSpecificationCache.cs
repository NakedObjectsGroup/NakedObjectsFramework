// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.SimpleSpecificationCache
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.reflect;

namespace org.nakedobjects.@object.reflect
{
  [JavaInterfaces("1;org/nakedobjects/object/reflect/SpecificationCache;")]
  public class SimpleSpecificationCache : SpecificationCache
  {
    private Hashtable specs;

    public virtual NakedObjectSpecification get(Class cls) => (NakedObjectSpecification) this.specs.get((object) cls.getName());

    public virtual void cache(Class cls, NakedObjectSpecification spec) => this.specs.put((object) cls.getName(), (object) spec);

    public virtual void cache(string className, NakedObjectSpecification spec) => this.specs.put((object) className, (object) spec);

    public virtual void clear() => this.specs.clear();

    public virtual NakedObjectSpecification[] allSpecifications()
    {
      int length = this.specs.size();
      NakedObjectSpecification[] objectSpecificationArray1 = length >= 0 ? new NakedObjectSpecification[length] : throw new NegativeArraySizeException();
      Enumeration enumeration = this.specs.elements();
      int num1 = 0;
      while (enumeration.hasMoreElements())
      {
        NakedObjectSpecification[] objectSpecificationArray2 = objectSpecificationArray1;
        int num2;
        num1 = (num2 = num1) + 1;
        int index = num2;
        NakedObjectSpecification objectSpecification = (NakedObjectSpecification) enumeration.nextElement();
        objectSpecificationArray2[index] = objectSpecification;
      }
      return objectSpecificationArray1;
    }

    public SimpleSpecificationCache() => this.specs = new Hashtable();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      SimpleSpecificationCache specificationCache = this;
      ObjectImpl.clone((object) specificationCache);
      return ((object) specificationCache).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
