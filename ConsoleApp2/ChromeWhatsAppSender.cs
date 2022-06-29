
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics;
using System.Drawing;

public class ChromeWhatsAppSender
{
    private IWebDriver _driver;
    private WebDriverWait wait;

    public async Task<bool> VerifyWhatssAppIsLogged()
    {
        var i = 0;
        bool logged = false;
        //try to verify 10 times if whatsapp is logged
        while (i < 10)
        {
            logged = CheckLoggedIn();
            if (logged == true)
            {
                //if whatsapp is logged then break
                break;
            }
            //wait 10 seconds before performing next verify
            await Task.Delay(10000);
        }

        return logged;
    }

    public ChromeDriver InitializeDriver()
    {
        foreach (Process p in Process.GetProcessesByName("chrome")) p.Kill();
        var chromeOptions = new ChromeOptions();
        var user = Environment.UserName;
        chromeOptions.AddExcludedArgument("enable-automation");
        chromeOptions.AddAdditionalOption("useAutomationExtension", false);
        chromeOptions.AddArgument($"--user-data-dir=C:\\Users\\{user}\\AppData\\Local\\Google\\Chrome\\User Data");
        return new ChromeDriver(chromeOptions);
    }
    public bool CheckLoggedIn()
    {
        //Thread.Sleep(5000);
        var wait = new WebDriverWait(_driver, TimeSpan.FromMinutes(1));
        var checked1 = _driver.FindElements(By.XPath("//*[@id='app']/div/div/div[2]/div[1]/div/div[2]/div")).FirstOrDefault();
        if (checked1 == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool AttachFileWithCustomMessage(string sendTo, string filePath, string message = " ")
    {
        bool isSend = false;
        try
        {

            using (_driver = InitializeDriver())
            {
                Thread.Sleep(1000);

                _driver.Navigate().GoToUrl("https://web.whatsapp.com");

                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

                var isWhatsappLogged = VerifyWhatssAppIsLogged().ConfigureAwait(false).GetAwaiter().GetResult();

                if (!isWhatsappLogged)
                {
                    throw new Exception($"Whatssap not found logged while trying to send a message to {sendTo}");
                }

                var mobile = sendTo;

                var msg = message;
                _driver.Navigate().GoToUrl("https://api.whatsapp.com/send?phone=" + mobile + "&text=" + Uri.EscapeDataString(msg));
                //Thread.Sleep(3000);
                var wait = new WebDriverWait(_driver, TimeSpan.FromMinutes(1));
                var actionButtonElement = _driver.FindElement(By.Id("action-button"));
                wait.Until(ElementIsClickable(actionButtonElement));
                actionButtonElement.Click();
                //_driver.FindElement(By.Id("action-button")).Click();

                _driver.FindElement(By.LinkText("use WhatsApp Web")).Click();

                _driver.FindElement(By.CssSelector("span[data-icon='clip']")).Click();

                //add file path
                _driver.FindElement(By.CssSelector("input[type='file']")).SendKeys(filePath);

                //send attachment
                _driver.FindElement(By.XPath("//*[@id='app']/div/div/div[2]/div[2]/span/div/span/div/div/div[2]/div/div[2]/div[2]/div/div")).Click(); //Click SEND Arrow Button

                if (message != " ")
                {
                    //send text
                    var sendButtonElement = _driver.FindElement(By.CssSelector("#main > footer > div._2BU3P.tm2tP.copyable-area > div > span:nth-child(2) > div > div._2lMWa > div._3HQNh._1Ae7k > button")); //Click SEND Arrow Button
                    wait.Until(ElementIsClickable(sendButtonElement));
                    sendButtonElement.Click();
                }

                isSend = true;
                Thread.Sleep(1000);

            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return isSend;

    }
    public bool SendTextMessage(string sendTo, string message)
    {
        bool isSend = false;
        try
        {

            using (_driver = InitializeDriver())
            {
                Thread.Sleep(1000);

                _driver.Navigate().GoToUrl("https://web.whatsapp.com");

                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                //first of all we are going to verify if whatssap is logged or not
                var isWhatsappLogged = VerifyWhatssAppIsLogged().ConfigureAwait(false).GetAwaiter().GetResult();

                if (!isWhatsappLogged)
                {
                    throw new Exception($"Whatssap not found logged while trying to send a message to {sendTo}");
                }

                var mobile = sendTo;

                //go to url used for sending the message, this is no an external API or something, this is used by Whatsapp, intern, to send message.
                var msg = message;
                _driver.Navigate().GoToUrl("https://api.whatsapp.com/send?phone=" + mobile + "&text=" + Uri.EscapeDataString(msg));
                //Thread.Sleep(2000);

                //from now on it effectively imitates the way a person works
                //var element = _driver.FindElement(By.Id("action-button"));
                var wait = new WebDriverWait(_driver, TimeSpan.FromMinutes(1));
                var actionButtonElement = _driver.FindElement(By.Id("action-button"));
                wait.Until(ElementIsClickable(actionButtonElement));
                actionButtonElement.Click();

                var useWhatAappWebElement = _driver.FindElement(By.LinkText("use WhatsApp Web"));
                wait.Until(ElementIsClickable(useWhatAappWebElement));
                useWhatAappWebElement.Click();
                Thread.Sleep(1000);

                var sendButtonElement = _driver.FindElement(By.CssSelector("#main > footer > div._2BU3P.tm2tP.copyable-area > div > span:nth-child(2) > div > div._2lMWa > div._3HQNh._1Ae7k > button")); //Click SEND Arrow Button
                wait.Until(ElementIsClickable(sendButtonElement));
                sendButtonElement.Click();
                isSend = true;
                Console.WriteLine("Message Send Successfully ");
                Thread.Sleep(1000);

            }
        }
        catch (Exception ex)
        { throw ex; }

        return isSend;

    }
    public static Func<IWebDriver, IWebElement> ElementIsClickable(IWebElement element)
    {
        if (element is null) return null;
        return driver =>
        {
            return (element != null && element.Displayed && element.Enabled) ? element : null;
        };
    }

}
