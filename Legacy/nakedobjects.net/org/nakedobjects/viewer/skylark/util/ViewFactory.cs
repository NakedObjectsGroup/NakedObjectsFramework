// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.util.ViewFactory
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.metal;
using System;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.util
{
  [JavaInterfaces("1;org/nakedobjects/utility/DebugInfo;")]
  public class ViewFactory : DebugInfo
  {
    private static readonly ViewSpecification fallback;
    public const int INTERNAL = 2;
    private static readonly org.apache.log4j.Logger LOG;
    public const int WINDOW = 1;
    private ViewSpecification emptyFieldSpecification;
    private readonly Vector rootViews;
    private ViewSpecification subviewIconSpecification;
    private readonly Vector subviews;
    private readonly Vector valueFields;
    private ViewSpecification workspaceClassIconSpecification;
    private ViewSpecification workspaceObjectIconSpecification;
    private ViewSpecification rootWorkspaceSpecification;
    private ViewSpecification workspaceSpecification;

    public virtual void addClassIconSpecification(ViewSpecification spec) => this.workspaceClassIconSpecification = spec;

    public virtual void addCompositeRootViewSpecification(ViewSpecification spec) => this.rootViews.addElement((object) spec);

    public virtual void addCompositeSubviewViewSpecification(ViewSpecification spec) => this.subviews.addElement((object) spec);

    public virtual void addEmptyFieldSpecification(ViewSpecification spec) => this.emptyFieldSpecification = spec;

    public virtual void addObjectIconSpecification(ViewSpecification spec) => this.workspaceObjectIconSpecification = spec;

    public virtual void addSubviewIconSpecification(ViewSpecification spec) => this.subviewIconSpecification = spec;

    public virtual void addValueFieldSpecification(ViewSpecification spec) => this.valueFields.addElement((object) spec);

    public virtual void addRootWorkspaceSpecification(ViewSpecification spec) => this.rootWorkspaceSpecification = spec;

    public virtual void addWorkspaceSpecification(ViewSpecification spec) => this.workspaceSpecification = spec;

    public virtual Enumeration closedSubviews(Content forContent, View replacingView)
    {
      Vector vector = new Vector();
      if (forContent is ObjectContent)
        vector.addElement((object) this.subviewIconSpecification);
      return vector.elements();
    }

    public virtual View createIcon(Content content)
    {
      View view = this.createView(this.getIconizedRootViewSpecification(content), content);
      if (ViewFactory.LOG.isDebugEnabled())
        ViewFactory.LOG.debug((object) new StringBuffer().append("creating ").append((object) view).append(" (icon) for ").append((object) content).ToString());
      return view;
    }

    public virtual View createWindow(Content content)
    {
      View view = this.createView(this.getOpenRootViewSpecification(content), content);
      if (ViewFactory.LOG.isDebugEnabled())
        ViewFactory.LOG.debug((object) new StringBuffer().append("creating ").append((object) view).append(" (window) for ").append((object) content).ToString());
      return view;
    }

    private View createView(ViewSpecification specification, Content content)
    {
      if (specification == null)
      {
        if (ViewFactory.LOG.isWarnEnabled())
          ViewFactory.LOG.warn((object) new StringBuffer().append("no suitable view for ").append((object) content).append(" using fallback view").ToString());
        specification = (ViewSpecification) new FallbackView.Specification();
      }
      return specification.createView(content, (ViewAxis) null);
    }

    public virtual View createInnerWorkspace(Content content)
    {
      if (ViewFactory.LOG.isDebugEnabled())
        ViewFactory.LOG.debug((object) new StringBuffer().append("creating inner workspace for ").append((object) content).ToString());
      return this.createView(this.workspaceSpecification, content);
    }

    private ViewSpecification defaultViewSpecification(
      Vector availableViews,
      Content content)
    {
      Enumeration enumeration = availableViews.elements();
      while (enumeration.hasMoreElements())
      {
        ViewSpecification viewSpecification = (ViewSpecification) enumeration.nextElement();
        if (viewSpecification.canDisplay(content))
          return viewSpecification;
      }
      if (ViewFactory.LOG.isWarnEnabled())
        ViewFactory.LOG.warn((object) new StringBuffer().append("no suitable view for ").append((object) content).append(" using fallback view").ToString());
      return (ViewSpecification) new FallbackView.Specification();
    }

    private ViewSpecification ensureView(ViewSpecification spec)
    {
      if (spec != null)
        return spec;
      ViewFactory.LOG.error((object) "missing view; using fallback");
      return (ViewSpecification) new FallbackView.Specification();
    }

    public virtual void debugData(DebugString sb)
    {
      sb.append((object) "RootsViews\n");
      Enumeration enumeration1 = this.rootViews.elements();
      while (enumeration1.hasMoreElements())
      {
        ViewSpecification viewSpecification = (ViewSpecification) enumeration1.nextElement();
        sb.append((object) "  ");
        sb.append((object) viewSpecification);
        sb.append((object) "\n");
      }
      sb.append((object) "\n\n");
      sb.append((object) "Subviews\n");
      Enumeration enumeration2 = this.subviews.elements();
      while (enumeration2.hasMoreElements())
      {
        ViewSpecification viewSpecification = (ViewSpecification) enumeration2.nextElement();
        sb.append((object) "  ");
        sb.append((object) viewSpecification);
        sb.append((object) "\n");
      }
      sb.append((object) "\n\n");
      sb.append((object) "Value fields\n");
      Enumeration enumeration3 = this.valueFields.elements();
      while (enumeration3.hasMoreElements())
      {
        ViewSpecification viewSpecification = (ViewSpecification) enumeration3.nextElement();
        sb.append((object) "  ");
        sb.append((object) viewSpecification);
        sb.append((object) "\n");
      }
      sb.append((object) "\n\n");
    }

    public virtual string getDebugTitle() => "View factory entries";

    public virtual ViewSpecification getEmptyFieldSpecification()
    {
      if (this.emptyFieldSpecification != null)
        return this.emptyFieldSpecification;
      ViewFactory.LOG.error((object) "missing empty field specification; using fallback");
      return ViewFactory.fallback;
    }

    public virtual ViewSpecification getIconizedRootViewSpecification(
      Content content)
    {
      if (content.getNaked().getObject() is NakedClass)
      {
        if (this.workspaceClassIconSpecification != null)
          return this.ensureView(this.workspaceClassIconSpecification);
        ViewFactory.LOG.error((object) "missing workspace class icon specification; using fallback");
        return ViewFactory.fallback;
      }
      if (this.workspaceObjectIconSpecification != null)
        return this.ensureView(this.workspaceObjectIconSpecification);
      ViewFactory.LOG.error((object) "missing workspace object icon specification; using fallback");
      return ViewFactory.fallback;
    }

    public virtual ViewSpecification getIconizedSubViewSpecification(
      Content content)
    {
      if (content.getNaked() == null)
        return this.getEmptyFieldSpecification();
      if (this.subviewIconSpecification != null)
        return this.ensureView(this.subviewIconSpecification);
      ViewFactory.LOG.error((object) "missing sub view icon specification; using fallback");
      return ViewFactory.fallback;
    }

    private ViewSpecification getOpenRootViewSpecification(Content content) => this.defaultViewSpecification(this.rootViews, content);

    [Obsolete(null, false)]
    public virtual ViewSpecification getOpenSubViewSpecification(
      ObjectContent content)
    {
      return this.defaultViewSpecification(this.subviews, (Content) content);
    }

    public virtual ViewSpecification getValueFieldSpecification(
      ValueContent content)
    {
      NakedValue nakedValue = content.getObject();
      return nakedValue == null || \u003CVerifierFix\u003E.isInstanceOfString(nakedValue.getObject()) || nakedValue.getObject() is Date ? (ViewSpecification) new TextFieldSpecification() : this.defaultViewSpecification(this.valueFields, (Content) content);
    }

    public virtual Enumeration openRootViews(Content forContent, View replacingView) => this.viewSpecifications(this.rootViews, forContent);

    public virtual Enumeration openSubviews(Content forContent, View replacingView) => forContent is ObjectContent ? this.viewSpecifications(this.subviews, forContent) : new Vector().elements();

    public virtual Enumeration valueViews(Content forContent, View replacingView) => new Vector().elements();

    private Enumeration viewSpecifications(Vector availableViews, Content content)
    {
      Vector vector = new Vector();
      Enumeration enumeration = availableViews.elements();
      while (enumeration.hasMoreElements())
      {
        ViewSpecification viewSpecification = (ViewSpecification) enumeration.nextElement();
        if (viewSpecification.canDisplay(content))
          vector.addElement((object) viewSpecification);
      }
      return vector.elements();
    }

    public ViewFactory()
    {
      this.rootViews = new Vector();
      this.subviews = new Vector();
      this.valueFields = new Vector();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static ViewFactory()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ViewFactory viewFactory = this;
      ObjectImpl.clone((object) viewFactory);
      return ((object) viewFactory).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
