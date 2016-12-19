using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VDITroubleshooter.BL;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace VDITroubleshooter.BLTest
{
    // test comment
    [TestClass]
    public class VirtualDesktopTest
    {
        [TestMethod]
        public void ValidateVirtualDesktop_DefaultConstructor_NoError()
        {
            // Arrange
            var virtualDesktop = new VirtualDesktop()
            {
                HostedMachineName = "hmnValue",
                AdminAddress = "aaValue",
                SessionState = "stateValue",
                DesktopGroupName = "dgnValue"
            };
            
            // Act
            bool isValid = Validator.TryValidateObject(virtualDesktop, new ValidationContext(virtualDesktop), null, true);

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void ValidateVirtualDesktop_OverloadStrStrStrStr_NoError()
        {
            // Arrange
            var virtualDesktop = new VirtualDesktop("hmnValue", "aaValue", "stateValue", "dgnValue");

            // Act
            bool isValid = Validator.TryValidateObject(virtualDesktop, new ValidationContext(virtualDesktop), null, true);

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void ToString_NullDesktopGroup()
        {
            // Arrange
            string expected = "hmnValue (stateValue)";

            var desktop = new VirtualDesktop()
            {
                HostedMachineName = "hmnValue",
                SessionState = "stateValue"
            };

            // Act
            string actual = desktop.ToString();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreNotSame(expected, actual);
        }

        [TestMethod]
        public void ToString_NullSessionState()
        {
            // Arrange
            string expected = "hmnValue - dgnValue";

            var desktop = new VirtualDesktop()
            {
                HostedMachineName = "hmnValue",
                DesktopGroupName = "dgnValue"
            };

            // Act
            string actual = desktop.ToString();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreNotSame(expected, actual);
        }

        [TestMethod]
        public void ToString_NullHostedMachineName()
        {
            // Arrange
            string expected = "";

            var desktop = new VirtualDesktop()
            {
                SessionState = "stateValue",
                DesktopGroupName = "dgnValue"
            };

            // Act
            string actual = desktop.ToString();

            // Assert
            Assert.AreSame(expected, actual);
        }
    }
}
