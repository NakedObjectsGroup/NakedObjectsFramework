// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.CollectionContent
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.basic;
using org.nakedobjects.viewer.skylark.util;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace org.nakedobjects.viewer.skylark
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/Content;")]
  public abstract class CollectionContent : AbstractContent, Content
  {
    private static readonly TypeComparator TYPE_COMPARATOR;
    private static readonly TitleComparator TITLE_COMPARATOR;
    private static readonly CollectionSorter sorter;
    private Comparator order;
    private bool reverse;

    [JavaFlags(17)]
    public Enumeration allElements()
    {
      NakedObject[] element = this.elements();
      CollectionContent.sorter.sort(element, this.order, this.reverse);
      return (Enumeration) new CollectionContent.\u0031(this, element);
    }

    public override void debugDetails(DebugString debug)
    {
      debug.appendln("order", (object) this.order);
      debug.appendln("reverse order", this.reverse);
    }

    public abstract NakedObject[] elements();

    public abstract NakedCollection getCollection();

    public override void contentMenuOptions(UserActionSet options)
    {
      NakedCollection collection = this.getCollection();
      OptionFactory.addObjectMenuOptions((NakedReference) collection, options);
      options.add((UserAction) new CollectionContent.\u0032(this, "Clear resolved", UserAction.DEBUG, collection));
    }

    public override void viewMenuOptions(UserActionSet options)
    {
      UserActionSet userActionSet = new UserActionSet("Sort", options);
      options.add((UserAction) userActionSet);
      userActionSet.add((UserAction) new CollectionContent.\u0033(this, "Clear"));
      if (this.reverse)
        userActionSet.add((UserAction) new CollectionContent.\u0034(this, "Normal sort order"));
      else
        userActionSet.add((UserAction) new CollectionContent.\u0035(this, "Reverse sort order"));
      userActionSet.add((UserAction) new CollectionContent.\u0036(this, "Sort by title"));
      userActionSet.add((UserAction) new CollectionContent.\u0037(this, "Sort by type"));
      NakedCollection collection = this.getCollection();
      if (!(collection is TypedNakedCollection))
        return;
      foreach (NakedObjectField field in ((TypedNakedCollection) collection).getElementSpecification().getFields())
        userActionSet.add((UserAction) new CollectionContent.\u0038(this, new StringBuffer().append("Sort by ").append(field.getName()).ToString(), field));
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/InvalidEntryException;")]
    public override void parseTextEntry(string entryText) => throw new UnexpectedCallException();

    public virtual void setOrder(Comparator order) => this.order = order;

    public virtual void setOrderByField(NakedObjectField field)
    {
      if (this.order is FieldComparator && ((FieldComparator) this.order).getField() == field)
      {
        this.reverse = ((this.reverse ? 1 : 0) ^ 1) != 0;
      }
      else
      {
        this.order = (Comparator) new FieldComparator(field);
        this.reverse = false;
      }
    }

    public virtual void setOrderByElement()
    {
      if (this.order == CollectionContent.TITLE_COMPARATOR)
      {
        this.reverse = ((this.reverse ? 1 : 0) ^ 1) != 0;
      }
      else
      {
        this.order = (Comparator) CollectionContent.TITLE_COMPARATOR;
        this.reverse = false;
      }
    }

    public virtual NakedObjectField getFieldSortOrder() => this.order is FieldComparator ? ((FieldComparator) this.order).getField() : (NakedObjectField) null;

    public override Image getIconPicture(int iconHeight)
    {
      NakedCollection collection = this.getCollection();
      if (collection == null)
        return ImageFactory.getInstance().loadIcon("emptyField", iconHeight);
      object obj = collection.getObject();
      if (obj is NakedClass)
      {
        NakedObjectSpecification specification = ((NakedClass) obj).forObjectType();
        return ImageFactory.getInstance().loadClassIcon(specification, "", iconHeight);
      }
      NakedObjectSpecification specification1 = collection.getSpecification();
      return ImageFactory.getInstance().loadObjectIcon(specification1, "", iconHeight);
    }

    public virtual bool getOrderByElement() => this.order == CollectionContent.TITLE_COMPARATOR;

    public virtual bool getReverseSortOrder() => this.reverse;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static CollectionContent()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(32)]
    [Inner]
    [JavaInterfaces("1;java/util/Enumeration;")]
    public class \u0031 : Enumeration
    {
      [JavaFlags(0)]
      public int i;
      [JavaFlags(0)]
      public int size;
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private CollectionContent this\u00240;
      [JavaFlags(16)]
      public readonly NakedObject[] elements_\u003E;

      public virtual bool hasMoreElements() => this.i < this.size;

      public virtual object nextElement()
      {
        NakedObject[] elements = this.elements_\u003E;
        int i;
        this.i = (i = this.i) + 1;
        int index = i;
        return (object) elements[index];
      }

      public \u0031(CollectionContent _param1, [In] NakedObject[] obj1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.elements_\u003E = obj1;
        this.i = 0;
        this.size = this.elements_\u003E.Length;
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        CollectionContent.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [Inner]
    [JavaFlags(32)]
    public class \u0032 : AbstractUserAction
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private CollectionContent this\u00240;
      [JavaFlags(16)]
      public readonly NakedCollection collection_\u003E;

      public override Consent disabled(View component) => AbstractConsent.allow(this.collection_\u003E == null || this.collection_\u003E.getResolveState() != ResolveState.TRANSIENT || this.collection_\u003E.getResolveState() == ResolveState.GHOST);

      public override void execute(Workspace workspace, View view, Location at) => this.collection_\u003E.debugClearResolved();

      public \u0032(
        CollectionContent _param1,
        string dummy0,
        Action.Type dummy1,
        [In] NakedCollection obj3)
        : base(dummy0, dummy1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.collection_\u003E = obj3;
      }
    }

    [JavaFlags(32)]
    [Inner]
    public class \u0033 : AbstractUserAction
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private CollectionContent this\u00240;

      public override Consent disabled(View component) => AbstractConsent.allow(this.this\u00240.order != null);

      public override void execute(Workspace workspace, View view, Location at)
      {
        this.this\u00240.order = (Comparator) null;
        view.invalidateContent();
      }

      public \u0033(CollectionContent _param1, string dummy0)
        : base(dummy0)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }

    [JavaFlags(32)]
    [Inner]
    public class \u0034 : AbstractUserAction
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private CollectionContent this\u00240;

      public override Consent disabled(View component) => AbstractConsent.allow(this.this\u00240.order != null);

      public override void execute(Workspace workspace, View view, Location at)
      {
        this.this\u00240.reverse = false;
        view.invalidateContent();
      }

      public \u0034(CollectionContent _param1, string dummy0)
        : base(dummy0)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }

    [JavaFlags(32)]
    [Inner]
    public class \u0035 : AbstractUserAction
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private CollectionContent this\u00240;

      public override Consent disabled(View component) => AbstractConsent.allow(this.this\u00240.order != null);

      public override void execute(Workspace workspace, View view, Location at)
      {
        this.this\u00240.reverse = true;
        view.invalidateContent();
      }

      public \u0035(CollectionContent _param1, string dummy0)
        : base(dummy0)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }

    [Inner]
    [JavaFlags(32)]
    public class \u0036 : AbstractUserAction
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private CollectionContent this\u00240;

      public override Consent disabled(View component) => AbstractConsent.allow(this.this\u00240.order != CollectionContent.TITLE_COMPARATOR);

      public override void execute(Workspace workspace, View view, Location at)
      {
        this.this\u00240.order = (Comparator) CollectionContent.TITLE_COMPARATOR;
        view.invalidateContent();
      }

      public \u0036(CollectionContent _param1, string dummy0)
        : base(dummy0)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }

    [Inner]
    [JavaFlags(32)]
    public class \u0037 : AbstractUserAction
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private CollectionContent this\u00240;

      public override Consent disabled(View component) => AbstractConsent.allow(this.this\u00240.order != CollectionContent.TYPE_COMPARATOR);

      public override void execute(Workspace workspace, View view, Location at)
      {
        this.this\u00240.order = (Comparator) CollectionContent.TYPE_COMPARATOR;
        view.invalidateContent();
      }

      public \u0037(CollectionContent _param1, string dummy0)
        : base(dummy0)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }

    [JavaFlags(32)]
    [Inner]
    public class \u0038 : AbstractUserAction
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private CollectionContent this\u00240;
      [JavaFlags(16)]
      public readonly NakedObjectField field_\u003E;

      public override void execute(Workspace workspace, View view, Location at)
      {
        this.this\u00240.order = (Comparator) new FieldComparator(this.field_\u003E);
        view.invalidateContent();
      }

      public \u0038(CollectionContent _param1, string dummy0, [In] NakedObjectField obj2)
        : base(dummy0)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.field_\u003E = obj2;
      }
    }
  }
}
