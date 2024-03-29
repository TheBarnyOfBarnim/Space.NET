﻿/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/TheBarnyOfBarnim/Space.NET
 * Copyright (C) 2022-2024 Endric Barnekow <mail@e-barnekow.de>
 * https://github.com/TheBarnyOfBarnim/Space.NET/blob/master/LICENSE.md
 */

using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace SpaceNET.API
{
    public static class SQL
    {
        public static DataBase ConnectDataBase(string Server, uint Port, string DataBase, string Username, string Password)
        {
            try
            {
                MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
                builder.Server = Server;
                builder.Port = Port;
                builder.UserID = Username;
                builder.Password = Password;
                builder.Database = DataBase;
                builder.CharacterSet = "UTF8";

                return new DataBase(new MySqlConnection(builder.ConnectionString));
            }
            catch (Exception)
            {

                return null;
            }

        }

        public class DataBase : IDisposable
        {
            private MySqlConnection Connection;

            internal DataBase(MySqlConnection conn)
            {
                Connection = conn;
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
            }

            public void Dispose()
            {
                Connection.Close();
                Connection.Dispose();
            }

            public DataTable ExecCommand(string SQLCommand, string[] Parameters = null, object[] Values = null)
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                try
                {
                    Parameters = Parameters == null ? new string[0] : Parameters;
                    Values = Values == null ? new object[0] : Values;

                    using (MySqlCommand command = new MySqlCommand(SQLCommand, Connection))
                    {

                        if (Parameters.Length == Values.Length)
                        {
                            for (int i = 0; i < Parameters.Length; i++)
                            {
                                command.Parameters.AddWithValue(Parameters[i], Values[i]);
                            }
                        }
                        else
                        {
                            if (Parameters.Length > 0 || Values.Length > 0)
                                throw new ArgumentException("", "Parameters, Values");
                        }

                        DataTable Table = new DataTable();
                        Table.Load(command.ExecuteReader());

                        return Table;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
