using System;
using System.Configuration;
using System.Diagnostics;
using System.Text.RegularExpressions;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace AutomationFrameworkWithFluent.Framework.Common
{
    public abstract class TestBase : IDisposable
    {
        #region Driver Setup
        private static readonly string config = ConfigurationManager.AppSettings.Get("selenium.browser.shutdown");
        public IWebDriver driver;

        /// <summary>
        /// Gets the Driver from Config file
        /// </summary>

        protected TestBase()
        {
            driver = CreateBrowserDriver(ConfigurationManager.AppSettings.Get("selenium.browser.driver"));
        }
        #endregion Driver Setup

        /// <summary>
        /// Dispose the test after we are done
        /// </summary>
        public virtual void Dispose()
        {
            if (ShouldCloseBroswerAfterTest())
                driver.Quit();
        }

        /// <summary>
        /// Generate a browser driver for test
        /// </summary>
        /// <param name="BrowserDriver"></param>
        /// <returns></returns>
        private static IWebDriver CreateBrowserDriver(string browserDriver)
        {
            if (browserDriver == null)
                return new ChromeDriver();

            switch(browserDriver)
            {
                case "chrome":
                    ChromeOptions chrome = new ChromeOptions();
                    chrome.AddArgument("--start-maximized");
                    return new ChromeDriver(chrome);

                case "headless":
                    ChromeOptions headless = new ChromeOptions();
                    headless.AddArguments("--headless", "--start-maximized");
                    return new ChromeDriver(headless);

                case "firefox": return new FirefoxDriver();

                case "grid":
                    ChromeOptions remoteDriver = new ChromeOptions();
                    remoteDriver.AddArguments("--headless", "--start-maximized");
                    return new RemoteWebDriver(new Uri("enter your cloud address to run test in cloud"), remoteDriver.ToCapabilities(), TimeSpan.FromSeconds(60));

                default:
                    throw new Exception("Cannot find browser driver with the name: " + browserDriver);

            }
        }

        /// <summary>
        /// Close the browser after test
        /// </summary>
        /// <returns></returns>
        private bool ShouldCloseBroswerAfterTest()
        {
            if (!IsHeadlessMode() && config != null)
                return Convert.ToBoolean(config);
            return true;
        }

        /// <summary>
        /// Are we running in a mode we cant see, mainly for force close of browser.
        /// </summary>
        /// <returns></returns>
        private bool IsHeadlessMode()
        {
            return ConfigurationManager.AppSettings.Get("selenium.browser.driver") == "headless" || ConfigurationManager.AppSettings.Get("selenium.browser.driver") == "grid";
        }

        /// <summary>
        /// Open an browser in the browser
        /// </summary>
        /// <param name="url"></param>
        public void VisitUrl(string url)
        {
            try
            {
                driver.Navigate().GoToUrl(url);
            }
            catch (Exception)
            {
                throw new Exception(" The url was opened { " + driver.Url.ToString() + " } But was expecting this Url - " + url);
            }
        }

        /// <summary>
        /// Used to Wait for element to be clickable then move to the element
        /// </summary>
        /// <param name="selector"></param>
        private void WaitForElementTobeClickableAndMoveToElement(By selector)
        {
            IWebElement element = Wait().Until(ExpectedConditions.ElementToBeClickable(selector));
            MoveToElement(selector);

            element.Click();

        }

        /// <summary>
        /// This moves to the specific element
        /// </summary>
        /// <param name="selector"></param>
        public void MoveToElement(By selector)
        {
            try
            {
                IWebElement onlyElement = driver.FindElement(selector);
                Action().MoveToElement(onlyElement).Perform();
            }
            catch (Exception)
            {
                throw new Exception("Could not find element to move to it - " + selector);
            }
        }

        /// <summary>
        /// Proper use of this is as per below
        /// Example: TryTo.Action().SendKeys(Keys.Arrowdown)
        /// </summary>
        /// <returns></returns>
        public Actions Action()
        {
            return new Actions(driver);
        }

        /// <summary>
        /// This is a polling method to wait for an Expected Condition and periodically check it and fail if time runs out
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public WebDriverWait Wait(int seconds = 15)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds))
            {
                PollingInterval = TimeSpan.FromMilliseconds(200)
            };
            return wait;
        }

        /// <summary>
        /// Press the button with the given selector.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public TestBase Click(By selector)
        {
            try
            {
                RetryTimer(() =>
                {
                    try
                    {
                        WaitForElementTobeClickableAndMoveToElement(selector);
                    }
                    catch (ElementNotVisibleException)
                    {
                        WaitForElementTobeClickableAndMoveToElement(selector);
                    }
                    catch (StaleElementReferenceException)
                    {
                        WaitForElementTobeClickableAndMoveToElement(selector);
                    }
                });
            }
            catch (Exception)
            {
                throw new Exception("Unable to click element - " + selector);
            }

            return this;
        }

        /// <summary>
        /// This is a wrapper retry function ..it will retry a function for the set amount of seconds then fail
        /// </summary>
        /// <param name="function"></param>
        public static void RetryTimer(Action function)
        {
            int seconds = 10;
            bool elementFound;
            Stopwatch stopWatch = new Stopwatch();

            stopWatch.Start();

            while (stopWatch.Elapsed < TimeSpan.FromSeconds(seconds))
            {
                try
                {
                    function();
                    elementFound = true;
                }
                catch (Exception)
                {
                    if (stopWatch.Elapsed <= TimeSpan.FromSeconds(seconds))
                    {
                        elementFound = false;
                    }
                    else
                    {
                        throw;
                    }
                }

                if (elementFound)
                    break;
            }
            stopWatch.Reset();
        }

        /// <summary>
        /// Waits for element to be visible
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public IWebElement WaitForElementToBeVisible(By selector)
        {
            try
            {
                Wait().Until(ExpectedConditions.ElementIsVisible(selector));

                return driver.FindElement(selector);
            }
            catch (Exception)
            {
                throw new Exception("This element is not visible when it should be - " + selector);
            }
        }

        /// <summary>
        /// Type the given value in the given field without clearing it.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public TestBase Append(By selector, string value)
        {
            try
            {
                WaitForElementToBeVisible(selector).SendKeys(value);
            }
            catch (Exception)
            {
                throw new Exception("Could not append this text { " + value + " } to this - " + selector);
            }

            return this;
        }


        /// <summary>
        /// Clear the given field
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public TestBase Clear(By selector)
        {
            try
            {
                WaitForElementToBeVisible(selector).Clear();
            }
            catch (Exception)
            {
                throw new Exception("Could not clear text from this Element - " + selector);
            }

            return this;
        }
        /// <summary>
        /// Type the given value in the given field.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public TestBase Type(By selector, string value)
        {
            try
            {
                Clear(selector).Append(selector, value);
            }
            catch (InvalidElementStateException)
            {
                Append(selector, value);
            }
            catch (Exception)
            {
                throw new Exception("Could not enter this text { " + value + " } to this - " + selector);
            }

            return this;
        }

        /// <summary>
        /// This takes the text of an element and compares it against text given to verify if its correct
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="elementText"></param>
        public void VerifyElementTextIsCorrect(By selector, string elementText)
        {
            string actualElementText = "";
            string noWhiteSpaceExpectedElementText = "";
            string noWhiteSpaceActualElementText = "";
            int breakWhileLoopCounter = 100;

            try
            {
                RetryTimer(() =>
                {
                    while (true)
                    {
                        actualElementText = driver.FindElement(selector).Text;

                        noWhiteSpaceExpectedElementText = Regex.Replace(elementText, " ", "");
                        noWhiteSpaceActualElementText = Regex.Replace(actualElementText, " ", "");


                        if (noWhiteSpaceActualElementText == noWhiteSpaceExpectedElementText)
                            break;

                        breakWhileLoopCounter--;

                        if (breakWhileLoopCounter == 0)
                            break;
                    }
                });

                noWhiteSpaceActualElementText.Should().Contain(noWhiteSpaceExpectedElementText);

            }
            catch (Exception)
            {
                throw new Exception("Unable to find this text { " + elementText + " } In this text  - " + actualElementText);
            }
        }

        /// <summary>
        /// Select an option from the dropdown list, can pass value or text
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="option"></param>

        public void Select(By selector, string option)
        {
            RetryTimer(() =>
            {
                try
                {
                    Click(selector);

                    try
                    {
                        WaitForElementTextToBeVisibleThenClickByText(selector, option);
                    }
                    catch (ElementNotVisibleException)
                    {
                        WaitForElementTextToBeVisibleThenClickByText(selector, option);
                    }
                    catch (StaleElementReferenceException)
                    {
                        WaitForElementTextToBeVisibleThenClickByText(selector, option);
                    }
                }
                catch (NoSuchElementException)
                {
                    SelectElement element = new SelectElement(WaitForElementToBeVisible(selector));
                    element.SelectByValue(option);
                }
                catch (TimeoutException)
                {
                    throw new Exception("Unable to find this value {" + option + "} in dropdwon - "+ selector);
                }
            });
        }

        /// <summary>
        /// This is used to wait for element to be visible then check that the text is present in the element and then select option by Text
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="option"></param>
        private void WaitForElementTextToBeVisibleThenClickByText(By selector, string option)
        {
            RetryTimer(() =>
            {
                SelectElement element = new SelectElement(WaitForElementToBeVisible(selector));
                Wait().Until(ExpectedConditions.TextToBePresentInElementLocated(selector, option));

                element.SelectByText(option);
            });

        }
    }
}
