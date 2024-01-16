﻿// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.Database.Storage.ComicStorageMySql
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using MySql.Data.MySqlClient;
using System;
using System.Data.Common;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.Database.Storage
{
  public class ComicStorageMySql : ComicStorageBaseSql
  {
    protected override DbConnection CreateConnection(string connection)
    {
      return (DbConnection) new MySqlConnection(connection);
    }

    protected override bool CreateTables()
    {
      try
      {
        this.ExecuteCommand("Create table changes (id varchar(40) not null primary key, update_counter bigint default 0, delete_counter bigint default 0)");
        this.ExecuteCommand("Create table comics (id varchar(40) not null primary key, update_counter bigint default 0, data mediumtext)");
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }
  }
}
