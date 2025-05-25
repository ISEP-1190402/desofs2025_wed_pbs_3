using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Domain.Common;
using NUnit.Framework;
using System;

namespace LibraryOnlineRentalSystem.Tests.Domain.Books
{
    [TestFixture]
    [TestOf(typeof(Category))]
    public class CategoryTest
    {
        [Test]
        public void Constructor_WithValidCategory_SetsValue()
        {
            var category = new Category("Science Fiction");
            Assert.That(category.BookCategoryName, Is.EqualTo("Science Fiction"));
        }

        [Test]
        public void Constructor_TrimsWhitespace()
        {
            var category = new Category("  Fantasy  ");
            Assert.That(category.BookCategoryName, Is.EqualTo("Fantasy"));
        }

        [Test]
        public void Constructor_Null_ThrowsBusinessRulesException()
        {
            Assert.Throws<BusinessRulesException>(() => new Category(null));
        }

        [Test]
        public void Constructor_Empty_ThrowsBusinessRulesException()
        {
            Assert.Throws<BusinessRulesException>(() => new Category(""));
        }

        [Test]
        public void GetBookCategoryName_ReturnsCorrectValue()
        {
            var category = new Category("History");
            Assert.That(category.GetBookCategoryName(), Is.EqualTo("History"));
        }

        [Test]
        public void ValueOf_ReturnsNewInstanceWithGivenName()
        {
            var category = new Category("Art");
            var newCategory = category.ValueOf("Biography");
            Assert.That(newCategory.BookCategoryName, Is.EqualTo("Biography"));
        }

        [Test]
        public void Equals_SameValues_CaseInsensitive_ReturnsTrue()
        {
            var a = new Category("Romance");
            var b = new Category("romance");
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            var a = new Category("Thriller");
            var b = new Category("Drama");
            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            var a = new Category("Adventure");
            Assert.That(a.Equals(null), Is.False);
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var a = new Category("Mystery");
            Assert.That(a.Equals("Mystery"), Is.False);
        }

        [Test]
        public void ToString_ReturnsBookCategoryName()
        {
            var category = new Category("Horror");
            Assert.That(category.ToString(), Is.EqualTo("Horror"));
        }

        [Test]
        public void GetHashCode_ReturnsSameForEqualCategories()
        {
            var a = new Category("Poetry");
            var b = new Category("Poetry");
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCode_ReturnsDifferentForDifferentCategories()
        {
            var a = new Category("Comics");
            var b = new Category("Biography");
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
        }
    }
}
