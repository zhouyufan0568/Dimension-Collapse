using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;
using System.Data;

namespace DimensionCollapse
{
    public sealed class MySqlManagers
    {
        /// <summary>
        /// Manage connections between the client and database.
        /// </summary>
        private class ConnManager
        {
            private const string connStr = "server=10.82.82.27;" +
                           "uid=client;" +
                           "pwd=111222333;" +
                           "database=dimensioncollapse";

            public static ConnManager INSTANCE;
            static ConnManager()
            {
                INSTANCE = new ConnManager();
            }
            private ConnManager() { }

            public MySqlConnection OpenConnection()
            {
                MySqlConnection conn = null;
                try
                {
                    conn = new MySqlConnection(connStr);
                    conn.Open();
                }
                catch (MySqlException e)
                {
                    Debug.Log(e);
                }
                return conn;
            }

            public async Task<MySqlConnection> OpenConnectionAsync()
            {
                return await Task.Run(() => OpenConnection());
            }
        }

        /// <summary>
        /// Manage transactions related to accounts.
        /// </summary>
        public class AccountManager
        {
            public static AccountManager INSTANCE;
            private ConnManager connManager;

            private string sqlForOnlineTest = 
                "select `state` from `dimensioncollapse`.`login_state` where id = (select `id` " +
                "from `dimensioncollapse`.`user` " +
                "where `account`=\"{0}\");";

            private string sqlForAccountExistenceTest = "select `user`.account from `dimensioncollapse`.user where account = \"{0}\";";
            static AccountManager()
            {
                INSTANCE = new AccountManager();
            }
            private AccountManager()
            {
                connManager = ConnManager.INSTANCE;
            }

            public void ShowAllAccounts()
            {
                MySqlConnection conn = connManager.OpenConnection();
                if (conn == null || conn.State == System.Data.ConnectionState.Closed)
                {
                    Debug.Log("Fail to connect to the database.");
                    return;
                }

                string sql = "SELECT `user`.`account`,`user`.`password`" +
                    " FROM `dimensioncollapse`.`user`; ";
                MySqlCommand command = new MySqlCommand(sql, conn);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Debug.Log(reader["account"].ToString() + ' ' + reader["password"].ToString());
                }

                conn.Dispose();
            }

            public async void ShowAllAccountsAsync()
            {
                await Task.Run(() => ShowAllAccounts());
            }

            /// <summary>
            /// Check whether the given account is online.
            /// This is a sync method.
            /// </summary>
            /// <param name="account">the account to be checked</param>
            /// <returns>
            /// Result.DISCONNECTED : Fail to connect to the database;
            /// Result.NOTEXIST     : The account given doesn't exist in the database.
            /// Result.YES          : The account given is online.
            /// Result.NO           : The account given is offline.
            /// </returns>
            public Result IsOnline(string account)
            {
                string sql = String.Format(sqlForOnlineTest, account);
                return DbHelper.TemplateQueryForYesAndNo(sql, reader =>
                {
                    try
                    {
                        int state = Convert.ToInt32(reader["state"]);
                        switch (state)
                        {
                            case 1:
                                return true;
                            default:
                                return false;
                        }
                    }
                    catch (FormatException e)
                    {
                        Debug.Log(e);
                        return false;
                    }
                }
                );
            }

            /// <summary>
            /// Check whether the given account is online.
            /// This is an async method.
            /// </summary>
            /// <param name="account">the account to be checked</param>
            /// <returns>
            /// Result.DISCONNECTED : Fail to connect to the database;
            /// Result.NOTEXIST     : The account given doesn't exist in the database.
            /// Result.YES          : The account given is online.
            /// Result.NO           : The account given is offline.
            /// </returns>
            public async Task<Result> IsOnlineAsync(string account)
            {
                return await Task.Run(() => IsOnline(account));
            }

            /// <summary>
            /// Check if the account exists in the database.
            /// This is a sync method.
            /// </summary>
            /// <param name="account">The account to be checked.</param>
            /// <returns>
            /// Result.DISCONNECTED : Fail to connect to the database;
            /// Result.NOTEXIST     : The account given doesn't exist in the database.
            /// Result.YES          : The account given already exists.
            /// </returns>
            public Result IsAccountExists(string account)
            {
                string sql = string.Format(sqlForAccountExistenceTest, account);
                return DbHelper.TemplateQueryForYesAndNo(sql, reader =>
                {
                    return true;
                }
                );
            }

            /// <summary>
            /// Check if the account exists in the database.
            /// This is an async method.
            /// </summary>
            /// <param name="account">The account to be checked.</param>
            /// <returns>
            /// Result.DISCONNECTED : Fail to connect to the database;
            /// Result.NOTEXIST     : The account given doesn't exist in the database.
            /// Result.YES          : The account given already exists.
            /// </returns>
            public async Task<Result> IsAccountExistsAsync(string account)
            {
                return await Task.Run(() => IsAccountExists(account));
            }
        }

        /// <summary>
        /// Used for describing the result of the transaction between client and database.
        /// </summary>
        public enum Result
        {
            DISCONNECTED,
            NOTEXIST,
            YES,
            NO
        }

        /// <summary>
        /// Contains some methods that can make the transaction between client and database easier.
        /// </summary>
        private static class DbHelper
        {
            public static Result TemplateQueryForYesAndNo(string sql, Func<MySqlDataReader, bool> func)
            {
                MySqlConnection conn = ConnManager.INSTANCE.OpenConnection();
                if (conn == null || conn.State == System.Data.ConnectionState.Closed)
                {
                    Debug.Log("Fail to connect to the database.");
                    return Result.DISCONNECTED;
                }

                MySqlCommand command = new MySqlCommand(sql, conn);
                MySqlDataReader reader = command.ExecuteReader();

                Result res = Result.NOTEXIST;
                if (reader.Read())
                {
                    res = func(reader) ? Result.YES : Result.NO;
                }

                conn.Dispose();
                return res;
            }

            public static async Task<Result> TemplateQueryForYesAndNoAsync(string sql, Func<MySqlDataReader, bool> func)
            {
                return await Task.Run(() => TemplateQueryForYesAndNo(sql, func));
            }
        }

        /// <summary>
        /// Unfinished.
        /// </summary>
        private sealed class SQLBuilder
        {
            private class Clauses
            {
                StringBuilder select;
                StringBuilder from;
                StringBuilder where;

                public Clauses()
                {
                    select = new StringBuilder();
                    from = new StringBuilder();
                    where = new StringBuilder();
                }

                public void AppendSelect(string column)
                {
                    if (column == null)
                    {
                        Debug.Log("trying to append a null string into SQL.");
                        return;
                    }

                    if (select.Capacity != 0)
                    {
                        select.Append(",");
                    }
                    select.Append(column);
                }
            }
        }

        /// <summary>
        /// Unfinished
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private abstract class Request<T> : CustomYieldInstruction
        {
            public abstract bool IsDone { get; }
            public abstract T Target { get; }
        }
    }
}