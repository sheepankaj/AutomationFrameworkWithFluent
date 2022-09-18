using System;
using AutomationFrameworkWithFluent.Framework.Common;
using AutomationFrameworkWithFluent.Framework.POM;
using Xunit;

namespace AutomationFrameworkWithFluent
{
    public class HomePageTests : TestBase
    {
        [Fact]
        public void OpenAndSearchTShirt()
        {
            new HomePage(this, true)
                .EnterValueintheSearchField()
                .ClickSearchButton()
                .ValidateSearchText();
        }
    }
}
