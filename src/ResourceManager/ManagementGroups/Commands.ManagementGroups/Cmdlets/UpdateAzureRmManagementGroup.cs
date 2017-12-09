﻿using System.Management.Automation;
using Microsoft.Azure.Commands.ManagementGroups.Common;
using Microsoft.Azure.Management.ManagementGroups;
using Microsoft.Azure.Management.ManagementGroups.Models;
using Microsoft.Azure.Commands.ManagementGroups.Models;

namespace Microsoft.Azure.Commands.ManagementGroups.Cmdlets
{
    [Cmdlet("Update", "AzureRmManagementGroup", DefaultParameterSetName = Constants.ParameterSetNames.GroupOperationsParameterSet, SupportsShouldProcess = false, ConfirmImpact = ConfirmImpact.Medium), OutputType(typeof(string))]
    public class UpdateAzureRmManagementGroup : AzureManagementGroupsCmdletBase
    {
        [Parameter(ParameterSetName = Constants.ParameterSetNames.GroupOperationsParameterSet, Mandatory = true, HelpMessage = Constants.HelpMessages.GroupId, Position = 0)]
        [ValidateNotNullOrEmpty]
        public string GroupId { get; set; }

        [Parameter(ParameterSetName = Constants.ParameterSetNames.GroupOperationsParameterSet, Mandatory = false,
            HelpMessage = Constants.HelpMessages.GroupId, Position = 1)]
        [ValidateNotNullOrEmpty]
        public string DisplayName { get; set; } = null;

        [Parameter(ParameterSetName = Constants.ParameterSetNames.GroupOperationsParameterSet, Mandatory = false,
            HelpMessage = Constants.HelpMessages.GroupId, Position = 2)]
        [ValidateNotNullOrEmpty]
        public string ParentId { get; set; } = null;

        public override void ExecuteCmdlet()
        {
            try
            {
                CreateGroupRequest createGroupRequest = new CreateGroupRequest(DisplayName, ParentId);
                ManagementGroupsApiClient.GroupId = GroupId;

                var response = ManagementGroupsApiClient.ManagementGroups.Update(createGroupRequest);
                WriteObject(new PSManagementGroup(response));

            }
            catch (ErrorResponseException ex)
            {
                Utility.HandleErrorResponseException(ex);
            }
        }
    }
}
