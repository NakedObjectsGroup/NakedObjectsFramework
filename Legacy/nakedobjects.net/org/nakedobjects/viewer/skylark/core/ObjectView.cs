// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.ObjectView
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.viewer.skylark.basic;
using org.nakedobjects.viewer.skylark.tree;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.core
{
  public abstract class ObjectView : AbstractView
  {
    public ObjectView(Content content, ViewSpecification design, ViewAxis axis)
      : base(content, design, axis)
    {
      switch (content)
      {
        case ObjectContent _:
        case CollectionContent _:
          this.getViewManager().addToNotificationList((View) this);
          break;
        default:
          throw new IllegalArgumentException(new StringBuffer().append("Content must be ObjectContent or AssociateContent: ").append((object) content).ToString());
      }
    }

    public override void dispose()
    {
      this.getViewManager().removeFromNotificationList((View) this);
      base.dispose();
    }

    public override void dragIn(ContentDrag drag)
    {
      Consent consent = this.getContent().canDrop(drag.getSourceContent());
      string description = this.getContent().getDescription();
      if (consent.isAllowed())
      {
        this.getViewManager().setStatus(new StringBuffer().append(consent.getReason()).append(" ").append(description).ToString());
        this.getState().setCanDrop();
      }
      else
      {
        this.getViewManager().setStatus(new StringBuffer().append(consent.getReason()).append(" ").append(description).ToString());
        this.getState().setCantDrop();
      }
      this.markDamaged();
    }

    public override void dragOut(ContentDrag drag)
    {
      this.getState().clearObjectIdentified();
      this.markDamaged();
    }

    public override Drag dragStart(DragStart drag)
    {
      View view = this.subviewFor(drag.getLocation());
      if (view != null)
      {
        drag.subtract(view.getLocation());
        return view.dragStart(drag);
      }
      if (drag.isCtrl())
      {
        View dragView = (View) new DragViewOutline(this.getView());
        return (Drag) new ViewDrag((View) this, new Offset(drag.getLocation()), dragView);
      }
      View dragView1 = (View) new DragContentIcon(this.getContent());
      return (Drag) new ContentDrag((View) this, drag.getLocation(), dragView1);
    }

    public override void drop(ContentDrag drag)
    {
      if (!(drag.getSourceContent() is ObjectContent))
        return;
      Naked @object = this.getContent().drop(drag.getSourceContent());
      this.getParent().invalidateContent();
      if (@object != null)
      {
        View subviewFor = this.getWorkspace().createSubviewFor(@object, false);
        Location point = new Location();
        point.move(10, 10);
        subviewFor.setLocation(point);
        this.getWorkspace().addView(subviewFor);
      }
      this.getViewManager().showMessages();
      this.markDamaged();
    }

    public override void firstClick(Click click)
    {
      View view1 = this.subviewFor(click.getLocation());
      if (view1 != null)
      {
        click.subtract(view1.getLocation());
        view1.firstClick(click);
      }
      else
      {
        if (!click.button2())
          return;
        View view2 = new TreeBrowserFormSpecification().createView(Skylark.getContentFactory().createRootContent(this.getContent().getNaked()), (ViewAxis) null);
        View view3 = (View) new PanelBorder(2, Color.GRAY, Color.LIGHT_GRAY, view2);
        Size maximumSize = view3.getMaximumSize();
        new Location(click.getLocationWithinViewer()).subtract(maximumSize.getWidth() / 2, maximumSize.getHeight() / 2);
        this.getViewManager().setOverlayView(view3);
      }
    }

    public override void invalidateContent() => this.invalidateLayout();

    public override void secondClick(Click click)
    {
      View view = this.subviewFor(click.getLocation());
      if (view != null)
      {
        click.subtract(view.getLocation());
        view.secondClick(click);
      }
      else
      {
        Location absoluteLocation = this.getAbsoluteLocation();
        absoluteLocation.translate(click.getLocation());
        View window = Skylark.getViewFactory().createWindow(this.getContent());
        window.setLocation(absoluteLocation);
        this.getWorkspace().addView(window);
      }
    }

    public override string ToString() => new StringBuffer().append(base.ToString()).append(": ").append((object) this.getContent()).ToString();

    public override void contentMenuOptions(UserActionSet options)
    {
      base.contentMenuOptions(options);
      options.add((UserAction) new ObjectView.\u0031(this, "Refresh", Action.USER));
    }

    [JavaFlags(32)]
    [Inner]
    public new class \u0031 : AbstractUserAction
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private ObjectView this\u00240;

      public override void execute(Workspace workspace, View view, Location at)
      {
        Naked naked = this.this\u00240.getContent().getNaked();
        view.getViewManager().setBusy(view);
        this.this\u00240.getViewManager().setStatus("Loading...");
        NakedObjects.getObjectPersistor().refresh((NakedReference) naked, (Hashtable) null);
        this.this\u00240.invalidateContent();
        view.getViewManager().clearBusy(view);
        this.this\u00240.getViewManager().clearStatus();
      }

      public \u0031(ObjectView _param1, string dummy0, Action.Type dummy1)
        : base(dummy0, dummy1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }
  }
}
