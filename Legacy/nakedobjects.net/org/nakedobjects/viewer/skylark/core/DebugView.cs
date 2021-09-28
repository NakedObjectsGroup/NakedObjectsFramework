// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.DebugView
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.skylark.core
{
  [JavaInterfaces("1;org/nakedobjects/utility/DebugInfo;")]
  public class DebugView : DebugInfo
  {
    private readonly View view;

    public DebugView(View display) => this.view = display;

    public virtual void debugData(DebugString debug)
    {
      debug.append((object) this.view.getView());
      debug.blankLine();
      debug.blankLine();
      debug.appendTitle("VIEW");
      debug.append((object) this.view.debugDetails());
      debug.append((object) "\n");
      Content content = this.view.getContent();
      debug.appendTitle("CONTENT");
      if (content != null)
      {
        string name = ObjectImpl.getClass((object) content).getName();
        string str = StringImpl.substring(name, StringImpl.lastIndexOf(name, 46) + 1);
        debug.appendln("Content", (object) str);
        content.debugDetails(debug);
        debug.indent();
        debug.appendln("Icon name", (object) content.getIconName());
        debug.appendln("Icon ", (object) content.getIconPicture(32));
        debug.appendln("Window title", (object) content.windowTitle());
        debug.appendln("Persistable", content.isPersistable());
        debug.appendln("Object", content.isObject());
        debug.appendln("Collection", content.isCollection());
        debug.appendln("Value", content.isValue());
        debug.unindent();
      }
      else
        debug.appendln("Content", (object) "none");
      debug.blankLine();
      switch (content)
      {
        case ObjectContent _:
          NakedObject nakedObject = ((ObjectContent) content).getObject();
          debug.blankLine();
          this.dumpObject((Naked) nakedObject, debug);
          this.dumpSpecification((Naked) nakedObject, debug);
          debug.blankLine();
          this.dumpGraph((Naked) nakedObject, debug);
          break;
        case CollectionContent _:
          NakedCollection collection = ((CollectionContent) content).getCollection();
          this.dumpObject((Naked) collection, debug);
          debug.blankLine();
          this.dumpSpecification((Naked) collection, debug);
          debug.blankLine();
          this.dumpGraph((Naked) collection, debug);
          break;
      }
      debug.append((object) "\n\nDRAWING\n");
      debug.append((object) "------\n");
      this.view.draw((Canvas) new DebugCanvas(debug, new Bounds(this.view.getBounds())));
    }

    public virtual string getDebugTitle() => new StringBuffer().append("Debug: ").append((object) this.view).append((object) this.view).ToString() == null ? "" : new StringBuffer().append("/").append((object) this.view.getContent()).ToString();

    public virtual void dumpGraph(Naked @object, DebugString info)
    {
      if (@object == null)
        return;
      info.appendTitle("GRAPH");
      Dump.graph(@object, info);
    }

    public virtual void dumpObject(Naked @object, DebugString info)
    {
      if (@object == null)
        return;
      info.appendTitle("OBJECT");
      Dump.adapter(@object, info);
    }

    private void dumpSpecification(Naked @object, DebugString info)
    {
      if (@object == null)
        return;
      info.appendTitle("SPECIFICATION");
      Dump.specification(@object, info);
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      DebugView debugView = this;
      ObjectImpl.clone((object) debugView);
      return ((object) debugView).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
