// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.parser.SimpleHashtable
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using com.ms.vjsharp.util;
using java.lang;
using java.util;

namespace org.apache.crimson.parser
{
  [JavaInterfaces("1;java/util/Enumeration;")]
  [JavaFlags(48)]
  public sealed class SimpleHashtable : Enumeration
  {
    private SimpleHashtable.Entry[] table;
    private SimpleHashtable.Entry current;
    private int currentBucket;
    private int count;
    private int threshold;
    private const float loadFactor = 0.75f;

    public SimpleHashtable(int initialCapacity)
    {
      this.current = (SimpleHashtable.Entry) null;
      this.currentBucket = 0;
      if (initialCapacity < 0)
        throw new IllegalArgumentException(new StringBuffer().append("Illegal Capacity: ").append(initialCapacity).ToString());
      if (initialCapacity == 0)
        initialCapacity = 1;
      int length = initialCapacity;
      this.table = length >= 0 ? new SimpleHashtable.Entry[length] : throw new NegativeArraySizeException();
      this.threshold = Utilities.floatToInt((float) initialCapacity * 0.75f);
    }

    public SimpleHashtable()
      : this(11)
    {
    }

    public virtual void clear()
    {
      this.count = 0;
      this.currentBucket = 0;
      this.current = (SimpleHashtable.Entry) null;
      for (int index = 0; index < this.table.Length; ++index)
        this.table[index] = (SimpleHashtable.Entry) null;
    }

    public virtual int size() => this.count;

    public virtual Enumeration keys()
    {
      this.currentBucket = 0;
      this.current = (SimpleHashtable.Entry) null;
      return (Enumeration) this;
    }

    public virtual bool hasMoreElements()
    {
      if (this.current != null)
        return true;
      while (this.currentBucket < this.table.Length)
      {
        SimpleHashtable.Entry[] table = this.table;
        int currentBucket;
        this.currentBucket = (currentBucket = this.currentBucket) + 1;
        int index = currentBucket;
        this.current = table[index];
        if (this.current != null)
          return true;
      }
      return false;
    }

    public virtual object nextElement()
    {
      object obj = this.current != null ? this.current.key : throw new IllegalStateException();
      this.current = this.current.next;
      return obj;
    }

    public virtual object get(string key)
    {
      SimpleHashtable.Entry[] table = this.table;
      int num = StringImpl.hashCode(key);
      int index = (num & int.MaxValue) % table.Length;
      for (SimpleHashtable.Entry next = table[index]; next != null; next = next.next)
      {
        if (next.hash == num && next.key == (object) key)
          return next.value;
      }
      return (object) null;
    }

    public virtual object getNonInterned(string key)
    {
      SimpleHashtable.Entry[] table = this.table;
      int num = StringImpl.hashCode(key);
      int index = (num & int.MaxValue) % table.Length;
      for (SimpleHashtable.Entry next = table[index]; next != null; next = next.next)
      {
        if (next.hash == num && next.key.Equals((object) key))
          return next.value;
      }
      return (object) null;
    }

    private void rehash()
    {
      int length1 = this.table.Length;
      SimpleHashtable.Entry[] table = this.table;
      int num1 = length1 * 2 + 1;
      int length2 = num1;
      SimpleHashtable.Entry[] entryArray = length2 >= 0 ? new SimpleHashtable.Entry[length2] : throw new NegativeArraySizeException();
      this.threshold = Utilities.floatToInt((float) num1 * 0.75f);
      this.table = entryArray;
      int index1 = length1;
label_7:
      int num2;
      index1 = (num2 = index1) - 1;
      if (num2 <= 0)
        return;
      SimpleHashtable.Entry next = table[index1];
      while (next != null)
      {
        SimpleHashtable.Entry entry = next;
        next = next.next;
        int index2 = (entry.hash & int.MaxValue) % num1;
        entry.next = entryArray[index2];
        entryArray[index2] = entry;
      }
      goto label_7;
    }

    public virtual object put(object key, object value)
    {
      if (value == null)
        throw new NullPointerException();
      SimpleHashtable.Entry[] table = this.table;
      object obj1 = key;
      int hash = !(obj1 is string) ? ObjectImpl.hashCode(obj1) : StringImpl.hashCode((string) obj1);
      int index = (hash & int.MaxValue) % table.Length;
      for (SimpleHashtable.Entry next = table[index]; next != null; next = next.next)
      {
        if (next.hash == hash && next.key == key)
        {
          object obj2 = next.value;
          next.value = value;
          return obj2;
        }
      }
      if (this.count >= this.threshold)
      {
        this.rehash();
        table = this.table;
        index = (hash & int.MaxValue) % table.Length;
      }
      SimpleHashtable.Entry entry = new SimpleHashtable.Entry(hash, key, value, table[index]);
      table[index] = entry;
      ++this.count;
      return (object) null;
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      SimpleHashtable simpleHashtable = this;
      ObjectImpl.clone((object) simpleHashtable);
      return ((object) simpleHashtable).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaFlags(42)]
    private class Entry
    {
      [JavaFlags(0)]
      public int hash;
      [JavaFlags(0)]
      public object key;
      [JavaFlags(0)]
      public object value;
      [JavaFlags(0)]
      public SimpleHashtable.Entry next;

      [JavaFlags(4)]
      public Entry(int hash, object key, object value, SimpleHashtable.Entry next)
      {
        this.hash = hash;
        this.key = key;
        this.value = value;
        this.next = next;
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        SimpleHashtable.Entry entry = this;
        ObjectImpl.clone((object) entry);
        return ((object) entry).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
