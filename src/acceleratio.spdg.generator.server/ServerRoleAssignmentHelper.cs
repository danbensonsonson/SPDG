﻿using System;
using System.Collections.Generic;
using System.Linq;
using Acceleratio.SPDG.Generator.Server.SPModel;
using Acceleratio.SPDG.Generator.SPModel;
using Microsoft.SharePoint;

namespace Acceleratio.SPDG.Generator.Server
{
    class ServerRoleAssignmentHelper
    {
        public static void AddRoleAssignment(SPSecurableObject securableObject, SPDGPrincipal principal, IEnumerable<SPDGRoleDefinition> roleDefinitions)
        {
            SPPrincipal spPrincipal;
            if (principal is SPDGServerUser)
            {
                spPrincipal = ((SPDGServerUser)principal).SPUser;
            }
            else
            {
                spPrincipal = ((SPDGServerGroup)principal).Group;
            }
            SPRoleAssignment roleAss = new SPRoleAssignment(spPrincipal);
            foreach (var spdgRoleDefinition in roleDefinitions)
            {
                var spRoleDef = ((SPDGServerRoleDefinition)spdgRoleDefinition).RoleDefinition;
                roleAss.RoleDefinitionBindings.Add(spRoleDef);
            }
            securableObject.RoleAssignments.Add(roleAss);            
        }

        public static void RemoveRoleAssignment(SPSecurableObject securableObject)
        {
            int count = securableObject.RoleAssignments.Count;
            int index = SampleData.GetRandomNumber(0, count - 1);
            securableObject.RoleAssignments.Remove(index);
        }

        public static void RemoveRoleAssignment(SPSecurableObject securableObject, SPDGPrincipal principal)
        {
            SPPrincipal spPrincipal;
            if (principal is SPDGServerUser)
            {
                spPrincipal = ((SPDGServerUser)principal).SPUser;
            }
            else
            {
                spPrincipal = ((SPDGServerGroup)principal).Group;
            }
            try
            {
                securableObject.RoleAssignments.Remove(spPrincipal);
            }
            catch (Exception ex)
            {
                Errors.Log(ex);
            }
        }

        public static SPDGRoleAssignment GetRoleAssignmentByPrincipal(SPSecurableObject securableObject, SPDGPrincipal principal)
        {
            SPPrincipal spPrincipal;
            if (principal is SPDGServerUser)
            {
                spPrincipal = ((SPDGServerUser)principal).SPUser;
            }
            else
            {
                spPrincipal = ((SPDGServerGroup)principal).Group;
            }
            try
            {
                var spRoleAss = securableObject.RoleAssignments.GetAssignmentByPrincipal(spPrincipal);
                return new SPDGServerRoleAssignment(spRoleAss, principal, spRoleAss.RoleDefinitionBindings.Cast<SPRoleDefinition>().Select(x => (SPDGRoleDefinition)new SPDGServerRoleDefinition(x)));
            }
            catch (Exception)
            {
                //ugly but will do
                return null;
            }
        }
    }
}
