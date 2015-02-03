using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Warenlager.API
{
    internal static class MySqlExtensions
    {
        public static string GetStringSave(this MySqlDataReader reader, int ordinal)
        {
            if (reader == null)
            {
                ShowError("in GetString(" + ordinal + ") reader was null");
                return "";
            }
            if (reader.IsClosed)
            {
                ShowError("in GetString(" + ordinal + ") reader was closed");
                return "";
            }
            if (reader.IsDBNull(ordinal))
            {
                ShowError("in GetString(" + ordinal + ") DB was null ordinal: " + ordinal);
                return "";
            }
            return reader.GetString(ordinal);
        }

        public static string GetStringSave(this MySqlDataReader reader, string name)
        {
            var ordinal = reader.GetOrdinal(name);
            return reader.GetStringSave(ordinal);
        }

        public static string GetStringNull(this MySqlDataReader reader, int ordinal)
        {
            if (reader == null)
            {
                ShowError("in GetString(" + ordinal + ") reader was null");
                return "";
            }
            if (reader.IsClosed)
            {
                ShowError("in GetString(" + ordinal + ") reader was closed");
                return "";
            }
            if (reader.IsDBNull(ordinal))
            {
                return null;
            }
            return reader.GetString(ordinal);
        }

        public static string GetStringNull(this MySqlDataReader reader, string name)
        {
            var ordinal = reader.GetOrdinal(name);
            return reader.GetStringNull(ordinal);
        }

        public static int GetInt32Save(this MySqlDataReader reader, int ordinal)
        {
            if (reader == null)
            {
                ShowError("in GetIn32(" + ordinal + ") reader was null");
                return 0;
            }
            if (reader.IsClosed)
            {
                ShowError("in GetIn32(" + ordinal + ") reader was closed");
                return 0;
            }
            if (reader.IsDBNull(ordinal))
            {
                ShowError("in GetIn32(" + ordinal + ") DB was null");
                return 0;
            }
            return reader.GetInt32(ordinal);
        }

        public static int GetInt32Save(this MySqlDataReader reader, string name)
        {
            var ordinal = reader.GetOrdinal(name);
            return reader.GetInt32Save(ordinal);
        }

        public static int? GetInt32Null(this MySqlDataReader reader, string name)
        {
            var ordinal = reader.GetOrdinal(name);
            return reader.GetInt32Null(ordinal);
        }

        public static int? GetInt32Null(this MySqlDataReader reader, int ordinal)
        {
            if (reader == null)
            {
                ShowError("in GetIn32(" + ordinal + ") reader was null");
                return 0;
            }
            if (reader.IsClosed)
            {
                ShowError("in GetIn32(" + ordinal + ") reader was closed");
                return 0;
            }
            if (reader.IsDBNull(ordinal))
            {
                return null;
            }
            return reader.GetInt32(ordinal);
        }

        public static double GetDoubleSave(this MySqlDataReader reader, int ordinal)
        {
            if (reader == null)
            {
                ShowError("in GeDouble(" + ordinal + ") reader was null");
                return 0;
            }
            if (reader.IsClosed)
            {
                ShowError("in GeDouble(" + ordinal + ") reader was closed");
                return 0;
            }
            if (reader.IsDBNull(ordinal))
            {
                ShowError("in GeDouble(" + ordinal + ") DB was null");
                return 0;
            }
            return reader.GetDouble(ordinal);
        }

        public static double GetDoubleSave(this MySqlDataReader reader, string name)
        {
            var ordinal = reader.GetOrdinal(name);
            return reader.GetDoubleSave(ordinal);
        }

        public static DateTime? GetDateTimeNull(this MySqlDataReader reader, int ordinal)
        {
            if (reader == null)
                return null;
            if (reader.IsClosed)
                return null;
            if (reader.IsDBNull(ordinal))
                return null;
            return reader.GetDateTime(ordinal);
        }

        public static DateTime? GetDateTimeNull(this MySqlDataReader reader, string name)
        {
            var ordinal = reader.GetOrdinal(name);
            return reader.GetDateTimeNull(ordinal);
        }

        private static void ShowError(object custom)
        {
            MessageBox.Show("Something went wrong retrieving data from DB. " + Environment.StackTrace +
                            custom);
        }

        public static object GetValue(this MySqlDataReader reader, string name)
        {
            var ordinal = reader.GetOrdinal(name);
            return reader.GetValue(ordinal);
        }
    }
}
