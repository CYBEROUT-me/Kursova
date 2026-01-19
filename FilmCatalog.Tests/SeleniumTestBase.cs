using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace FilmCatalog.Tests
{
    /// <summary>
    /// Базовий клас для Selenium тестів
    /// Забезпечує налаштування та очищення WebDriver
    /// </summary>
    public abstract class SeleniumTestBase
    {
        protected IWebDriver Driver { get; private set; } = null!;
        protected WebDriverWait Wait { get; private set; } = null!;
        
        // URL додатку - змініть на свій порт!
        protected static string BaseUrl = Environment.GetEnvironmentVariable("TEST_BASE_URL") ?? "http://localhost:5000";
        
        /// <summary>
        /// Налаштування перед кожним тестом (Arrange)
        /// </summary>
        [SetUp]
        public void Setup()
        {
            // Arrange: Налаштування Chrome WebDriver
            var options = new ChromeOptions();
            // Закоментуйте наступний рядок, щоб бачити браузер під час тестів
            // options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--ignore-certificate-errors"); // Ігнорувати помилки сертифікату
            
            Driver = new ChromeDriver(options);
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
        }
        
        /// <summary>
        /// Очищення після кожного тесту
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            Driver?.Quit();
            Driver?.Dispose();
        }
        
        /// <summary>
        /// Допоміжний метод для навігації на сторінку
        /// </summary>
        protected void NavigateTo(string path)
        {
            Driver.Navigate().GoToUrl($"{BaseUrl}{path}");
        }
        
        /// <summary>
        /// Допоміжний метод для очікування елемента
        /// </summary>
        protected IWebElement WaitForElement(By locator)
        {
            return Wait.Until(d => d.FindElement(locator));
        }
        
        /// <summary>
        /// Допоміжний метод для очікування клікабельності елемента
        /// </summary>
        protected IWebElement WaitForClickable(By locator)
        {
            return Wait.Until(d => {
                var element = d.FindElement(locator);
                return (element.Displayed && element.Enabled) ? element : null;
            })!;
        }
        
        /// <summary>
        /// Допоміжний метод для перевірки наявності елемента
        /// </summary>
        protected bool ElementExists(By locator)
        {
            try
            {
                Driver.FindElement(locator);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        
        /// <summary>
        /// Допоміжний метод для заповнення текстового поля
        /// </summary>
        protected void FillInput(string elementId, string value)
        {
            var element = Driver.FindElement(By.Id(elementId));
            element.Clear();
            element.SendKeys(value);
        }
        
        /// <summary>
        /// Допоміжний метод для вибору елемента з випадаючого списку
        /// </summary>
        protected void SelectDropdownByValue(string elementId, string value)
        {
            var selectElement = new SelectElement(Driver.FindElement(By.Id(elementId)));
            selectElement.SelectByValue(value);
        }
        
        /// <summary>
        /// Допоміжний метод для отримання тексту повідомлення про успіх
        /// </summary>
        protected string GetSuccessMessage()
        {
            try
            {
                var alert = Wait.Until(d => d.FindElement(By.Id("success-alert")));
                return alert.Text;
            }
            catch
            {
                return string.Empty;
            }
        }
        
        /// <summary>
        /// Допоміжний метод для отримання тексту повідомлення про помилку
        /// </summary>
        protected string GetErrorMessage()
        {
            try
            {
                var alert = Wait.Until(d => d.FindElement(By.Id("error-alert")));
                return alert.Text;
            }
            catch
            {
                return string.Empty;
            }
        }
        
        /// <summary>
        /// Очікування перенаправлення на URL
        /// </summary>
        protected void WaitForUrlContains(string urlPart)
        {
            Wait.Until(d => d.Url.Contains(urlPart));
        }
    }
}
