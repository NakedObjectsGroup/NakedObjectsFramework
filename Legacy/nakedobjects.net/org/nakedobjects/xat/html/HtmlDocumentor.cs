// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.html.HtmlDocumentor
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.text;
using java.util;
using org.nakedobjects.@object;

namespace org.nakedobjects.xat.html
{
  public class HtmlDocumentor : AbstractDocumentor
  {
    private bool isList;
    private PrintWriter writer;
    private File dir;

    public HtmlDocumentor(string directory)
    {
      this.isList = false;
      this.dir = new File(directory);
      if (this.dir.exists())
        return;
      this.dir.mkdirs();
    }

    public virtual void open(string className, string name)
    {
      try
      {
        string str1 = new StringBuffer().append(StringImpl.replace(className, ' ', '_')).append("-xat.html").ToString();
        if (this.writer != null)
          return;
        this.writer = new PrintWriter((Writer) new OutputStreamWriter((OutputStream) new FileOutputStream(new File(this.dir, str1))));
        string str2 = this.makeTitle(name);
        this.writer.println("<html>");
        this.writer.println("<head>");
        this.writer.println("<style type=\"text/css\">");
        this.writer.println("<!--");
        this.writer.println("H1 {color: darkblue}");
        this.writer.println("P{line-height: 150%}");
        this.writer.println("OL LI {line-height: 150%; margin-bottom: 8pt}");
        this.writer.println("EM  {background-color: #cccccc; border-left: solid #cccccc 3px; border-right: solid #cccccc 3px; border-top: solid #cccccc 1px; border-bottom: solid #cccccc 1px ; font-style: normal}");
        this.writer.println("CODE  {background-color: #cccccc; border-top: solid #cccccc 1px; border-bottom: solid black 1px; font-family: sans-serif}");
        this.writer.println("IMG  {padding-right: 4px}");
        this.writer.println("-->");
        this.writer.println("</style>");
        this.writer.println(new StringBuffer().append("<title>").append(str2).append("</title>").ToString());
        this.writer.println("</head>");
        this.writer.println("<body>");
        this.writer.write(new StringBuffer().append("<h1>Documentation: ").append(str2).append("</h1>").ToString());
        this.writer.write(new StringBuffer().append("<small>Generated ").append(DateFormat.getDateTimeInstance().format(new Date())).append("</small>").ToString());
      }
      catch (IOException ex)
      {
        ((Throwable) ex).printStackTrace();
      }
    }

    public override void close()
    {
      this.writer.println("</body>");
      this.writer.println("</html>");
      this.writer.close();
    }

    public override void doc(string text)
    {
      if (!this.isGenerating())
        return;
      this.writer.print(text);
    }

    public override void docln(string text)
    {
      if (!this.isGenerating())
        return;
      this.doc(new StringBuffer().append(text).append("\n").ToString());
    }

    public override void flush() => this.writer.flush();

    public virtual string objectString(Naked @object)
    {
      string str1 = @object.titleString();
      StringBuffer stringBuffer = new StringBuffer();
      stringBuffer.append(new StringBuffer().append("<b>").append(@object.getSpecification().getShortName()).append("</b> object ").ToString());
      stringBuffer.append("<em>");
      string str2 = !(@object is NakedObject) ? @object.getSpecification().getShortName() : ((NakedReference) @object).getIconName();
      stringBuffer.append(new StringBuffer().append("<img width=\"16\" height=\"16\" align=\"Center\" src=\"images/").append(str2).append(".gif\">").ToString());
      stringBuffer.append(new StringBuffer().append("<font face=\"sans-serif\">").append(str1).append("</font>").ToString());
      stringBuffer.append("</em>");
      return stringBuffer.ToString();
    }

    public virtual string simpleObjectString(Naked @object)
    {
      StringBuffer stringBuffer = new StringBuffer();
      stringBuffer.append("<em>");
      string str = !(@object is NakedObject) ? @object.getSpecification().getShortName() : ((NakedReference) @object).getIconName();
      stringBuffer.append(new StringBuffer().append("<img width=\"16\" height=\"16\" align=\"Center\" src=\"images/").append(str).append(".gif\">").ToString());
      stringBuffer.append(new StringBuffer().append("<font face=\"sans-serif\">").append(@object.titleString()).append("</font>").ToString());
      stringBuffer.append("</em>");
      return stringBuffer.ToString();
    }

    public override void step(string text)
    {
      if (!this.isGenerating())
        return;
      text = StringImpl.trim(text);
      if (!this.isList)
      {
        this.docln("<ol>");
        this.isList = true;
      }
      this.docln(new StringBuffer().append("<li>").append(text).append(!StringImpl.endsWith(text, ".") ? ".  " : " ").ToString());
      this.flush();
    }

    public override void subtitle(string text)
    {
      if (!this.isGenerating())
        return;
      if (this.isList)
      {
        this.docln("</ol>");
        this.isList = false;
      }
      if (StringImpl.equals(text, (object) ""))
        return;
      this.docln(new StringBuffer().append("<h3>").append(text).append("</h3>").ToString());
    }

    public override void title(string text)
    {
      if (!this.isGenerating())
        return;
      this.docln(new StringBuffer().append("<h2>").append(text).append("</h2>").ToString());
    }
  }
}
