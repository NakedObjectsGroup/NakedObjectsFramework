// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.persistence.file.XmlDataManager
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.persistence;
using org.nakedobjects.utility;
using org.xml.sax;
using org.xml.sax.helpers;
using System.ComponentModel;

namespace org.nakedobjects.persistence.file
{
  [JavaInterfaces("1;org/nakedobjects/persistence/file/DataManager;")]
  public class XmlDataManager : DataManager
  {
    public const string DEFAULT_ENCODING = "ISO-8859-1";
    private const string ENCODING_PROPERTY = "nakedobjects.xmlos.encoding";
    private static readonly string[] escapeString;
    private static readonly string[] specialChars;
    private string charset;
    private File directory;

    public static string directory() => NakedObjects.getConfiguration().getString("nakedobjects.xmlos.dir", "xml");

    public virtual string getDebugData() => new StringBuffer().append("Data directory ").append(XmlDataManager.directory()).ToString();

    [JavaFlags(12)]
    public static void clearTestDirectory()
    {
      File file = new File(new StringBuffer().append("tmp").append((string) File.separator).append("tests").ToString());
      string[] strArray = file.list((FilenameFilter) new XmlDataManager.\u0031());
      if (strArray == null)
        return;
      for (int index = 0; index < strArray.Length; ++index)
        new File(file, strArray[index]).delete();
    }

    public static string getValueWithSpecialsEscaped(string s)
    {
      string str = s;
      for (int index = 0; index < XmlDataManager.specialChars.Length; ++index)
      {
        string specialChar = XmlDataManager.specialChars[index];
        int num = -1;
        while (true)
        {
          num = StringImpl.indexOf(str, specialChar, num + 1);
          if (num >= 0)
            str = new StringBuffer().append(StringImpl.substring(str, 0, num)).append(XmlDataManager.escapeString[index]).append(StringImpl.substring(str, num + StringImpl.length(specialChar))).ToString();
          else
            break;
        }
      }
      return str;
    }

    public XmlDataManager()
      : this(XmlDataManager.directory())
    {
    }

    public XmlDataManager(string directory)
    {
      this.directory = new File(directory);
      if (!this.directory.exists())
        this.directory.mkdirs();
      this.charset = NakedObjects.getConfiguration().getString("nakedobjects.xmlos.encoding", "ISO-8859-1");
    }

    [JavaFlags(4)]
    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual void addData(SerialOid oid, string type, Data data) => this.writeData(oid, data);

    [JavaFlags(4)]
    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual void addInstance(SerialOid oid, string type)
    {
      Vector instances = this.loadInstances(type);
      instances.addElement((object) oid);
      this.writeInstanceFile(type, instances);
    }

    private string attribute(string name, string value) => new StringBuffer().append(" ").append(name).append("=\"").append(value).append("\"").ToString();

    [JavaFlags(17)]
    [JavaThrownExceptions("1;org/nakedobjects/persistence/file/PersistorException;")]
    public SerialOid createOid() => new SerialOid(this.nextId());

    [JavaFlags(4)]
    public virtual void deleteData(SerialOid oid, string type) => this.file(this.filename(oid)).delete();

    private string encodedOid(SerialOid oid) => StringImpl.toUpperCase(Long.toHexString(oid.getSerialNo()));

    private File file(string fileName) => new File(this.directory, new StringBuffer().append(fileName).append(".xml").ToString());

    private string filename(SerialOid oid) => this.encodedOid(oid);

    public virtual ObjectDataVector getInstances(ObjectData pattern)
    {
      Vector vector = this.loadInstances(pattern.getClassName());
      if (vector == null)
        return new ObjectDataVector();
      ObjectDataVector objectDataVector = new ObjectDataVector();
      for (int index = 0; index < vector.size(); ++index)
      {
        SerialOid oid = (SerialOid) vector.elementAt(index);
        ObjectData objectData = (ObjectData) this.loadData(oid);
        if (objectData == null)
          throw new NakedObjectRuntimeException(new StringBuffer().append("No data found for ").append((object) oid).append(" (possible missing file)").ToString());
        if (this.matchesPattern(pattern, objectData))
          objectDataVector.addElement(objectData);
      }
      return objectDataVector;
    }

    public virtual void getNakedClass(string name)
    {
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    [JavaFlags(17)]
    public void insert(Data data)
    {
      string type = data.getOid() != null && !data.getOid().isNull() ? data.getClassName() : throw new IllegalArgumentException("Oid must be non-null");
      SerialOid oid = data.getOid();
      this.addData(oid, type, data);
      this.addInstance(oid, type);
    }

    [JavaFlags(17)]
    public CollectionData loadCollectionData(SerialOid oid) => (CollectionData) this.loadData(oid);

    public virtual Data loadData(SerialOid oid)
    {
      XmlDataManager.DataHandler dataHandler = new XmlDataManager.DataHandler(this);
      this.parse((ContentHandler) dataHandler, this.filename(oid));
      return dataHandler.@object != null ? (Data) dataHandler.@object : (Data) dataHandler.collection;
    }

    private Vector loadInstances(string type)
    {
      XmlDataManager.InstanceHandler instanceHandler = new XmlDataManager.InstanceHandler(this);
      this.parse((ContentHandler) instanceHandler, type);
      return instanceHandler.instances;
    }

    [JavaFlags(17)]
    public ObjectData loadObjectData(SerialOid oid) => (ObjectData) this.loadData(oid);

    [JavaFlags(4)]
    public virtual bool matchesPattern(ObjectData patternData, ObjectData testData)
    {
      if (patternData == null || testData == null)
        throw new NullPointerException(new StringBuffer().append("Can't match on nulls ").append((object) patternData).append(" ").append((object) testData).ToString());
      if (!StringImpl.equals(patternData.getClassName(), (object) testData.getClassName()))
        return false;
      Enumeration enumeration = patternData.fields();
      while (enumeration.hasMoreElements())
      {
        string fieldName = \u003CVerifierFix\u003E.genCastToString(enumeration.nextElement());
        object obj1 = patternData.get(fieldName);
        object obj2 = testData.get(fieldName);
        if (obj2 is ReferenceVector)
        {
          ReferenceVector referenceVector1 = (ReferenceVector) obj1;
          for (int index1 = 0; index1 < referenceVector1.size(); ++index1)
          {
            SerialOid serialOid = referenceVector1.elementAt(index1);
            bool flag = false;
            ReferenceVector referenceVector2 = (ReferenceVector) obj2;
            for (int index2 = 0; index2 < referenceVector2.size(); ++index2)
            {
              if (serialOid.Equals((object) referenceVector2.elementAt(index2)))
              {
                flag = true;
                break;
              }
            }
            if (!flag)
              return false;
          }
        }
        else if (!obj1.Equals(obj2))
          return false;
      }
      return true;
    }

    [JavaFlags(4)]
    [JavaThrownExceptions("1;org/nakedobjects/persistence/file/PersistorException;")]
    public virtual long nextId()
    {
      XmlDataManager.NumberHandler numberHandler = new XmlDataManager.NumberHandler(this);
      this.parse((ContentHandler) numberHandler, "oid");
      StringBuffer buf = new StringBuffer();
      buf.append("<number>");
      buf.append(numberHandler.value + 1L);
      buf.append("</number>");
      this.writeXml("oid", buf);
      return numberHandler.value + 1L;
    }

    public virtual int numberOfInstances(ObjectData pattern)
    {
      Vector vector = this.loadInstances(pattern.getClassName());
      if (vector == null)
        return 0;
      int num = 0;
      for (int index = 0; index < vector.size(); ++index)
      {
        ObjectData testData = (ObjectData) this.loadData((SerialOid) vector.elementAt(index));
        if (testData != null && this.matchesPattern(pattern, testData))
          ++num;
      }
      return num;
    }

    private bool parse(ContentHandler handler, string fileName)
    {
      XMLReader xmlReader;
      try
      {
        xmlReader = XMLReaderFactory.createXMLReader();
      }
      catch (SAXException ex1)
      {
        try
        {
          xmlReader = XMLReaderFactory.createXMLReader("org.apache.xerces.parsers.SAXParser");
        }
        catch (SAXException ex2)
        {
          try
          {
            xmlReader = XMLReaderFactory.createXMLReader("org.apache.crimson.parser.XMLReaderImpl");
          }
          catch (SAXException ex3)
          {
            throw new NakedObjectRuntimeException("Couldn't locate a SAX parser");
          }
        }
      }
      try
      {
        xmlReader.setContentHandler(handler);
        xmlReader.parse(new InputSource((Reader) new InputStreamReader((InputStream) new FileInputStream(this.file(fileName)), this.charset)));
        return true;
      }
      catch (FileNotFoundException ex)
      {
        return false;
      }
      catch (IOException ex)
      {
        throw new NakedObjectRuntimeException("Error reading XML file", (Throwable) ex);
      }
      catch (SAXParseException ex)
      {
        throw new NakedObjectRuntimeException(new StringBuffer().append("Error while parsing: ").append(ex.getMessage()).append(" in ").append((object) this.file(fileName)).append(")").ToString(), (Throwable) ex);
      }
      catch (SAXException ex)
      {
        throw new NakedObjectRuntimeException(new StringBuffer().append("?? Error parsing XML file ").append((object) this.file(fileName)).append(" ").append((object) ObjectImpl.getClass((object) ex)).ToString(), (Throwable) ex.getException());
      }
    }

    [JavaThrownExceptions("2;org/nakedobjects/object/ObjectNotFoundException;org/nakedobjects/object/ObjectPerstsistenceException;")]
    [JavaFlags(17)]
    public void remove(SerialOid oid)
    {
      string className = this.loadData(oid).getClassName();
      this.removeInstance(oid, className);
      this.deleteData(oid, className);
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    [JavaFlags(4)]
    public virtual void removeInstance(SerialOid oid, string type)
    {
      Vector instances = this.loadInstances(type);
      instances.removeElement((object) oid);
      this.writeInstanceFile(type, instances);
    }

    [JavaFlags(17)]
    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public void save(Data data) => this.writeData(data.getOid(), data);

    public virtual void shutdown()
    {
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    private void writeData(SerialOid xoid, Data data)
    {
      StringBuffer buf = new StringBuffer();
      bool flag = data is ObjectData;
      string str = !flag ? "collection" : "naked-object";
      buf.append(new StringBuffer().append("<").append(str).ToString());
      buf.append(this.attribute("type", data.getClassName()));
      buf.append(this.attribute("id", new StringBuffer().append("").append(this.encodedOid(data.getOid())).ToString()));
      buf.append(">\n");
      if (flag)
      {
        ObjectData objectData = (ObjectData) data;
        Enumeration enumeration = objectData.fields();
        while (enumeration.hasMoreElements())
        {
          string fieldName = \u003CVerifierFix\u003E.genCastToString(enumeration.nextElement());
          object obj1 = objectData.get(fieldName);
          switch (obj1)
          {
            case SerialOid _:
              buf.append(new StringBuffer().append("  <association field=\"").append(fieldName).append("\" ").ToString());
              buf.append(new StringBuffer().append("ref=\"").append(this.encodedOid((SerialOid) obj1)).append("\"/>\n").ToString());
              continue;
            case ReferenceVector _:
              ReferenceVector referenceVector = (ReferenceVector) obj1;
              int num = referenceVector.size();
              if (num > 0)
              {
                buf.append(new StringBuffer().append("  <multiple-association field=\"").append(fieldName).append("\" ").ToString());
                buf.append(">\n");
                for (int index = 0; index < num; ++index)
                {
                  object obj2 = (object) referenceVector.elementAt(index);
                  buf.append("    <element ");
                  buf.append(new StringBuffer().append("ref=\"").append(this.encodedOid((SerialOid) obj2)).append("\"/>\n").ToString());
                }
                buf.append("  </multiple-association>\n");
                continue;
              }
              continue;
            default:
              buf.append(new StringBuffer().append("  <value field=\"").append(fieldName).append("\">").ToString());
              buf.append(XmlDataManager.getValueWithSpecialsEscaped(obj1.ToString()));
              buf.append("</value>\n");
              continue;
          }
        }
      }
      else
      {
        ReferenceVector referenceVector = ((CollectionData) data).references();
        for (int index = 0; index < referenceVector.size(); ++index)
        {
          object obj = (object) referenceVector.elementAt(index);
          buf.append("  <element ");
          buf.append(new StringBuffer().append("ref=\"").append(this.encodedOid((SerialOid) obj)).append("\"/>\n").ToString());
        }
      }
      buf.append(new StringBuffer().append("</").append(str).append(">\n").ToString());
      this.writeXml(this.filename(xoid), buf);
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    private void writeInstanceFile(string name, Vector instances)
    {
      StringBuffer buf = new StringBuffer();
      buf.append(new StringBuffer().append("<instances name=\"").append(name).append("\">\n").ToString());
      for (int index = 0; index < instances.size(); ++index)
        buf.append(new StringBuffer().append("  <instance id=\"").append(this.encodedOid((SerialOid) instances.elementAt(index))).append("\"/>\n").ToString());
      buf.append("</instances>");
      this.writeXml(name, buf);
    }

    private void writeXml(string name, StringBuffer buf)
    {
      try
      {
        OutputStreamWriter outputStreamWriter = new OutputStreamWriter((OutputStream) new FileOutputStream(this.file(name)), this.charset);
        ((Writer) outputStreamWriter).write(new StringBuffer().append("<?xml version=\"1.0\" encoding=\"").append(this.charset).append("\" ?>\n").ToString());
        ((Writer) outputStreamWriter).write("\n");
        ((Writer) outputStreamWriter).write(buf.ToString());
        ((Writer) outputStreamWriter).write("\n");
        outputStreamWriter.close();
      }
      catch (IOException ex)
      {
        throw new NakedObjectRuntimeException("Problems writing data files", (Throwable) ex);
      }
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static XmlDataManager()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      XmlDataManager xmlDataManager = this;
      ObjectImpl.clone((object) xmlDataManager);
      return ((object) xmlDataManager).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [Inner]
    [JavaFlags(34)]
    private class DataHandler : DefaultHandler
    {
      [JavaFlags(0)]
      public CollectionData collection;
      [JavaFlags(0)]
      public StringBuffer data;
      [JavaFlags(0)]
      public string fieldName;
      [JavaFlags(0)]
      public ObjectData @object;
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private XmlDataManager this\u00240;

      [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
      public override void characters(char[] ch, int start, int end) => this.data.append(StringImpl.createString(ch, start, end));

      [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
      public override void endElement(string ns, string name, string tagName)
      {
        if (this.@object == null || !StringImpl.equals(tagName, (object) "value"))
          return;
        this.@object.set(this.fieldName, this.data.ToString());
      }

      [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
      public override void startElement(string ns, string name, string tagName, Attributes attrs)
      {
        if (this.@object != null)
        {
          if (StringImpl.equals(tagName, (object) "value"))
          {
            this.fieldName = attrs.getValue("field");
            this.data.setLength(0);
          }
          else if (StringImpl.equals(tagName, (object) "association"))
            this.@object.set(attrs.getValue("field"), (object) new SerialOid(Long.valueOf(attrs.getValue("ref"), 16).longValue()));
          else if (StringImpl.equals(tagName, (object) "element"))
          {
            this.@object.addElement(this.fieldName, new SerialOid(Long.valueOf(attrs.getValue("ref"), 16).longValue()));
          }
          else
          {
            if (!StringImpl.equals(tagName, (object) "multiple-association"))
              return;
            this.fieldName = attrs.getValue("field");
            this.@object.initCollection((SerialOid) null, this.fieldName);
          }
        }
        else if (this.collection != null)
        {
          if (!StringImpl.equals(tagName, (object) "element"))
            return;
          this.collection.addElement(new SerialOid(Long.valueOf(attrs.getValue("ref"), 16).longValue()));
        }
        else if (StringImpl.equals(tagName, (object) "naked-object"))
        {
          string name1 = attrs.getValue("type");
          long serialNo = Long.valueOf(attrs.getValue("id"), 16).longValue();
          this.@object = new ObjectData(NakedObjects.getSpecificationLoader().loadSpecification(name1), new SerialOid(serialNo));
        }
        else
        {
          if (!StringImpl.equals(tagName, (object) "collection"))
            throw new SAXException("Invalid data");
          string name2 = attrs.getValue("type");
          long serialNo = Long.valueOf(attrs.getValue("id"), 16).longValue();
          this.collection = new CollectionData(NakedObjects.getSpecificationLoader().loadSpecification(name2), new SerialOid(serialNo));
        }
      }

      [JavaFlags(2)]
      public DataHandler(XmlDataManager _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.data = new StringBuffer();
      }
    }

    [Inner]
    [JavaFlags(34)]
    private class InstanceHandler : DefaultHandler
    {
      [JavaFlags(0)]
      public Vector instances;
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private XmlDataManager this\u00240;

      [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
      public override void characters(char[] arg0, int arg1, int arg2)
      {
      }

      [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
      public override void startElement(string ns, string name, string tagName, Attributes attrs)
      {
        if (!StringImpl.equals(tagName, (object) "instance"))
          return;
        this.instances.addElement((object) new SerialOid(Long.valueOf(attrs.getValue("id"), 16).longValue()));
      }

      [JavaFlags(2)]
      public InstanceHandler(XmlDataManager _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.instances = new Vector();
      }
    }

    [Inner]
    [JavaFlags(34)]
    private class NumberHandler : DefaultHandler
    {
      [JavaFlags(0)]
      public bool captureValue;
      [JavaFlags(0)]
      public long value;
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private XmlDataManager this\u00240;

      [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
      public override void characters(char[] arg0, int arg1, int arg2)
      {
        if (!this.captureValue)
          return;
        this.value = Long.valueOf(StringImpl.createString(arg0, arg1, arg2), 16).longValue();
      }

      [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
      public override void startElement(string ns, string name, string tagName, Attributes attrs) => this.captureValue = StringImpl.equals(tagName, (object) "number");

      [JavaFlags(2)]
      public NumberHandler(XmlDataManager _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.captureValue = false;
        this.value = 0L;
      }
    }

    [JavaInterfaces("1;java/io/FilenameFilter;")]
    [JavaFlags(32)]
    [Inner]
    public class \u0031 : FilenameFilter
    {
      public virtual bool accept(File arg0, string name) => StringImpl.endsWith(name, ".xml");

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        XmlDataManager.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
