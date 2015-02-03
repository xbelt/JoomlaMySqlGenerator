namespace JoomlaMySqlGenerator
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.dbServer = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dbPrefix = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dbPassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.dbUser = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.dbName = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dbServer
            // 
            this.dbServer.Location = new System.Drawing.Point(88, 13);
            this.dbServer.Name = "dbServer";
            this.dbServer.Size = new System.Drawing.Size(184, 20);
            this.dbServer.TabIndex = 0;
            this.dbServer.TextChanged += new System.EventHandler(this.dbServer_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "DB Server";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "DB Prefix";
            // 
            // dbPrefix
            // 
            this.dbPrefix.Location = new System.Drawing.Point(88, 91);
            this.dbPrefix.Name = "dbPrefix";
            this.dbPrefix.Size = new System.Drawing.Size(184, 20);
            this.dbPrefix.TabIndex = 2;
            this.dbPrefix.TextChanged += new System.EventHandler(this.dbPrefix_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "DB password";
            // 
            // dbPassword
            // 
            this.dbPassword.Location = new System.Drawing.Point(88, 65);
            this.dbPassword.Name = "dbPassword";
            this.dbPassword.Size = new System.Drawing.Size(184, 20);
            this.dbPassword.TabIndex = 4;
            this.dbPassword.TextChanged += new System.EventHandler(this.dbPassword_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "DB user";
            // 
            // dbUser
            // 
            this.dbUser.Location = new System.Drawing.Point(88, 39);
            this.dbUser.Name = "dbUser";
            this.dbUser.Size = new System.Drawing.Size(184, 20);
            this.dbUser.TabIndex = 6;
            this.dbUser.TextChanged += new System.EventHandler(this.dbUser_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(197, 143);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "Get data";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(116, 143);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 9;
            this.button2.Text = "Show data";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 120);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "DB Name";
            // 
            // dbName
            // 
            this.dbName.Location = new System.Drawing.Point(88, 117);
            this.dbName.Name = "dbName";
            this.dbName.Size = new System.Drawing.Size(184, 20);
            this.dbName.TabIndex = 10;
            this.dbName.TextChanged += new System.EventHandler(this.dbName_TextChanged);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(35, 142);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 12;
            this.button3.Text = "Fill data";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dbName);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dbUser);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dbPassword);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dbPrefix);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dbServer);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox dbServer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox dbPrefix;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox dbPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox dbUser;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox dbName;
        private System.Windows.Forms.Button button3;
    }
}

