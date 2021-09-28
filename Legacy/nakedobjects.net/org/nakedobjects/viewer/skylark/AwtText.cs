// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.AwtText
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/Text;")]
  public class AwtText : Text
  {
    private static readonly string FONT_PROPERTY_STEM;
    private static readonly org.apache.log4j.Logger LOG;
    private static readonly string SPACING_PROPERTYSTEM;
    private Font font;
    private Frame fontMetricsComponent;
    private int lineSpacing;
    private int maxCharWidth;
    private FontMetrics metrics;
    private string propertyName;
    private Hashtable stringWidthByString;

    [JavaFlags(4)]
    public AwtText(string propertyName, string defaultFont)
    {
      this.fontMetricsComponent = new Frame();
      this.stringWidthByString = new Hashtable();
      NakedObjectConfiguration configuration = NakedObjects.getConfiguration();
      this.font = configuration.getFont(new StringBuffer().append(AwtText.FONT_PROPERTY_STEM).append(propertyName).ToString(), Font.decode(defaultFont));
      if (AwtText.LOG.isInfoEnabled())
        AwtText.LOG.info((object) new StringBuffer().append("font ").append(propertyName).append(" loaded as ").append((object) this.font).ToString());
      this.propertyName = propertyName;
      if (this.font == null)
        this.font = configuration.getFont(new StringBuffer().append(AwtText.FONT_PROPERTY_STEM).append("default").ToString(), new Font("SansSerif", 0, 12));
      this.metrics = ((Component) this.fontMetricsComponent).getFontMetrics(this.font);
      this.maxCharWidth = this.metrics.getMaxAdvance() + 1;
      if (this.maxCharWidth == 0)
        this.maxCharWidth = this.charWidth('X') + 3;
      this.lineSpacing = configuration.getInteger(new StringBuffer().append(AwtText.SPACING_PROPERTYSTEM).append(propertyName).ToString(), 0);
      if (!AwtText.LOG.isDebugEnabled())
        return;
      AwtText.LOG.debug((object) new StringBuffer().append("font ").append(propertyName).append(" height=").append(this.metrics.getHeight()).append(", leading=").append(this.metrics.getLeading()).append(", ascent=").append(this.metrics.getAscent()).append(", descent=").append(this.metrics.getDescent()).append(", line spacing=").append(this.lineSpacing).ToString());
    }

    public virtual int charWidth(char c) => this.metrics.charWidth(c);

    public virtual int getAscent() => this.metrics.getAscent();

    public virtual Font getAwtFont() => this.font;

    public virtual int getDescent() => this.metrics.getDescent();

    public virtual int getLineHeight() => this.metrics.getHeight() + this.getLineSpacing();

    public virtual int getLineSpacing() => this.lineSpacing;

    public virtual string getName() => this.propertyName;

    public virtual int getMidPoint() => this.getAscent() / 2;

    public virtual int getTextHeight() => this.metrics.getHeight();

    public virtual int stringWidth(string text)
    {
      int[] numArray1 = (int[]) this.stringWidthByString.get((object) text);
      if (numArray1 == null)
      {
        int length = 1;
        int[] numArray2 = length >= 0 ? new int[length] : throw new NegativeArraySizeException();
        numArray2[0] = this.stringWidthInternal(text);
        numArray1 = numArray2;
        this.stringWidthByString.put((object) text, (object) numArray1);
      }
      return numArray1[0];
    }

    private int stringWidthInternal(string text)
    {
      int num1 = this.metrics.stringWidth(text);
      if (num1 > StringImpl.length(text) * this.maxCharWidth)
      {
        if (AwtText.LOG.isDebugEnabled())
          AwtText.LOG.debug((object) new StringBuffer().append("spurious width of string; calculating manually: ").append(num1).append(" for ").append((object) this).append(": ").append(text).ToString());
        num1 = 0;
        for (int index = 0; index < StringImpl.length(text); ++index)
        {
          int num2 = this.charWidth(StringImpl.charAt(text, index));
          if (num2 > this.maxCharWidth)
          {
            if (AwtText.LOG.isDebugEnabled())
              AwtText.LOG.debug((object) new StringBuffer().append("spurious width of character; using max width: ").append(num2).append(" for ").append(StringImpl.charAt(text, index)).ToString());
            num2 = this.maxCharWidth;
          }
          num1 += num2;
          if (AwtText.LOG.isDebugEnabled())
            AwtText.LOG.debug((object) new StringBuffer().append(index).append(" ").append(num1).ToString());
        }
      }
      return num1;
    }

    public override string ToString() => this.font.ToString();

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static AwtText()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AwtText awtText = this;
      ObjectImpl.clone((object) awtText);
      return ((object) awtText).MemberwiseClone();
    }
  }
}
