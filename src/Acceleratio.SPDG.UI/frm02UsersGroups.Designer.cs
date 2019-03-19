namespace Acceleratio.SPDG.UI
{
    partial class frm02UsersGroups
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ucSteps1 = new Acceleratio.SPDG.UI.ucSteps();
            this.chkGenerateUsers = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.tbFqdn = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboOrganizationalUnit = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.trackNumberOfUsers = new System.Windows.Forms.TrackBar();
            this.lblNumUsers = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.trackNumberOfSecGroups = new System.Windows.Forms.TrackBar();
            this.lblGroups = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.trackMaxNumberOfUsersInSecurityGroups = new System.Windows.Forms.TrackBar();
            this.lblMaxNumberOfUsersInSecurityGroups = new System.Windows.Forms.Label();
            this.cboDomains = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkAddDomain = new System.Windows.Forms.CheckBox();
            this.debugLabel = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackNumberOfUsers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackNumberOfSecGroups)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackMaxNumberOfUsersInSecurityGroups)).BeginInit();
            this.SuspendLayout();
            // 
            // btnBack
            // 
            this.btnBack.FlatAppearance.BorderSize = 0;
            // 
            // btnNext
            // 
            this.btnNext.FlatAppearance.BorderSize = 0;
            // 
            // btnHelp
            // 
            this.btnHelp.FlatAppearance.BorderSize = 0;
            // 
            // btnClose
            // 
            this.btnClose.FlatAppearance.BorderSize = 0;
            // 
            // ucSteps1
            // 
            this.ucSteps1.Location = new System.Drawing.Point(0, 135);
            this.ucSteps1.Name = "ucSteps1";
            this.ucSteps1.Size = new System.Drawing.Size(235, 480);
            this.ucSteps1.TabIndex = 7;
            // 
            // chkGenerateUsers
            // 
            this.chkGenerateUsers.AutoSize = true;
            this.chkGenerateUsers.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkGenerateUsers.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.chkGenerateUsers.Location = new System.Drawing.Point(268, 193);
            this.chkGenerateUsers.Name = "chkGenerateUsers";
            this.chkGenerateUsers.Size = new System.Drawing.Size(277, 19);
            this.chkGenerateUsers.TabIndex = 8;
            this.chkGenerateUsers.Text = "Generate Users and Security Groups in Directory";
            this.chkGenerateUsers.UseVisualStyleBackColor = true;
            this.chkGenerateUsers.CheckedChanged += new System.EventHandler(this.chkGenerateUsers_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.flowLayoutPanel1);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.groupBox1.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.groupBox1.Location = new System.Drawing.Point(268, 226);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(616, 314);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Users";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.label6);
            this.flowLayoutPanel1.Controls.Add(this.tbFqdn);
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.cboOrganizationalUnit);
            this.flowLayoutPanel1.Controls.Add(this.label3);
            this.flowLayoutPanel1.Controls.Add(this.trackNumberOfUsers);
            this.flowLayoutPanel1.Controls.Add(this.lblNumUsers);
            this.flowLayoutPanel1.Controls.Add(this.label4);
            this.flowLayoutPanel1.Controls.Add(this.trackNumberOfSecGroups);
            this.flowLayoutPanel1.Controls.Add(this.lblGroups);
            this.flowLayoutPanel1.Controls.Add(this.label5);
            this.flowLayoutPanel1.Controls.Add(this.trackMaxNumberOfUsersInSecurityGroups);
            this.flowLayoutPanel1.Controls.Add(this.lblMaxNumberOfUsersInSecurityGroups);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 19);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(610, 292);
            this.flowLayoutPanel1.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(166, 15);
            this.label6.TabIndex = 12;
            this.label6.Text = "Fully Qualified Domain Name:";
            // 
            // tbFqdn
            // 
            this.tbFqdn.Location = new System.Drawing.Point(175, 3);
            this.tbFqdn.Name = "tbFqdn";
            this.tbFqdn.Size = new System.Drawing.Size(359, 23);
            this.tbFqdn.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label2.Location = new System.Drawing.Point(3, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Organizational Unit:";
            // 
            // cboOrganizationalUnit
            // 
            this.cboOrganizationalUnit.FormattingEnabled = true;
            this.cboOrganizationalUnit.Location = new System.Drawing.Point(3, 47);
            this.cboOrganizationalUnit.Name = "cboOrganizationalUnit";
            this.cboOrganizationalUnit.Size = new System.Drawing.Size(531, 23);
            this.cboOrganizationalUnit.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label3.Location = new System.Drawing.Point(3, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(147, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Number of users to create:";
            // 
            // trackNumberOfUsers
            // 
            this.trackNumberOfUsers.LargeChange = 1;
            this.trackNumberOfUsers.Location = new System.Drawing.Point(3, 91);
            this.trackNumberOfUsers.Maximum = 300000;
            this.trackNumberOfUsers.Name = "trackNumberOfUsers";
            this.trackNumberOfUsers.Size = new System.Drawing.Size(517, 45);
            this.trackNumberOfUsers.SmallChange = 10;
            this.trackNumberOfUsers.TabIndex = 3;
            this.trackNumberOfUsers.TickFrequency = 10;
            this.trackNumberOfUsers.ValueChanged += new System.EventHandler(this.trackNumberOfUsers_ValueChanged);
            // 
            // lblNumUsers
            // 
            this.lblNumUsers.AutoSize = true;
            this.lblNumUsers.Location = new System.Drawing.Point(526, 88);
            this.lblNumUsers.Name = "lblNumUsers";
            this.lblNumUsers.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.lblNumUsers.Size = new System.Drawing.Size(13, 20);
            this.lblNumUsers.TabIndex = 6;
            this.lblNumUsers.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 139);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(203, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "Number of Security Groups to create:";
            // 
            // trackNumberOfSecGroups
            // 
            this.trackNumberOfSecGroups.LargeChange = 1;
            this.trackNumberOfSecGroups.Location = new System.Drawing.Point(3, 157);
            this.trackNumberOfSecGroups.Maximum = 400000;
            this.trackNumberOfSecGroups.Name = "trackNumberOfSecGroups";
            this.trackNumberOfSecGroups.Size = new System.Drawing.Size(517, 45);
            this.trackNumberOfSecGroups.TabIndex = 5;
            this.trackNumberOfSecGroups.ValueChanged += new System.EventHandler(this.trackNumberOfSecGroups_ValueChanged);
            // 
            // lblGroups
            // 
            this.lblGroups.AutoSize = true;
            this.lblGroups.Location = new System.Drawing.Point(526, 154);
            this.lblGroups.Name = "lblGroups";
            this.lblGroups.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.lblGroups.Size = new System.Drawing.Size(13, 20);
            this.lblGroups.TabIndex = 7;
            this.lblGroups.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 205);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(289, 15);
            this.label5.TabIndex = 8;
            this.label5.Text = "Maximum number of users in created security groups";
            // 
            // trackMaxNumberOfUsersInSecurityGroups
            // 
            this.trackMaxNumberOfUsersInSecurityGroups.LargeChange = 1;
            this.trackMaxNumberOfUsersInSecurityGroups.Location = new System.Drawing.Point(3, 223);
            this.trackMaxNumberOfUsersInSecurityGroups.Maximum = 200;
            this.trackMaxNumberOfUsersInSecurityGroups.Name = "trackMaxNumberOfUsersInSecurityGroups";
            this.trackMaxNumberOfUsersInSecurityGroups.Size = new System.Drawing.Size(517, 45);
            this.trackMaxNumberOfUsersInSecurityGroups.SmallChange = 2;
            this.trackMaxNumberOfUsersInSecurityGroups.TabIndex = 9;
            this.trackMaxNumberOfUsersInSecurityGroups.TickFrequency = 2;
            this.trackMaxNumberOfUsersInSecurityGroups.ValueChanged += new System.EventHandler(this.trackMaxNumberOfUsersInSecurityGroups_ValueChanged);
            // 
            // lblMaxNumberOfUsersInSecurityGroups
            // 
            this.lblMaxNumberOfUsersInSecurityGroups.AutoSize = true;
            this.lblMaxNumberOfUsersInSecurityGroups.Location = new System.Drawing.Point(526, 220);
            this.lblMaxNumberOfUsersInSecurityGroups.Name = "lblMaxNumberOfUsersInSecurityGroups";
            this.lblMaxNumberOfUsersInSecurityGroups.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.lblMaxNumberOfUsersInSecurityGroups.Size = new System.Drawing.Size(13, 20);
            this.lblMaxNumberOfUsersInSecurityGroups.TabIndex = 10;
            this.lblMaxNumberOfUsersInSecurityGroups.Text = "0";
            // 
            // cboDomains
            // 
            this.cboDomains.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cboDomains.FormattingEnabled = true;
            this.cboDomains.Location = new System.Drawing.Point(580, 152);
            this.cboDomains.Name = "cboDomains";
            this.cboDomains.Size = new System.Drawing.Size(237, 23);
            this.cboDomains.TabIndex = 10;
            this.cboDomains.Leave += new System.EventHandler(this.cboDomains_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label1.Location = new System.Drawing.Point(265, 155);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(300, 15);
            this.label1.TabIndex = 11;
            this.label1.Text = "Use this domain to create SharePoint users and groups: ";
            // 
            // chkAddDomain
            // 
            this.chkAddDomain.AutoSize = true;
            this.chkAddDomain.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkAddDomain.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.chkAddDomain.Location = new System.Drawing.Point(271, 547);
            this.chkAddDomain.Name = "chkAddDomain";
            this.chkAddDomain.Size = new System.Drawing.Size(139, 19);
            this.chkAddDomain.TabIndex = 13;
            this.chkAddDomain.Text = "Add Another Domain";
            this.chkAddDomain.UseVisualStyleBackColor = true;
            this.chkAddDomain.CheckedChanged += new System.EventHandler(this.chkAddDomain_CheckedChanged);
            // 
            // debugLabel
            // 
            this.debugLabel.AutoSize = true;
            this.debugLabel.Location = new System.Drawing.Point(443, 549);
            this.debugLabel.Name = "debugLabel";
            this.debugLabel.Size = new System.Drawing.Size(0, 13);
            this.debugLabel.TabIndex = 14;
            // 
            // frm02UsersGroups
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(921, 644);
            this.Controls.Add(this.debugLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboDomains);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkGenerateUsers);
            this.Controls.Add(this.ucSteps1);
            this.Controls.Add(this.chkAddDomain);
            this.Name = "frm02UsersGroups";
            this.Text = "frm02UsersGroups";
            this.Controls.SetChildIndex(this.chkAddDomain, 0);
            this.Controls.SetChildIndex(this.ucSteps1, 0);
            this.Controls.SetChildIndex(this.chkGenerateUsers, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.cboDomains, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.lblTitle, 0);
            this.Controls.SetChildIndex(this.lblDescription, 0);
            this.Controls.SetChildIndex(this.btnBack, 0);
            this.Controls.SetChildIndex(this.btnNext, 0);
            this.Controls.SetChildIndex(this.btnHelp, 0);
            this.Controls.SetChildIndex(this.btnClose, 0);
            this.Controls.SetChildIndex(this.debugLabel, 0);
            this.groupBox1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackNumberOfUsers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackNumberOfSecGroups)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackMaxNumberOfUsersInSecurityGroups)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ucSteps ucSteps1;
        private System.Windows.Forms.CheckBox chkGenerateUsers;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboOrganizationalUnit;
        private System.Windows.Forms.TrackBar trackNumberOfSecGroups;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TrackBar trackNumberOfUsers;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboDomains;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblGroups;
        private System.Windows.Forms.Label lblNumUsers;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar trackMaxNumberOfUsersInSecurityGroups;
        private System.Windows.Forms.Label lblMaxNumberOfUsersInSecurityGroups;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbFqdn;
        private System.Windows.Forms.CheckBox chkAddDomain;
        private System.Windows.Forms.Label debugLabel;
    }
}