using NUnit.Framework;
using OpenQA.Selenium;

namespace FilmCatalog.Tests
{
    /// <summary>
    /// Selenium тести для функціоналу управління жанрами
    /// Використовується методологія BDD та принцип AAA
    /// </summary>
    [TestFixture]
    public class GenreTests : SeleniumTestBase
    {
        #region Позитивні тести

        /// <summary>
        /// Тест: Успішне відображення списку жанрів
        /// Given: Користувач хоче переглянути жанри
        /// When: Користувач переходить на сторінку жанрів
        /// Then: Відображається таблиця з жанрами
        /// </summary>
        [Test]
        public void Genres_Index_DisplaysGenresList_Success()
        {
            // Arrange: Підготовка - перехід на сторінку
            NavigateTo("/Genres");
            
            // Act: Дія - пошук таблиці з жанрами
            var table = WaitForElement(By.Id("genres-table"));
            
            // Assert: Перевірка - таблиця відображається
            Assert.That(table.Displayed, Is.True, "Таблиця жанрів повинна відображатися");
            Assert.That(Driver.Title, Does.Contain("Жанри"), "Заголовок сторінки має містити 'Жанри'");
        }

        /// <summary>
        /// Тест: Успішне створення нового жанру
        /// Given: Користувач на сторінці створення жанру
        /// When: Користувач вводить коректну назву
        /// Then: Жанр створюється успішно
        /// </summary>
        [Test]
        public void Genres_Create_WithValidName_Success()
        {
            // Arrange: Підготовка - перехід на сторінку створення
            NavigateTo("/Genres/Create");
            
            // Act: Дія - введення назви жанру
            FillInput("genre-name", "Мюзикл");
            var submitButton = Driver.FindElement(By.Id("submit-btn"));
            submitButton.Click();
            
            // Assert: Перевірка - перенаправлення та повідомлення
            Wait.Until(d => d.Url.Contains("/Genres") && !d.Url.Contains("/Create"));
            var successMessage = GetSuccessMessage();
            Assert.That(successMessage, Does.Contain("успішно"), 
                "Має відображатися повідомлення про успішне створення");
        }

        /// <summary>
        /// Тест: Успішне редагування жанру
        /// Given: Існує жанр у базі даних
        /// When: Користувач змінює назву жанру
        /// Then: Зміни зберігаються успішно
        /// </summary>
        [Test]
        public void Genres_Edit_WithValidData_Success()
        {
            // Arrange: Підготовка - перехід на сторінку редагування
            NavigateTo("/Genres/Edit/1");
            
            // Act: Дія - зміна назви жанру
            var nameInput = Driver.FindElement(By.Id("genre-name"));
            nameInput.Clear();
            nameInput.SendKeys("Драма (Оновлено)");
            
            var submitButton = Driver.FindElement(By.Id("submit-btn"));
            submitButton.Click();
            
            // Assert: Перевірка - перенаправлення та повідомлення
            Wait.Until(d => d.Url.Contains("/Genres") && !d.Url.Contains("/Edit"));
            var successMessage = GetSuccessMessage();
            Assert.That(successMessage, Does.Contain("оновлено"), 
                "Має відображатися повідомлення про успішне оновлення");
        }

        /// <summary>
        /// Тест: Успішний перегляд деталей жанру
        /// Given: Існує жанр у базі даних
        /// When: Користувач переходить на сторінку деталей
        /// Then: Відображається інформація про жанр
        /// </summary>
        [Test]
        public void Genres_Details_ExistingGenre_DisplaysInfo()
        {
            // Arrange: Підготовка - перехід на сторінку деталей
            NavigateTo("/Genres/Details/1");
            
            // Act: Дія - пошук елементів з інформацією
            var name = WaitForElement(By.Id("detail-name"));
            
            // Assert: Перевірка - назва відображається
            Assert.That(name.Text, Is.Not.Empty, "Назва жанру має відображатися");
        }

        #endregion

        #region Негативні тести

        /// <summary>
        /// Тест: Валідація пустої назви жанру
        /// Given: Користувач на сторінці створення
        /// When: Користувач не заповнює назву
        /// Then: Відображається помилка валідації
        /// </summary>
        [Test]
        public void Genres_Create_WithEmptyName_ShowsValidationError()
        {
            // Arrange: Підготовка - перехід на сторінку створення
            NavigateTo("/Genres/Create");
            
            // Act: Дія - відправка пустої форми
            var submitButton = Driver.FindElement(By.Id("submit-btn"));
            submitButton.Click();
            
            // Assert: Перевірка - помилка валідації
            var validationError = Driver.FindElement(By.CssSelector("[data-valmsg-for='Name']"));
            Assert.That(validationError.Text, Does.Contain("обов'язков"), 
                "Має відображатися помилка про обов'язковість назви");
        }

        /// <summary>
        /// Тест: Валідація занадто короткої назви
        /// Given: Користувач на сторінці створення
        /// When: Користувач вводить назву з 1 символу
        /// Then: Відображається помилка валідації
        /// </summary>
        [Test]
        public void Genres_Create_WithShortName_ShowsValidationError()
        {
            // Arrange: Підготовка - перехід на сторінку створення
            NavigateTo("/Genres/Create");
            
            // Act: Дія - введення занадто короткої назви
            FillInput("genre-name", "А");
            var submitButton = Driver.FindElement(By.Id("submit-btn"));
            submitButton.Click();
            
            // Assert: Перевірка - помилка валідації довжини
            var validationError = Driver.FindElement(By.CssSelector("[data-valmsg-for='Name']"));
            Assert.That(validationError.Text, Does.Contain("2").Or.Contain("символ"), 
                "Має відображатися помилка про мінімальну довжину");
        }

        /// <summary>
        /// Тест: Неможливість видалення жанру з фільмами
        /// Given: Жанр має пов'язані фільми
        /// When: Користувач намагається видалити жанр
        /// Then: Відображається помилка
        /// </summary>
        [Test]
        public void Genres_Delete_WithRelatedFilms_ShowsError()
        {
            // Arrange: Підготовка - перехід на видалення жанру з фільмами
            NavigateTo("/Genres/Delete/4"); // Фантастика має фільми
            
            // Act: Дія - підтвердження видалення
            var confirmButton = Driver.FindElement(By.Id("confirm-delete-btn"));
            confirmButton.Click();
            
            // Assert: Перевірка - повідомлення про помилку
            Wait.Until(d => d.Url.Contains("/Genres"));
            var errorMessage = GetErrorMessage();
            Assert.That(errorMessage, Does.Contain("пов'язан").Or.Contain("фільм"), 
                "Має відображатися помилка про пов'язані фільми");
        }

        /// <summary>
        /// Тест: Перехід на неіснуючий жанр
        /// Given: Жанр з вказаним ID не існує
        /// When: Користувач намагається переглянути деталі
        /// Then: Відображається сторінка 404
        /// </summary>
        [Test]
        public void Genres_Details_NonExistentGenre_ReturnsNotFound()
        {
            // Arrange & Act: Перехід на неіснуючий жанр
            NavigateTo("/Genres/Details/99999");
            
            // Assert: Перевірка - сторінка не знайдена
            Assert.That(Driver.PageSource, Does.Contain("404").Or.Contain("Not Found").Or.Contain("Error"),
                "Має відображатися сторінка помилки для неіснуючого жанру");
        }

        #endregion
    }
}
