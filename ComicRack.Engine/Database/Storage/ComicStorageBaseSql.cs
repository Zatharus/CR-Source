// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Database.Storage.ComicStorageBaseSql
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Xml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Database.Storage
{
  public abstract class ComicStorageBaseSql : IComicStorage
  {
    public static readonly Guid StorageGuid = Guid.Empty;
    public const int DbVersion = 1;
    private DbConnection dbConnection;
    private readonly Dictionary<Guid, long> updateMap = new Dictionary<Guid, long>();
    private long lastUpdate;
    private long lastDelete;
    private readonly Stack<DbTransaction> transactionStack = new Stack<DbTransaction>();

    protected abstract DbConnection CreateConnection(string connection);

    protected abstract bool CreateTables();

    private DbCommand CreateCommand(string command, params object[] data)
    {
      if (this.dbConnection == null)
        throw new InvalidOperationException();
      if (this.dbConnection.State == ConnectionState.Broken || this.dbConnection.State == ConnectionState.Closed)
      {
        if (this.dbConnection.State == ConnectionState.Broken)
        {
          try
          {
            this.dbConnection.Close();
          }
          catch
          {
          }
        }
        this.dbConnection.Open();
        lock (this.transactionStack)
          this.transactionStack.Clear();
      }
      DbCommand command1 = this.dbConnection.CreateCommand();
      command1.CommandText = string.Format(command, data);
      lock (this.transactionStack)
      {
        DbTransaction dbTransaction = this.transactionStack.Count == 0 ? (DbTransaction) null : this.transactionStack.Peek();
        if (dbTransaction != null)
          command1.Transaction = dbTransaction;
      }
      return command1;
    }

    protected int ExecuteCommand(string command, params object[] data)
    {
      using (DbCommand command1 = this.CreateCommand(command, data))
        return command1.ExecuteNonQuery();
    }

    private long ReadCounter(string name)
    {
      using (DbCommand command = this.CreateCommand("Select " + name + " from changes"))
      {
        object obj = command.ExecuteScalar();
        return obj == null ? 0L : (long) obj;
      }
    }

    private void WriteCounter(string name, long counter)
    {
      using (DbCommand command = this.CreateCommand("Update changes set " + name + "={1} where id='{0}'", (object) ComicStorageBaseSql.StorageGuid, (object) counter))
      {
        if (command.ExecuteNonQuery() != 0)
          return;
        command.CommandText = string.Format("Insert into changes values ('{0}', 0, 0)", (object) ComicStorageBaseSql.StorageGuid);
        command.ExecuteNonQuery();
        this.WriteCounter(name, counter);
      }
    }

    private long ReadUpdateCounter() => this.ReadCounter("update_counter");

    private void WriteUpdateCounter(long counter)
    {
      this.WriteCounter("update_counter", counter);
      this.lastUpdate = counter;
    }

    private long ReadDeleteCounter() => this.ReadCounter("delete_counter");

    private void WriteDeleteCounter(long counter)
    {
      this.WriteCounter("delete_counter", counter);
      this.lastDelete = counter;
    }

    private void UpdateBook(ComicBook targetBook, ComicBook sourceBook)
    {
      targetBook.CopyFrom(sourceBook);
    }

    public bool Open(string connection)
    {
      this.dbConnection = this.CreateConnection(connection);
      this.dbConnection.Open();
      lock (this.transactionStack)
        this.transactionStack.Clear();
      return this.CreateTables();
    }

    public void Close()
    {
      this.dbConnection.Close();
      this.dbConnection = (DbConnection) null;
      lock (this.transactionStack)
        this.transactionStack.Clear();
    }

    public bool IsConnected
    {
      get
      {
        return this.dbConnection != null && this.dbConnection.State != ConnectionState.Closed && this.dbConnection.State != ConnectionState.Broken;
      }
    }

    public void Delete(ComicBook book)
    {
      if (this.ExecuteCommand("Delete from comics where id='{0}';", (object) book.Id) <= 0)
        return;
      this.updateMap.Remove(book.Id);
      this.WriteDeleteCounter(this.ReadDeleteCounter() + 1L);
    }

    public bool Write(ComicBook book)
    {
      string str = XmlUtility.ToString((object) book);
      long num = this.ReadUpdateCounter() + 1L;
      using (DbCommand command = this.CreateCommand("Update comics set data=@data, update_counter={1} where id='{0}'", (object) book.Id, (object) num))
      {
        DbParameter parameter = command.CreateParameter();
        parameter.DbType = DbType.String;
        parameter.ParameterName = "@data";
        parameter.Value = (object) str;
        command.Parameters.Add((object) parameter);
        if (command.ExecuteNonQuery() == 0)
        {
          command.CommandText = string.Format("Insert into comics values ('{0}', {1}, @data)", (object) book.Id, (object) num);
          command.ExecuteNonQuery();
        }
      }
      this.WriteUpdateCounter(this.ReadUpdateCounter() + 1L);
      return false;
    }

    public IEnumerable<ComicBook> Load()
    {
      using (DbCommand cmd = this.CreateCommand("Select update_counter, data from comics"))
      {
        this.lastUpdate = this.ReadUpdateCounter();
        using (DbDataReader reader = cmd.ExecuteReader())
        {
          while (reader.Read())
          {
            ComicBook comicBook;
            try
            {
              comicBook = XmlUtility.FromString<ComicBook>(reader[1].ToString());
              this.updateMap[comicBook.Id] = (long) reader[0];
            }
            catch
            {
              comicBook = (ComicBook) null;
            }
            if (comicBook != null)
              yield return comicBook;
          }
        }
      }
    }

    public bool Refresh(ComicBookContainer bookContainer)
    {
      bool flag = false;
      if (this.lastUpdate < this.ReadUpdateCounter())
      {
        using (DbCommand command = this.CreateCommand("Select id, update_counter, data from comics where update_counter>{0}", (object) this.lastUpdate))
        {
          using (DbDataReader dbDataReader = command.ExecuteReader())
          {
            while (dbDataReader.Read())
            {
              Guid id = new Guid(dbDataReader[0].ToString());
              long num = (long) dbDataReader[1];
              ComicBook comicBook = XmlUtility.FromString<ComicBook>(dbDataReader[2].ToString());
              if (bookContainer.Books[id] != null)
              {
                ComicBook book = bookContainer.Books[id];
                this.UpdateBook(book, comicBook);
                this.updateMap[book.Id] = num;
              }
              else
              {
                bookContainer.Add(comicBook);
                this.updateMap[comicBook.Id] = num;
              }
            }
          }
        }
        this.lastUpdate = this.ReadUpdateCounter();
        flag = true;
      }
      if (this.lastDelete < this.ReadDeleteCounter())
      {
        using (DbCommand command = this.CreateCommand("Select id from comics"))
        {
          using (DbDataReader dbDataReader = command.ExecuteReader())
          {
            HashSet<Guid> ids = new HashSet<Guid>();
            while (dbDataReader.Read())
              ids.Add(new Guid(dbDataReader[0].ToString()));
            IEnumerable<ComicBook> list = ((IEnumerable<ComicBook>) bookContainer.Books.ToArray()).Where<ComicBook>((System.Func<ComicBook, bool>) (cb => !ids.Contains(cb.Id)));
            bookContainer.Books.RemoveRange(list);
          }
        }
        this.lastDelete = this.ReadDeleteCounter();
        flag = true;
      }
      return flag;
    }

    public void BeginTransaction()
    {
      lock (this.transactionStack)
        this.transactionStack.Push(this.dbConnection.BeginTransaction());
    }

    public void CommitTransaction()
    {
      lock (this.transactionStack)
        this.transactionStack.Pop()?.Commit();
    }

    public void RollbackTransaction()
    {
      lock (this.transactionStack)
        this.transactionStack.Pop()?.Rollback();
    }
  }
}
