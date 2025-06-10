using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Domain.Common;
using NUnit.Framework;
using System;

namespace LibraryOnlineRentalSystem.Tests.Domain.User
{
    [TestFixture]
    [TestOf(typeof(Name))]
    public class NameTest
    {
        [Test]
        public void Constructor_ValidName_SetsFullName()
        {
            var name = new Name("Alice Johnson");
            Assert.That(name.FullName, Is.EqualTo("Alice Johnson"));
        }

        [Test]
        public void Constructor_TrimsWhitespace()
        {
            var name = new Name("  John Smith  ");
            Assert.That(name.FullName, Is.EqualTo("John Smith"));
        }

        [TestCase("")]
        [TestCase("    ")]
        [TestCase(null)]
        public void Constructor_NullOrEmpty_ThrowsArgumentNullException(string value)
        {
            Assert.Throws<ArgumentNullException>(() => new Name(value));
        }

        [TestCase("John1")]
        [TestCase("2Smith")]
        [TestCase("Anne3Marie")]
        public void Constructor_WithDigits_ThrowsBusinessRulesException(string value)
        {
            Assert.Throws<BusinessRulesException>(() => new Name(value));
        }

        [TestCase("John!")]
        [TestCase("Jane#Doe")]
        [TestCase("Marta*Silva")]
        [TestCase("John.Smith")]
        [TestCase("Mary-Jane")]
        public void Constructor_WithSpecialChars_ThrowsBusinessRulesException(string value)
        {
            Assert.Throws<BusinessRulesException>(() => new Name(value));
        }

        [Test]
        public void Constructor_TooLong_ThrowsBusinessRulesException()
        {
            var longName = new string('a', 41);
            Assert.Throws<BusinessRulesException>(() => new Name(longName));
        }

        [Test]
        public void Clone_ReturnsNewInstanceWithSameName()
        {
            var name = new Name("Charlie Parker");
            var clone = (Name)name.Clone();
            Assert.That(clone, Is.Not.SameAs(name));
            Assert.That(clone.FullName, Is.EqualTo("Charlie Parker"));
        }

        [Test]
        public void ToString_ReturnsFullName()
        {
            var name = new Name("Diana Prince");
            Assert.That(name.ToString(), Is.EqualTo("Diana Prince"));
        }

        [Test]
        public void Equals_SameName_CaseInsensitive_ReturnsTrue()
        {
            var a = new Name("Bruce Wayne");
            var b = new Name("bruce wayne");
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void Equals_DifferentNames_ReturnsFalse()
        {
            var a = new Name("Clark Kent");
            var b = new Name("Lois Lane");
            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            var a = new Name("Lex Luthor");
            Assert.That(a.Equals(null), Is.False);
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var a = new Name("Barry Allen");
            Assert.That(a.Equals("Barry Allen"), Is.False);
        }

        [Test]
        public void GetHashCode_SameName_ReturnsSameHash()
        {
            var a = new Name("Hal Jordan");
            var b = new Name("hal jordan");
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCode_DifferentNames_ReturnDifferentHash()
        {
            var a = new Name("Arthur Curry");
            var b = new Name("Victor Stone");
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
        }
    }
}
