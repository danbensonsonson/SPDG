﻿using System.Collections.Generic;

namespace Acceleratio.SPDG.Generator.SPModel
{
    public abstract class SPDGSecurableObject
    {
        public abstract SPDGRoleAssignment GetRoleAssignmentByPrincipal(SPDGPrincipal principal);
        public abstract void AddRoleAssignment(SPDGPrincipal principal, IEnumerable<SPDGRoleDefinition> roledefinitions);
        public abstract void RemoveRoleAssignment();
        public abstract void RemoveRoleAssignment(SPDGPrincipal principal);
        public abstract void BreakRoleInheritance(bool copyRoleAssignments);        
        public abstract bool HasUniqueRoleAssignments { get; }
        public abstract int NumUniqueRoleAssignments { get; }
    }
}
