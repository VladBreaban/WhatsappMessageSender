
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics;
using System.Drawing;

public class ChromeWhatsAppSender
{
    private IWebDriver _driver;

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
        Thread.Sleep(5000);
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
                _driver.Manage().Window.Position = new Point(-2000, 0);

                Thread.Sleep(3000);

                _driver.Navigate().GoToUrl("https://web.whatsapp.com");

                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);

                var isWhatsappLogged = VerifyWhatssAppIsLogged().ConfigureAwait(false).GetAwaiter().GetResult();

                if (!isWhatsappLogged)
                {
                    throw new Exception($"Whatssap not found logged while trying to send a message to {sendTo}");
                }

                var mobile = sendTo;

                var msg = message;
                _driver.Navigate().GoToUrl("https://api.whatsapp.com/send?phone=" + mobile + "&text=" + Uri.EscapeDataString(msg));
                Thread.Sleep(3000);
                _driver.FindElement(By.Id("action-button")).Click();

                _driver.FindElement(By.LinkText("use WhatsApp Web")).Click();
                //add a delay
                var errormsg = _driver.FindElements(By.XPath("//*[@id='app']/div/span[2]/div/span/div/div/div/div/div/div[2]/div/div/div/div")).SingleOrDefault();
                if (errormsg != null)
                {
                    Thread.Sleep(2000);
                    var messageshow = _driver.FindElement(By.XPath("//*[@id='app']/div/span[2]/div/span/div/div/div/div/div/div[1]")).Text;

                    throw new Exception("Message Failed Due To " + messageshow);

                }
                else
                {
                    _driver.FindElement(By.CssSelector("span[data-icon='clip']")).Click();

                    //add file path
                    _driver.FindElement(By.CssSelector("input[type='file']")).SendKeys(filePath);

                    //send attachment
                    _driver.FindElement(By.XPath("//*[@id='app']/div/div/div[2]/div[2]/span/div/span/div/div/div[2]/div/div[2]/div[2]/div/div")).Click(); //Click SEND Arrow Button

                    if (message != " ")
                    {
                        //send text
                        _driver.FindElement(By.CssSelector("#main > footer > div._2BU3P.tm2tP.copyable-area > div > span:nth-child(2) > div > div._2lMWa > div._3HQNh._1Ae7k > button")).Click(); //Click SEND Arrow Button
                    }

                    isSend = true;
                    Thread.Sleep(2000);

                }

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
                _driver.Manage().Window.Position= new Point(-2000, 0);
                Thread.Sleep(3000);

                _driver.Navigate().GoToUrl("https://web.whatsapp.com");

                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
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
                Thread.Sleep(2000);

                //from now on it effectively imitates the way a person works
                _driver.FindElement(By.Id("action-button")).Click();

                _driver.FindElement(By.LinkText("use WhatsApp Web")).Click();

                var errormsg = _driver.FindElements(By.XPath("//*[@id='app']/div/span[2]/div/span/div/div/div/div/div/div[2]/div/div/div/div")).SingleOrDefault();
                if (errormsg != null)
                {
                    Thread.Sleep(2000);
                    var messageshow = _driver.FindElement(By.XPath("//*[@id='app']/div/span[2]/div/span/div/div/div/div/div/div[1]")).Text;

                    throw new Exception("Message Failed Due To " + messageshow);

                }
                else
                {
                    _driver.FindElement(By.CssSelector("#main > footer > div._2BU3P.tm2tP.copyable-area > div > span:nth-child(2) > div > div._2lMWa > div._3HQNh._1Ae7k > button")).Click(); //Click SEND Arrow Button
                    isSend = true;
                    Console.WriteLine("Message Send Successfully ");
                    Thread.Sleep(2000);

                }

            }
        }
        catch (Exception ex)
        { throw ex; }

        return isSend;

    }


}
