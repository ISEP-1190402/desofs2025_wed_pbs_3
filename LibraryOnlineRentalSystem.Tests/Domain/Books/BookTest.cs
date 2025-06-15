using LibraryOnlineRentalSystem.Domain.Book;
using NUnit.Framework;

namespace LibraryOnlineRentalSystem.Tests.Domain.Books
{
    [TestFixture]
    [TestOf(typeof(Book))]
    public class BookTest
    {
        [Test]
        public void Constructor_SetsAllPropertiesCorrectly()
        {
            var book = new Book(
                "book1", "Title", 3, "Author Name", "Category A", "Description X", "9783161484100", "Publisher Y");

            Assert.That(book.Id.AsString(), Is.EqualTo("book1"));
            Assert.That(book.AmountOfCopies.BookAmountOfCopies, Is.EqualTo(3));
            Assert.That(book.Author.BookAuthor, Is.EqualTo("Author Name"));
            Assert.That(book.Category.BookCategoryName, Is.EqualTo("Category A"));
            Assert.That(book.Description.BookDescription, Is.EqualTo("Description X"));
            Assert.That(book.Isbn.GetISBN(), Is.EqualTo("9783161484100"));
            Assert.That(book.Publisher.GetBookPublisher(), Is.EqualTo("Publisher Y"));
            Assert.That(book.Active, Is.True);
        }

        [Test]
        public void IsBookDeleted_ActiveBook_ReturnsFalse()
        {
            var book = new Book(
                "book2", "Title", 2, "Author Name", "Category B", "Description C", "9780306406157", "Publisher E");
            Assert.That(book.isBookDeleted(), Is.False);
        }

        [Test]
        public void DeleteBook_SetsActiveToFalse()
        {
            var book = new Book(
                "book3", "Title", 1, "Author Name", "Category B", "Description C", "9781861972712", "Publisher E");
            book.deleteBook();
            Assert.That(book.Active, Is.False);
        }

        [Test]
        public void IsBookDeleted_AfterDeleteBook_ReturnsTrue()
        {
            var book = new Book(
                "book4", "Title", 5, "Author Name", "Category B", "Description C", "9780140449136", "Publisher E");
            book.deleteBook();
            Assert.That(book.isBookDeleted(), Is.True);
        }

        [Test]
        public void ToDTO_ReturnsCorrectDTO()
        {
            var book = new Book(
                "book5", "Title", 10, "AuthorZ", "Fiction", "Great Book", "9780307476463", "PubHouse");
            var dto = book.toDTO();

            Assert.That(dto.Id, Is.EqualTo("book5"));
            Assert.That(dto.AmountOfCopies, Is.EqualTo(10));
            Assert.That(dto.Author, Is.EqualTo("AuthorZ"));
            Assert.That(dto.Category, Is.EqualTo("Fiction"));
            Assert.That(dto.Description, Is.EqualTo("Great Book"));
            Assert.That(dto.Isbn, Is.EqualTo("9780307476463"));
            Assert.That(dto.Publisher, Is.EqualTo("PubHouse"));
        }

        [Test]
        public void UpdateStock_UpdatesAmountOfCopies()
        {
            var book = new Book(
                "book6", "Title", 2, "Author Name", "Category B", "Description C", "9780553382563", "Publisher E");
            book.UpdateStock(8);
            Assert.That(book.AmountOfCopies.BookAmountOfCopies, Is.EqualTo(8));
        }
    }
}
