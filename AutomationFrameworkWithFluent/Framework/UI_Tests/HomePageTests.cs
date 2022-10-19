using System;
using Allure.Xunit.Attributes;
using AutomationFrameworkWithFluent.Framework.Common;
using AutomationFrameworkWithFluent.Framework.POM;
using Xunit;

namespace AutomationFrameworkWithFluent
{

    [AllureSuite("Shopping-Search and sort by multiple times")]
    [Collection("Shopping-Search and sort by multiple times")]
    public class HomePageTests : TestBase
    {
        [AllureFeature("Open website and search a T-shirt")]
        [Fact]
        public void OpenAndSearchTShirt()
        {
            new HomePage(this, true)
                .EnterValueintheSearchField()
                .ClickSearchButton()
                .ValidateSearchText();
        }

        [AllureStory("Select multiple time iteratively from the drop down list")]
        [Fact]
        public void SelectValuesFromDropDownList()
        {
            new HomePage(this, true)
                .ClickWomenMenu()
                .ClickSortByButtonAndSelectValueFromDrodDown();
        }
    }
}
