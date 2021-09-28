// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.SimpleCollectionSorter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.util;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.skylark
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/CollectionSorter;")]
  public class SimpleCollectionSorter : CollectionSorter
  {
    public virtual void sort(NakedObject[] elements, Comparator order, bool reverse)
    {
      if (order == null)
        return;
      Vector vector = new Vector(elements.Length);
label_9:
      for (int index = 0; index < elements.Length; ++index)
      {
        NakedObject element = elements[index];
        order.init(element);
        int num = 0;
        Enumeration enumeration = vector.elements();
        while (enumeration.hasMoreElements())
        {
          NakedObject sortedElement = (NakedObject) enumeration.nextElement();
          if (sortedElement != null && ((order.compare(sortedElement) > 0 ? 1 : 0) ^ (reverse ? 1 : 0)) != 0)
          {
            vector.insertElementAt((object) element, num);
            goto label_9;
          }
          else
            ++num;
        }
        vector.addElement((object) element);
      }
      vector.copyInto((object[]) elements);
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      SimpleCollectionSorter collectionSorter = this;
      ObjectImpl.clone((object) collectionSorter);
      return ((object) collectionSorter).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
