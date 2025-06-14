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
        [Test]
        public void Constructor_ValidISBN_DoesNotThrow()
        {
            // Test valid ISBN-10 with and without hyphens
            var isbn1 = new ISBN("0-306-40615-2");
            Assert.That(isbn1.BookISBN, Is.EqualTo("0306406152"), "ISBN-10 with hyphens should be normalized");
            
            var isbn2 = new ISBN("0306406152");
            Assert.That(isbn2.BookISBN, Is.EqualTo("0306406152"), "ISBN-10 without hyphens should be preserved");
            
            // Test valid ISBN-13 with and without hyphens
            var isbn3 = new ISBN("978-3-16-148410-0");
            Assert.That(isbn3.BookISBN, Is.EqualTo("9783161484100"), "ISBN-13 with hyphens should be normalized");
            
            var isbn4 = new ISBN("9783161484100");
            Assert.That(isbn4.BookISBN, Is.EqualTo("9783161484100"), "ISBN-13 without hyphens should be preserved");
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

        [Test]
        public void Constructor_ValidatesISBNCorrectly()
        {
            // Test valid ISBNs
            var validIsbns = new[]
            {
                "0306406152",     // Valid ISBN-10
                "9783161484100",  // Valid ISBN-13
                "123456789X",     // Valid ISBN-10 with X check digit
                "0-306-40615-2",  // Valid ISBN-10 with hyphens
                "978-3-16-148410-0" // Valid ISBN-13 with hyphens
            };
            
            foreach (var isbn in validIsbns)
            {
                Assert.DoesNotThrow(() => new ISBN(isbn), $"ISBN '{isbn}' should be valid");
            }
            
            // Test invalid ISBNs
            var invalidIsbns = new[]
            {
                "1234567890",    // Invalid check digit for ISBN-10
                "9783161484104",  // Invalid check digit for ISBN-13
                "abcdefghij",     // Invalid characters
                "123",            // Too short
                "12345678901234567890", // Too long
                "",               // Empty string
                " ",              // Whitespace only
                null              // Null
            };
            
            foreach (var isbn in invalidIsbns)
            {
                if (isbn == null)
                {
                    Assert.Throws<BusinessRulesException>(() => new ISBN(isbn), "Null ISBN should be invalid");
                }
                else
                {
                    Assert.Throws<BusinessRulesException>(() => new ISBN(isbn), $"ISBN '{isbn}' should be invalid");
                }
            }
        }
    }
}
