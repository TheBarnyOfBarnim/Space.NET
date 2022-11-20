/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/PylonDev/Space.NET
 * Copyright (C) 2022 Endric Barnekow <pylon@pylonmediagroup.de>
 * https://github.com/PylonDev/Space.NET/blob/master/LICENSE.md
 */

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space.NET.API
{
    public static class SQL
    {
        public static DataBase ConnectDataBase(string Server, string DataBase, string Username, string Password)
        {
            try
            {
                MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
                builder.Server = Server;
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
                Connection.Open();
            }

            public void Dispose()
            {
                Connection.Close();
                Connection.Dispose();
            }

            public DataTable ExecCommand(string SQLCommand, string[] Parameters = null, object[] Values = null)
            {
                try
                {
                    using (MySqlCommand command = new MySqlCommand(SQLCommand, Connection))
                    {
                        
                        if((Parameters != null && Parameters.Length != 0) && (Values != null && Values.Length != 0) && Parameters.Length == Values.Length)
                        {
                            for (int i = 0; i < Parameters.Length; i++)
                            {
                                command.Parameters.AddWithValue(Parameters[i], Values[i]);
                            }
                        }
                        else
                        {
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
