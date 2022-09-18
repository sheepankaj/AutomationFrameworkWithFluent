using System;
using AutomationFrameworkWithFluent.Framework.Common;
using OpenQA.Selenium;

namespace AutomationFrameworkWithFluent.Framework.POM
{
    public class HomePage
    {
        #region Page Driver Setup
        /// <summary>
        /// Giving the Driver to class
        /// </summary>
        private readonly TestBase TryTo;

        private readonly static By SearchField = By.Id("search_query_top");
        private readonly static By SearchButton = By.Name("submit_search");
        private readonly static By SearchTextValidation = By.XPath("//*[@id='center_column']/h1/span[1]");

        /// <summary>
        /// Opening the Home page of the URL
        /// </summary>
        /// <param name="test"></param>
        /// <param name="startingPage"></param>
        public HomePage(TestBase test, bool startingPage = false)
        {
            TryTo = test;

            if(startingPage)
            {
                test.VisitUrl("http://automationpractice.com/index.php");
            }
        }
        #endregion

        /// <summary>
        /// Enter value to search
        /// </summary>
        /// <returns></returns>
        public HomePage EnterValueintheSearchField()
        {
            TryTo.Type(SearchField, "Faded Short Sleeve T-shirts");

            return new HomePage(TryTo);
        }

        public HomePage ClickSearchButton()
        {
            TryTo.Click(SearchButton);

            return new HomePage(TryTo);
        }

        public HomePage ValidateSearchText()
        {
            TryTo.VerifyElementTextIsCorrect(SearchTextValidation, "FADED SHORT SLEEVE T-SHIRTS");

            return new HomePage(TryTo);
        }
    }
}
