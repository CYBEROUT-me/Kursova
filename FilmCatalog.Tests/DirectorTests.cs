using NUnit.Framework;
using OpenQA.Selenium;

namespace FilmCatalog.Tests
{
    /// <summary>
    /// Selenium тести для функціоналу управління режисерами
    /// Використовується методологія BDD та принцип AAA
    /// </summary>
    [TestFixture]
    public class DirectorTests : SeleniumTestBase
    {
        #region Позитивні тести

        /// <summary>
        /// Тест: Успішне відображення списку режисерів
        /// Given: Користувач хоче переглянути режисерів
        /// When: Користувач переходить на сторінку режисерів
        /// Then: Відображається таблиця з режисерами
        /// </summary>
        [Test]
        public void Directors_Index_DisplaysDirectorsList_Success()
        {
            // Arrange: Підготовка - перехід на сторінку
            NavigateTo("/Directors");
            
            // Act: Дія - пошук таблиці з режисерами
            var table = WaitForElement(By.Id("directors-table"));
            
            // Assert: Перевірка - таблиця відображається
            Assert.That(table.Displayed, Is.True, "Таблиця режисерів повинна відображатися");
            Assert.That(Driver.Title, Does.Contain("Режисери"), "Заголовок сторінки має містити 'Режисери'");
        }

        /// <summary>
        /// Тест: Успішне створення нового режисера
        /// Given: Користувач на сторінці створення режисера
        /// When: Користувач заповнює форму коректними даними
        /// Then: Режисер створюється успішно
        /// </summary>
        [Test]
        public void Directors_Create_WithValidData_Success()
        {
            // Arrange: Підготовка - перехід на сторінку створення
            NavigateTo("/Directors/Create");
            
            // Act: Дія - заповнення форми
            FillInput("director-name", "Джеймс Кемерон");
            FillInput("director-country", "Канада");
            var submitButton = Driver.FindElement(By.Id("submit-btn"));
            submitButton.Click();
            
            // Assert: Перевірка - перенаправлення та повідомлення
            Wait.Until(d => d.Url.Contains("/Directors") && !d.Url.Contains("/Create"));
            var successMessage = GetSuccessMessage();
            Assert.That(successMessage, Does.Contain("успішно"), 
                "Має відображатися повідомлення про успішне створення");
        }

        /// <summary>
        /// Тест: Успішне редагування режисера
        /// Given: Існує режисер у базі даних
        /// When: Користувач редагує дані режисера
        /// Then: Зміни зберігаються успішно
        /// </summary>
        [Test]
        public void Directors_Edit_WithValidData_Success()
        {
            // Arrange: Підготовка - перехід на сторінку редагування
            NavigateTo("/Directors/Edit/1");
            
            // Act: Дія - зміна країни режисера
            var countryInput = Driver.FindElement(By.Id("director-country"));
            countryInput.Clear();
            countryInput.SendKeys("Велика Британія / США");
            
            var submitButton = Driver.FindElement(By.Id("submit-btn"));
            submitButton.Click();
            
            // Assert: Перевірка - перенаправлення та повідомлення
            Wait.Until(d => d.Url.Contains("/Directors") && !d.Url.Contains("/Edit"));
            var successMessage = GetSuccessMessage();
            Assert.That(successMessage, Does.Contain("оновлено"), 
                "Має відображатися повідомлення про успішне оновлення");
        }

        /// <summary>
        /// Тест: Успішний перегляд деталей режисера
        /// Given: Існує режисер у базі даних
        /// When: Користувач переходить на сторінку деталей
        /// Then: Відображається повна інформація про режисера
        /// </summary>
        [Test]
        public void Directors_Details_ExistingDirector_DisplaysAllInfo()
        {
            // Arrange: Підготовка - перехід на сторінку деталей
            NavigateTo("/Directors/Details/1");
            
            // Act: Дія - пошук елементів з інформацією
            var name = WaitForElement(By.Id("detail-name"));
            var country = Driver.FindElement(By.Id("detail-country"));
            
            // Assert: Перевірка - всі поля заповнені
            Assert.That(name.Text, Is.Not.Empty, "Ім'я режисера має відображатися");
            Assert.That(country.Text, Is.Not.Empty, "Країна має відображатися");
        }

        /// <summary>
        /// Тест: Створення режисера без країни (необов'язкове поле)
        /// Given: Користувач на сторінці створення
        /// When: Користувач вводить тільки ім'я
        /// Then: Режисер створюється успішно
        /// </summary>
        [Test]
        public void Directors_Create_WithoutCountry_Success()
        {
            // Arrange: Підготовка - перехід на сторінку створення
            NavigateTo("/Directors/Create");
            
            // Act: Дія - введення тільки імені
            FillInput("director-name", "Невідомий Режисер");
            // Країну не вводимо
            var submitButton = Driver.FindElement(By.Id("submit-btn"));
            submitButton.Click();
            
            // Assert: Перевірка - режисер створюється без країни
            Wait.Until(d => d.Url.Contains("/Directors") && !d.Url.Contains("/Create"));
            var successMessage = GetSuccessMessage();
            Assert.That(successMessage, Does.Contain("успішно"), 
                "Режисер має створюватися без вказання країни");
        }

        #endregion

        #region Негативні тести

        /// <summary>
        /// Тест: Валідація пустого імені режисера
        /// Given: Користувач на сторінці створення
        /// When: Користувач не заповнює ім'я
        /// Then: Відображається помилка валідації
        /// </summary>
        [Test]
        public void Directors_Create_WithEmptyName_ShowsValidationError()
        {
            // Arrange: Підготовка - перехід на сторінку створення
            NavigateTo("/Directors/Create");
            
            // Act: Дія - відправка пустої форми
            FillInput("director-country", "США"); // Тільки країна
            var submitButton = Driver.FindElement(By.Id("submit-btn"));
            submitButton.Click();
            
            // Assert: Перевірка - помилка валідації
            var validationError = Driver.FindElement(By.CssSelector("[data-valmsg-for='Name']"));
            Assert.That(validationError.Text, Does.Contain("обов'язков"), 
                "Має відображатися помилка про обов'язковість імені");
        }

        /// <summary>
        /// Тест: Валідація занадто короткого імені
        /// Given: Користувач на сторінці створення
        /// When: Користувач вводить ім'я з 1 символу
        /// Then: Відображається помилка валідації
        /// </summary>
        [Test]
        public void Directors_Create_WithShortName_ShowsValidationError()
        {
            // Arrange: Підготовка - перехід на сторінку створення
            NavigateTo("/Directors/Create");
            
            // Act: Дія - введення занадто короткого імені
            FillInput("director-name", "А");
            var submitButton = Driver.FindElement(By.Id("submit-btn"));
            submitButton.Click();
            
            // Assert: Перевірка - помилка валідації довжини
            var validationError = Driver.FindElement(By.CssSelector("[data-valmsg-for='Name']"));
            Assert.That(validationError.Text, Does.Contain("2").Or.Contain("символ"), 
                "Має відображатися помилка про мінімальну довжину");
        }

        /// <summary>
        /// Тест: Неможливість видалення режисера з фільмами
        /// Given: Режисер має пов'язані фільми
        /// When: Користувач намагається видалити режисера
        /// Then: Відображається помилка
        /// </summary>
        [Test]
        public void Directors_Delete_WithRelatedFilms_ShowsError()
        {
            // Arrange: Підготовка - перехід на видалення режисера з фільмами
            NavigateTo("/Directors/Delete/1"); // Крістофер Нолан має фільми
            
            // Act: Дія - підтвердження видалення
            var confirmButton = Driver.FindElement(By.Id("confirm-delete-btn"));
            confirmButton.Click();
            
            // Assert: Перевірка - повідомлення про помилку
            Wait.Until(d => d.Url.Contains("/Directors"));
            var errorMessage = GetErrorMessage();
            Assert.That(errorMessage, Does.Contain("пов'язан").Or.Contain("фільм"), 
                "Має відображатися помилка про пов'язані фільми");
        }

        /// <summary>
        /// Тест: Перехід на неіснуючого режисера
        /// Given: Режисер з вказаним ID не існує
        /// When: Користувач намагається переглянути деталі
        /// Then: Відображається сторінка 404
        /// </summary>
        [Test]
        public void Directors_Details_NonExistentDirector_ReturnsNotFound()
        {
            // Arrange & Act: Перехід на неіснуючого режисера
            NavigateTo("/Directors/Details/99999");
            
            // Assert: Перевірка - сторінка не знайдена
            Assert.That(Driver.PageSource, Does.Contain("404").Or.Contain("Not Found").Or.Contain("Error"),
                "Має відображатися сторінка помилки для неіснуючого режисера");
        }

        #endregion
    }
}
