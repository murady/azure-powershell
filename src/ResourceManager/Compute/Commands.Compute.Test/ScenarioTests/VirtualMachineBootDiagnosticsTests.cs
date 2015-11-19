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

using Microsoft.WindowsAzure.Commands.ScenarioTest;
using Xunit;

namespace Microsoft.Azure.Commands.Compute.Test.ScenarioTests
{
    public partial class VirtualMachineBootDiagnosticsTests
    {
        [Fact(Skip = "TODO: only works for live mode")]
        [Trait(Category.RunType, Category.LiveOnly)]
        public void TestVirtualMachineBootDiagnostics()
        {
            ComputeTestController.NewInstance.RunPsTest("Test-VirtualMachineBootDiagnostics");
        }


        [Fact(Skip = "TODO, [#108248038]: Enable scenario tests")]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestVirtualMachineBootDiagnosticsPremium()
        {
            ComputeTestController.NewInstance.RunPsTest("Test-VirtualMachineBootDiagnosticsPremium");
        }

        [Fact(Skip = "TODO, [#108248038]: Enable scenario tests")]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestLinuxVirtualMachineBootDiagnostics()
        {
            ComputeTestController.NewInstance.RunPsTest("Test-LinuxVirtualMachineBootDiagnostics");
        }

        [Fact(Skip = "TODO, [#108248038]: Enable scenario tests")]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestVirtualMachineBootDiagnosticsSet()
        {
            ComputeTestController.NewInstance.RunPsTest("Test-VirtualMachineBootDiagnosticsSet");
        }
    }
}