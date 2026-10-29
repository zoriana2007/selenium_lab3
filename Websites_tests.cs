using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace selenium_lab3;

public class Tests
{
    ChromeDriver driver;
    private const string BaseUrl = "https://the-internet.herokuapp.com/";

    [SetUp]
    public void Setup()
    {
        driver = new ChromeDriver();
        driver.Manage().Window.Maximize();
    }

    //ForgotPassword
    [Test]
    public void ForgotPassword()
    {
        driver.Navigate().GoToUrl(BaseUrl + "forgot_password");
        
        var input =driver.FindElement(By.Id("email"));
        input.Clear();
        input.SendKeys("smith@admlucid.com");
        Assert.That(input.GetAttribute("value"), Is.Not.Empty);

        var button=driver.FindElement(By.Id("form_submit"));
        button.Click();

        var content=driver.FindElement(By.Id("content"));

        Assert.That(content.Text, Is.EqualTo("Your e-mail's been sent!"));
    }

    //Horizontal Slider
    [Test]
    public void HorizontalSlider()
    {
        driver.Navigate().GoToUrl(BaseUrl + "horizontal_slider");

        var slider=driver.FindElement(By.XPath("/html/body/div[2]/div/div/div/input"));

        slider.SendKeys(Keys.ArrowRight);
        slider.SendKeys(Keys.ArrowRight);
        slider.SendKeys(Keys.ArrowRight);

        var range=driver.FindElement(By.Id("range"));

        Assert.That(range.Text, Is.EqualTo("1.5"));
    }

    //Dropdown
    [Test]
    public void Dropdown()
    {
        driver.Navigate().GoToUrl(BaseUrl + "dropdown");

        var dropdown=new SelectElement(driver.FindElement(By.Id("dropdown")));

        dropdown.SelectByText("Option 1");
        Assert.That(dropdown.SelectedOption.Text, Is.EqualTo("Option 1"));

        dropdown.SelectByText("Option 2");
        Assert.That(dropdown.SelectedOption.Text, Is.EqualTo("Option 2"));
    }

    //Typos
    [Test]
    public void Typos()
    {
        const string expected_text_1="Sometimes you'll see a typo, other times you won't.";
        const string expected_text_2="Sometimes you'll see a typo, other times you won,t.";

        driver.Navigate().GoToUrl(BaseUrl + "typos");

        var content=driver.FindElement(By.XPath("/html/body/div[2]/div/div/p[2]"));

        Assert.That(content.Text==expected_text_1||content.Text==expected_text_2, Is.True);
    }

    //Entry Ad
   [Test]
   public void EntryAd()
   {
        driver.Navigate().GoToUrl(BaseUrl + "entry_ad");
        
        WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
        
        var modal = wait.Until(d =>
        {
            var element = d.FindElement(By.ClassName("modal"));
            if (element.Displayed)
            return element;
            
            return null;
        });
        Assert.That(modal.Displayed, Is.True);
        
        var close = driver.FindElement(By.XPath("//div[contains(@class,'modal-footer')]//p"));
        close.Click();

        Assert.That(modal.Displayed, Is.False);

        driver.Navigate().Refresh();
        var modalAfterReload=driver.FindElement(By.ClassName("modal"));
        Assert.That(modalAfterReload.Displayed, Is.False);
    }

    //File Download
    [Test]
    public void FileDownload()
    {
        string downloadPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "Images.txt");

        if (File.Exists(downloadPath))
        {
            File.Delete(downloadPath);
        }

        driver.Navigate().GoToUrl(BaseUrl + "download");

        var file=driver.FindElement(By.LinkText("Images.txt"));
        file.Click();
        
        WebDriverWait wait = new(driver, TimeSpan.FromSeconds(15));
        bool downloaded = wait.Until(d => File.Exists(downloadPath));
        
        Assert.That(downloaded, Is.True);
    }

    //Basic Auth
    [Test]
    public void BasicAuth()
    {
        string user = "admin";
        string pass = "admin";
        string url = $"https://{user}:{pass}@the-internet.herokuapp.com/basic_auth";

        driver.Navigate().GoToUrl(url);
        var message = driver.FindElement(By.CssSelector("p"));
        
        Assert.That(message.Text, Does.Contain("Congratulations!"));
    }

    //Dynamic Loading
    [Test]
    public void DynamicLoading1()
    {
        driver.Navigate().GoToUrl(BaseUrl + "dynamic_loading/1");
        
        var message = driver.FindElement(By.Id("finish"));
        Assert.That(message.Displayed, Is.False);

        var start_button=driver.FindElement(By.CssSelector("#start button"));
        start_button.Click();

        WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
        wait.Until(d => d.FindElement(By.Id("finish")).Displayed);

        Assert.That(message.Text, Is.EqualTo("Hello World!"));
    }

    [Test]
    public void DynamicLoading2()
    {
        driver.Navigate().GoToUrl(BaseUrl + "dynamic_loading/2");

        var start_button=driver.FindElement(By.CssSelector("#start button"));
        start_button.Click();

        WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
        var message = wait.Until(d =>
        {
            var element = d.FindElement(By.Id("finish"));
            if (element.Displayed)
            return element;
            
            return null;
        });

        Assert.That(message.Text, Is.EqualTo("Hello World!"));
    }


    //Context Menu
    [Test]
    public void ContextMenu()
    {
        driver.Navigate().GoToUrl(BaseUrl + "context_menu");

        var box=driver.FindElement(By.Id("hot-spot"));
        Actions actions = new Actions(driver);
        actions.ContextClick(box).Perform();
        
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        IAlert alert = wait.Until(d =>d.SwitchTo().Alert());

        Assert.That(alert.Text, Is.EqualTo("You selected a context menu"));
        
        alert.Accept();
    }

    //Redirect Link
    [Test]
    public void RedirectLink()
    {
        driver.Navigate().GoToUrl(BaseUrl + "redirector");
        var redirect=driver.FindElement(By.Id("redirect"));
        redirect.Click();

        Assert.That(driver.Url, Is.EqualTo(BaseUrl + "status_codes"));

        string header = driver.FindElement(By.TagName("h3")).Text;
        
        Assert.That(header, Is.EqualTo("Status Codes"));
    }

    [TearDown]
    public void TearDown()
    {
        driver.Quit();
        driver.Dispose();
    }
}
