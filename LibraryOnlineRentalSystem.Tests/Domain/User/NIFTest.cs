using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Domain.Common;
using NUnit.Framework;
using System;

namespace LibraryOnlineRentalSystem.Tests.Domain.User
{
    [TestFixture]
    [TestOf(typeof(NIF))]
    public class NIFTest
    {
        [Test]
        public void Constructor_ValidNIF_SetsTaxID()
        {
            var nif = new NIF("123456789");
            Assert.That(nif.TaxID, Is.EqualTo("123456789"));
        }

        [Test]
        public void Constructor_TrimsWhitespace()
        {
            var nif = new NIF(" 123456789 ");
            Assert.That(nif.TaxID, Is.EqualTo("123456789"));
        }

        [TestCase("")]
        [TestCase(null)]
        public void Constructor_NullOrEmpty_ThrowsArgumentNullException(string value)
        {
            Assert.Throws<ArgumentNullException>(() => new NIF(value));
        }

        [TestCase("12345678")]       // Too short
        [TestCase("1234567890")]     // Too long
        [TestCase("abcdefghj")]      // Letters
        [TestCase("12345678a")]      // Mixed
        [TestCase("1234 5678")]      // Space
        [TestCase("123-456789")]     // Dash
        public void Constructor_InvalidFormat_ThrowsBusinessRulesException(string value)
        {
            Assert.Throws<BusinessRulesException>(() => new NIF(value));
        }

        [Test]
        public void Clone_ReturnsNewInstanceWithSameTaxID()
        {
            var nif = new NIF("123456789");
            var clone = (NIF)nif.Clone();
            Assert.That(clone, Is.Not.SameAs(nif));
            Assert.That(clone.TaxID, Is.EqualTo("123456789"));
        }

        [Test]
        public void ToString_ReturnsTaxID()
        {
            var nif = new NIF("987654321");
            Assert.That(nif.ToString(), Is.EqualTo("987654321"));
        }

        [Test]
        public void Equals_SameTaxID_ReturnsTrue()
        {
            var a = new NIF("111222333");
            var b = new NIF("111222333");
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void Equals_DifferentTaxID_ReturnsFalse()
        {
            var a = new NIF("123456789");
            var b = new NIF("987654321");
            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            var a = new NIF("123456789");
            Assert.That(a.Equals(null), Is.False);
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var a = new NIF("123456789");
            Assert.That(a.Equals("123456789"), Is.False);
        }

        [Test]
        public void GetHashCode_SameTaxID_ReturnsSameHash()
        {
            var a = new NIF("123456789");
            var b = new NIF("123456789");
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCode_DifferentTaxID_ReturnDifferentHash()
        {
            var a = new NIF("123456789");
            var b = new NIF("987654321");
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
        }
    }
}
