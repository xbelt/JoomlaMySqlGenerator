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
        private Dictionary<string, int> _diseaseToarticleId = new Dictionary<string, int>(); 
        private Dictionary<string, int> _catsToArticleId = new Dictionary<string, int>();

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
            Config.Settings["server"]["MySQL"]["Prefix"].Value = dbPrefix.Text;
            Config.Commit();
        }

        private void dbName_TextChanged(object sender, EventArgs e)
        {
            Config.Settings["server"]["MySQL"]["Database"].Value = dbName.Text;
            Config.Commit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var window = new FillData();
            window.Show();
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (
                var command = new MySqlCommand("DELETE FROM " + dbPrefix.Text + "_jockham_reports",
                    MySqlConnectionGenerator.ShortConnection()))
            {
                command.ExecuteNonQuery();
            }

            var id = numericUpDown2.Value;
            foreach(var cat in _categories)
            {
                var names = new[] {
                    "Extrakte - " + cat,
                    "Pulver - " + cat,
                    "Pulver Getreidebasis - " + cat
                };

                var sql = new string[3];

                GenerateSql(ref sql, id);

                InsertIntoJockham(sql, names, (int) id);
                id++;
            }

            foreach (var value in _values)
            {
                var names = new[] {
                    "Extrakte - " + value.Item2,
                    "Pulver - " + value.Item2,
                    "Pulver Getreidebasis - " + value.Item2
                };

                var sql = new string[3];

                GenerateSql(ref sql, id);
                InsertIntoJockham(sql, names, (int) id);
                id++;
            }

            //Delete existing modules
            using (
                var command = new MySqlCommand("DELETE FROM " + dbPrefix.Text + "_modules WHERE id > 123",
                    MySqlConnectionGenerator.ShortConnection()))
            {
                command.ExecuteNonQuery();
            }

            //Create modules
            id = numericUpDown2.Value;
            foreach (var cat in _categories)
            {
                var names = new[] {
                    "E" + cat,
                    "P" + cat,
                    "PG" + cat
                };

                var names2 = new string[3];
                for (var i = 0; i < names.Length; i++)
                {
                    names2[i] = String.Format("{0:X}", names[i].GetHashCode());
                }

                for (int i = 0; i < names.Length; i++)
                {
                    using (
                        var command =
                            new MySqlCommand(
                                "INSERT INTO " + dbPrefix.Text +
                                "_modules(title, ordering, checked_out, published, module, access, showtitle, params, client_id, language)" +
                                "VALUES('" + names2[i] +
                                "', 1, 0, 1, 'mod_jockham_reports', 1, 1, '{\"idreport\":\"" + (3 * (id - 1) + 1 + i) + "\",\"moduleclass_sfx\":\"\",\"showreportname\":\"1\",\"showexporttoexcel\":\"0\",\"showheader\":\"1\",\"showpagination\":\"1\"}', 0, '*')",
                                MySqlConnectionGenerator.ShortConnection()))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                id++;
            }

            foreach (var value in _values)
            {
                var names = new[] {
                    "E" + value,
                    "P" + value,
                    "PG" + value
                };

                var names2 = new string[3];
                for (var i = 0; i < names.Length; i++)
                {
                    names2[i] = String.Format("{0:X}", names[i].GetHashCode());
                }

                for (int i = 0; i < names.Length; i++)
                {
                    using (
                        var command =
                            new MySqlCommand(
                                "INSERT INTO " + dbPrefix.Text +
                                "_modules(title, ordering, checked_out, published, module, access, showtitle, params, client_id, language)" +
                                "VALUES('" + names2[i] +
                                "', 1, 0, 1, 'mod_jockham_reports', 1, 1, '{\"idreport\":\"" + (3 * (id - 1) + 1 + i) + "\",\"moduleclass_sfx\":\"\",\"showreportname\":\"1\",\"showexporttoexcel\":\"0\",\"showheader\":\"1\",\"showpagination\":\"1\"}', 0, '*')",
                                MySqlConnectionGenerator.ShortConnection()))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                id++;
            }

            //DELETE existing articles
            using (
                var command = new MySqlCommand("DELETE FROM " + dbPrefix.Text + "_content WHERE id > 55",
                    MySqlConnectionGenerator.ShortConnection()))
            {
                command.ExecuteNonQuery();
            }

            using (
                var command = new MySqlCommand("DELETE FROM " + dbPrefix.Text + "_assets WHERE id > 110", MySqlConnectionGenerator.ShortConnection()))
            {
                command.ExecuteNonQuery();
            }

            //INSERT articles
            //We need to start with the specific diseases so we know the id 
            int parent_id = 27;
            int lft = 53;
            int level = 3;
            int startNameId = 57;

            int lastid;
            foreach (var value in _values)
            {
                var names = new[] {
                    "E" + value,
                    "P" + value,
                    "PG" + value
                };

                var names2 = new string[3];
                for (var i = 0; i < names.Length; i++)
                {
                    names2[i] = String.Format("{0:X}", names[i].GetHashCode());
                }

                using (
                    var command =
                        new MySqlCommand(
                            "INSERT INTO " + dbPrefix.Text + "_assets(parent_id, lft, rgt, level, name, title, rules) " +
                            "VALUES(" + parent_id + ", " + lft++ + ", " + lft++ + ", " + level +
                            ", 'com_content.article" + startNameId++ + "', '" + value.Item2 + "', '" +
                            "{\"core.delete\":{\"6\":1},\"core.edit\":{\"6\":1,\"4\":1},\"core.edit.state\":{\"6\":1,\"5\":1}}" +
                            "')", MySqlConnectionGenerator.ShortConnection()))
                {
                    command.ExecuteNonQuery();
                    lastid = (int) command.LastInsertedId;
                }

                using (var command = new MySqlCommand(
                    "INSERT INTO " + dbPrefix.Text +
                    "_content(`asset_id`, `title`, `alias`, `title_alias`, `introtext`, `fulltext`, `state`, `sectionid`, `mask`, `catid`, `created`, `created_by`, `created_by_alias`, `modified`, `modified_by`, `checked_out`, `checked_out_time`, `publish_up`, `publish_down`, `images`, `urls`, `attribs`, `version`, `parentid`, `ordering`, `metakey`, `metadesc`, `access`, `hits`, `metadata`, `featured`, `language`, `xreference`)" +
                    "VALUES(" + lastid + ", '" + value.Item2 + "', '" +
                    String.Format("{0:X}", value.Item2.GetHashCode()) +
                    "', '', '<p>Hier finden Sie die Resultate zur Krankheit " + value.Item2 + ".</p>\\n<p>{module " +
                    names2[0] + "}</p>\\n<p>{module " + names2[1] + "}</p>\\n<p>{module " + names2[2] + "}</p>\\n" +
                    "', '', 1, 0, 0, 2, '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") +
                    "', 84, '', '0000-00-00 00:00:00', 0, 0, '0000-00-00 00:00:00', '" +
                    DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "', '0000-00-00 00:00:00', '" +
                    "{\"image_intro\":\"\",\"float_intro\":\"\",\"image_intro_alt\":\"\",\"image_intro_caption\":\"\",\"image_fulltext\":\"\",\"float_fulltext\":\"\",\"image_fulltext_alt\":\"\",\"image_fulltext_caption\":\"\"}" +
                    "', '" +
                    "{\"urla\":null,\"urlatext\":\"\",\"targeta\":\"\",\"urlb\":null,\"urlbtext\":\"\",\"targetb\":\"\",\"urlc\":null,\"urlctext\":\"\",\"targetc\":\"\"}" +
                    "', '" +
                    "{\"show_title\":\"\",\"link_titles\":\"\",\"show_intro\":\"\",\"show_category\":\"\",\"link_category\":\"\",\"show_parent_category\":\"\",\"link_parent_category\":\"\",\"show_author\":\"\",\"link_author\":\"\",\"show_create_date\":\"\",\"show_modify_date\":\"\",\"show_publish_date\":\"\",\"show_item_navigation\":\"\",\"show_icons\":\"\",\"show_print_icon\":\"\",\"show_email_icon\":\"\",\"show_vote\":\"\",\"show_hits\":\"\",\"show_noauth\":\"\",\"urls_position\":\"\",\"alternative_readmore\":\"\",\"article_layout\":\"\",\"show_publishing_options\":\"\",\"show_article_options\":\"\",\"show_urls_images_backend\":\"\",\"show_urls_images_frontend\":\"\"}" +
                    "', 1,0,0,'','',1, 1, '" + "{\"robots\":\"\",\"author\":\"\",\"rights\":\"\",\"xreference\":\"\"}" +
                    "', 0, '*', '')", MySqlConnectionGenerator.ShortConnection()))
                {
                    command.ExecuteNonQuery();
                    if (!_diseaseToarticleId.ContainsKey(value.Item2))
                        _diseaseToarticleId.Add(value.Item2, (int) command.LastInsertedId);
                }
            }

            foreach (var category in _categories)
            {
                var names = new[] {
                    "E" + category,
                    "P" + category,
                    "PG" + category
                };

                var names2 = new string[3];
                for (var i = 0; i < names.Length; i++)
                {
                    names2[i] = String.Format("{0:X}", names[i].GetHashCode());
                }

                using (
                    var command =
                        new MySqlCommand(
                            "INSERT INTO " + dbPrefix.Text + "_assets(parent_id, lft, rgt, level, name, title, rules) " +
                            "VALUES(" + parent_id + ", " + lft++ + ", " + lft++ + ", " + level +
                            ", 'com_content.article" + startNameId++ + "', '" + category + "', '" +
                            "{\"core.delete\":{\"6\":1},\"core.edit\":{\"6\":1,\"4\":1},\"core.edit.state\":{\"6\":1,\"5\":1}}" +
                            "')", MySqlConnectionGenerator.ShortConnection()))
                {
                    command.ExecuteNonQuery();
                    lastid = (int)command.LastInsertedId;
                }

                using (var command = new MySqlCommand(
                    "INSERT INTO " + dbPrefix.Text +
                    "_content(`asset_id`, `title`, `alias`, `title_alias`, `introtext`, `fulltext`, `state`, `sectionid`, `mask`, `catid`, `created`, `created_by`, `created_by_alias`, `modified`, `modified_by`, `checked_out`, `checked_out_time`, `publish_up`, `publish_down`, `images`, `urls`, `attribs`, `version`, `parentid`, `ordering`, `metakey`, `metadesc`, `access`, `hits`, `metadata`, `featured`, `language`, `xreference`)" +
                    "VALUES(" + lastid + ", '" + category + "', '" +
                    String.Format("{0:X}", category.GetHashCode()) +
                    "', '', '<p>Hier finden Sie die zusammengefasste Auswertung aller " + category + ". Nach der Auswertung finden Sie die Links zu den Auswertungen der specifischen Erkrankungen</p>\\n<p>{module " +
                    names2[0] + "}</p>\\n<p>{module " + names2[1] + "}</p>\\n<p>{module " + names2[2] + "}</p>\\n" + GenerateLinkString(category) +
                    "', '', 1, 0, 0, 2, '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") +
                    "', 84, '', '0000-00-00 00:00:00', 0, 0, '0000-00-00 00:00:00', '" +
                    DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "', '0000-00-00 00:00:00', '" +
                    "{\"image_intro\":\"\",\"float_intro\":\"\",\"image_intro_alt\":\"\",\"image_intro_caption\":\"\",\"image_fulltext\":\"\",\"float_fulltext\":\"\",\"image_fulltext_alt\":\"\",\"image_fulltext_caption\":\"\"}" +
                    "', '" +
                    "{\"urla\":null,\"urlatext\":\"\",\"targeta\":\"\",\"urlb\":null,\"urlbtext\":\"\",\"targetb\":\"\",\"urlc\":null,\"urlctext\":\"\",\"targetc\":\"\"}" +
                    "', '" +
                    "{\"show_title\":\"\",\"link_titles\":\"\",\"show_intro\":\"\",\"show_category\":\"\",\"link_category\":\"\",\"show_parent_category\":\"\",\"link_parent_category\":\"\",\"show_author\":\"\",\"link_author\":\"\",\"show_create_date\":\"\",\"show_modify_date\":\"\",\"show_publish_date\":\"\",\"show_item_navigation\":\"\",\"show_icons\":\"\",\"show_print_icon\":\"\",\"show_email_icon\":\"\",\"show_vote\":\"\",\"show_hits\":\"\",\"show_noauth\":\"\",\"urls_position\":\"\",\"alternative_readmore\":\"\",\"article_layout\":\"\",\"show_publishing_options\":\"\",\"show_article_options\":\"\",\"show_urls_images_backend\":\"\",\"show_urls_images_frontend\":\"\"}" +
                    "', 1,0,0,'','',1, 1, '" + "{\"robots\":\"\",\"author\":\"\",\"rights\":\"\",\"xreference\":\"\"}" +
                    "', 0, '*', '')", MySqlConnectionGenerator.ShortConnection()))
                {
                    command.ExecuteNonQuery();
                    if (!_catsToArticleId.ContainsKey(category))
                    {
                        _catsToArticleId.Add(category, (int) command.LastInsertedId);
                    }
                }
            }

            //create main page
            using (
                var command =
                    new MySqlCommand(
                        "INSERT INTO " + dbPrefix.Text + "_assets(parent_id, lft, rgt, level, name, title, rules) " +
                        "VALUES(" + parent_id + ", " + lft++ + ", " + lft++ + ", " + level +
                        ", 'com_content.article" + startNameId++ + "', '" + "Auswertung" + "', '" +
                        "{\"core.delete\":{\"6\":1},\"core.edit\":{\"6\":1,\"4\":1},\"core.edit.state\":{\"6\":1,\"5\":1}}" +
                        "')", MySqlConnectionGenerator.ShortConnection()))
            {
                command.ExecuteNonQuery();
                lastid = (int)command.LastInsertedId;
            }

            using (var command = new MySqlCommand(
                    "INSERT INTO " + dbPrefix.Text +
                    "_content(`asset_id`, `title`, `alias`, `title_alias`, `introtext`, `fulltext`, `state`, `sectionid`, `mask`, `catid`, `created`, `created_by`, `created_by_alias`, `modified`, `modified_by`, `checked_out`, `checked_out_time`, `publish_up`, `publish_down`, `images`, `urls`, `attribs`, `version`, `parentid`, `ordering`, `metakey`, `metadesc`, `access`, `hits`, `metadata`, `featured`, `language`, `xreference`)" +
                    "VALUES(" + lastid + ", '" + "Auswertung" + "', '" +
                    String.Format("{0:X}", "Auswertung".GetHashCode()) +
                    "', '', '<p>Um die Auswertung einer bestimmten Krankheit zu betrachten bitte wählen Sie zuerst die dazu gehörige Kategorie:</p>\\n" + GenerateMainPageLinks() +
                    "', '', 1, 0, 0, 2, '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") +
                    "', 84, '', '0000-00-00 00:00:00', 0, 0, '0000-00-00 00:00:00', '" +
                    DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "', '0000-00-00 00:00:00', '" +
                    "{\"image_intro\":\"\",\"float_intro\":\"\",\"image_intro_alt\":\"\",\"image_intro_caption\":\"\",\"image_fulltext\":\"\",\"float_fulltext\":\"\",\"image_fulltext_alt\":\"\",\"image_fulltext_caption\":\"\"}" +
                    "', '" +
                    "{\"urla\":null,\"urlatext\":\"\",\"targeta\":\"\",\"urlb\":null,\"urlbtext\":\"\",\"targetb\":\"\",\"urlc\":null,\"urlctext\":\"\",\"targetc\":\"\"}" +
                    "', '" +
                    "{\"show_title\":\"\",\"link_titles\":\"\",\"show_intro\":\"\",\"show_category\":\"\",\"link_category\":\"\",\"show_parent_category\":\"\",\"link_parent_category\":\"\",\"show_author\":\"\",\"link_author\":\"\",\"show_create_date\":\"\",\"show_modify_date\":\"\",\"show_publish_date\":\"\",\"show_item_navigation\":\"\",\"show_icons\":\"\",\"show_print_icon\":\"\",\"show_email_icon\":\"\",\"show_vote\":\"\",\"show_hits\":\"\",\"show_noauth\":\"\",\"urls_position\":\"\",\"alternative_readmore\":\"\",\"article_layout\":\"\",\"show_publishing_options\":\"\",\"show_article_options\":\"\",\"show_urls_images_backend\":\"\",\"show_urls_images_frontend\":\"\"}" +
                    "', 1,0,0,'','',1, 1, '" + "{\"robots\":\"\",\"author\":\"\",\"rights\":\"\",\"xreference\":\"\"}" +
                    "', 0, '*', '')", MySqlConnectionGenerator.ShortConnection()))
            {
                command.ExecuteNonQuery();
            }

            MessageBox.Show("Finished");
        }

        private string GenerateMainPageLinks()
        {
            var result = "";
            foreach (var cat in _catsToArticleId)
            {
                result += "<p>-&nbsp;<a href=\"index.php?option=com_content&amp;view=article&amp;id=" +
                                cat.Value + "&amp;catid=2\">" + cat.Key + " </a></p>\\n";
            }
            return result;
        }

        private string GenerateLinkString(string category)
        {
            var result = new List<string>();
            var catId = 1;
            using (
                var reader = (new MySqlCommand("SELECT id FROM categories WHERE DE = '" + category + "'",
                    MySqlConnectionGenerator.ShortConnection())).ExecuteReader())
            {
                while (reader.Read())
                {
                    catId = reader.GetInt32Save(0);
                }
            }

            using (var reader = (new MySqlCommand("SELECT de FROM diseases WHERE category_id = " + catId, MySqlConnectionGenerator.ShortConnection())).ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(reader.GetStringSave(0));
                }
            }

            var resultString = "";
            foreach (var res in result)
            {
                resultString += "<p>-&nbsp;<a href=\"index.php?option=com_content&amp;view=article&amp;id=" +
                                _diseaseToarticleId[res] + "&amp;catid=2\">" + res + " </a></p>\\n";
            }
            return resultString;
            //<p>-&nbsp;<a href=\"index.php?option=com_content&amp;view=article&amp;id=
        }

        private void InsertIntoJockham(string[] sql, string[] names, int id)
        {
            for (int i = 0; i < sql.Length; i++)
            {
                sql[i] = Base64Encode(sql[i]);

                using (
                    var command =
                        new MySqlCommand("INSERT INTO " + dbPrefix.Text +
                                         "_jockham_reports (id, name, sqlquery, description, creation) VALUES(" + (3*(id - 1) + 1 + i) + ", '" +
                                         names[i] + "', '" + sql[i] + "', 'generated', '" +
                                         DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "')", MySqlConnectionGenerator.ShortConnection()))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private void GenerateSql(ref string[] sql, decimal id)
        {
            for (int i = (int) numericUpDown1.Value; i < numericUpDown1.Value + 3; i++)
            {
                sql[(int) (i - numericUpDown1.Value)] =
                    "SELECT Pilzart, FORMAT(AVG(data.a),1) AS 'Arithmetisches Mittel', data.b AS 'Gesammtzahl der Teilnehmer'  FROM " +
                    "(" +
                    "SELECT f.ftext AS Pilzart, AVG(fs.ordering + 1) AS a, COUNT(DISTINCT(ans.start_id)) AS b " +
                    "FROM y1trf_survey_force_user_answers AS ans " +
                    "LEFT JOIN y1trf_survey_force_scales AS fs ON ans.ans_field = fs.id " +
                    "JOIN y1trf_survey_force_fields AS f ON ans.answer = f.id " +
                    "WHERE ans.start_id IN " +
                    "( SELECT ans.start_id FROM y1trf_survey_force_user_answers AS ans WHERE ans.answer = " + i +
                    "  ) " +
                    "AND ans.start_id IN " +
                    "( SELECT ans.start_id FROM y1trf_survey_force_user_answers AS ans WHERE ans.answer = " + id +
                    "  ) " +
                    "AND (ans.quest_id = 32 OR ans.quest_id = 37) " +
                    "GROUP BY ans.start_id " +
                    "ORDER BY AVG(fs.ordering + 1) DESC " +
                    ") AS data " +
                    "GROUP BY Pilzart " +
                    "ORDER BY AVG('Arithmetisches Mittel') DESC; ";
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
