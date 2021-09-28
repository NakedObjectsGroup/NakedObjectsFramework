// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.WorkspaceBuilder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.util;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class WorkspaceBuilder : AbstractViewBuilder
  {
    private static readonly org.apache.log4j.Logger LOG;
    private const int PADDING = 10;
    public static readonly Location UNPLACED;

    public override void build(View view)
    {
      NakedObject nakedObject1 = ((ObjectContent) view.getContent()).getObject();
      if (nakedObject1 == null || view.getSubviews().Length != 0)
        return;
      NakedObjectField[] visibleFields = nakedObject1.getVisibleFields();
      ViewFactory viewFactory = Skylark.getViewFactory();
      ContentFactory contentFactory = Skylark.getContentFactory();
      for (int index = 0; index < visibleFields.Length; ++index)
      {
        NakedObjectField field1 = visibleFields[index];
        Naked field2 = nakedObject1.getField(field1);
        if (StringImpl.equals(field1.getId(), (object) "classes") && field1.isCollection())
        {
          Enumeration enumeration = ((NakedCollection) field2).elements();
          while (enumeration.hasMoreElements())
          {
            NakedObject nakedObject2 = (NakedObject) enumeration.nextElement();
            Content rootContent = contentFactory.createRootContent((Naked) nakedObject2);
            View icon = viewFactory.createIcon(rootContent);
            icon.setLocation(WorkspaceBuilder.UNPLACED);
            view.addView(icon);
          }
        }
        else if (StringImpl.equals(field1.getId(), (object) "objects") && field1.isCollection())
        {
          Enumeration enumeration = ((NakedCollection) field2).elements();
          while (enumeration.hasMoreElements())
          {
            NakedObject nakedObject3 = (NakedObject) enumeration.nextElement();
            Content rootContent = contentFactory.createRootContent((Naked) nakedObject3);
            View icon = viewFactory.createIcon(rootContent);
            view.addView(icon);
          }
        }
      }
    }

    public virtual bool canDisplay(Naked @object) => @object is NakedObject && @object != null;

    public override Size getRequiredSize(View view) => new Size(500, 500);

    public virtual string getName() => "Simple Workspace";

    [MethodImpl(MethodImplOptions.Synchronized)]
    public override void layout(View view, Size maximumSize)
    {
      View[] subviews = view.getSubviews();
      for (int index = 0; index < subviews.Length; ++index)
        subviews[index].layout(new Size(maximumSize));
      Size size = view.getSize();
      size.contract(view.getPadding());
      if (WorkspaceBuilder.LOG.isDebugEnabled())
        WorkspaceBuilder.LOG.debug((object) new StringBuffer().append("laying out workspace within ").append((object) size).ToString());
      int height = size.getHeight();
      int width = size.getWidth();
      int x1 = 10;
      int y1 = 10;
      int num1 = 0;
      int num2 = width - 10;
      int y2 = 10;
      int x2 = 150;
      int y3 = 10;
      int x3 = 1;
      int num3 = height - 1;
      for (int index = 0; index < subviews.Length; ++index)
      {
        View view1 = subviews[index];
        Size requiredSize = view1.getRequiredSize(new Size(size));
        view1.setSize(requiredSize);
        if (view1 is MinimizedView)
        {
          Size maximumSize1 = view1.getMaximumSize();
          if (x3 + maximumSize1.getWidth() > width)
          {
            x3 = 1;
            num3 -= maximumSize1.getHeight() + 1;
          }
          view1.setLocation(new Location(x3, num3 - maximumSize1.getHeight()));
          x3 += maximumSize1.getWidth() + 1;
        }
        else if (view1.getLocation().Equals((object) WorkspaceBuilder.UNPLACED))
        {
          int num4 = requiredSize.getHeight() + 6;
          if (view1.getSpecification().isOpen())
          {
            view1.setLocation(new Location(x2, y3));
            y3 += num4;
          }
          else if (view1.getContent().getNaked().getObject() is NakedClass)
          {
            if (y1 + num4 > height)
            {
              y1 = 10;
              x1 += num1 + 10;
              num1 = 0;
              if (WorkspaceBuilder.LOG.isDebugEnabled())
                WorkspaceBuilder.LOG.debug((object) new StringBuffer().append("creating new column at ").append(x1).append(", ").append(y1).ToString());
            }
            if (WorkspaceBuilder.LOG.isDebugEnabled())
              WorkspaceBuilder.LOG.debug((object) new StringBuffer().append("class icon at ").append(x1).append(", ").append(y1).ToString());
            view1.setLocation(new Location(x1, y1));
            num1 = Math.max(num1, requiredSize.getWidth());
            y1 += num4;
          }
          else
          {
            view1.setLocation(new Location(num2 - requiredSize.getWidth(), y2));
            y2 += num4;
          }
        }
        view1.limitBoundsWithin(maximumSize);
      }
    }

    public override View createCompositeView(
      Content content,
      CompositeViewSpecification specification,
      ViewAxis axis)
    {
      throw new NotImplementedException();
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static WorkspaceBuilder()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
