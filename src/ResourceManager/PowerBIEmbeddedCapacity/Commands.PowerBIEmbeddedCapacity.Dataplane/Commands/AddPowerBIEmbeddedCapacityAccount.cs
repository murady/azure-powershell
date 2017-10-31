﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Microsoft.Azure.Commands.Common.Authentication;
using Microsoft.Azure.Commands.Common.Authentication.Models;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Security;
using Microsoft.Azure.Commands.PowerBIEmbeddedCapacity.Dataplane.Properties;
using Microsoft.WindowsAzure.Commands.Utilities.Common;
using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using Microsoft.WindowsAzure.Commands.Common;
using System;

namespace Microsoft.Azure.Commands.PowerBIEmbeddedCapacity.Dataplane
{
    /// <summary>
    /// Cmdlet to log into an PowerBI Embedded Capacity environment
    /// </summary>
    [Cmdlet("Add", "AzurePowerBIEmbeddedCapacityAccount", DefaultParameterSetName = "UserParameterSetName", SupportsShouldProcess =true)]
    [Alias("Login-AzurePowerBIEmbeddedCapacityAccount")]
    [OutputType(typeof(PBIProfile))]
    public class AddAzurePBIEAccountCommand : AzurePSCmdlet, IModuleAssemblyInitializer
    {
        private const string UserParameterSet = "UserParameterSetName";
        private const string ServicePrincipalWithPasswordParameterSet = "ServicePrincipalWithPasswordParameterSetName";
        private const string ServicePrincipalWithCertificateParameterSet = "ServicePrincipalWithCertificateParameterSetName";

        [Parameter(ParameterSetName = UserParameterSet,
            Mandatory = false, HelpMessage = "Name of the Azure PowerBI Embedded Capacity environment to which to logon to", Position = 0)]
        [Parameter(ParameterSetName = ServicePrincipalWithPasswordParameterSet,
            Mandatory = true, HelpMessage = "Name of the Azure PowerBI Embedded Capacity environment to which to logon to")]
        [Parameter(ParameterSetName = ServicePrincipalWithCertificateParameterSet,
            Mandatory = true, HelpMessage = "Name of the Azure PowerBI Embedded Capacity environment to which to logon to")]
        public string RolloutEnvironment { get; set; }

        [Parameter(ParameterSetName = UserParameterSet,
            Mandatory = false, HelpMessage = "Login credentials to the Azure PowerBI Embedded Capacity environment", Position = 1)]
        [Parameter(ParameterSetName = ServicePrincipalWithPasswordParameterSet,
            Mandatory = true, HelpMessage = "Login credentials to the Azure PowerBI Embedded Capacity environment")]
        public PSCredential Credential { get; set; }

        [Parameter(ParameterSetName = ServicePrincipalWithPasswordParameterSet,
            Mandatory = true)]
        [Parameter(ParameterSetName = ServicePrincipalWithCertificateParameterSet,
            Mandatory = true)]
        public SwitchParameter ServicePrincipal { get; set; }

        [Parameter(ParameterSetName = ServicePrincipalWithPasswordParameterSet,
            Mandatory = true, HelpMessage = "Tenant name or ID")]
        [Parameter(ParameterSetName = ServicePrincipalWithCertificateParameterSet,
            Mandatory = true, HelpMessage = "Tenant name or ID")]
        [ValidateNotNullOrEmpty]
        public string TenantId { get; set; }

        [Parameter(ParameterSetName = ServicePrincipalWithCertificateParameterSet,
            Mandatory = true, HelpMessage = "The application ID.")]
        [ValidateNotNullOrEmpty]
        public string ApplicationId { get; set; }

        [Parameter(ParameterSetName = ServicePrincipalWithCertificateParameterSet,
            Mandatory = true, HelpMessage = "Certificate Hash (Thumbprint)")]
        [ValidateNotNullOrEmpty]
        public string CertificateThumbprint { get; set; }

        protected PBIEnvironment AsEnvironment;

        protected override IAzureContext DefaultContext
        {
            get
            {
                // Nothing to do with Azure Resource Managment context
                return null;
            }
        }

        protected override string DataCollectionWarning
        {
            get
            {
                return Resources.ARMDataCollectionMessage;
            }
        }

        protected override void BeginProcessing()
        {
            this._dataCollectionProfile = new AzurePSDataCollectionProfile(false);

            if (string.IsNullOrEmpty(RolloutEnvironment))
            {
                RolloutEnvironment = PBIClientSession.GetDefaultEnvironmentName();
            }

            if (PBIClientSession.Instance.Profile.Environments.ContainsKey(RolloutEnvironment))
            {
                AsEnvironment = (PBIEnvironment)PBIClientSession.Instance.Profile.Environments[RolloutEnvironment];
            }
            else
            {
                AsEnvironment = PBIClientSession.Instance.Profile.CreateEnvironment(RolloutEnvironment);
            }
            base.BeginProcessing();
        }

        protected override void InitializeQosEvent()
        {
            // nothing to do here.
        }

        protected override void SetupDebuggingTraces()
        {
            // nothing to do here.
        }

        protected override void TearDownDebuggingTraces()
        {
            // nothing to do here.
        }

        protected override void SetupHttpClientPipeline()
        {
            // nothing to do here.
        }

        protected override void TearDownHttpClientPipeline()
        {
            // nothing to do here.
        }

        public override void ExecuteCmdlet()
        {
            PBIAccount azureAccount = new PBIAccount();
            azureAccount.Type = ServicePrincipal ? PBIAccount.AccountType.ServicePrincipal : PBIAccount.AccountType.User;

            SecureString password = null;
            if (Credential != null)
            {
                azureAccount.Id = Credential.UserName;
                password = Credential.Password;
            }

            if (ServicePrincipal)
            {
                azureAccount.Tenant = TenantId;

                if (!string.IsNullOrEmpty(ApplicationId))
                {
                    azureAccount.Id = ApplicationId;
                }
                if (!string.IsNullOrEmpty(CertificateThumbprint))
                {
                    azureAccount.CertificateThumbprint = CertificateThumbprint;
                }
            }

            if (ShouldProcess(string.Format(Resources.LoginTarget, AsEnvironment.Name), "log in"))
            {
                var currentProfile = PBIClientSession.Instance.Profile;
                var currentContext = currentProfile.Context;

                // If there is no current context create one. If there is one already then
                // if the current credentials (userid) match the one that is already in context then use it.
                // if either the userid that is logging in or the environment to which login is happening is
                // different than the one in the context then clear the current context and proceed to login.
                // At any given point in time, we should only have one context i.e. one user logged in to one
                // environment.
                if (currentContext == null || Credential == null ||
                    string.IsNullOrEmpty(currentContext.Account.Id) || 
                    !currentContext.Account.Id.Equals(Credential.UserName) ||
                    !RolloutEnvironment.Equals(currentContext.Environment.Name))
                {
                    PBIClientSession.Instance.SetCurrentContext(azureAccount, AsEnvironment);
                }

                var asAzureProfile = PBIClientSession.Instance.Login(currentProfile.Context, password);

                WriteObject(asAzureProfile);
            }
        }

        public void OnImport()
        {
            try
            {
                System.Management.Automation.PowerShell invoker = null;
                invoker = System.Management.Automation.PowerShell.Create(RunspaceMode.CurrentRunspace);
                invoker.AddScript(File.ReadAllText(FileUtilities.GetContentFilePath(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "PBIEDataplaneStartup.ps1")));
                invoker.Invoke();
            }
            catch
            {
                // This will throw exception for tests, ignore.
            }
        }
    }
}