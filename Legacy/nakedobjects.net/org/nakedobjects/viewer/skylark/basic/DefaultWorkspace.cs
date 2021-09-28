// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.DefaultWorkspace
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.skylark.core;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.viewer.skylark.basic
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/Workspace;")]
  public class DefaultWorkspace : CompositeView, Workspace
  {
    private static readonly Logger LOG;
    [JavaFlags(4)]
    public Workspace newWorkspace;
    private DefaultWorkspace.ShowDesktopUserAction showDesktopUserAction;

    public DefaultWorkspace(
      Content content,
      CompositeViewSpecification specification,
      ViewAxis axis)
      : base(content, specification, axis)
    {
      this.showDesktopUserAction = new DefaultWorkspace.ShowDesktopUserAction(this);
    }

    public override void addView(View view)
    {
      base.addView(view);
      this.getViewManager().setKeyboardFocus(view);
      view.getFocusManager().focusFirstChildView();
      this.showDesktopUserAction.setShowDesktopToggle(true);
    }

    public virtual View addOpenViewFor(Naked @object, Location at)
    {
      View view = this.openViewFor(@object, at, false);
      this.showDesktopUserAction.setShowDesktopToggle(true);
      return view;
    }

    private View openViewFor(Naked @object, Location at, bool asIcon)
    {
      View subviewFor = this.createSubviewFor(@object, asIcon);
      subviewFor.setLocation(at);
      this.addView(subviewFor);
      return subviewFor;
    }

    public virtual View addIconFor(Naked @object, Location at) => this.openViewFor(@object, at, true);

    public override void drop(ContentDrag drag)
    {
      this.getViewManager().showArrowCursor();
      if (!drag.getSourceContent().isObject())
        return;
      if (drag.getSourceContent().getNaked() == this.getContent().getNaked())
      {
        this.getViewManager().setStatus("can' drop self on workspace");
      }
      else
      {
        NakedObject nakedObject = ((ObjectContent) drag.getSourceContent()).getObject();
        View view;
        if (nakedObject.getObject() is NakedClass && drag.isCtrl())
        {
          NakedClass nakedClass = (NakedClass) nakedObject.getObject();
          if (nakedClass.useCreate().isVetoed() || !this.getViewManager().isRunningAsExploration())
            return;
          NakedObjectSpecification cls = nakedClass.forObjectType();
          if (DefaultWorkspace.LOG.isInfoEnabled())
            DefaultWorkspace.LOG.info((object) new StringBuffer().append("new ").append(cls.getShortName()).append(" instance").ToString());
          view = this.newInstance(cls, ((drag.isCtrl() ? 1 : 0) ^ 1) != 0);
        }
        else if (drag.isShift())
        {
          view = this.createSubviewFor((Naked) nakedObject, false);
        }
        else
        {
          View source = drag.getSource();
          if (!source.getSpecification().isOpen())
          {
            foreach (View subview in this.getSubviews())
            {
              if (subview == source)
              {
                source.markDamaged();
                Location targetLocation = drag.getTargetLocation();
                targetLocation.translate(drag.getOffset());
                source.setLocation(targetLocation);
                source.markDamaged();
                return;
              }
            }
          }
          view = this.createSubviewFor((Naked) nakedObject, true);
        }
        Location targetLocation1 = drag.getTargetLocation();
        view.setLocation(targetLocation1);
        drag.getTargetView().addView(view);
      }
    }

    public virtual View createSubviewFor(Naked @object, bool asIcon)
    {
      Content rootContent = Skylark.getContentFactory().createRootContent(@object);
      return !asIcon ? Skylark.getViewFactory().createWindow(rootContent) : Skylark.getViewFactory().createIcon(rootContent);
    }

    public override void drop(ViewDrag drag)
    {
      this.getViewManager().showDefaultCursor();
      View sourceView = drag.getSourceView();
      if (sourceView.getSpecification() != null && sourceView.getSpecification().isSubView())
      {
        if (sourceView.getSpecification().isOpen())
          return;
        Location viewDropLocation = drag.getViewDropLocation();
        this.addOpenViewFor(sourceView.getContent().getNaked(), viewDropLocation);
      }
      else
      {
        sourceView.markDamaged();
        Location viewDropLocation = drag.getViewDropLocation();
        sourceView.setLocation(viewDropLocation);
        sourceView.limitBoundsWithin(this.getSize());
        sourceView.markDamaged();
      }
    }

    public override Padding getPadding() => new Padding();

    public override Workspace getWorkspace() => (Workspace) this;

    public virtual void lower(View view)
    {
      if (!this.views.contains((object) view))
        return;
      this.views.removeElement((object) view);
      this.views.insertElementAt((object) view, 0);
      this.markDamaged();
    }

    private View newInstance(NakedObjectSpecification cls, bool openAView)
    {
      NakedObjectPersistor objectPersistor = NakedObjects.getObjectPersistor();
      objectPersistor.startTransaction();
      NakedObject persistentInstance = objectPersistor.createPersistentInstance(cls);
      objectPersistor.endTransaction();
      return this.createSubviewFor((Naked) persistentInstance, openAView);
    }

    public virtual void raise(View view)
    {
      if (!this.views.contains((object) view))
        return;
      this.views.removeElement((object) view);
      this.views.addElement((object) view);
      this.markDamaged();
    }

    public override void removeView(View view)
    {
      view.markDamaged();
      base.removeView(view);
    }

    public virtual void removeViewsFor(NakedObject @object)
    {
      foreach (View subview in this.getSubviews())
      {
        if (subview.getContent().getNaked() == @object)
          subview.dispose();
      }
    }

    public virtual void filterKeyShortcuts(KeyboardAction keyboardAction)
    {
      if (keyboardAction.getKeyCode() != 68 || (keyboardAction.getModifiers() & 2) <= 0)
        return;
      this.showDesktopUserAction.execute(this.getWorkspace(), this.getView(), new Location(0, 0));
    }

    public override void secondClick(Click click)
    {
      if (this.subviewFor(click.getLocation()) == null)
        return;
      base.secondClick(click);
    }

    public override string ToString() => new StringBuffer().append("Workspace").append(this.getId()).ToString();

    public override void viewMenuOptions(UserActionSet options)
    {
      options.setColor(Style.WORKSPACE_MENU);
      this.getViewManager().menuOptions(options);
      options.add((UserAction) new DefaultWorkspace.\u0031(this, "About..."));
      options.add((UserAction) new DefaultWorkspace.\u0032(this, "Naked Classes...", UserAction.DEBUG));
      options.add((UserAction) this.showDesktopUserAction);
      options.add((UserAction) new DefaultWorkspace.\u0033(this, "Tidy up views"));
      options.add((UserAction) new DefaultWorkspace.\u0034(this, "Tidy up icons"));
      options.add((UserAction) new DefaultWorkspace.\u0035(this, "Tidy up all"));
      options.add((UserAction) new DefaultWorkspace.\u0036(this, "Close all"));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static DefaultWorkspace()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(34)]
    [Inner]
    private class ShowDesktopUserAction : AbstractUserAction
    {
      private bool showDesktopToggle;
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private DefaultWorkspace this\u00240;

      public ShowDesktopUserAction(DefaultWorkspace _param1)
        : base("Show desktop (toggle)   Ctrl+D")
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.showDesktopToggle = true;
      }

      [MethodImpl(MethodImplOptions.Synchronized)]
      public virtual void setShowDesktopToggle(bool showDesktopToggle) => this.showDesktopToggle = showDesktopToggle;

      [MethodImpl(MethodImplOptions.Synchronized)]
      public override void execute(Workspace workspace, View view, Location at)
      {
        if (this.showDesktopToggle)
        {
          this.minimizeAll();
          this.showDesktopToggle = false;
        }
        else
        {
          this.restoreAll();
          this.showDesktopToggle = true;
        }
        this.this\u00240.markDamaged();
      }

      private void minimizeAll()
      {
        foreach (View subview in this.this\u00240.getSubviews())
        {
          if (subview.getSpecification().isOpen())
            subview.minimize();
        }
      }

      private void restoreAll()
      {
        foreach (View subview in this.this\u00240.getSubviews())
        {
          if (subview.getSpecification().isOpen())
            subview.restore();
        }
      }
    }

    [JavaFlags(32)]
    [Inner]
    public new class \u0031 : AbstractUserAction
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private DefaultWorkspace this\u00240;

      public override void execute(Workspace workspace, View view, Location at)
      {
        AboutView aboutView = new AboutView();
        Size requiredSize = aboutView.getRequiredSize(new Size());
        Size size = this.this\u00240.getWorkspace().getSize();
        int x = size.getWidth() / 2 - requiredSize.getWidth() / 2;
        int y = size.getHeight() / 2 - requiredSize.getHeight() / 2;
        this.this\u00240.getWorkspace().addView((View) aboutView);
        aboutView.setLocation(new Location(x, y));
      }

      public \u0031(DefaultWorkspace _param1, string dummy0)
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
    public new class \u0032 : AbstractUserAction
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private DefaultWorkspace this\u00240;

      public override void execute(Workspace workspace, View view, Location at)
      {
        NakedObjectSpecification[] objectSpecificationArray = NakedObjects.getSpecificationLoader().allSpecifications();
        Vector vector = new Vector();
        for (int index = 0; index < objectSpecificationArray.Length; ++index)
        {
          NakedObjectSpecification specification = objectSpecificationArray[index];
          if (specification.isObject())
            vector.addElement((object) NakedObjects.getObjectLoader().createAdapterForTransient((object) NakedObjects.getObjectPersistor().getNakedClass(specification)));
        }
        View subviewFor = this.this\u00240.createSubviewFor((Naked) NakedObjects.getObjectLoader().createAdapterForTransient((object) vector), false);
        subviewFor.setLocation(at);
        this.this\u00240.addView(subviewFor);
      }

      public \u0032(DefaultWorkspace _param1, string dummy0, Action.Type dummy1)
        : base(dummy0, dummy1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }

    [JavaFlags(32)]
    [Inner]
    public new class \u0033 : AbstractUserAction
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private DefaultWorkspace this\u00240;

      public override void execute(Workspace workspace, View view, Location at)
      {
        foreach (View subview in this.this\u00240.getSubviews())
        {
          if (subview.getSpecification().isOpen())
            subview.setLocation(WorkspaceBuilder.UNPLACED);
        }
        workspace.invalidateLayout();
        this.this\u00240.markDamaged();
      }

      public \u0033(DefaultWorkspace _param1, string dummy0)
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
    public new class \u0034 : AbstractUserAction
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private DefaultWorkspace this\u00240;

      public override void execute(Workspace workspace, View view, Location at)
      {
        foreach (View subview in this.this\u00240.getSubviews())
        {
          if (!subview.getSpecification().isOpen())
            subview.setLocation(WorkspaceBuilder.UNPLACED);
        }
        workspace.invalidateLayout();
        this.this\u00240.markDamaged();
      }

      public \u0034(DefaultWorkspace _param1, string dummy0)
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
      private DefaultWorkspace this\u00240;

      public override void execute(Workspace workspace, View view, Location at)
      {
        foreach (View subview in this.this\u00240.getSubviews())
          subview.setLocation(WorkspaceBuilder.UNPLACED);
        workspace.invalidateLayout();
        this.this\u00240.markDamaged();
      }

      public \u0035(DefaultWorkspace _param1, string dummy0)
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
    public class \u0036 : AbstractUserAction
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private DefaultWorkspace this\u00240;

      public override void execute(Workspace workspace, View view, Location at)
      {
        foreach (View subview in this.this\u00240.getSubviews())
        {
          if (subview.getSpecification().isOpen())
          {
            NakedObjects.getObjectPersistor().refresh((NakedReference) subview.getContent().getNaked(), (Hashtable) null);
            subview.dispose();
          }
        }
        this.this\u00240.markDamaged();
      }

      public \u0036(DefaultWorkspace _param1, string dummy0)
        : base(dummy0)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }
  }
}
