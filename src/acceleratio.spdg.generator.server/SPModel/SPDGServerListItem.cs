﻿using System.Collections.Generic;
using System.Linq;
using Acceleratio.SPDG.Generator.SPModel;
using Microsoft.SharePoint;

namespace Acceleratio.SPDG.Generator.Server.SPModel
{
    public class SPDGServerListItem: SPDGListItem
    {
        private readonly SPListItem _item;
        private ISPDGListItemAttachmentInfo _attachment;

        public SPDGServerListItem(SPListItem item)
        {
            _item = item;
        }

        public override object this[string fieldName]
        {
            get { return _item[fieldName]; }
            set { _item[fieldName] = value; }
        }

        public override IEnumerable<string> GetAvailableFields()
        {
            return _item.Fields.Cast<SPField>().Select(x => x.InternalName);
        }

        public override SPDGRoleAssignment GetRoleAssignmentByPrincipal(SPDGPrincipal principal)
        {
            return ServerRoleAssignmentHelper.GetRoleAssignmentByPrincipal(_item, principal);
        }

        public override void AddRoleAssignment(SPDGPrincipal principal, IEnumerable<SPDGRoleDefinition> roledefinitions)
        {
            ServerRoleAssignmentHelper.AddRoleAssignment(_item, principal, roledefinitions);
        }

        public override void BreakRoleInheritance(bool copyRoleAssignments)
        {
            _item.BreakRoleInheritance(copyRoleAssignments);
        }

        public override bool HasUniqueRoleAssignments
        {
            get { return _item.HasUniqueRoleAssignments; }
        }

        public override int Id
        {
            get { return _item.ID; }
        }

        public override string DisplayName
        {
            get { return _item.DisplayName; }
        }

        public override void Update()
        {
            _item.Update();
        }

        public override bool SupportsSharing { get; }
        public override void ShareWithPeople(IEnumerable<string> emails, bool isEdit)
        {
            // nothing for on prem
        }

        public override ISPDGListItemAttachmentInfo Attachment
        {
            set { _attachment = value; }
            get { return _attachment; }
        }
    }
}
