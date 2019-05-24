﻿namespace Acceleratio.SPDG.UI
{
    partial class frm11Permissions
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
            this.chkAssignPermissions = new System.Windows.Forms.CheckBox();
            this.txtPercentSites = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPercentLists = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPercentLibFolders = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtPercentListItems = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtPercentDirectlyToUsers = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtPercentGroupCases = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.trackPermissionsPerObject = new System.Windows.Forms.TrackBar();
            this.label14 = new System.Windows.Forms.Label();
            this.lblPermissionsPerObject = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.trackPermissionsDeletePerObject = new System.Windows.Forms.TrackBar();
            this.lblPermissionsDeletePerObject = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackPermissionsPerObject)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackPermissionsDeletePerObject)).BeginInit();
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
            this.ucSteps1.Size = new System.Drawing.Size(232, 375);
            this.ucSteps1.TabIndex = 7;
            // 
            // chkAssignPermissions
            // 
            this.chkAssignPermissions.AutoSize = true;
            this.chkAssignPermissions.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkAssignPermissions.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.chkAssignPermissions.Location = new System.Drawing.Point(265, 138);
            this.chkAssignPermissions.Name = "chkAssignPermissions";
            this.chkAssignPermissions.Size = new System.Drawing.Size(127, 19);
            this.chkAssignPermissions.TabIndex = 8;
            this.chkAssignPermissions.Text = "Assign permissions";
            this.chkAssignPermissions.UseVisualStyleBackColor = true;
            this.chkAssignPermissions.CheckedChanged += new System.EventHandler(this.chkAssignPermissions_CheckedChanged);
            // 
            // txtPercentSites
            // 
            this.txtPercentSites.Enabled = false;
            this.txtPercentSites.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPercentSites.Location = new System.Drawing.Point(454, 173);
            this.txtPercentSites.Name = "txtPercentSites";
            this.txtPercentSites.Size = new System.Drawing.Size(43, 23);
            this.txtPercentSites.TabIndex = 10;
            this.txtPercentSites.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label2.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label2.Location = new System.Drawing.Point(503, 176);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 15);
            this.label2.TabIndex = 11;
            this.label2.Text = "% of sites";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label3.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label3.Location = new System.Drawing.Point(503, 209);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 15);
            this.label3.TabIndex = 14;
            this.label3.Text = "% of lists";
            // 
            // txtPercentLists
            // 
            this.txtPercentLists.Enabled = false;
            this.txtPercentLists.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPercentLists.Location = new System.Drawing.Point(454, 206);
            this.txtPercentLists.Name = "txtPercentLists";
            this.txtPercentLists.Size = new System.Drawing.Size(43, 23);
            this.txtPercentLists.TabIndex = 13;
            this.txtPercentLists.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label4.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label4.Location = new System.Drawing.Point(503, 244);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(164, 15);
            this.label4.TabIndex = 17;
            this.label4.Text = "% of document library folders";
            // 
            // txtPercentLibFolders
            // 
            this.txtPercentLibFolders.Enabled = false;
            this.txtPercentLibFolders.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPercentLibFolders.Location = new System.Drawing.Point(454, 241);
            this.txtPercentLibFolders.Name = "txtPercentLibFolders";
            this.txtPercentLibFolders.Size = new System.Drawing.Size(43, 23);
            this.txtPercentLibFolders.TabIndex = 16;
            this.txtPercentLibFolders.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label5.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label5.Location = new System.Drawing.Point(503, 277);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(81, 15);
            this.label5.TabIndex = 20;
            this.label5.Text = "% of list items";
            // 
            // txtPercentListItems
            // 
            this.txtPercentListItems.Enabled = false;
            this.txtPercentListItems.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPercentListItems.Location = new System.Drawing.Point(454, 274);
            this.txtPercentListItems.Name = "txtPercentListItems";
            this.txtPercentListItems.Size = new System.Drawing.Size(43, 23);
            this.txtPercentListItems.TabIndex = 19;
            this.txtPercentListItems.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label6.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label6.Location = new System.Drawing.Point(283, 330);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 15);
            this.label6.TabIndex = 21;
            this.label6.Text = "Assign";
            // 
            // txtPercentDirectlyToUsers
            // 
            this.txtPercentDirectlyToUsers.Enabled = false;
            this.txtPercentDirectlyToUsers.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPercentDirectlyToUsers.Location = new System.Drawing.Point(331, 327);
            this.txtPercentDirectlyToUsers.Name = "txtPercentDirectlyToUsers";
            this.txtPercentDirectlyToUsers.Size = new System.Drawing.Size(43, 23);
            this.txtPercentDirectlyToUsers.TabIndex = 22;
            this.txtPercentDirectlyToUsers.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label7.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label7.Location = new System.Drawing.Point(376, 330);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(206, 15);
            this.label7.TabIndex = 23;
            this.label7.Text = "% of the permissions directly to users,";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label8.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label8.Location = new System.Drawing.Point(285, 361);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(148, 15);
            this.label8.TabIndex = 24;
            this.label8.Text = "asign SharePoint groups in";
            // 
            // txtPercentGroupCases
            // 
            this.txtPercentGroupCases.Enabled = false;
            this.txtPercentGroupCases.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPercentGroupCases.Location = new System.Drawing.Point(441, 358);
            this.txtPercentGroupCases.Name = "txtPercentGroupCases";
            this.txtPercentGroupCases.Size = new System.Drawing.Size(43, 23);
            this.txtPercentGroupCases.TabIndex = 25;
            this.txtPercentGroupCases.Text = "0";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label9.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label9.Location = new System.Drawing.Point(490, 361);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 15);
            this.label9.TabIndex = 26;
            this.label9.Text = "% of cases,";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label10.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label10.Location = new System.Drawing.Point(283, 393);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(251, 15);
            this.label10.TabIndex = 27;
            this.label10.Text = "use existing AD groups in the remaining cases.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label1.Location = new System.Drawing.Point(283, 176);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(165, 15);
            this.label1.TabIndex = 28;
            this.label1.Text = "Create unique permissions for";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label11.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label11.Location = new System.Drawing.Point(283, 209);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(165, 15);
            this.label11.TabIndex = 29;
            this.label11.Text = "Create unique permissions for";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label12.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label12.Location = new System.Drawing.Point(283, 244);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(165, 15);
            this.label12.TabIndex = 30;
            this.label12.Text = "Create unique permissions for";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label13.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label13.Location = new System.Drawing.Point(283, 277);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(165, 15);
            this.label13.TabIndex = 31;
            this.label13.Text = "Create unique permissions for";
            // 
            // trackPermissionsPerObject
            // 
            this.trackPermissionsPerObject.Enabled = false;
            this.trackPermissionsPerObject.LargeChange = 1;
            this.trackPermissionsPerObject.Location = new System.Drawing.Point(278, 465);
            this.trackPermissionsPerObject.Maximum = 30;
            this.trackPermissionsPerObject.Minimum = 1;
            this.trackPermissionsPerObject.Name = "trackPermissionsPerObject";
            this.trackPermissionsPerObject.Size = new System.Drawing.Size(528, 45);
            this.trackPermissionsPerObject.TabIndex = 34;
            this.trackPermissionsPerObject.Value = 1;
            this.trackPermissionsPerObject.ValueChanged += new System.EventHandler(this.trackPermissionsPerObject_ValueChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label14.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label14.Location = new System.Drawing.Point(283, 445);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(224, 15);
            this.label14.TabIndex = 35;
            this.label14.Text = "Number of permissions to add per object";
            // 
            // lblPermissionsPerObject
            // 
            this.lblPermissionsPerObject.AutoSize = true;
            this.lblPermissionsPerObject.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPermissionsPerObject.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblPermissionsPerObject.Location = new System.Drawing.Point(813, 468);
            this.lblPermissionsPerObject.Name = "lblPermissionsPerObject";
            this.lblPermissionsPerObject.Size = new System.Drawing.Size(13, 15);
            this.lblPermissionsPerObject.TabIndex = 36;
            this.lblPermissionsPerObject.Text = "1";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label15.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.label15.Location = new System.Drawing.Point(283, 504);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(363, 15);
            this.label15.TabIndex = 37;
            this.label15.Text = "Number of permissions to remove per object (Incremental updates)";
            // 
            // trackPermissionsDeletePerObject
            // 
            this.trackPermissionsDeletePerObject.Enabled = false;
            this.trackPermissionsDeletePerObject.LargeChange = 1;
            this.trackPermissionsDeletePerObject.Location = new System.Drawing.Point(278, 531);
            this.trackPermissionsDeletePerObject.Maximum = 30;
            this.trackPermissionsDeletePerObject.Name = "trackPermissionsDeletePerObject";
            this.trackPermissionsDeletePerObject.Size = new System.Drawing.Size(528, 45);
            this.trackPermissionsDeletePerObject.TabIndex = 38;
            this.trackPermissionsDeletePerObject.ValueChanged += new System.EventHandler(this.trackPermissionsDeletePerObject_ValueChanged);
            // 
            // lblPermissionsDeletePerObject
            // 
            this.lblPermissionsDeletePerObject.AutoSize = true;
            this.lblPermissionsDeletePerObject.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPermissionsDeletePerObject.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblPermissionsDeletePerObject.Location = new System.Drawing.Point(812, 537);
            this.lblPermissionsDeletePerObject.Name = "lblPermissionsDeletePerObject";
            this.lblPermissionsDeletePerObject.Size = new System.Drawing.Size(13, 15);
            this.lblPermissionsDeletePerObject.TabIndex = 39;
            this.lblPermissionsDeletePerObject.Text = "0";
            // 
            // frm11Permissions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(921, 644);
            this.Controls.Add(this.lblPermissionsDeletePerObject);
            this.Controls.Add(this.trackPermissionsDeletePerObject);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.lblPermissionsPerObject);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.trackPermissionsPerObject);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtPercentGroupCases);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtPercentDirectlyToUsers);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtPercentListItems);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtPercentLibFolders);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtPercentLists);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtPercentSites);
            this.Controls.Add(this.chkAssignPermissions);
            this.Controls.Add(this.ucSteps1);
            this.Name = "frm11Permissions";
            this.Text = "frm11Permissions";
            this.Controls.SetChildIndex(this.lblTitle, 0);
            this.Controls.SetChildIndex(this.lblDescription, 0);
            this.Controls.SetChildIndex(this.btnBack, 0);
            this.Controls.SetChildIndex(this.btnNext, 0);
            this.Controls.SetChildIndex(this.btnHelp, 0);
            this.Controls.SetChildIndex(this.btnClose, 0);
            this.Controls.SetChildIndex(this.ucSteps1, 0);
            this.Controls.SetChildIndex(this.chkAssignPermissions, 0);
            this.Controls.SetChildIndex(this.txtPercentSites, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.txtPercentLists, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.txtPercentLibFolders, 0);
            this.Controls.SetChildIndex(this.label4, 0);
            this.Controls.SetChildIndex(this.txtPercentListItems, 0);
            this.Controls.SetChildIndex(this.label5, 0);
            this.Controls.SetChildIndex(this.label6, 0);
            this.Controls.SetChildIndex(this.txtPercentDirectlyToUsers, 0);
            this.Controls.SetChildIndex(this.label7, 0);
            this.Controls.SetChildIndex(this.label8, 0);
            this.Controls.SetChildIndex(this.txtPercentGroupCases, 0);
            this.Controls.SetChildIndex(this.label9, 0);
            this.Controls.SetChildIndex(this.label10, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.label11, 0);
            this.Controls.SetChildIndex(this.label12, 0);
            this.Controls.SetChildIndex(this.label13, 0);
            this.Controls.SetChildIndex(this.trackPermissionsPerObject, 0);
            this.Controls.SetChildIndex(this.label14, 0);
            this.Controls.SetChildIndex(this.lblPermissionsPerObject, 0);
            this.Controls.SetChildIndex(this.label15, 0);
            this.Controls.SetChildIndex(this.trackPermissionsDeletePerObject, 0);
            this.Controls.SetChildIndex(this.lblPermissionsDeletePerObject, 0);
            ((System.ComponentModel.ISupportInitialize)(this.trackPermissionsPerObject)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackPermissionsDeletePerObject)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ucSteps ucSteps1;
        private System.Windows.Forms.CheckBox chkAssignPermissions;
        private System.Windows.Forms.TextBox txtPercentSites;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPercentLists;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPercentLibFolders;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPercentListItems;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtPercentDirectlyToUsers;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtPercentGroupCases;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TrackBar trackPermissionsPerObject;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lblPermissionsPerObject;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TrackBar trackPermissionsDeletePerObject;
        private System.Windows.Forms.Label lblPermissionsDeletePerObject;
    }
}