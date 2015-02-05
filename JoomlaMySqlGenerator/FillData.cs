using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Excel = Microsoft.Office.Interop.Excel;

namespace JoomlaMySqlGenerator
{
    public partial class FillData : Form
    {
        private string _file;

        public FillData()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var file = new OpenFileDialog())
            {
                var res = file.ShowDialog();
                if (res == DialogResult.OK)
                {
                    _file = file.FileName;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var command = new MySqlCommand("DELETE FROM categories", MySqlConnectionGenerator.ShortConnection()))
            {
                command.ExecuteNonQuery();
            }

            using (var command = new MySqlCommand("DELETE FROM diseases", MySqlConnectionGenerator.ShortConnection()))
            {
                command.ExecuteNonQuery();
            }

            var tables = textBox1.Text.Split(';');
            if (tables.Length != 2)
            {
                MessageBox.Show("Failure");
                return;
            }
            var excelApp = new Excel.Application();
            excelApp.Visible = false;

            var workbook = excelApp.Workbooks.Open(_file);
            var worksheet = (Excel.Worksheet) excelApp.ActiveSheet;
            var lastId = 0;
            for (int i = 1; i < 300; i++)
            {
                var catDe = (string) (worksheet.Cells[i, "A"] as Excel.Range).Value;
                var catEn = (string) (worksheet.Cells[i, "B"] as Excel.Range).Value;
                if (catDe != "" && catDe != null)
                {
                    using (var command = new MySqlCommand("INSERT INTO " + tables[0] + " (De, En) VALUES ('" + catDe + "', '" + catEn + "')", MySqlConnectionGenerator.ShortConnection()))
                    {
                        command.ExecuteNonQuery();
                        lastId = (int) command.LastInsertedId;
                    }
                }

                var disDe = (string) (worksheet.Cells[i, "C"] as Excel.Range).Value;
                var disEn = (string) (worksheet.Cells[i, "D"] as Excel.Range).Value;

                if (disDe != "" && disDe != null)
                {
                    using (var command = new MySqlCommand("INSERT INTO " + tables[1] + " (de, en, category_id) VALUES ('" + disDe + "', '" + disEn + "', '" + lastId + "')", MySqlConnectionGenerator.ShortConnection()))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
