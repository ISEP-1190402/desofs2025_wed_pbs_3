using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Domain.Common;
using NUnit.Framework;
using System;

namespace LibraryOnlineRentalSystem.Tests.Domain.User
{
    [TestFixture]
    [TestOf(typeof(Biography))]
    public class BiographyTest
    {
        [Test]
        public void Constructor_WithValidBiography_SetsDescription()
        {
            var bio = new Biography("Software developer and avid reader");
            Assert.That(bio.Description, Is.EqualTo("Software developer and avid reader"));
        }

        [Test]
        public void Constructor_TrimsWhitespace()
        {
            var bio = new Biography("  Hello World  ");
            Assert.That(bio.Description, Is.EqualTo("Hello World"));
        }

        [Test]
        public void Constructor_EmptyOrWhitespace_SetsDescriptionToEmpty()
        {
            var bio1 = new Biography("");
            var bio2 = new Biography("    ");
            Assert.That(bio1.Description, Is.EqualTo(""));
            Assert.That(bio2.Description, Is.EqualTo(""));
        }

        [Test]
        public void Constructor_TooLong_ThrowsBusinessRulesException()
        {
            var longText = new string('a', 151);
            Assert.Throws<BusinessRulesException>(() => new Biography(longText));
        }

        [TestCase("Text with !")]
        [TestCase("Hello, world.")]
        [TestCase("Bio with emoji 😊")]
        [TestCase("Bio with #hashtag")]
        public void Constructor_SpecialCharacters_ThrowsBusinessRulesException(string input)
        {
            Assert.Throws<BusinessRulesException>(() => new Biography(input));
        }

        [Test]
        public void Clone_ReturnsNewInstanceWithSameDescription()
        {
            var bio = new Biography("Clone me");
            var clone = (Biography)bio.Clone();
            Assert.That(clone, Is.Not.SameAs(bio));
            Assert.That(clone.Description, Is.EqualTo("Clone me"));
        }

        [Test]
        public void ToString_ReturnsDescription()
        {
            var bio = new Biography("Just a bio");
            Assert.That(bio.ToString(), Is.EqualTo("Just a bio"));
        }

        [Test]
        public void Equals_SameDescription_CaseInsensitive_ReturnsTrue()
        {
            var a = new Biography("A biography");
            var b = new Biography("a BIOGRAPHY");
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void Equals_DifferentDescriptions_ReturnsFalse()
        {
            var a = new Biography("Bio 1");
            var b = new Biography("Bio 2");
            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            var a = new Biography("Bio");
            Assert.That(a.Equals(null), Is.False);
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var a = new Biography("Bio");
            Assert.That(a.Equals("Bio"), Is.False);
        }

        [Test]
        public void GetHashCode_SameDescription_ReturnsSameHash()
        {
            var a = new Biography("Same Bio");
            var b = new Biography("same bio");
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCode_DifferentDescriptions_ReturnsDifferentHash()
        {
            var a = new Biography("First Bio");
            var b = new Biography("Second Bio");
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
        }
    }
}
