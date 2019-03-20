using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Acceleratio.SPDG.Generator;

namespace Acceleratio.SPDG.UI
{
    public partial class frm02UsersGroups : frmWizardMaster
    {
        public frm02UsersGroups()
        {
            InitializeComponent();

            base.lblTitle.Text = "Users && Groups";
            base.lblDescription.Text = "Create user and group accounts in your Active Directory";

            btnNext.Click += btnNext_Click;
            btnBack.Click += btnBack_Click;

            this.Text = Common.APP_TITLE;
            ucSteps1.showStep(2);

            loadData();
            chkGenerateUsers_CheckedChanged(null, EventArgs.Empty);
            cboDomains.SelectedIndexChanged += cboDomains_SelectedIndexChanged;
        }      
        void btnBack_Click(object sender, EventArgs e)
        {
            RootForm.MovePrevious(this);
            preventCloseMessage = true;
        }

        void btnNext_Click(object sender, EventArgs e)
        {
            if (chkAddDomain.Checked)
            {
                // do something to go back to this wizard page
                RootForm.MoveAt(2, this);
            }
            else
            {
                RootForm.MoveNext(this);
            }
            preventCloseMessage = true;

        }

        public override void loadData()
        {
            this.Show();
            this.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            if (!WorkingDefinition.IsClientObjectModel)
            {
                List<string> domains = AD.GetDomainList();

                foreach (string domain in domains)
                {
                    ComboboxItem item = new ComboboxItem();
                    item.Text = domain;
                    item.Value = domain;
                    cboDomains.Items.Add(item);
                }
                cboDomains.Text = domains[0];
          
                List<string> subdomains = AD.GetDomainList2();

                foreach (string domain in subdomains)
                {
                    ComboboxItem item = new ComboboxItem();
                    item.Text = domain;
                    item.Value = domain;
                    cboDomains.Items.Add(item);
                }
            }
            else
            {
                label2.Visible = false;
                cboOrganizationalUnit.Visible = false;
                label1.Visible = false;
                cboDomains.Enabled = false;
                cboDomains.Visible = false;
                chkAddDomain.Visible = false;
            }

            // Check if we are in the process of adding multiple domains...            
            var serverDefinition = WorkingDefinition as ServerGeneratorDefinition;
            if (serverDefinition != null)
            {
                if (!string.IsNullOrEmpty(serverDefinition.ADDomainName))
                {
                    cboDomains.Text = serverDefinition.ADDomainName;
                }
                cboOrganizationalUnit.Text = serverDefinition.ADOrganizationalUnit;


                if (serverDefinition.ServerUGDefinition != null && serverDefinition.ServerUGDefinition.Count > 0)
                {
                    ServerUsersGroupsDefinition sugd = serverDefinition.ServerUGDefinition[0];
                    cboDomains.Text = sugd.ADDomainName;
                    cboOrganizationalUnit.Text = sugd.ADOrganizationalUnit;
                    trackNumberOfUsers.Value = sugd.NumberOfUsersToCreate;
                    trackNumberOfSecGroups.Value = sugd.NumberOfSecurityGroupsToCreate;
                    trackMaxNumberOfUsersInSecurityGroups.Value = sugd.MaxNumberOfUsersInCreatedSecurityGroups;
                }
                
            }
            else // not a server definition
            {
                cboOrganizationalUnit.Enabled = false;
                chkGenerateUsers.Checked = WorkingDefinition.GenerateUsersAndSecurityGroupsInDirectory;
                trackNumberOfUsers.Value = WorkingDefinition.NumberOfUsersToCreate;
                trackNumberOfSecGroups.Value = WorkingDefinition.NumberOfSecurityGroupsToCreate;
                trackMaxNumberOfUsersInSecurityGroups.Value = WorkingDefinition.MaxNumberOfUsersInCreatedSecurityGroups;
            }
            

            this.Show();
            this.Enabled = true;
            this.Cursor = Cursors.Default;
        }

        // Need to change the WorkingDefinition to support multiple of these...
        public override bool saveData()
        {
            WorkingDefinition.GenerateUsersAndSecurityGroupsInDirectory = chkGenerateUsers.Checked;

            WorkingDefinition.NumberOfUsersToCreate = trackNumberOfUsers.Value + WorkingDefinition.NumberOfUsersToCreate; // Keep a total in the case of multiple domains
            WorkingDefinition.NumberOfSecurityGroupsToCreate = trackNumberOfSecGroups.Value + WorkingDefinition.NumberOfSecurityGroupsToCreate;
            WorkingDefinition.MaxNumberOfUsersInCreatedSecurityGroups = trackMaxNumberOfUsersInSecurityGroups.Value;
            var serverDefinition = WorkingDefinition as ServerGeneratorDefinition;
            if (serverDefinition != null)
            {
                serverDefinition.ADDomainName = cboDomains.Text;
                serverDefinition.ADOrganizationalUnit = cboOrganizationalUnit.Text;
                serverDefinition.Fqdn = tbFqdn.Text;
                // Allow for multiple domains
                ServerUsersGroupsDefinition sugd = new ServerUsersGroupsDefinition();
                sugd.ADDomainName = cboDomains.Text;
                sugd.FQDN = tbFqdn.Text;
                sugd.ADOrganizationalUnit = cboOrganizationalUnit.Text;
                sugd.NumberOfUsersToCreate = trackNumberOfUsers.Value;
                sugd.NumberOfSecurityGroupsToCreate = trackNumberOfSecGroups.Value;
                sugd.MaxNumberOfUsersInCreatedSecurityGroups = trackMaxNumberOfUsersInSecurityGroups.Value;
                serverDefinition.ServerUGDefinition.Add(sugd);

            }

            return true;
        }

        private void chkGenerateUsers_CheckedChanged(object sender, EventArgs e)
        {
            if( chkGenerateUsers.Checked)
            {
                groupBox1.Enabled = true;
                // Added the following statements to support adding another domain. Zero out on the next screen
                trackNumberOfUsers.Value = 0;
                trackNumberOfSecGroups.Value = 0;
                trackMaxNumberOfUsersInSecurityGroups.Value = 0;
                tbFqdn.Text = "";
            }
            else
            {
                groupBox1.Enabled = false;
                WorkingDefinition.NumberOfUsersToCreate = 0;
                WorkingDefinition.NumberOfSecurityGroupsToCreate = 0;
            }
        }

        void cboDomains_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboDomains.SelectedItem == null)
            {
                return;
            }

            fillOUs();
        }


        private void cboDomains_Leave(object sender, EventArgs e)
        {
            if(cboDomains.Text == string.Empty)
            {
                return;
            }

            fillOUs();
        }

        private void fillOUs()
        {
            this.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            List<string> ous = AD.ListOU(cboDomains.Text);
            cboOrganizationalUnit.Items.Clear();
            foreach (string ou in ous)
            {
                ComboboxItem item = new ComboboxItem();
                item.Text = ou;
                item.Value = ou;
                cboOrganizationalUnit.Items.Add(item);
            }

            this.Enabled = true;
            this.Cursor = Cursors.Default;
        }

        private void trackNumberOfUsers_ValueChanged(object sender, EventArgs e)
        {
            lblNumUsers.Text = trackNumberOfUsers.Value.ToString();
        }

        private void trackNumberOfSecGroups_ValueChanged(object sender, EventArgs e)
        {
            lblGroups.Text = trackNumberOfSecGroups.Value.ToString();
        }

        private void trackMaxNumberOfUsersInSecurityGroups_ValueChanged(object sender, EventArgs e)
        {
            lblMaxNumberOfUsersInSecurityGroups.Text = trackMaxNumberOfUsersInSecurityGroups.Value.ToString();
        }

        private void chkAddDomain_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
