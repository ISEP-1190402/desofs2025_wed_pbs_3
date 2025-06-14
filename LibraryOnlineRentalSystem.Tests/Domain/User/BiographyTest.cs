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
            // Test with basic text
            var bio1 = new Biography("Software developer and avid reader");
            Assert.That(bio1.Description, Is.EqualTo("Software developer and avid reader"));
            
            // Test with allowed special characters (only those in AllowedSpecialChars)
            var bio2 = new Biography("Developer with experience in C and .NET");
            Assert.That(bio2.Description, Is.EqualTo("Developer with experience in C and .NET"));
            
            // Test with other allowed characters
            var bio3 = new Biography("Hello, this is a test. Uses: commas, periods. (parentheses) and spaces are allowed! Also hyphens - and colons: and semicolons;");
            Assert.That(bio3.Description, Is.EqualTo("Hello, this is a test. Uses: commas, periods. (parentheses) and spaces are allowed! Also hyphens - and colons: and semicolons;"));
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

        [Test]
        public void Constructor_WithInvalidCharacters_ThrowsBusinessRulesException()
        {
            // Test with XSS patterns
            Assert.Throws<BusinessRulesException>(() => new Biography("<script>alert('xss')</script>"),
                "Should reject script tags");
                
            Assert.Throws<BusinessRulesException>(() => new Biography("javascript:alert('xss')"),
                "Should reject javascript: protocol");
            
            // Test with disallowed special characters
            Assert.Throws<BusinessRulesException>(() => new Biography("Bio with #hashtag"),
                "Should reject # character");
                
            Assert.Throws<BusinessRulesException>(() => new Biography("Bio with @mention"),
                "Should reject @ character");
                
            Assert.Throws<BusinessRulesException>(() => new Biography("Bio with $dollar"),
                "Should reject $ character");
                
            // Test with emoji (not in allowed characters)
            Assert.Throws<BusinessRulesException>(() => new Biography("Bio with emoji 😊"),
                "Should reject emoji characters");
                
            // Test with XSS patterns in the middle of text
            Assert.Throws<BusinessRulesException>(
                () => new Biography("Normal text <script>alert('xss')</script> more text"),
                "Should detect XSS in the middle of text");
                
            // Test with other invalid patterns
            Assert.Throws<BusinessRulesException>(
                () => new Biography("onload=alert('xss') normal text"),
                "Should detect onload handler");
                
            // Test with newlines (should be allowed as they're in AllowedSpecialChars)
            Assert.DoesNotThrow(() => new Biography("Line 1\nLine 2"),
                "Should allow newlines");
                
            // Test with allowed special characters (should pass)
            Assert.DoesNotThrow(() => new Biography("Test with allowed chars: .,!?()-:;' \n\r"),
                "Should allow basic punctuation and whitespace");
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
