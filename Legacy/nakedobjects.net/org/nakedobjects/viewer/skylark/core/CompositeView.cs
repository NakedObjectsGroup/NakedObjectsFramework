// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.CompositeView
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.core
{
  public class CompositeView : ObjectView
  {
    private static readonly org.apache.log4j.Logger LOG;
    private int buildCount;
    private CompositeViewBuilder builder;
    private bool buildInvalid;
    private bool canDragView;
    private int layoutCount;
    private bool layoutInvalid;
    [JavaFlags(4)]
    public Vector views;
    private FocusManager focusManager;

    public CompositeView(Content content, CompositeViewSpecification specification, ViewAxis axis)
      : base(content, (ViewSpecification) specification, axis)
    {
      this.buildCount = 0;
      this.buildInvalid = true;
      this.canDragView = true;
      this.layoutCount = 0;
      this.layoutInvalid = true;
      this.views = new Vector();
      this.builder = specification.getSubviewBuilder();
    }

    public override void refresh()
    {
      foreach (View subview in this.getSubviews())
        subview.refresh();
    }

    public override void addView(View view)
    {
      if (CompositeView.LOG.isDebugEnabled())
        CompositeView.LOG.debug((object) new StringBuffer().append("adding ").append((object) view).append(" to ").append((object) this).ToString());
      this.views.addElement((object) view);
      view.setParent(this.getView());
      this.invalidateLayout();
    }

    public virtual bool canDragView() => this.canDragView;

    public override string debugDetails()
    {
      DebugString debugString = new DebugString();
      debugString.append((object) base.debugDetails());
      debugString.appendTitle("Composite view");
      debugString.appendln("Built", (object) new StringBuffer().append(!this.buildInvalid ? "yes" : "no").append(", ").append(this.buildCount).append(" builds").ToString());
      debugString.appendln(new StringBuffer().append("Laid out:  ").append(!this.layoutInvalid ? "yes" : "no").append(", ").append(this.layoutCount).append(" layouts").ToString());
      debugString.appendln("Subviews");
      View[] subviews = this.getSubviews();
      debugString.indent();
      for (int index = 0; index < subviews.Length; ++index)
      {
        View view = subviews[index];
        debugString.appendln(AbstractView.name(view));
        debugString.indent();
        debugString.appendln("Bounds", (object) view.getBounds());
        debugString.appendln("Required size ", (object) view.getRequiredSize(new Size()));
        debugString.appendln("Content", (object) view.getContent().getId());
        debugString.unindent();
      }
      debugString.unindent();
      debugString.append((object) "\n");
      return debugString.ToString();
    }

    public override void dispose()
    {
      foreach (View subview in this.getSubviews())
        subview.dispose();
      base.dispose();
    }

    public override void draw(Canvas canvas)
    {
      foreach (View subview in this.getSubviews())
      {
        Bounds bounds = subview.getBounds();
        if (AbstractView.debug && CompositeView.LOG.isDebugEnabled())
          CompositeView.LOG.debug((object) new StringBuffer().append("compare: ").append((object) bounds).append("  ").append((object) canvas).ToString());
        if (canvas.overlaps(bounds))
        {
          Canvas subcanvas = canvas.createSubcanvas(bounds.getX(), bounds.getY(), bounds.getWidth() - 0, bounds.getSize().getHeight());
          if (AbstractView.debug)
          {
            if (CompositeView.LOG.isDebugEnabled())
              CompositeView.LOG.debug((object) new StringBuffer().append("-- repainting ").append((object) subview).ToString());
            if (CompositeView.LOG.isDebugEnabled())
              CompositeView.LOG.debug((object) new StringBuffer().append("subcanvas ").append((object) subcanvas).ToString());
          }
          subview.draw(subcanvas);
          if (!AbstractView.debug)
            ;
        }
      }
    }

    public override int getBaseline()
    {
      View[] subviews = this.getSubviews();
      return subviews.Length == 0 ? 14 : subviews[0].getBaseline();
    }

    public override FocusManager getFocusManager() => this.focusManager == null ? base.getFocusManager() : this.focusManager;

    public override Size getMaximumSize()
    {
      Size requiredSize = this.builder.getRequiredSize((View) this);
      requiredSize.extend(this.getPadding());
      requiredSize.ensureHeight(1);
      return requiredSize;
    }

    public override View[] getSubviews()
    {
      if (this.buildInvalid)
      {
        this.getViewManager().showWaitCursor();
        this.buildInvalid = false;
        this.builder.build(this.getView());
        ++this.buildCount;
        this.getViewManager().showDefaultCursor();
      }
      int length = this.views.size();
      View[] viewArray = length >= 0 ? new View[length] : throw new NegativeArraySizeException();
      this.views.copyInto((object[]) viewArray);
      return viewArray;
    }

    public override void invalidateContent()
    {
      this.buildInvalid = true;
      this.invalidateLayout();
    }

    public override void invalidateLayout()
    {
      this.layoutInvalid = true;
      base.invalidateLayout();
    }

    public override void layout(Size maximumSize)
    {
      if (!this.layoutInvalid)
        return;
      this.getViewManager().showWaitCursor();
      this.layoutInvalid = false;
      ++this.layoutCount;
      this.markDamaged();
      this.builder.layout(this.getView(), new Size(maximumSize));
      this.markDamaged();
      this.getViewManager().showDefaultCursor();
    }

    public virtual bool isLayoutInvalid() => this.layoutInvalid;

    public override View subviewFor(Location location)
    {
      Location location1 = new Location(location);
      Padding padding = this.getPadding();
      location1.subtract(padding.getLeft(), padding.getTop());
      View[] subviews = this.getSubviews();
      for (int index = subviews.Length - 1; index >= 0; index += -1)
      {
        if (subviews[index].getBounds().contains(location1))
          return subviews[index];
      }
      return (View) null;
    }

    public override View pickupView(Location location) => this.canDragView ? base.pickupView(location) : (View) null;

    public override void removeView(View view)
    {
      if (!this.views.contains((object) view))
        throw new NakedObjectRuntimeException(new StringBuffer().append((object) view).append(" not in ").append((object) this.getView()).ToString());
      if (CompositeView.LOG.isDebugEnabled())
        CompositeView.LOG.debug((object) new StringBuffer().append("removing ").append((object) view).append(" from ").append((object) this).ToString());
      this.views.removeElement((object) view);
      this.markDamaged();
      this.invalidateLayout();
    }

    public override void replaceView(View toReplace, View replacement)
    {
      for (int index = 0; index < this.views.size(); ++index)
      {
        if (this.views.elementAt(index) == toReplace)
        {
          replacement.setParent(this.getView());
          replacement.setLocation(toReplace.getLocation());
          this.views.insertElementAt((object) replacement, index);
          this.invalidateLayout();
          toReplace.dispose();
          return;
        }
      }
      throw new NakedObjectRuntimeException(new StringBuffer().append((object) toReplace).append(" not found to replace").ToString());
    }

    public virtual void setCanDragView(bool canDragView) => this.canDragView = canDragView;

    public override void setFocusManager(FocusManager focusManager) => this.focusManager = focusManager;

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this, this.getId());
      toString.append("type", this.getSpecification().getName());
      return toString.ToString();
    }

    public override void update(Naked @object)
    {
      if (CompositeView.LOG.isDebugEnabled())
        CompositeView.LOG.debug((object) new StringBuffer().append("update notify on ").append((object) this).ToString());
      this.invalidateContent();
    }

    public override ViewAreaType viewAreaType(Location location)
    {
      View view = this.subviewFor(location);
      if (view == null)
        return ViewAreaType.VIEW;
      location.subtract(view.getLocation());
      return view.viewAreaType(location);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static CompositeView()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
