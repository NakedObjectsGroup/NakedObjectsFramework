// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.ViewUpdateNotifier
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.defaults;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark
{
  [JavaInterfaces("1;org/nakedobjects/utility/DebugInfo;")]
  public class ViewUpdateNotifier : DirtyObjectSetImpl, DebugInfo
  {
    private static readonly org.apache.log4j.Logger LOG;
    [JavaFlags(4)]
    public Hashtable views;

    public virtual void add(View view)
    {
      Content associateWithView = this.getContentToAssociateWithView(view);
      if (!(associateWithView is ObjectContent))
        return;
      Naked naked = associateWithView.getNaked();
      if (naked == null)
        return;
      Vector vector;
      if (this.views.containsKey((object) naked))
      {
        vector = (Vector) this.views.get((object) naked);
      }
      else
      {
        vector = new Vector();
        this.views.put((object) naked, (object) vector);
      }
      if (vector.contains((object) view))
        throw new NakedObjectRuntimeException(new StringBuffer().append((object) view).append(" already being notified").ToString());
      vector.addElement((object) view);
      if (!ViewUpdateNotifier.LOG.isDebugEnabled())
        return;
      ViewUpdateNotifier.LOG.debug((object) new StringBuffer().append("added ").append((object) view).append(" to observers for ").append((object) naked).ToString());
    }

    public virtual void debugData(DebugString buf)
    {
      Enumeration enumeration1 = this.views.keys();
      while (enumeration1.hasMoreElements())
      {
        object obj = enumeration1.nextElement();
        Enumeration enumeration2 = ((Vector) this.views.get(obj)).elements();
        buf.append((object) new StringBuffer().append("Views for ").append(obj).append(" \n").ToString());
        while (enumeration2.hasMoreElements())
        {
          View view = (View) enumeration2.nextElement();
          buf.append((object) new StringBuffer().append("        ").append((object) view).ToString());
          buf.append((object) "\n");
        }
        buf.append((object) "\n");
      }
    }

    public virtual string getDebugTitle() => "Views for object details (observers)";

    public virtual void remove(View view)
    {
      Content associateWithView = this.getContentToAssociateWithView(view);
      if (ViewUpdateNotifier.LOG.isDebugEnabled())
        ViewUpdateNotifier.LOG.debug((object) new StringBuffer().append("removing ").append((object) associateWithView).append(" for ").append((object) view).ToString());
      if (!(associateWithView is ObjectContent))
        return;
      Naked naked = (Naked) ((ObjectContent) associateWithView).getObject();
      Vector vector = naked != null && this.views.containsKey((object) naked) ? (Vector) this.views.get((object) naked) : throw new NakedObjectRuntimeException(new StringBuffer().append("Tried to remove a non-existant view ").append((object) view).append(" from observers for ").append((object) naked).ToString());
      vector.removeElement((object) view);
      if (ViewUpdateNotifier.LOG.isDebugEnabled())
        ViewUpdateNotifier.LOG.debug((object) new StringBuffer().append("removed ").append((object) view).append(" from observers for ").append((object) naked).ToString());
      if (vector.size() != 0)
        return;
      this.views.remove((object) naked);
      if (!ViewUpdateNotifier.LOG.isDebugEnabled())
        return;
      ViewUpdateNotifier.LOG.debug((object) new StringBuffer().append("removed observer list for ").append((object) naked).ToString());
    }

    public override void shutdown() => this.views.clear();

    public virtual void invalidateViewsForChangedObjects()
    {
      Enumeration enumeration1 = this.dirtyObjects();
      while (enumeration1.hasMoreElements())
      {
        NakedObject nakedObject = (NakedObject) enumeration1.nextElement();
        if (ViewUpdateNotifier.LOG.isDebugEnabled())
          ViewUpdateNotifier.LOG.debug((object) new StringBuffer().append("invalidate views for ").append((object) nakedObject).ToString());
        object obj = this.views.get((object) nakedObject);
        if (obj != null)
        {
          Enumeration enumeration2 = ((Vector) obj).elements();
          while (enumeration2.hasMoreElements())
          {
            View view = (View) enumeration2.nextElement();
            if (ViewUpdateNotifier.LOG.isDebugEnabled())
              ViewUpdateNotifier.LOG.debug((object) new StringBuffer().append("   - ").append((object) view).ToString());
            view.invalidateContent();
          }
        }
      }
    }

    private Content getContentToAssociateWithView(View view)
    {
      Content content = view.getContent();
      if (content is OneToManyField)
      {
        Naked parent = (Naked) ((OneToManyField) content).getParent();
        if (parent != null && parent is NakedObject)
          return Skylark.getContentFactory().createRootContent(parent);
      }
      return content;
    }

    public ViewUpdateNotifier() => this.views = new Hashtable();

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static ViewUpdateNotifier()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
