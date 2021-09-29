// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.collection.InternalCollection
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using System;
using System.Collections;
using System.Runtime.CompilerServices;

// ReSharper disable InconsistentNaming

namespace Legacy.NakedObjects.Application.Collection {
    public class InternalCollection  : IList {

        #region original code

        private readonly ArrayList arrayList;
        private readonly string type;

        public InternalCollection(string type) {
            arrayList = new ArrayList();
            this.type = type;
        }

        public virtual void add(object @object) {
            //if (@object == null)
            //  throw new NullPointerException("Cannot add null");
            arrayList.Add(@object);
        }

        public virtual bool contains(object @object) {
            if (@object != null) {
                return arrayList.Contains(@object);
            }

            //throw new IllegalArgumentException("null is not a valid element for a collection");
            return false; // to compile
        }

        public virtual object elementAt(int index) => arrayList[index];

        public virtual IEnumerator elements() => arrayList.GetEnumerator();

        public virtual string getType() => type;

        public virtual bool isEmpty() => size() == 0;

        public virtual void init(object[] initElements) {
            //Assert.assertEquals("Collection not empty", 0, this.arrayList.size());
            foreach (var t in initElements) {
                arrayList.Add(t);
            }
        }

        public virtual void remove(object @object) {
            //if (@object == null)
            //  throw new NullPointerException("Cannot remove null");
            arrayList.Remove(@object);
        }

        public virtual void removeAllElements() => arrayList.Clear();

        public virtual int size() => arrayList.Count;

        public override string ToString() =>
            //StringBuffer stringBuffer = new StringBuffer();
            //stringBuffer.append("InternalCollectionVector");
            //stringBuffer.append(" [");
            //stringBuffer.append(",size=");
            //stringBuffer.append(this.size());
            //stringBuffer.append("]");
            //stringBuffer.append(new StringBuffer().append("  ").append(StringImpl.toUpperCase(Long.toHexString((long) this.GetHashCode()))).ToString());
            //return stringBuffer.ToString();
            $"InternalCollectionVector [,size={size()}] {GetHashCode():X}";

        
        //[JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
        //[JavaFlags(4227077)]
        public new virtual object MemberwiseClone() =>
            //InternalCollection internalCollection = this;
            //ObjectImpl.clone((object) internalCollection);
            //return ((object) internalCollection).MemberwiseClone();
            null; // to compile

        #endregion

        #region Ilist

        public IEnumerator GetEnumerator() => arrayList.GetEnumerator();

        public void CopyTo(Array array, int index) => arrayList.CopyTo(array, index);

        public int Count => arrayList.Count;
        public bool IsSynchronized => arrayList.IsSynchronized;
        public object SyncRoot => arrayList.SyncRoot;
        public int Add(object value) => arrayList.Add(value);

        public void Clear() => arrayList.Clear();

        public bool Contains(object value) => arrayList.Contains(value);

        public int IndexOf(object value) => arrayList.IndexOf(value);

        public void Insert(int index, object value) => arrayList.Insert(index, value);

        public void Remove(object value) => arrayList.Remove(value);

        public void RemoveAt(int index) => arrayList.RemoveAt(index);

        public bool IsFixedSize => arrayList.IsFixedSize;
        public bool IsReadOnly => arrayList.IsReadOnly;

        public object this[int index] {
            get => arrayList[index];
            set => arrayList[index] = value;
        }

        #endregion
    }
}