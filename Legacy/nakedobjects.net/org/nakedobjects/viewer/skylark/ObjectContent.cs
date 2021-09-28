// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.ObjectContent
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.basic;
using org.nakedobjects.viewer.skylark.util;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace org.nakedobjects.viewer.skylark
{
  public abstract class ObjectContent : AbstractContent
  {
    public abstract Consent canClear();

    public override Consent canDrop(Content sourceContent)
    {
      if (!(sourceContent is ObjectContent))
        return (Consent) new Veto(new StringBuffer().append("Can't drop ").append((object) sourceContent.getNaked()).ToString());
      NakedObject source = ((ObjectContent) sourceContent).getObject();
      NakedObject target = this.getObject();
      Action action1 = this.dropAction(source, target);
      if (action1 != null)
      {
        NakedObject nakedObject = target;
        Action action2 = action1;
        int length = 1;
        NakedObject[] nakedObjectArray = length >= 0 ? new NakedObject[length] : throw new NegativeArraySizeException();
        nakedObjectArray[0] = source;
        return nakedObject.isValid(action2, (Naked[]) nakedObjectArray);
      }
      if (target.getResolveState().isTransient() && source.getResolveState().isPersistent())
        return (Consent) new Veto("Can't set field in persistent object with reference to non-persistent object");
      NakedObjectField[] visibleFields = target.getVisibleFields();
      for (int index = 0; index < visibleFields.Length; ++index)
      {
        if (source.getSpecification().isOfType(visibleFields[index].getSpecification()) && target.getField(visibleFields[index]) == null)
          return (Consent) new Allow(new StringBuffer().append("Set field ").append(visibleFields[index].getName()).ToString());
      }
      return (Consent) new Veto(new StringBuffer().append("No empty field accepting object of type ").append(source.getSpecification().getSingularName()).ToString());
    }

    public abstract Consent canSet(NakedObject dragSource);

    public abstract void clear();

    public override Naked drop(Content sourceContent)
    {
      NakedObject naked = (NakedObject) sourceContent.getNaked();
      Assert.assertNotNull((object) naked);
      NakedObject target = this.getObject();
      Assert.assertNotNull((object) target);
      if (this.canDrop(sourceContent).isAllowed())
      {
        Action action1 = this.dropAction(naked, target);
        if (action1 != null)
        {
          NakedObject nakedObject1 = target;
          Action action2 = action1;
          int length1 = 1;
          NakedObject[] nakedObjectArray1 = length1 >= 0 ? new NakedObject[length1] : throw new NegativeArraySizeException();
          nakedObjectArray1[0] = naked;
          if (nakedObject1.isValid(action2, (Naked[]) nakedObjectArray1).isAllowed())
          {
            NakedObject nakedObject2 = target;
            Action action3 = action1;
            int length2 = 1;
            NakedObject[] nakedObjectArray2 = length2 >= 0 ? new NakedObject[length2] : throw new NegativeArraySizeException();
            nakedObjectArray2[0] = naked;
            return nakedObject2.execute(action3, (Naked[]) nakedObjectArray2);
          }
        }
        NakedObjectField[] visibleFields = target.getVisibleFields();
        for (int index = 0; index < visibleFields.Length; ++index)
        {
          if (naked.getSpecification().isOfType(visibleFields[index].getSpecification()) && target.getField(visibleFields[index]) == null)
          {
            target.setAssociation(visibleFields[index], naked);
            break;
          }
        }
      }
      return (Naked) null;
    }

    private Action dropAction(NakedObject source, NakedObject target)
    {
      Action action;
      if (target.getObject() is NakedClass)
      {
        NakedObjectSpecification objectSpecification = ((NakedClass) target.getObject()).forObjectType();
        Action.Type user = Action.USER;
        int length = 1;
        NakedObjectSpecification[] parameters = length >= 0 ? new NakedObjectSpecification[length] : throw new NegativeArraySizeException();
        parameters[0] = source.getSpecification();
        action = objectSpecification.getClassAction(user, (string) null, parameters);
      }
      else
      {
        NakedObjectSpecification specification = target.getSpecification();
        Action.Type user = Action.USER;
        int length = 1;
        NakedObjectSpecification[] parameters = length >= 0 ? new NakedObjectSpecification[length] : throw new NegativeArraySizeException();
        parameters[0] = source.getSpecification();
        action = specification.getObjectAction(user, (string) null, parameters);
      }
      return action;
    }

    public virtual Hint getHint() => (Hint) null;

    public abstract NakedObject getObject();

    public override bool isPersistable() => this.getObject().persistable() == Persistable.USER_PERSISTABLE;

    public override void contentMenuOptions(UserActionSet options)
    {
      NakedObject nakedObject = this.getObject();
      OptionFactory.addObjectMenuOptions((NakedReference) nakedObject, options);
      if (this.getObject() == null)
        OptionFactory.addClassMenuOptions(this.getSpecification(), options);
      options.add((UserAction) new ObjectContent.\u0031(this, "Instances", UserAction.EXPLORATION, nakedObject));
      options.add((UserAction) new ObjectContent.\u0032(this, "Class", UserAction.EXPLORATION, nakedObject));
      options.add((UserAction) new ObjectContent.\u0033(this, "Clone", UserAction.EXPLORATION, nakedObject));
      options.add((UserAction) new ObjectContent.\u0034(this, "Clear resolved", UserAction.DEBUG, nakedObject));
    }

    public virtual void contentButtonOptions(UserActionSet options) => OptionFactory.addObjectButtonOptions((NakedReference) this.getObject(), options);

    [JavaThrownExceptions("1;org/nakedobjects/object/InvalidEntryException;")]
    public override void parseTextEntry(string entryText) => throw new UnexpectedCallException();

    public abstract void setObject(NakedObject @object);

    public override string getIconName() => ((NakedReference) this.getObject())?.getIconName();

    public override Image getIconPicture(int iconHeight)
    {
      NakedObject nakedObject = this.getObject();
      if (nakedObject == null)
        return ImageFactory.getInstance().loadIcon("emptyField", iconHeight);
      object obj = nakedObject.getObject();
      if (obj is NakedClass)
      {
        NakedObjectSpecification specification = ((NakedClass) obj).forObjectType();
        return ImageFactory.getInstance().loadClassIcon(specification, "", iconHeight);
      }
      NakedObjectSpecification specification1 = nakedObject.getSpecification();
      return ImageFactory.getInstance().loadObjectIcon(specification1, "", iconHeight);
    }

    public override void viewMenuOptions(UserActionSet options)
    {
      NakedObject nakedObject = this.getObject();
      if (!(nakedObject is UserContext))
        return;
      options.add((UserAction) new ObjectContent.\u0035(this, "New Workspace", nakedObject));
    }

    [Inner]
    [JavaFlags(32)]
    public class \u0031 : AbstractUserAction
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private ObjectContent this\u00240;
      [JavaFlags(16)]
      public readonly NakedObject object_\u003E;

      public override Consent disabled(View component) => AbstractConsent.allow(this.object_\u003E != null);

      public override void execute(Workspace workspace, View view, Location at)
      {
        NakedObjectSpecification specification = this.this\u00240.getObject().getSpecification();
        TypedNakedCollection typedNakedCollection = NakedObjects.getObjectPersistor().allInstances(specification, false);
        Content rootContent = Skylark.getContentFactory().createRootContent((Naked) typedNakedCollection);
        View window = Skylark.getViewFactory().createWindow(rootContent);
        window.setLocation(at);
        workspace.addView(window);
      }

      public \u0031(ObjectContent _param1, string dummy0, Action.Type dummy1, [In] NakedObject obj3)
        : base(dummy0, dummy1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.object_\u003E = obj3;
      }
    }

    [Inner]
    [JavaFlags(32)]
    public class \u0032 : AbstractUserAction
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private ObjectContent this\u00240;
      [JavaFlags(16)]
      public readonly NakedObject object_\u003E;

      public override Consent disabled(View component) => AbstractConsent.allow(this.object_\u003E != null);

      public override void execute(Workspace workspace, View view, Location at)
      {
      }

      public \u0032(ObjectContent _param1, string dummy0, Action.Type dummy1, [In] NakedObject obj3)
        : base(dummy0, dummy1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.object_\u003E = obj3;
      }
    }

    [JavaFlags(32)]
    [Inner]
    public class \u0033 : AbstractUserAction
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private ObjectContent this\u00240;
      [JavaFlags(16)]
      public readonly NakedObject object_\u003E;

      public override Consent disabled(View component) => AbstractConsent.allow(this.object_\u003E != null);

      public override void execute(Workspace workspace, View view, Location at)
      {
        NakedObject nakedObject = this.this\u00240.getObject();
        NakedObjectSpecification specification = nakedObject.getSpecification();
        NakedObject transientInstance = NakedObjects.getObjectPersistor().createTransientInstance(specification);
        NakedObjectField[] fields = specification.getFields();
        for (int index = 0; index < fields.Length; ++index)
        {
          Naked field = nakedObject.getField(fields[index]);
          if (fields[index].isObject())
            transientInstance.setAssociation(fields[index], (NakedObject) field);
          else if (fields[index].isValue())
            transientInstance.setValue((OneToOneAssociation) fields[index], field.getObject());
          else if (!fields[index].isCollection())
            ;
        }
        Content rootContent = Skylark.getContentFactory().createRootContent((Naked) transientInstance);
        View window = Skylark.getViewFactory().createWindow(rootContent);
        window.setLocation(at);
        workspace.addView(window);
      }

      public \u0033(ObjectContent _param1, string dummy0, Action.Type dummy1, [In] NakedObject obj3)
        : base(dummy0, dummy1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.object_\u003E = obj3;
      }
    }

    [JavaFlags(32)]
    [Inner]
    public class \u0034 : AbstractUserAction
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private ObjectContent this\u00240;
      [JavaFlags(16)]
      public readonly NakedObject object_\u003E;

      public override Consent disabled(View component) => AbstractConsent.allow(this.object_\u003E == null || this.object_\u003E.getResolveState() != ResolveState.TRANSIENT || this.object_\u003E.getResolveState() == ResolveState.GHOST);

      public override void execute(Workspace workspace, View view, Location at) => this.object_\u003E.debugClearResolved();

      public \u0034(ObjectContent _param1, string dummy0, Action.Type dummy1, [In] NakedObject obj3)
        : base(dummy0, dummy1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.object_\u003E = obj3;
      }
    }

    [JavaFlags(32)]
    [Inner]
    public class \u0035 : AbstractUserAction
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private ObjectContent this\u00240;
      [JavaFlags(16)]
      public readonly NakedObject object_\u003E;

      public override Consent disabled(View component) => AbstractConsent.allow(this.object_\u003E is UserContext);

      public override void execute(Workspace workspace, View view, Location at)
      {
        Content rootContent = Skylark.getContentFactory().createRootContent((Naked) this.object_\u003E);
        View innerWorkspace = Skylark.getViewFactory().createInnerWorkspace(rootContent);
        innerWorkspace.setLocation(at);
        workspace.addView(innerWorkspace);
        innerWorkspace.markDamaged();
      }

      public \u0035(ObjectContent _param1, string dummy0, [In] NakedObject obj2)
        : base(dummy0)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.object_\u003E = obj2;
      }
    }
  }
}
