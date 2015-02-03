using System;
using System.Threading;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace JoomlaMySqlGenerator {
    public static class MySqlConnectionGenerator {
        private static MySqlConnection _mySqlConnection1;

        public static MySqlConnection ShortConnection()
        {
            return TimedConnection(10000);
        }

        public static MySqlConnection LongConnection()
        {
            return TimedConnection(100000);
        }

        public static MySqlConnection ExtraLongConnection()
        {
            return TimedConnection(1000000);
        }

        public static MySqlConnection TimedConnection(int timeout) {
            if (_mySqlConnection1 == null)
            {
                var connectString =
                    "server=" + Config.Settings["server"]["MySQL"]["ServerIP"].Value + ";" +
                    "user=" + Config.Settings["server"]["MySQL"]["User"].Value + ";" +
                    "database=" + Config.Settings["server"]["MySQL"]["Database"].Value + ";" +
                    "port=" + Config.Settings["server"]["MySQL"]["Port"].Value + ";" +
                    "password=" + Config.Settings["server"]["MySQL"]["Password"].Value + ";";
#if DEBUG
                MessageBox.Show("We are currently in DEBUG modus. Server-ip: " +
                                Config.Settings["server"]["MySQL"]["ServerIP"].Value);
#endif
                var connection = new MySqlConnection(connectString);
                try {
                    connection.Open();
                    _mySqlConnection1 = connection;
                }
                catch (Exception e) {
                    Console.WriteLine(e.StackTrace);
                    MessageBox.Show(
                        @"Couldn't connect to mysql server. Please verify credentials and upstatus of MySql server");
                    Application.Exit();
                }
            }
            var newConnection = (MySqlConnection) _mySqlConnection1.Clone();
            newConnection.Open();
            new Thread(() => {
                Thread.Sleep(timeout);
                newConnection.Close();}
                ).Start();
            return newConnection;
        }
    }
}