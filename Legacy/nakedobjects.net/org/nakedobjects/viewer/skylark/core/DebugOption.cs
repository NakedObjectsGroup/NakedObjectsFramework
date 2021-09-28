// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.DebugOption
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.skylark.core
{
  public class DebugOption : AbstractUserAction
  {
    public DebugOption()
      : base("Debug...", UserAction.DEBUG)
    {
    }

    public override void execute(Workspace workspace, View view, Location at)
    {
      InfoDebugFrame infoDebugFrame = new InfoDebugFrame();
      Naked naked = view.getContent().getNaked();
      DebugInfo[] debugInfoArray1;
      if (naked == null)
      {
        int length = 0;
        debugInfoArray1 = length >= 0 ? new DebugInfo[length] : throw new NegativeArraySizeException();
      }
      else
      {
        int length = 3;
        debugInfoArray1 = length >= 0 ? new DebugInfo[length] : throw new NegativeArraySizeException();
        debugInfoArray1[0] = (DebugInfo) new DebugAdapter(naked);
        debugInfoArray1[1] = (DebugInfo) new DebugObjectGraph(naked);
        debugInfoArray1[2] = (DebugInfo) new DebugObjectSpecification(naked);
      }
      DebugInfo[] debugInfoArray2 = debugInfoArray1;
      int length1 = 4;
      DebugInfo[] debugInfoArray3 = length1 >= 0 ? new DebugInfo[length1] : throw new NegativeArraySizeException();
      debugInfoArray3[0] = (DebugInfo) new DebugViewStructure(view);
      debugInfoArray3[1] = (DebugInfo) new DebugContent(view);
      debugInfoArray3[2] = (DebugInfo) new DebugDrawing(view);
      debugInfoArray3[3] = (DebugInfo) new DebugDrawingAbsolute(view);
      DebugInfo[] debugInfoArray4 = debugInfoArray3;
      int length2 = debugInfoArray2.Length;
      int length3 = debugInfoArray4.Length;
      int length4 = length2 + length3;
      DebugInfo[] info = length4 >= 0 ? new DebugInfo[length4] : throw new NegativeArraySizeException();
      System.arraycopy((object) debugInfoArray2, 0, (object) info, 0, length2);
      System.arraycopy((object) debugInfoArray4, 0, (object) info, length2, length3);
      infoDebugFrame.setInfo(info);
      infoDebugFrame.show(at.getX() + 50, at.getY() + 6);
    }

    public override string getDescription(View view) => new StringBuffer().append("Open debug window about ").append((object) view).ToString();
  }
}
