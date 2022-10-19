using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Click Search button
        /// </summary>
        /// <returns></returns>
        public HomePage ClickSearchButton()
        {
            TryTo.Click(SearchButton);

            return new HomePage(TryTo);
        }
        /// <summary>
        /// Validate the search text
        /// </summary>
        /// <returns></returns>
        public HomePage ValidateSearchText()
        {
            TryTo.VerifyElementTextIsCorrect(SearchTextValidation, "FADED SHORT SLEEVE T-SHIRTS");

            return new HomePage(TryTo);
        }

        /// <summary>
        /// Click the Women menu and validate after it
        /// </summary>
        /// <returns></returns>
        public HomePage ClickWomenMenu()
        {
            TryTo.Click(By.XPath("//*[@id='block_top_menu']/ul/li[1]/a"));
            TryTo.VerifyElementTextIsCorrect(By.XPath("//*[@id='center_column']/h1/span[1]"), "WOMEN");

            return new HomePage(TryTo);
        }

        /// <summary>
        /// Select values from the drop down list at sort by button
        /// </summary>
        /// <returns></returns>
        public HomePage ClickSortByButtonAndSelectValueFromDrodDown()
        {
            List<string> sortbyDropDownlistValue = new List<string>
            {
                "Price: Lowest first",
                "Price: Highest first",
                "Product Name: A to Z",
                "Product Name: A to Z",
                "In stock",
                "Reference: Lowest first",
                "Reference: Highest first"
            };

            TryTo.WaitForElementToBeVisible(By.Id("productsSortForm"));
            TryTo.LoopSelectFromDropDownList(By.XPath("//*[@id='selectProductSort']"),sortbyDropDownlistValue);

            return new HomePage(TryTo);
        }
    }
}
