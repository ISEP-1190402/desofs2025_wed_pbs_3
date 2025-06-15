using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Domain.Common;
using NUnit.Framework;
using System;

namespace LibraryOnlineRentalSystem.Tests.Domain.Books
{
    [TestFixture]
    [TestOf(typeof(Author))]
    public class AuthorTest
    {
        [Test]
        public void Constructor_WithValidAuthor_SetsValue()
        {
            var author = new Author("J.K. Rowling");
            Assert.That(author.BookAuthor, Is.EqualTo("J.K. Rowling"));
        }

        [Test]
        public void Constructor_TrimsWhitespace()
        {
            var author = new Author("  Stephen King ");
            Assert.That(author.BookAuthor, Is.EqualTo("Stephen King"));
        }

        [Test]
        public void Constructor_Null_ThrowsBusinessRulesException()
        {
            Assert.Throws<BusinessRulesException>(() => new Author(null));
        }

        [Test]
        public void Constructor_Empty_ThrowsBusinessRulesException()
        {
            Assert.Throws<BusinessRulesException>(() => new Author(""));
        }

        [Test]
        public void GetBookAuthor_ReturnsCorrectValue()
        {
            var author = new Author("George Orwell");
            Assert.That(author.GetBookAuthor(), Is.EqualTo("George Orwell"));
        }

        [Test]
        public void ValueOf_ReturnsNewInstanceWithGivenAuthor()
        {
            var author = new Author("J.R.R. Tolkien");
            var newAuthor = author.ValueOf("Agatha Christie");
            Assert.That(newAuthor.BookAuthor, Is.EqualTo("Agatha Christie"));
        }

        [Test]
        public void Equals_SameValues_CaseInsensitive_ReturnsTrue()
        {
            var a = new Author("Jane Austen");
            var b = new Author("jane austen");
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            var a = new Author("Arthur Conan Doyle");
            var b = new Author("H.G. Wells");
            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            var a = new Author("Isaac Asimov");
            Assert.That(a.Equals(null), Is.False);
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var a = new Author("Margaret Atwood");
            Assert.That(a.Equals("Margaret Atwood"), Is.False);
        }

        [Test]
        public void ToString_ReturnsBookAuthor()
        {
            var author = new Author("Roald Dahl");
            Assert.That(author.ToString(), Is.EqualTo("Roald Dahl"));
        }

        [Test]
        public void GetHashCode_ReturnsSameForEqualAuthors()
        {
            var a = new Author("Kazuo Ishiguro");
            var b = new Author("Kazuo Ishiguro");
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCode_ReturnsDifferentForDifferentAuthors()
        {
            var a = new Author("Haruki Murakami");
            var b = new Author("Gabriel Garcia Marquez");
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
        }
    }
}
