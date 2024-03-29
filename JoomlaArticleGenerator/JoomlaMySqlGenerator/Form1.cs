﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Warenlager.API;
using Warenlager.Windows;

namespace JoomlaMySqlGenerator
{
    public partial class Form1 : Form
    {
        private readonly List<string> _categories = new List<string>();
        private readonly List<string> _categoriesEn = new List<string>(); 
        private readonly List<Tuple<int, string>> _values = new List<Tuple<int, string>>();
        private readonly List<Tuple<int, string>> _valuesEn = new List<Tuple<int, string>>(); 
        private readonly List<string> _species = new List<string>();
        private readonly List<string> _speciesEn = new List<string>();
        private readonly Dictionary<string, int> _diseaseToarticleId = new Dictionary<string, int>(); 
        private readonly Dictionary<string, int> _enDiseaseToarticleId = new Dictionary<string, int>(); 
        private readonly Dictionary<string, int> _speciesToarticleId = new Dictionary<string, int>(); 
        private readonly Dictionary<string, int> _enSpeciesToarticleId = new Dictionary<string, int>(); 
        private readonly Dictionary<string, int> _catsToArticleId = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _enCatsToArticleId = new Dictionary<string, int>();
        private readonly HashSet<string> _hashes = new HashSet<string>();
        private decimal _finalId;
        private int _parentId = 27;
        private int _lft = 53;
        private int _level = 3;
        private int _startNameId = 57;

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
            using (
                var reader =
                    (new MySqlCommand("SELECT * FROM categories ORDER BY id ASC",
                        MySqlConnectionGenerator.ShortConnection())).ExecuteReader())
            {
                _categories.Clear();
                _categoriesEn.Clear();
                while (reader.Read())
                {
                    _categories.Add(reader.GetStringSave("de"));
                    _categoriesEn.Add(reader.GetStringSave("en"));
                }
            }

            using (
                var reader =
                    (new MySqlCommand("SELECT * FROM diseases ORDER BY category_id ASC, id ASC",
                        MySqlConnectionGenerator.ShortConnection())).ExecuteReader())
            {
                _values.Clear();
                _valuesEn.Clear();
                while (reader.Read())
                {
                    _values.Add(new Tuple<int, string>(reader.GetInt32Save("category_id"), reader.GetStringSave("de")));
                    _valuesEn.Add(new Tuple<int, string>(reader.GetInt32Save("category_id"),
                        reader.GetStringSave("en").Replace("'", "\\'")));
                }
            }

            using (
                var reader =
                    (new MySqlCommand("SELECT * FROM species ORDER BY id ASC",
                        MySqlConnectionGenerator.ShortConnection())).ExecuteReader())
            {
                _species.Clear();
                _speciesEn.Clear();
                while (reader.Read())
                {
                    _species.Add(reader.GetStringSave("de"));
                    _speciesEn.Add(reader.GetStringSave("en"));
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

        private void button4_Click(object sender, EventArgs e)
        {
            GenerateByDisease();
            GenerateBySpecies();

            MessageBox.Show(@"Finished");
        }

        private void GenerateBySpecies()
        {
            var id = numericUpDown3.Value;
            var progressForm = new ProgressForm(_species.Count, "Inserting data", "Inserting species queries");
            progressForm.Show();
            var id2 = _finalId;
            InsertSpeciesQueries(id, progressForm, ref id2);

            progressForm.Reset(_speciesEn.Count, "Inserting En species queries");
            InsertSpeciesQueriesEn(ref id, progressForm, ref id2);

            progressForm.Reset(_species.Count * 3, "Insert species modules");
            id2 = _finalId;
            InsertSpeciesModules(ref id2, progressForm);

            progressForm.Reset(_speciesEn.Count * 3, "Insert en species modules");
            InsertEnSpeciesModules(ref id2, progressForm);

            progressForm.Reset(_species.Count, "Insert species articles");
            InsertSpeciesArticles(ref _parentId, ref _lft, ref _level, ref _startNameId, progressForm);

            progressForm.Reset(_speciesEn.Count, "Insert en species articles");
            InsertEnSpeciesArticles(ref _parentId, ref _lft, ref _level, ref _startNameId, progressForm);

            CreateSpeciesMainPage(ref _parentId, ref _lft, ref _level, ref _startNameId);
            CreateSpeciesMainPageEn(ref _parentId, ref _lft, ref _level, ref _startNameId);

            progressForm.Close();
        }

        private void CreateSpeciesMainPageEn(ref int parentId, ref int lft, ref int level, ref int startNameId)
        {
            int lastid;
            using (
                var command =
                    new MySqlCommand(
                        "INSERT INTO " + dbPrefix.Text + "_assets(parent_id, lft, rgt, level, name, title, rules) " +
                        "VALUES(" + parentId + ", " + lft++ + ", " + lft++ + ", " + level +
                        ", 'com_content.article" + startNameId++ + "', '" + "Reports nach Pilzart" + "', '" +
                        "{\"core.delete\":{\"6\":1},\"core.edit\":{\"6\":1,\"4\":1},\"core.edit.state\":{\"6\":1,\"5\":1}}" +
                        "')", MySqlConnectionGenerator.ShortConnection()))
            {
                command.ExecuteNonQuery();
                lastid = (int)command.LastInsertedId;
            }

            using (var command = new MySqlCommand(
                "INSERT INTO " + dbPrefix.Text +
                "_content(`asset_id`, `title`, `alias`, `title_alias`, `introtext`, `fulltext`, `state`, `sectionid`, `mask`, `catid`, `created`, `created_by`, `created_by_alias`, `modified`, `modified_by`, `checked_out`, `checked_out_time`, `publish_up`, `publish_down`, `images`, `urls`, `attribs`, `version`, `parentid`, `ordering`, `metakey`, `metadesc`, `access`, `hits`, `metadata`, `featured`, `language`, `xreference`)" +
                "VALUES(" + lastid + ", '" + "Evaluation" + "', '" +
                String.Format("{0:X}", "Reports nach Pilzart".GetHashCode()) +
                "', '', '<p>In order to study the effects of treatments with a particular mushroom, please select below the corresponding category first:</p>\\n" +
                GenerateMainPageEnSpeciesLinks() +
                "', '', 1, 0, 0, 2, '" + DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd hh:mm:ss") +
                "', 84, '', '0000-00-00 00:00:00', 0, 0, '0000-00-00 00:00:00', '" +
                DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd hh:mm:ss") + "', '0000-00-00 00:00:00', '" +
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
        }

        private void CreateSpeciesMainPage(ref int parentId, ref int lft, ref int level, ref int startNameId)
        {
            int lastid;
            using (
                var command =
                    new MySqlCommand(
                        "INSERT INTO " + dbPrefix.Text + "_assets(parent_id, lft, rgt, level, name, title, rules) " +
                        "VALUES(" + parentId + ", " + lft++ + ", " + lft++ + ", " + level +
                        ", 'com_content.article" + startNameId++ + "', '" + "Auswertung nach Pilzart" + "', '" +
                        "{\"core.delete\":{\"6\":1},\"core.edit\":{\"6\":1,\"4\":1},\"core.edit.state\":{\"6\":1,\"5\":1}}" +
                        "')", MySqlConnectionGenerator.ShortConnection()))
            {
                command.ExecuteNonQuery();
                lastid = (int)command.LastInsertedId;
            }

            using (var command = new MySqlCommand(
                "INSERT INTO " + dbPrefix.Text +
                "_content(`asset_id`, `title`, `alias`, `title_alias`, `introtext`, `fulltext`, `state`, `sectionid`, `mask`, `catid`, `created`, `created_by`, `created_by_alias`, `modified`, `modified_by`, `checked_out`, `checked_out_time`, `publish_up`, `publish_down`, `images`, `urls`, `attribs`, `version`, `parentid`, `ordering`, `metakey`, `metadesc`, `access`, `hits`, `metadata`, `featured`, `language`, `xreference`)" +
                "VALUES(" + lastid + ", '" + "Auswertung nach Pilzart" + "', '" +
                String.Format("{0:X}", "Auswertung nach Pilzart".GetHashCode()) +
                "', '', '<p>Um die Behandlungserfolge bei einer bestimmten Pilzart zu betrachten, wählen Sie bitte zuerst unten die dazu gehörige Kategorie aus:</p>\\n" +
                GenerateMainPageSpeciesLinks() +
                "', '', 1, 0, 0, 2, '" + DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd hh:mm:ss") +
                "', 84, '', '0000-00-00 00:00:00', 0, 0, '0000-00-00 00:00:00', '" +
                DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd hh:mm:ss") + "', '0000-00-00 00:00:00', '" +
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
        }

        private string GenerateMainPageSpeciesLinks()
        {
            return _speciesToarticleId.Aggregate("", (current, cat) => current + ("<p>-&nbsp;<a href=\"index.php?option=com_content&amp;view=article&amp;id=" + cat.Value + "&amp;catid=2\">" + cat.Key + " </a></p>\\n"));
        }

        private string GenerateMainPageEnSpeciesLinks()
        {
            return _enSpeciesToarticleId.Aggregate("", (current, cat) => current + ("<p>-&nbsp;<a href=\"index.php?option=com_content&amp;view=article&amp;id=" + cat.Value + "&amp;catid=2\">" + cat.Key + " </a></p>\\n"));
        }

        private void InsertEnSpeciesArticles(ref int parentId, ref int lft, ref int level, ref int startNameId, ProgressForm progressForm)
        {
            foreach (var value in _species)
            {
                var names = new[]
                {
                    "ENE" + value,
                    "ENP" + value,
                    "ENPG" + value
                };

                var names2 = new string[3];
                for (var i = 0; i < names.Length; i++)
                {
                    names2[i] = String.Format("{0:X}", names[i].GetHashCode());
                }

                int lastid;
                using (
                    var command =
                        new MySqlCommand(
                            "INSERT INTO " + dbPrefix.Text + "_assets(parent_id, lft, rgt, level, name, title, rules) " +
                            "VALUES(" + parentId + ", " + lft++ + ", " + lft++ + ", " + level +
                            ", 'com_content.article" + startNameId++ + "', '" + value + "', '" +
                            "{\"core.delete\":{\"6\":1},\"core.edit\":{\"6\":1,\"4\":1},\"core.edit.state\":{\"6\":1,\"5\":1}}" +
                            "')", MySqlConnectionGenerator.ShortConnection()))
                {
                    command.ExecuteNonQuery();
                    lastid = (int)command.LastInsertedId;
                }

                using (var command = new MySqlCommand(
                    "INSERT INTO " + dbPrefix.Text +
                    "_content(`asset_id`, `title`, `alias`, `title_alias`, `introtext`, `fulltext`, `state`, `sectionid`, `mask`, `catid`, `created`, `created_by`, `created_by_alias`, `modified`, `modified_by`, `checked_out`, `checked_out_time`, `publish_up`, `publish_down`, `images`, `urls`, `attribs`, `version`, `parentid`, `ordering`, `metakey`, `metadesc`, `access`, `hits`, `metadata`, `featured`, `language`, `xreference`)" +
                    "VALUES(" + lastid + ", '" + value + "', '" +
                    String.Format("{0:X}", value.GetHashCode()) +
                    "', '', '<p>Results of treatment with " + value + ".</p>\\n<p>{module " +
                    names2[0] + "}</p>\\n<p>{module " + names2[1] + "}</p>\\n<p>{module " + names2[2] + "}</p>\\n" +
                    "', '', 1, 0, 0, 2, '" + DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd hh:mm:ss") +
                    "', 84, '', '0000-00-00 00:00:00', 0, 0, '0000-00-00 00:00:00', '" +
                    DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd hh:mm:ss") + "', '0000-00-00 00:00:00', '" +
                    "{\"image_intro\":\"\",\"float_intro\":\"\",\"image_intro_alt\":\"\",\"image_intro_caption\":\"\",\"image_fulltext\":\"\",\"float_fulltext\":\"\",\"image_fulltext_alt\":\"\",\"image_fulltext_caption\":\"\"}" +
                    "', '" +
                    "{\"urla\":null,\"urlatext\":\"\",\"targeta\":\"\",\"urlb\":null,\"urlbtext\":\"\",\"targetb\":\"\",\"urlc\":null,\"urlctext\":\"\",\"targetc\":\"\"}" +
                    "', '" +
                    "{\"show_title\":\"\",\"link_titles\":\"\",\"show_intro\":\"\",\"show_category\":\"\",\"link_category\":\"\",\"show_parent_category\":\"\",\"link_parent_category\":\"\",\"show_author\":\"\",\"link_author\":\"\",\"show_create_date\":\"\",\"show_modify_date\":\"\",\"show_publish_date\":\"\",\"show_item_navigation\":\"\",\"show_icons\":\"\",\"show_print_icon\":\"\",\"show_email_icon\":\"\",\"show_vote\":\"\",\"show_hits\":\"\",\"show_noauth\":\"\",\"urls_position\":\"\",\"alternative_readmore\":\"\",\"article_layout\":\"\",\"show_publishing_options\":\"\",\"show_article_options\":\"\",\"show_urls_images_backend\":\"\",\"show_urls_images_frontend\":\"\"}" +
                    "', 1,0,0,'','',1, 1, '" + "{\"robots\":\"\",\"author\":\"\",\"rights\":\"\",\"xreference\":\"\"}" +
                    "', 0, '*', '')", MySqlConnectionGenerator.ShortConnection()))
                {
                    command.ExecuteNonQuery();
                    if (!_enSpeciesToarticleId.ContainsKey(value))
                        _enSpeciesToarticleId.Add(value, (int)command.LastInsertedId);
                }

                progressForm.PerformStep();
            }
        }

        private void InsertSpeciesArticles(ref int parentId, ref int lft, ref int level, ref int startNameId, ProgressForm progressForm)
        {
            foreach (var value in _species)
            {
                var names = new[]
                {
                    "E" + value,
                    "P" + value,
                    "PG" + value
                };

                var names2 = new string[3];
                for (var i = 0; i < names.Length; i++)
                {
                    names2[i] = String.Format("{0:X}", names[i].GetHashCode());
                }

                int lastid;
                using (
                    var command =
                        new MySqlCommand(
                            "INSERT INTO " + dbPrefix.Text + "_assets(parent_id, lft, rgt, level, name, title, rules) " +
                            "VALUES(" + parentId + ", " + lft++ + ", " + lft++ + ", " + level +
                            ", 'com_content.article" + startNameId++ + "', '" + value + "', '" +
                            "{\"core.delete\":{\"6\":1},\"core.edit\":{\"6\":1,\"4\":1},\"core.edit.state\":{\"6\":1,\"5\":1}}" +
                            "')", MySqlConnectionGenerator.ShortConnection()))
                {
                    command.ExecuteNonQuery();
                    lastid = (int)command.LastInsertedId;
                }

                using (var command = new MySqlCommand(
                    "INSERT INTO " + dbPrefix.Text +
                    "_content(`asset_id`, `title`, `alias`, `title_alias`, `introtext`, `fulltext`, `state`, `sectionid`, `mask`, `catid`, `created`, `created_by`, `created_by_alias`, `modified`, `modified_by`, `checked_out`, `checked_out_time`, `publish_up`, `publish_down`, `images`, `urls`, `attribs`, `version`, `parentid`, `ordering`, `metakey`, `metadesc`, `access`, `hits`, `metadata`, `featured`, `language`, `xreference`)" +
                    "VALUES(" + lastid + ", '" + value + "', '" +
                    String.Format("{0:X}", value.GetHashCode()) +
                    "', '', '<p>Behandlungserfolg mit " + value + ".</p>\\n<p>{module " +
                    names2[0] + "}</p>\\n<p>{module " + names2[1] + "}</p>\\n<p>{module " + names2[2] + "}</p>\\n" +
                    "', '', 1, 0, 0, 2, '" + DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd hh:mm:ss") +
                    "', 84, '', '0000-00-00 00:00:00', 0, 0, '0000-00-00 00:00:00', '" +
                    DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd hh:mm:ss") + "', '0000-00-00 00:00:00', '" +
                    "{\"image_intro\":\"\",\"float_intro\":\"\",\"image_intro_alt\":\"\",\"image_intro_caption\":\"\",\"image_fulltext\":\"\",\"float_fulltext\":\"\",\"image_fulltext_alt\":\"\",\"image_fulltext_caption\":\"\"}" +
                    "', '" +
                    "{\"urla\":null,\"urlatext\":\"\",\"targeta\":\"\",\"urlb\":null,\"urlbtext\":\"\",\"targetb\":\"\",\"urlc\":null,\"urlctext\":\"\",\"targetc\":\"\"}" +
                    "', '" +
                    "{\"show_title\":\"\",\"link_titles\":\"\",\"show_intro\":\"\",\"show_category\":\"\",\"link_category\":\"\",\"show_parent_category\":\"\",\"link_parent_category\":\"\",\"show_author\":\"\",\"link_author\":\"\",\"show_create_date\":\"\",\"show_modify_date\":\"\",\"show_publish_date\":\"\",\"show_item_navigation\":\"\",\"show_icons\":\"\",\"show_print_icon\":\"\",\"show_email_icon\":\"\",\"show_vote\":\"\",\"show_hits\":\"\",\"show_noauth\":\"\",\"urls_position\":\"\",\"alternative_readmore\":\"\",\"article_layout\":\"\",\"show_publishing_options\":\"\",\"show_article_options\":\"\",\"show_urls_images_backend\":\"\",\"show_urls_images_frontend\":\"\"}" +
                    "', 1,0,0,'','',1, 1, '" + "{\"robots\":\"\",\"author\":\"\",\"rights\":\"\",\"xreference\":\"\"}" +
                    "', 0, '*', '')", MySqlConnectionGenerator.ShortConnection()))
                {
                    command.ExecuteNonQuery();
                    if (!_speciesToarticleId.ContainsKey(value))
                        _speciesToarticleId.Add(value, (int)command.LastInsertedId);
                }

                progressForm.PerformStep();
            }
        }

        private void InsertEnSpeciesModules(ref decimal id, ProgressForm progressForm)
        {
            foreach (var value in _speciesEn)
            {
                var names = new[]
                {
                    "ENE" + value,
                    "ENP" + value,
                    "ENPG" + value
                };

                var names2 = new string[3];
                for (var i = 0; i < names.Length; i++)
                {
                    names2[i] = String.Format("{0:X}", names[i].GetHashCode());
                }

                InsertModules(names, names2, id, progressForm);
                id++;
            }
        }

        private void InsertSpeciesModules(ref decimal id, ProgressForm progressForm)
        {
            foreach (var value in _species)
            {
                var names = new[]
                {
                    "E" + value,
                    "P" + value,
                    "PG" + value
                };

                var names2 = new string[3];
                for (var i = 0; i < names.Length; i++)
                {
                    names2[i] = String.Format("{0:X}", names[i].GetHashCode());
                    if (!_hashes.Contains(names2[i]))
                    {
                        _hashes.Add(names2[i]);
                    }
                    else
                    {
                        MessageBox.Show(@"Whyyy");
                    }
                }

                InsertModules(names, names2, id, progressForm);
                id++;
            }
        }

        private void InsertSpeciesQueriesEn(ref decimal id, ProgressForm progressForm, ref decimal id2)
        {
            foreach (var spec in _speciesEn)
            {
                var names2 = new[]
                {
                    "Extracts",
                    "Powders",
                    "Powders on cereal basis"
                };

                var sql = new string[3];

                GenerateSqlSpeciesEn(ref sql, id);

                InsertIntoJockham(sql, names2, (int)id2);
                progressForm.PerformStep();
                id++;
                id2++;
            }
        }

        private void GenerateSqlSpeciesEn(ref string[] sql, decimal id)
        {
            for (var i = (int)numericUpDown1.Value; i < numericUpDown1.Value + 3; i++)
            {
                sql[(int) (i - numericUpDown1.Value)] = GenerateSql("Disease", "Arithmetic mean",
                    "Number of participants", i, id);
            }
        }

        private void InsertSpeciesQueries(decimal id, ProgressForm progressForm, ref decimal id2)
        {
            //is is wrong need to continue from previous
            foreach (var spec in _species)
            {
                var names2 = new[]
                {
                    "Extrakte",
                    "Pulver",
                    "Pulver Getreidebasis"
                };

                var sql = new string[3];

                GenerateSqlSpecies(ref sql, id);

                InsertIntoJockham(sql, names2, (int)id2);
                progressForm.PerformStep();
                id++;
                id2++;
            }
        }

        private void GenerateByDisease()
        {
            DeleteSqls();

            var id = numericUpDown2.Value;
            var progressForm = new ProgressForm(_categories.Count, "Inserting data", "Inserting categories queries");
            progressForm.Show();
            InsertCategoryQueries(ref id, progressForm);

            var id2 = id;
            id = numericUpDown2.Value;
            progressForm.Reset(_categoriesEn.Count, "Inserting English categories queries");
            InsertEnCategoriesQueries(ref id, progressForm, ref id2);

            progressForm.Reset(_values.Count, "Inserting disease queries");
            InsertDiseaseQueries(id, progressForm, ref id2);

            progressForm.Reset(_valuesEn.Count, "Inserting English disease queries");
            InsertEnDiseaseQueries(ref id, progressForm, ref id2);

            _finalId = id2;

            //Delete existing modules
            DeleteModules();

            //Create modules
            id = numericUpDown2.Value;
            progressForm.Reset(_categories.Count*3, "Inserting category modules");
            InsertCategoryModule(ref id, progressForm);

            progressForm.Reset(_categoriesEn.Count*3, "Inserting English category modules");
            InsertEnCategoryModules(ref id, progressForm);

            progressForm.Reset(_values.Count*3, "Inserting disease modules");
            InsertDiseaseModule(ref id, progressForm);

            progressForm.Reset(_valuesEn.Count*3, "Inserting English disease modules");
            InsertEnDiseaseModule(id, progressForm);

            //DELETE existing articles
            DeleteArticles();

            //INSERT articles
            //We need to start with the specific diseases so we know the id 

            progressForm.Reset(_values.Count, "Inserting disease articles");
            InsertDiseaseArticles(ref _parentId, ref _lft, ref _level, ref _startNameId, progressForm);

            progressForm.Reset(_valuesEn.Count, "Inserting English disease articles");
            InsertEnDiseaseArticles(ref _parentId, ref _lft, ref _level, ref _startNameId, progressForm);

            progressForm.Reset(_categories.Count, "Inserting category articles");
            InsertCategoryArticles(ref _parentId, ref _lft, ref _level, ref _startNameId, progressForm);

            progressForm.Reset(_categoriesEn.Count, "Inserting English category articles");
            InsertEnCategoryArticles(ref _parentId, ref _lft, ref _level, ref _startNameId, progressForm);

            //create main page
            CreateMainPage(ref _parentId, ref _lft, ref _level, ref _startNameId);
            CreateEnMainPage(ref _parentId, ref _lft, ref _level, ref _startNameId);

            progressForm.Close();
        }

        private void CreateEnMainPage(ref int parentId, ref int lft, ref int level, ref int startNameId)
        {
            int lastid;
            using (
                var command =
                    new MySqlCommand(
                        "INSERT INTO " + dbPrefix.Text + "_assets(parent_id, lft, rgt, level, name, title, rules) " +
                        "VALUES(" + parentId + ", " + lft++ + ", " + lft++ + ", " + level +
                        ", 'com_content.article" + startNameId++ + "', '" + "Reports" + "', '" +
                        "{\"core.delete\":{\"6\":1},\"core.edit\":{\"6\":1,\"4\":1},\"core.edit.state\":{\"6\":1,\"5\":1}}" +
                        "')", MySqlConnectionGenerator.ShortConnection()))
            {
                command.ExecuteNonQuery();
                lastid = (int)command.LastInsertedId;
            }

            using (var command = new MySqlCommand(
                "INSERT INTO " + dbPrefix.Text +
                "_content(`asset_id`, `title`, `alias`, `title_alias`, `introtext`, `fulltext`, `state`, `sectionid`, `mask`, `catid`, `created`, `created_by`, `created_by_alias`, `modified`, `modified_by`, `checked_out`, `checked_out_time`, `publish_up`, `publish_down`, `images`, `urls`, `attribs`, `version`, `parentid`, `ordering`, `metakey`, `metadesc`, `access`, `hits`, `metadata`, `featured`, `language`, `xreference`)" +
                "VALUES(" + lastid + ", '" + "Reports" + "', '" +
                String.Format("{0:X}", "Reports".GetHashCode()) +
                "', '', '<p>In order to study the effects of treatments of a particular disease please select below the corresponding category first:</p>\\n" +
                GenerateMainPageLinksEn() +
                "', '', 1, 0, 0, 2, '" + DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd hh:mm:ss") +
                "', 84, '', '0000-00-00 00:00:00', 0, 0, '0000-00-00 00:00:00', '" +
                DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd hh:mm:ss") + "', '0000-00-00 00:00:00', '" +
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
        }

        private string GenerateMainPageLinksEn()
        {
            return _enCatsToArticleId.Aggregate("", (current, cat) => current + ("<p>-&nbsp;<a href=\"index.php?option=com_content&amp;view=article&amp;id=" + cat.Value + "&amp;catid=2\">" + cat.Key + " </a></p>\\n"));
        }

        private void InsertEnCategoryArticles(ref int parentId, ref int lft, ref int level, ref int startNameId, ProgressForm progressForm)
        {
            foreach (var category in _categoriesEn)
            {
                var names = new[]
                {
                    "E" + category,
                    "P" + category,
                    "PG" + category
                };

                var names2 = new string[3];
                for (var i = 0; i < names.Length; i++)
                {
                    names2[i] = String.Format("{0:X}", names[i].GetHashCode());
                }

                int lastid;
                using (
                    var command =
                        new MySqlCommand(
                            "INSERT INTO " + dbPrefix.Text + "_assets(parent_id, lft, rgt, level, name, title, rules) " +
                            "VALUES(" + parentId + ", " + lft++ + ", " + lft++ + ", " + level +
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
                    "', '', '<p>Treatment results of all diseases in the category " + category +
                    ".</p>\\n<p>{module " +
                    names2[0] + "}</p>\\n<p>{module " + names2[1] + "}</p>\\n<p>{module " + names2[2] + "}</p>\\n" +
                    "Here you will find the links to the ecaluations of all diseases in the category " + category + "\\n" + 
                    GenerateLinkStringEn(category) +
                    "', '', 1, 0, 0, 2, '" + DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd hh:mm:ss") +
                    "', 84, '', '0000-00-00 00:00:00', 0, 0, '0000-00-00 00:00:00', '" +
                    DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd hh:mm:ss") + "', '0000-00-00 00:00:00', '" +
                    "{\"image_intro\":\"\",\"float_intro\":\"\",\"image_intro_alt\":\"\",\"image_intro_caption\":\"\",\"image_fulltext\":\"\",\"float_fulltext\":\"\",\"image_fulltext_alt\":\"\",\"image_fulltext_caption\":\"\"}" +
                    "', '" +
                    "{\"urla\":null,\"urlatext\":\"\",\"targeta\":\"\",\"urlb\":null,\"urlbtext\":\"\",\"targetb\":\"\",\"urlc\":null,\"urlctext\":\"\",\"targetc\":\"\"}" +
                    "', '" +
                    "{\"show_title\":\"\",\"link_titles\":\"\",\"show_intro\":\"\",\"show_category\":\"\",\"link_category\":\"\",\"show_parent_category\":\"\",\"link_parent_category\":\"\",\"show_author\":\"\",\"link_author\":\"\",\"show_create_date\":\"\",\"show_modify_date\":\"\",\"show_publish_date\":\"\",\"show_item_navigation\":\"\",\"show_icons\":\"\",\"show_print_icon\":\"\",\"show_email_icon\":\"\",\"show_vote\":\"\",\"show_hits\":\"\",\"show_noauth\":\"\",\"urls_position\":\"\",\"alternative_readmore\":\"\",\"article_layout\":\"\",\"show_publishing_options\":\"\",\"show_article_options\":\"\",\"show_urls_images_backend\":\"\",\"show_urls_images_frontend\":\"\"}" +
                    "', 1,0,0,'','',1, 1, '" + "{\"robots\":\"\",\"author\":\"\",\"rights\":\"\",\"xreference\":\"\"}" +
                    "', 0, '*', '')", MySqlConnectionGenerator.ShortConnection()))
                {
                    command.ExecuteNonQuery();
                    if (!_enCatsToArticleId.ContainsKey(category))
                    {
                        _enCatsToArticleId.Add(category, (int)command.LastInsertedId);
                    }
                }
                progressForm.PerformStep();
            }
        }

        private string GenerateLinkStringEn(string category)
        {
            var result = new List<string>();
            var catId = 1;
            using (
                var reader = (new MySqlCommand("SELECT id FROM categories WHERE en = '" + category + "'",
                    MySqlConnectionGenerator.ShortConnection())).ExecuteReader())
            {
                while (reader.Read())
                {
                    catId = reader.GetInt32Save(0);
                }
            }

            using (var reader = (new MySqlCommand("SELECT en FROM diseases WHERE category_id = " + catId, MySqlConnectionGenerator.ShortConnection())).ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(reader.GetStringSave(0));
                }
            }

            var resultString = "";
            foreach (var res in result)
            {
                var res2 = res.Replace("'", "\\'");
                resultString += "<p>-&nbsp;<a href=\"index.php?option=com_content&amp;view=article&amp;id=" +
                                _enDiseaseToarticleId[res2] + "&amp;catid=2\">" + res2 + " </a></p>\\n";
            }
            return resultString;
        }

        private void InsertEnDiseaseArticles(ref int parentId, ref int lft, ref int level, ref int startNameId, ProgressForm progressForm)
        {
            foreach (var value in _valuesEn)
            {
                var names = new[]
                {
                    "E" + value,
                    "P" + value,
                    "PG" + value
                };

                var names2 = new string[3];
                for (var i = 0; i < names.Length; i++)
                {
                    names2[i] = String.Format("{0:X}", names[i].GetHashCode());
                }

                int lastid;
                using (
                    var command =
                        new MySqlCommand(
                            "INSERT INTO " + dbPrefix.Text + "_assets(parent_id, lft, rgt, level, name, title, rules) " +
                            "VALUES(" + parentId + ", " + lft++ + ", " + lft++ + ", " + level +
                            ", 'com_content.article" + startNameId++ + "', '" + value.Item2 + "', '" +
                            "{\"core.delete\":{\"6\":1},\"core.edit\":{\"6\":1,\"4\":1},\"core.edit.state\":{\"6\":1,\"5\":1}}" +
                            "')", MySqlConnectionGenerator.ShortConnection()))
                {
                    command.ExecuteNonQuery();
                    lastid = (int)command.LastInsertedId;
                }

                using (var command = new MySqlCommand(
                    "INSERT INTO " + dbPrefix.Text +
                    "_content(`asset_id`, `title`, `alias`, `title_alias`, `introtext`, `fulltext`, `state`, `sectionid`, `mask`, `catid`, `created`, `created_by`, `created_by_alias`, `modified`, `modified_by`, `checked_out`, `checked_out_time`, `publish_up`, `publish_down`, `images`, `urls`, `attribs`, `version`, `parentid`, `ordering`, `metakey`, `metadesc`, `access`, `hits`, `metadata`, `featured`, `language`, `xreference`)" +
                    "VALUES(" + lastid + ", '" + value.Item2 + "', '" +
                    String.Format("{0:X}", value.Item2.GetHashCode()) +
                    "', '', '<p>Treatment results of the disease " + value.Item2 + ".</p>\\n<p>{module " +
                    names2[0] + "}</p>\\n<p>{module " + names2[1] + "}</p>\\n<p>{module " + names2[2] + "}</p>\\n" +
                    "', '', 1, 0, 0, 2, '" + DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd hh:mm:ss") +
                    "', 84, '', '0000-00-00 00:00:00', 0, 0, '0000-00-00 00:00:00', '" +
                    DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd hh:mm:ss") + "', '0000-00-00 00:00:00', '" +
                    "{\"image_intro\":\"\",\"float_intro\":\"\",\"image_intro_alt\":\"\",\"image_intro_caption\":\"\",\"image_fulltext\":\"\",\"float_fulltext\":\"\",\"image_fulltext_alt\":\"\",\"image_fulltext_caption\":\"\"}" +
                    "', '" +
                    "{\"urla\":null,\"urlatext\":\"\",\"targeta\":\"\",\"urlb\":null,\"urlbtext\":\"\",\"targetb\":\"\",\"urlc\":null,\"urlctext\":\"\",\"targetc\":\"\"}" +
                    "', '" +
                    "{\"show_title\":\"\",\"link_titles\":\"\",\"show_intro\":\"\",\"show_category\":\"\",\"link_category\":\"\",\"show_parent_category\":\"\",\"link_parent_category\":\"\",\"show_author\":\"\",\"link_author\":\"\",\"show_create_date\":\"\",\"show_modify_date\":\"\",\"show_publish_date\":\"\",\"show_item_navigation\":\"\",\"show_icons\":\"\",\"show_print_icon\":\"\",\"show_email_icon\":\"\",\"show_vote\":\"\",\"show_hits\":\"\",\"show_noauth\":\"\",\"urls_position\":\"\",\"alternative_readmore\":\"\",\"article_layout\":\"\",\"show_publishing_options\":\"\",\"show_article_options\":\"\",\"show_urls_images_backend\":\"\",\"show_urls_images_frontend\":\"\"}" +
                    "', 1,0,0,'','',1, 1, '" + "{\"robots\":\"\",\"author\":\"\",\"rights\":\"\",\"xreference\":\"\"}" +
                    "', 0, '*', '')", MySqlConnectionGenerator.ShortConnection()))
                {
                    command.ExecuteNonQuery();
                    if (!_enDiseaseToarticleId.ContainsKey(value.Item2))
                        _enDiseaseToarticleId.Add(value.Item2, (int)command.LastInsertedId);
                }

                progressForm.PerformStep();
            }
        }

        private void InsertEnCategoryModules(ref decimal id, ProgressForm progressForm)
        {
            foreach (var cat in _categoriesEn)
            {
                var names = new[]
                {
                    "E" + cat,
                    "P" + cat,
                    "PG" + cat
                };

                var names2 = new string[3];
                for (var i = 0; i < names.Length; i++)
                {
                    names2[i] = String.Format("{0:X}", names[i].GetHashCode());
                }

                InsertModules(names, names2, id, progressForm);
                id++;
            }
        }

        private void InsertEnDiseaseModule(decimal id, ProgressForm progressForm)
        {
            foreach (var value in _valuesEn)
            {
                var names = new[]
                {
                    "E" + value,
                    "P" + value,
                    "PG" + value
                };

                var names2 = new string[3];
                for (var i = 0; i < names.Length; i++)
                {
                    names2[i] = String.Format("{0:X}", names[i].GetHashCode());
                }

                InsertModules(names, names2, id, progressForm);
                id++;
            }
        }

        private void InsertEnDiseaseQueries(ref decimal id, ProgressForm progressForm, ref decimal id2)
        {
            foreach (var value in _valuesEn)
            {
                var names = new[]
                {
                    "Extracts",
                    "Powders",
                    "Powders on cereal basis"
                };

                var sql = new string[3];

                GenerateSqlEn(ref sql, id);

                InsertIntoJockham(sql, names, (int)id2);
                progressForm.PerformStep();
                id++;
                id2++;
            }
        }

        private void InsertEnCategoriesQueries(ref decimal id, ProgressForm progressForm, ref decimal id2)
        {
            foreach (var cat in _categoriesEn)
            {
                var names = new[]
                {
                    "Extracts",
                    "Powders",
                    "Powders on cereal basis"
                };

                var sql = new string[3];

                GenerateSqlEn(ref sql, id);

                InsertIntoJockham(sql, names, (int)id2);
                progressForm.PerformStep();
                id++;
                id2++;
            }
        }

        private void CreateMainPage(ref int parentId, ref int lft, ref int level, ref int startNameId)
        {
            int lastid;
            using (
                var command =
                    new MySqlCommand(
                        "INSERT INTO " + dbPrefix.Text + "_assets(parent_id, lft, rgt, level, name, title, rules) " +
                        "VALUES(" + parentId + ", " + lft++ + ", " + lft++ + ", " + level +
                        ", 'com_content.article" + startNameId++ + "', '" + "Auswertung" + "', '" +
                        "{\"core.delete\":{\"6\":1},\"core.edit\":{\"6\":1,\"4\":1},\"core.edit.state\":{\"6\":1,\"5\":1}}" +
                        "')", MySqlConnectionGenerator.ShortConnection()))
            {
                command.ExecuteNonQuery();
                lastid = (int) command.LastInsertedId;
            }

            using (var command = new MySqlCommand(
                "INSERT INTO " + dbPrefix.Text +
                "_content(`asset_id`, `title`, `alias`, `title_alias`, `introtext`, `fulltext`, `state`, `sectionid`, `mask`, `catid`, `created`, `created_by`, `created_by_alias`, `modified`, `modified_by`, `checked_out`, `checked_out_time`, `publish_up`, `publish_down`, `images`, `urls`, `attribs`, `version`, `parentid`, `ordering`, `metakey`, `metadesc`, `access`, `hits`, `metadata`, `featured`, `language`, `xreference`)" +
                "VALUES(" + lastid + ", '" + "Auswertung" + "', '" +
                String.Format("{0:X}", "Auswertung".GetHashCode()) +
                "', '', '<p>Um die Behandlungserfolge bei einer bestimmten Krankheit zu betrachten, wählen Sie bitte zuerst unten die dazu gehörige Kategorie aus:</p>\\n" +
                GenerateMainPageLinks() +
                "', '', 1, 0, 0, 2, '" + DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd hh:mm:ss") +
                "', 84, '', '0000-00-00 00:00:00', 0, 0, '0000-00-00 00:00:00', '" +
                DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd hh:mm:ss") + "', '0000-00-00 00:00:00', '" +
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
        }

        private void InsertCategoryArticles(ref int parentId, ref int lft, ref int level, ref int startNameId, ProgressForm progressForm)
        {
            foreach (var category in _categories)
            {
                var names = new[]
                {
                    "E" + category,
                    "P" + category,
                    "PG" + category
                };

                var names2 = new string[3];
                for (var i = 0; i < names.Length; i++)
                {
                    names2[i] = String.Format("{0:X}", names[i].GetHashCode());
                }

                int lastid;
                using (
                    var command =
                        new MySqlCommand(
                            "INSERT INTO " + dbPrefix.Text + "_assets(parent_id, lft, rgt, level, name, title, rules) " +
                            "VALUES(" + parentId + ", " + lft++ + ", " + lft++ + ", " + level +
                            ", 'com_content.article" + startNameId++ + "', '" + category + "', '" +
                            "{\"core.delete\":{\"6\":1},\"core.edit\":{\"6\":1,\"4\":1},\"core.edit.state\":{\"6\":1,\"5\":1}}" +
                            "')", MySqlConnectionGenerator.ShortConnection()))
                {
                    command.ExecuteNonQuery();
                    lastid = (int) command.LastInsertedId;
                }

                using (var command = new MySqlCommand(
                    "INSERT INTO " + dbPrefix.Text +
                    "_content(`asset_id`, `title`, `alias`, `title_alias`, `introtext`, `fulltext`, `state`, `sectionid`, `mask`, `catid`, `created`, `created_by`, `created_by_alias`, `modified`, `modified_by`, `checked_out`, `checked_out_time`, `publish_up`, `publish_down`, `images`, `urls`, `attribs`, `version`, `parentid`, `ordering`, `metakey`, `metadesc`, `access`, `hits`, `metadata`, `featured`, `language`, `xreference`)" +
                    "VALUES(" + lastid + ", '" + category + "', '" +
                    String.Format("{0:X}", category.GetHashCode()) +
                    "', '', '<p>Behandlungserfolg aller Krankheiten der Kategorie " + category +
                    ". </p>\\n<p>{module " +
                    names2[0] + "}</p>\\n<p>{module " + names2[1] + "}</p>\\n<p>{module " + names2[2] + "}</p>\\nHier finden Sie die Links zu den Auswertungen aller Krankheiten der Kategorie " + category + 
                    GenerateLinkString(category) +
                    "', '', 1, 0, 0, 2, '" + DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd hh:mm:ss") +
                    "', 84, '', '0000-00-00 00:00:00', 0, 0, '0000-00-00 00:00:00', '" +
                    DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd hh:mm:ss") + "', '0000-00-00 00:00:00', '" +
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
                progressForm.PerformStep();
            }
        }

        private void InsertDiseaseArticles(ref int parentId, ref int lft, ref int level, ref int startNameId, ProgressForm progressForm)
        {
            foreach (var value in _values)
            {
                var names = new[]
                {
                    "E" + value,
                    "P" + value,
                    "PG" + value
                };

                var names2 = new string[3];
                for (var i = 0; i < names.Length; i++)
                {
                    names2[i] = String.Format("{0:X}", names[i].GetHashCode());
                }

                int lastid;
                using (
                    var command =
                        new MySqlCommand(
                            "INSERT INTO " + dbPrefix.Text + "_assets(parent_id, lft, rgt, level, name, title, rules) " +
                            "VALUES(" + parentId + ", " + lft++ + ", " + lft++ + ", " + level +
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
                    "', '', '<p>Behandlungserfolg der Krankheit " + value.Item2 + ".</p>\\n<p>{module " +
                    names2[0] + "}</p>\\n<p>{module " + names2[1] + "}</p>\\n<p>{module " + names2[2] + "}</p>\\n" +
                    "', '', 1, 0, 0, 2, '" + DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd hh:mm:ss") +
                    "', 84, '', '0000-00-00 00:00:00', 0, 0, '0000-00-00 00:00:00', '" +
                    DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd hh:mm:ss") + "', '0000-00-00 00:00:00', '" +
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

                progressForm.PerformStep();
            }
        }

        private void DeleteArticles()
        {
            using (
                var command = new MySqlCommand("DELETE FROM " + dbPrefix.Text + "_content WHERE id > 55",
                    MySqlConnectionGenerator.ShortConnection()))
            {
                command.ExecuteNonQuery();
            }

            using (
                var command = new MySqlCommand("DELETE FROM " + dbPrefix.Text + "_assets WHERE id > 110",
                    MySqlConnectionGenerator.ShortConnection()))
            {
                command.ExecuteNonQuery();
            }
        }

        private void DeleteModules()
        {
            using (
                var command = new MySqlCommand("DELETE FROM " + dbPrefix.Text + "_modules WHERE id > 123",
                    MySqlConnectionGenerator.ShortConnection()))
            {
                command.ExecuteNonQuery();
            }
        }

        private void DeleteSqls()
        {
            using (
                var command = new MySqlCommand("DELETE FROM " + dbPrefix.Text + "_jockham_reports",
                    MySqlConnectionGenerator.ShortConnection()))
            {
                command.ExecuteNonQuery();
            }
        }

        private void InsertDiseaseModule(ref decimal id, ProgressForm progressForm)
        {
            foreach (var value in _values)
            {
                var names = new[]
                {
                    "E" + value,
                    "P" + value,
                    "PG" + value
                };

                var names2 = new string[3];
                for (var i = 0; i < names.Length; i++)
                {
                    names2[i] = String.Format("{0:X}", names[i].GetHashCode());
                    if (!_hashes.Contains(names2[i]))
                    {
                        _hashes.Add(names2[i]);
                    }
                }

                InsertModules(names, names2, id, progressForm);
                id++;
            }
        }

        private void InsertCategoryModule(ref decimal id, ProgressForm progressForm)
        {
            foreach (var cat in _categories)
            {
                var names = new[]
                {
                    "E" + cat,
                    "P" + cat,
                    "PG" + cat
                };

                var names2 = new string[3];
                for (var i = 0; i < names.Length; i++)
                {
                    names2[i] = String.Format("{0:X}", names[i].GetHashCode());
                    if (!_hashes.Contains(names2[i]))
                    {
                        _hashes.Add(names2[i]);
                    }
                }

                InsertModules(names, names2, id, progressForm);
                id++;
            }
        }

        private void InsertDiseaseQueries(decimal id, ProgressForm progressForm, ref decimal id2)
        {
            foreach (var value in _values)
            {
                var names = new[]
                {
                    "Extrakte",
                    "Pulver",
                    "Pulver Getreidebasis"
                };

                var sql = new string[3];

                GenerateSql(ref sql, id);
                InsertIntoJockham(sql, names, (int) id2);
                progressForm.PerformStep();
                id++;
                id2++;
            }
        }

        private void InsertCategoryQueries(ref decimal id, ProgressForm progressForm)
        {
            foreach (var cat in _categories)
            {
                var names2 = new[]
                {
                    "Extrakte",
                    "Pulver",
                    "Pulver Getreidebasis"
                };

                var sql = new string[3];

                GenerateSql(ref sql, id);

                InsertIntoJockham(sql, names2, (int) id);
                progressForm.PerformStep();
                id++;
            }
        }

        private void InsertModules(string[] names, string[] names2, decimal id, ProgressForm progressForm)
        {
            for (int i = 0; i < names.Length; i++)
            {
                using (
                    var command =
                        new MySqlCommand(
                            "INSERT INTO " + dbPrefix.Text +
                            "_modules(title, ordering, checked_out, published, module, access, showtitle, params, client_id, language)" +
                            "VALUES('" + names2[i] +
                            "', 1, 0, 1, 'mod_jockham_reports', 1, 1, '{\"idreport\":\"" + (3*(id - 1) + 1 + i) +
                            "\",\"moduleclass_sfx\":\"\",\"showreportname\":\"1\",\"showexporttoexcel\":\"0\",\"showheader\":\"1\",\"showpagination\":\"1\"}', 0, '*')",
                            MySqlConnectionGenerator.ShortConnection()))
                {
                    command.ExecuteNonQuery();
                    progressForm.PerformStep();
                }
            }
        }

        private string GenerateMainPageLinks()
        {
            return _catsToArticleId.Aggregate("", (current, cat) => current + ("<p>-&nbsp;<a href=\"index.php?option=com_content&amp;view=article&amp;id=" + cat.Value + "&amp;catid=2\">" + cat.Key + " </a></p>\\n"));
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

            return result.Aggregate("", (current, res) => current + ("<p>-&nbsp;<a href=\"index.php?option=com_content&amp;view=article&amp;id=" + _diseaseToarticleId[res] + "&amp;catid=2\">" + res + " </a></p>\\n"));
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
            for (var i = (int) numericUpDown1.Value; i < numericUpDown1.Value + 3; i++)
            {
                sql[(int) (i - numericUpDown1.Value)] = GenerateSql("Pilzart", "Arithmetisches Mittel",
                    "Gesammtzahl der Teilnehmer", i, id);
            }
        }

        private void GenerateSqlEn(ref string[] sql, decimal id)
        {
            for (var i = (int) numericUpDown1.Value; i < numericUpDown1.Value + 3; i++)
            {
                sql[(int)(i - numericUpDown1.Value)] = GenerateSql("Mushroom species", "Arithmetic mean", "Number of participants", i, id);
            }
        }

        private void GenerateSqlSpecies(ref string[] sql, decimal id)
        {
            for (var i = (int) numericUpDown1.Value; i < numericUpDown1.Value + 3; i++)
            {
                sql[(int) (i - numericUpDown1.Value)] = GenerateSql("Krankheit", "Arithmetisches Mittel",
                    "Gesammtzahl der Teilnehmer", i, id);
            }
        }

        private string GenerateSql(string text1, string text2, string text3, int id1, decimal id2)
        {
            return "SELECT data.Pilzart AS '" + text1 + "', FORMAT(AVG(data.a),1) AS '" + text2 + "', COUNT(data.Pilzart) AS '" + text3 + "'  " +
                    "FROM (" +
                    "	SELECT f.ftext AS Pilzart, AVG(fs.ordering + 1) AS a, COUNT(DISTINCT(ans.start_id)) AS b " +
                    "	FROM y1trf_survey_force_user_answers AS ans " +
                    "	LEFT JOIN y1trf_survey_force_scales AS fs ON ans.ans_field = fs.id " +
                    "	JOIN y1trf_survey_force_fields AS f ON ans.answer = f.id " +
                    "	WHERE ans.start_id IN ( " +
                    "		SELECT ans.start_id " +
                    "		FROM y1trf_survey_force_user_answers AS ans " +
                    "		WHERE ans.answer = " + id1 + ") " +
                    "	AND ans.start_id IN ( " +
                    "		SELECT ans.start_id " +
                    "		FROM y1trf_survey_force_user_answers AS ans " +
                    "		WHERE ans.answer = " + id2 + ") " +
                    "	AND ((ans.quest_id >= 2 AND ans.quest_id <= 25) OR ans.quest_id = 37) " +
                    "	GROUP BY ans.start_id " +
                    "	ORDER BY AVG(fs.ordering + 1) DESC ) AS data " +
                    "GROUP BY data.Pilzart " +
                    "ORDER BY '" + text2 + "' DESC; ";
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
