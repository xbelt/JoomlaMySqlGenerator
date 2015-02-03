using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Warenlager.API;

namespace JoomlaMySqlGenerator
{
    public partial class Form1 : Form
    {
        private List<string> _categories = new List<string>();
        private List<Tuple<int, string>> _values = new List<Tuple<int, string>>(); 
        public Form1()
        {
            InitializeComponent();
            Config.Init();
            dbServer.Text = Config.Settings["server"]["MySQL"]["ServerIP"].Value;
            dbUser.Text = Config.Settings["server"]["MySQL"]["User"].Value;
            dbPassword.Text = Config.Settings["server"]["MySQL"]["Password"].Value;
            dbName.Text = Config.Settings["server"]["MySQL"]["Database"].Value;
            dbPrefix.Text = Config.Settings["server"]["MySQL"]["Prefix"].Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var reader = (new MySqlCommand("SELECT * FROM categories ORDER BY id ASC", MySqlConnectionGenerator.ShortConnection())).ExecuteReader())
            {
                _categories.Clear();
                while (reader.Read())
                {
                    _categories.Add(reader.GetStringSave("De"));    
                }
            }

            using (var reader = (new MySqlCommand("SELECT * FROM diseases ORDER BY category_id ASC", MySqlConnectionGenerator.ShortConnection())).ExecuteReader())
            {
                _values.Clear();
                while (reader.Read())
                {
                    _values.Add(new Tuple<int, string>(reader.GetInt32Save("category_id"), reader.GetStringSave("de")));
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void dbServer_TextChanged(object sender, EventArgs e)
        {
            Config.Settings["server"]["MySQL"]["ServerIP"].Value = dbServer.Text;
            Config.Commit();
        }

        private void dbUser_TextChanged(object sender, EventArgs e)
        {
            Config.Settings["server"]["MySQL"]["User"].Value = dbUser.Text;
            Config.Commit();
        }

        private void dbPassword_TextChanged(object sender, EventArgs e)
        {
            Config.Settings["server"]["MySQL"]["Password"].Value = dbPassword.Text;
            Config.Commit();
        }

        private void dbPrefix_TextChanged(object sender, EventArgs e)
        {
            Config.Settings["server"]["MySQL"]["Prefix"].Value = dbPassword.Text;
            Config.Commit();
        }

        private void dbName_TextChanged(object sender, EventArgs e)
        {
            Config.Settings["server"]["MySQL"]["Database"].Value = dbName.Text;
            Config.Commit();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
