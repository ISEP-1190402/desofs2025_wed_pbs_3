using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Domain.Common;
using NUnit.Framework;
using System;

namespace LibraryOnlineRentalSystem.Tests.Domain.Books
{
    [TestFixture]
    [TestOf(typeof(ISBN))]
    public class ISBNTest
    {
        [TestCase("0-306-40615-2")]
        [TestCase("0306406152")]
        [TestCase("978-3-16-148410-0")]
        [TestCase("9783161484100")]
        [TestCase("0-321-14653-0")]
        [TestCase("0321146530")]
        public void Constructor_ValidISBN_DoesNotThrow(string validISBN)
        {
            var isbn = new ISBN(validISBN);
            Assert.That(isbn.BookISBN, Is.EqualTo(validISBN.Trim()));
        }

        [TestCase("")]
        [TestCase(null)]
        public void Constructor_NullOrEmpty_ThrowsBusinessRulesException(string input)
        {
            Assert.Throws<BusinessRulesException>(() => new ISBN(input));
        }

        [TestCase("123")]
        [TestCase("abcdefghij")]
        [TestCase("123456789X1")]
        [TestCase("9783161484109")]
        [TestCase("978-3-16-148410-1")]
        public void Constructor_InvalidISBN_ThrowsBusinessRulesException(string invalidISBN)
        {
            Assert.Throws<BusinessRulesException>(() => new ISBN(invalidISBN));
        }

        [Test]
        public void Constructor_TrimsWhitespace()
        {
            var isbn = new ISBN(" 9783161484100 ");
            Assert.That(isbn.BookISBN, Is.EqualTo("9783161484100"));
        }

        [Test]
        public void GetISBN_ReturnsCorrectValue()
        {
            var isbn = new ISBN("0321146530");
            Assert.That(isbn.GetISBN(), Is.EqualTo("0321146530"));
        }

        [Test]
        public void ValueOf_ReturnsNewInstanceWithGivenISBN()
        {
            var isbn = new ISBN("0306406152");
            var newIsbn = isbn.ValueOf("9783161484100");
            Assert.That(newIsbn.BookISBN, Is.EqualTo("9783161484100"));
        }

        [Test]
        public void Equals_SameValues_CaseInsensitive_ReturnsTrue()
        {
            var a = new ISBN("9783161484100");
            var b = new ISBN("9783161484100");
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            var a = new ISBN("0321146530");
            var b = new ISBN("0306406152");
            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            var a = new ISBN("0321146530");
            Assert.That(a.Equals(null), Is.False);
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var a = new ISBN("0321146530");
            Assert.That(a.Equals("0321146530"), Is.False);
        }

        [Test]
        public void ToString_ReturnsBookISBN()
        {
            var isbn = new ISBN("9783161484100");
            Assert.That(isbn.ToString(), Is.EqualTo("9783161484100"));
        }

        [Test]
        public void GetHashCode_ReturnsSameForEqualISBNs()
        {
            var a = new ISBN("0321146530");
            var b = new ISBN("0321146530");
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCode_ReturnsDifferentForDifferentISBNs()
        {
            var a = new ISBN("0306406152");
            var b = new ISBN("9783161484100");
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
        }

        [TestCase("0-306-40615-2", true)]
        [TestCase("0306406152", true)]
        [TestCase("978-3-16-148410-0", true)]
        [TestCase("9783161484100", true)]
        [TestCase("9780306406157", true)]
        [TestCase("123456789X", true)]
        [TestCase("1234567890", false)]
        [TestCase("9783161484109", false)]
        [TestCase("abcdefghij", false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        public void IsISBNValid_WorksForVariousCases(string input, bool expected)
        {
            var result = ISBN.IsISBNValid(input);
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
