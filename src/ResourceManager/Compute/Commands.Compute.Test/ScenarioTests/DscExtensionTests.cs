﻿using Microsoft.WindowsAzure.Commands.ScenarioTest;
using Xunit;

namespace Microsoft.Azure.Commands.Compute.Test.ScenarioTests
{
    public class DscExtensionTests
    {
        [Fact(Skip = "TODO, [#108248038]: Enable scenario tests")]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestGetAzureRmVMDscExtension()
        {
            ComputeTestController.NewInstance.RunPsTest("Test-GetAzureRmVMDscExtension");
        }
    }
}