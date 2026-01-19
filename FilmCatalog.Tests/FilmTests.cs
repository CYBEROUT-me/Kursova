using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace FilmCatalog.Tests
{
    /// <summary>
    /// Selenium тести для функціоналу управління фільмами
    /// Використовується методологія BDD (Given-When-Then) та принцип AAA (Arrange-Act-Assert)
    /// </summary>
    [TestFixture]
    public class FilmTests : SeleniumTestBase
    {
        #region Позитивні тести

        /// <summary>
        /// Тест: Успішне відображення списку фільмів
        /// Given: Користувач на головній сторінці
        /// When: Користувач переходить на сторінку фільмів
        /// Then: Відображається таблиця з фільмами
        /// </summary>
        [Test]
        public void Films_Index_DisplaysFilmsList_Success()
        {
            // Arrange: Підготовка - перехід на сторінку
            NavigateTo("/Films");
            
            // Act: Дія - пошук таблиці з фільмами
            var table = WaitForElement(By.Id("films-table"));
            
            // Assert: Перевірка - таблиця відображається
            Assert.That(table.Displayed, Is.True, "Таблиця фільмів повинна відображатися");
            Assert.That(Driver.Title, Does.Contain("Фільми"), "Заголовок сторінки має містити 'Фільми'");
        }

        /// <summary>
        /// Тест: Успішне створення нового фільму
        /// Given: Користувач на сторінці створення фільму
        /// When: Користувач заповнює форму коректними даними та відправляє її
        /// Then: Фільм створюється та користувач перенаправляється на список
        /// </summary>
        [Test]
        public void Films_Create_WithValidData_Success()
        {
            // Arrange: Підготовка - перехід на сторінку створення
            NavigateTo("/Films/Create");
            
            // Act: Дія - заповнення форми
            FillInput("film-title", "Тестовий фільм");
            FillInput("film-year", "2024");
            FillInput("film-description", "Опис тестового фільму для Selenium тестування");
            FillInput("film-rating", "8.5");
            SelectDropdownByValue("film-genre", "1"); // Драма
            SelectDropdownByValue("film-director", "1"); // Крістофер Нолан
            
            // Відправка форми
            var submitButton = Driver.FindElement(By.Id("submit-btn"));
            submitButton.Click();
            
            // Assert: Перевірка - перенаправлення на список та повідомлення про успіх
            Wait.Until(d => d.Url.Contains("/Films"));
            var successMessage = GetSuccessMessage();
            Assert.That(successMessage, Does.Contain("успішно"), "Має відображатися повідомлення про успішне створення");
        }

        /// <summary>
        /// Тест: Успішне редагування існуючого фільму
        /// Given: Існує фільм у базі даних
        /// When: Користувач редагує назву фільму
        /// Then: Зміни зберігаються успішно
        /// </summary>
        [Test]
        public void Films_Edit_WithValidData_Success()
        {
            // Arrange: Підготовка - перехід на сторінку редагування першого фільму
            NavigateTo("/Films/Edit/1");
            
            // Act: Дія - зміна назви фільму
            var titleInput = Driver.FindElement(By.Id("film-title"));
            titleInput.Clear();
            titleInput.SendKeys("Інтерстеллар (Оновлено)");
            
            var submitButton = Driver.FindElement(By.Id("submit-btn"));
            submitButton.Click();
            
            // Assert: Перевірка - перенаправлення та повідомлення
            Wait.Until(d => d.Url.Contains("/Films") && !d.Url.Contains("/Edit"));
            var successMessage = GetSuccessMessage();
            Assert.That(successMessage, Does.Contain("оновлено"), "Має відображатися повідомлення про успішне оновлення");
        }

        /// <summary>
        /// Тест: Успішне видалення фільму
        /// Given: Існує фільм для видалення
        /// When: Користувач підтверджує видалення
        /// Then: Фільм видаляється успішно
        /// </summary>
        [Test]
        public void Films_Delete_ExistingFilm_Success()
        {
            // Arrange: Підготовка - спочатку створимо фільм для видалення
            NavigateTo("/Films/Create");
            FillInput("film-title", "Фільм для видалення");
            FillInput("film-year", "2020");
            SelectDropdownByValue("film-genre", "2");
            SelectDropdownByValue("film-director", "2");
            Driver.FindElement(By.Id("submit-btn")).Click();
            Wait.Until(d => d.Url.Contains("/Films") && !d.Url.Contains("/Create"));
            
            // Знаходимо створений фільм і переходимо до видалення
            NavigateTo("/Films");
            var deleteLinks = Driver.FindElements(By.CssSelector("a[href*='/Films/Delete/']"));
            var lastDeleteLink = deleteLinks.Last();
            lastDeleteLink.Click();
            
            // Act: Дія - підтвердження видалення
            Wait.Until(d => d.Url.Contains("/Films/Delete/"));
            var confirmButton = Driver.FindElement(By.Id("confirm-delete-btn"));
            confirmButton.Click();
            
            // Assert: Перевірка - перенаправлення та повідомлення
            Wait.Until(d => d.Url.EndsWith("/Films") || d.Url.EndsWith("/Films/"));
            var successMessage = GetSuccessMessage();
            Assert.That(successMessage, Does.Contain("видалено"), "Має відображатися повідомлення про успішне видалення");
        }

        /// <summary>
        /// Тест: Успішний перегляд деталей фільму
        /// Given: Існує фільм у базі даних
        /// When: Користувач переходить на сторінку деталей
        /// Then: Відображається повна інформація про фільм
        /// </summary>
        [Test]
        public void Films_Details_ExistingFilm_DisplaysAllInfo()
        {
            // Arrange: Підготовка - перехід на сторінку деталей
            NavigateTo("/Films/Details/1");
            
            // Act: Дія - пошук елементів з інформацією
            var title = WaitForElement(By.Id("detail-title"));
            var year = Driver.FindElement(By.Id("detail-year"));
            var genre = Driver.FindElement(By.Id("detail-genre"));
            var director = Driver.FindElement(By.Id("detail-director"));
            
            // Assert: Перевірка - всі поля заповнені
            Assert.That(title.Text, Is.Not.Empty, "Назва фільму має відображатися");
            Assert.That(year.Text, Is.Not.Empty, "Рік випуску має відображатися");
            Assert.That(genre.Text, Is.Not.Empty, "Жанр має відображатися");
            Assert.That(director.Text, Is.Not.Empty, "Режисер має відображатися");
        }

        /// <summary>
        /// Тест: Успішний пошук фільмів за назвою
        /// Given: Існують фільми у базі даних
        /// When: Користувач вводить пошуковий запит
        /// Then: Відображаються відфільтровані результати
        /// </summary>
        [Test]
        public void Films_Search_ByTitle_ReturnsFilteredResults()
        {
            // Arrange: Підготовка - перехід на сторінку фільмів
            NavigateTo("/Films");
            
            // Act: Дія - введення пошукового запиту
            FillInput("search-input", "Інтерстеллар");
            var searchButton = Driver.FindElement(By.Id("search-button"));
            searchButton.Click();
            
            // Assert: Перевірка - результати відфільтровані
            Wait.Until(d => d.Url.Contains("searchString"));
            var rows = Driver.FindElements(By.CssSelector("#films-table tbody tr"));
            
            foreach (var row in rows)
            {
                var titleCell = row.FindElement(By.CssSelector(".film-title"));
                Assert.That(titleCell.Text.ToLower(), Does.Contain("інтерстеллар".ToLower()), 
                    "Всі результати повинні містити пошуковий запит");
            }
        }

        /// <summary>
        /// Тест: Успішна фільтрація фільмів за жанром
        /// Given: Існують фільми різних жанрів
        /// When: Користувач обирає жанр у фільтрі
        /// Then: Відображаються тільки фільми обраного жанру
        /// </summary>
        [Test]
        public void Films_Filter_ByGenre_ReturnsFilteredResults()
        {
            // Arrange: Підготовка - перехід на сторінку фільмів
            NavigateTo("/Films");
            
            // Act: Дія - вибір жанру "Фантастика" (id=4)
            SelectDropdownByValue("genre-filter", "4");
            var searchButton = Driver.FindElement(By.Id("search-button"));
            searchButton.Click();
            
            // Assert: Перевірка - всі фільми обраного жанру
            Wait.Until(d => d.Url.Contains("genreId"));
            var rows = Driver.FindElements(By.CssSelector("#films-table tbody tr"));
            
            foreach (var row in rows)
            {
                var genreCell = row.FindElement(By.CssSelector(".film-genre"));
                Assert.That(genreCell.Text, Is.EqualTo("Фантастика"), 
                    "Всі результати повинні бути жанру 'Фантастика'");
            }
        }

        #endregion

        #region Негативні тести

        /// <summary>
        /// Тест: Валідація пустої назви фільму
        /// Given: Користувач на сторінці створення
        /// When: Користувач не заповнює обов'язкове поле назви
        /// Then: Відображається помилка валідації
        /// </summary>
        [Test]
        public void Films_Create_WithEmptyTitle_ShowsValidationError()
        {
            // Arrange: Підготовка - перехід на сторінку створення
            NavigateTo("/Films/Create");
            
            // Act: Дія - заповнення форми без назви
            FillInput("film-year", "2024");
            SelectDropdownByValue("film-genre", "1");
            SelectDropdownByValue("film-director", "1");
            
            var submitButton = Driver.FindElement(By.Id("submit-btn"));
            submitButton.Click();
            
            // Assert: Перевірка - помилка валідації
            var validationError = Driver.FindElement(By.CssSelector("[data-valmsg-for='Title']"));
            Assert.That(validationError.Text, Does.Contain("обов'язков"), 
                "Має відображатися помилка про обов'язковість назви");
        }

        /// <summary>
        /// Тест: Валідація некоректного року випуску
        /// Given: Користувач на сторінці створення
        /// When: Користувач вводить некоректний рік (до 1895)
        /// Then: Відображається помилка валідації
        /// </summary>
        [Test]
        public void Films_Create_WithInvalidYear_ShowsValidationError()
        {
            // Arrange: Підготовка - перехід на сторінку створення
            NavigateTo("/Films/Create");
            
            // Act: Дія - введення некоректного року
            FillInput("film-title", "Тестовий фільм");
            FillInput("film-year", "1800"); // Рік до винаходу кіно
            SelectDropdownByValue("film-genre", "1");
            SelectDropdownByValue("film-director", "1");
            
            var submitButton = Driver.FindElement(By.Id("submit-btn"));
            submitButton.Click();
            
            // Assert: Перевірка - помилка валідації року
            var validationError = Driver.FindElement(By.CssSelector("[data-valmsg-for='Year']"));
            Assert.That(validationError.Text, Does.Contain("1895"), 
                "Має відображатися помилка про допустимий діапазон років");
        }

        /// <summary>
        /// Тест: Валідація некоректного рейтингу
        /// Given: Користувач на сторінці створення
        /// When: Користувач вводить рейтинг більше 10
        /// Then: Відображається помилка валідації
        /// </summary>
        [Test]
        public void Films_Create_WithInvalidRating_ShowsValidationError()
        {
            // Arrange: Підготовка - перехід на сторінку створення
            NavigateTo("/Films/Create");
            
            // Act: Дія - введення некоректного рейтингу
            FillInput("film-title", "Тестовий фільм");
            FillInput("film-year", "2024");
            FillInput("film-rating", "15"); // Більше максимального
            SelectDropdownByValue("film-genre", "1");
            SelectDropdownByValue("film-director", "1");
            
            var submitButton = Driver.FindElement(By.Id("submit-btn"));
            submitButton.Click();
            
            // Assert: Перевірка - помилка валідації рейтингу
            var validationError = Driver.FindElement(By.CssSelector("[data-valmsg-for='Rating']"));
            Assert.That(validationError.Text, Does.Contain("10"), 
                "Має відображатися помилка про максимальний рейтинг");
        }

        /// <summary>
        /// Тест: Перехід на неіснуючий фільм
        /// Given: Фільм з вказаним ID не існує
        /// When: Користувач намагається переглянути деталі
        /// Then: Відображається сторінка 404
        /// </summary>
        [Test]
        public void Films_Details_NonExistentFilm_ReturnsNotFound()
        {
            // Arrange & Act: Перехід на неіснуючий фільм
            NavigateTo("/Films/Details/99999");
            
            // Assert: Перевірка - сторінка не знайдена або редірект
            Assert.That(Driver.PageSource, Does.Contain("404").Or.Contain("Not Found").Or.Contain("Error"),
                "Має відображатися сторінка помилки для неіснуючого фільму");
        }

        /// <summary>
        /// Тест: Валідація без вибору жанру
        /// Given: Користувач на сторінці створення
        /// When: Користувач не обирає жанр
        /// Then: Відображається помилка валідації
        /// </summary>
        [Test]
        public void Films_Create_WithoutGenre_ShowsValidationError()
        {
            // Arrange: Підготовка - перехід на сторінку створення
            NavigateTo("/Films/Create");
            
            // Act: Дія - заповнення без вибору жанру
            FillInput("film-title", "Тестовий фільм");
            FillInput("film-year", "2024");
            SelectDropdownByValue("film-director", "1");
            // Жанр не обрано
            
            var submitButton = Driver.FindElement(By.Id("submit-btn"));
            submitButton.Click();
            
            // Assert: Перевірка - помилка валідації жанру
            var validationError = Driver.FindElement(By.CssSelector("[data-valmsg-for='GenreId']"));
            Assert.That(validationError.Text, Does.Contain("обов'язков"), 
                "Має відображатися помилка про обов'язковість жанру");
        }

        /// <summary>
        /// Тест: Пошук без результатів
        /// Given: Пошуковий запит не відповідає жодному фільму
        /// When: Користувач виконує пошук
        /// Then: Відображається повідомлення про відсутність результатів
        /// </summary>
        [Test]
        public void Films_Search_NoResults_ShowsEmptyMessage()
        {
            // Arrange: Підготовка - перехід на сторінку фільмів
            NavigateTo("/Films");
            
            // Act: Дія - пошук неіснуючого фільму
            FillInput("search-input", "НеіснуючийФільм12345");
            var searchButton = Driver.FindElement(By.Id("search-button"));
            searchButton.Click();
            
            // Assert: Перевірка - повідомлення про відсутність результатів
            Wait.Until(d => d.Url.Contains("searchString"));
            var rows = Driver.FindElements(By.CssSelector("#films-table tbody tr"));
            
            if (rows.Count == 0)
            {
                var emptyMessage = Driver.FindElement(By.Id("no-films-message"));
                Assert.That(emptyMessage.Displayed, Is.True, 
                    "Має відображатися повідомлення про відсутність фільмів");
            }
            else
            {
                Assert.Fail("Пошук повинен повертати порожній результат для неіснуючого фільму");
            }
        }

        #endregion
    }
}
