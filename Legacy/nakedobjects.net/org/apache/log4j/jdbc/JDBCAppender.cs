// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.jdbc.JDBCAppender
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using java.sql;
using java.util;
using org.apache.log4j.spi;
using System;

namespace org.apache.log4j.jdbc
{
  [JavaInterfaces("1;org/apache/log4j/Appender;")]
  public class JDBCAppender : AppenderSkeleton, Appender
  {
    [JavaFlags(4)]
    public string databaseURL;
    [JavaFlags(4)]
    public string databaseUser;
    [JavaFlags(4)]
    public string databasePassword;
    [JavaFlags(4)]
    public Connection connection;
    [JavaFlags(4)]
    public string sqlStatement;
    [JavaFlags(4)]
    public int bufferSize;
    [JavaFlags(4)]
    public ArrayList buffer;
    [JavaFlags(4)]
    public ArrayList removes;

    public JDBCAppender()
    {
      this.databaseURL = "jdbc:odbc:myDB";
      this.databaseUser = "me";
      this.databasePassword = "mypassword";
      this.connection = (Connection) null;
      this.sqlStatement = "";
      this.bufferSize = 1;
      this.buffer = new ArrayList(this.bufferSize);
      this.removes = new ArrayList(this.bufferSize);
    }

    public override void append(LoggingEvent @event)
    {
      this.buffer.add((object) @event);
      if (this.buffer.size() < this.bufferSize)
        return;
      this.flushBuffer();
    }

    [JavaFlags(4)]
    public virtual string getLogStatement(LoggingEvent @event) => this.getLayout().format(@event);

    [JavaThrownExceptions("1;java/sql/SQLException;")]
    [JavaFlags(4)]
    public virtual void execute(string sql)
    {
      Statement statement = (Statement) null;
      Connection connection;
      try
      {
        connection = this.getConnection();
        statement = connection.createStatement();
        statement.executeUpdate(sql);
      }
      catch (SQLException ex)
      {
        SQLException sqlException1 = ex;
        statement?.close();
        SQLException sqlException2 = sqlException1;
        if (sqlException2 != ex)
          throw sqlException2;
        throw;
      }
      statement.close();
      this.closeConnection(connection);
    }

    [JavaFlags(4)]
    public virtual void closeConnection(Connection con)
    {
    }

    [JavaFlags(4)]
    [JavaThrownExceptions("1;java/sql/SQLException;")]
    public virtual Connection getConnection()
    {
      if (!DriverManager.getDrivers().hasMoreElements())
        this.setDriver("sun.jdbc.odbc.JdbcOdbcDriver");
      if (this.connection == null)
        this.connection = DriverManager.getConnection(this.databaseURL, this.databaseUser, this.databasePassword);
      return this.connection;
    }

    public override void close()
    {
      this.flushBuffer();
      try
      {
        if (this.connection != null)
        {
          if (!this.connection.isClosed())
            this.connection.close();
        }
      }
      catch (SQLException ex)
      {
        this.errorHandler.error("Error closing connection", (Exception) ex, 0);
      }
      this.closed = true;
    }

    public virtual void flushBuffer()
    {
      this.removes.ensureCapacity(this.buffer.size());
      Iterator iterator = ((AbstractList) this.buffer).iterator();
      while (iterator.hasNext())
      {
        try
        {
          LoggingEvent @event = (LoggingEvent) iterator.next();
          this.execute(this.getLogStatement(@event));
          this.removes.add((object) @event);
        }
        catch (SQLException ex)
        {
          this.errorHandler.error("Failed to excute sql", (Exception) ex, 2);
        }
      }
      ((AbstractCollection) this.buffer).removeAll((Collection) this.removes);
    }

    public override void Finalize()
    {
      try
      {
        this.close();
      }
      catch (Exception ex)
      {
      }
    }

    public override bool requiresLayout() => true;

    public virtual void setSql(string s)
    {
      this.sqlStatement = s;
      if (this.getLayout() == null)
        this.setLayout((Layout) new PatternLayout(s));
      else
        ((PatternLayout) this.getLayout()).setConversionPattern(s);
    }

    public virtual string getSql() => this.sqlStatement;

    public virtual void setUser(string user) => this.databaseUser = user;

    public virtual void setURL(string url) => this.databaseURL = url;

    public virtual void setPassword(string password) => this.databasePassword = password;

    public virtual void setBufferSize(int newBufferSize)
    {
      this.bufferSize = newBufferSize;
      this.buffer.ensureCapacity(this.bufferSize);
      this.removes.ensureCapacity(this.bufferSize);
    }

    public virtual string getUser() => this.databaseUser;

    public virtual string getURL() => this.databaseURL;

    public virtual string getPassword() => this.databasePassword;

    public virtual int getBufferSize() => this.bufferSize;

    public virtual void setDriver(string driverClass)
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
