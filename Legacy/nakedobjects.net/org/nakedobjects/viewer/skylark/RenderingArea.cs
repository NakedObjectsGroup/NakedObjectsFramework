// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.RenderingArea
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.awt;
using java.awt.@event;

namespace org.nakedobjects.viewer.skylark
{
  [JavaInterface]
  public interface RenderingArea
  {
    Dimension getSize();

    Image createImage(int w, int h);

    Insets getInsets();

    void repaint();

    void repaint(int x, int y, int width, int height);

    void setCursor(Cursor cursor);

    void dispose();

    void setBounds(int i, int j, int k, int l);

    void show();

    void addMouseMotionListener(MouseMotionListener interactionHandler);

    void addMouseListener(MouseListener interactionHandler);

    void addKeyListener(KeyListener interactionHandler);

    string selectFilePath(string title, string directory);
  }
}
