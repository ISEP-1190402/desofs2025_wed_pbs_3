using System;
using System.Linq;
using System.Text.RegularExpressions;
using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.User
{
    public class Biography : ICloneable, IValueObject
    {
        // Allowed special characters - minimal safe set
        private static readonly HashSet<char> AllowedSpecialChars = new() 
        { 
            ' ', '.', ',', '!', '?', '\'', '(', ')', '-', ':', ';', '\n', '\r' 
        };

        public Biography() 
        { 
            Description = string.Empty;
        }

        public Biography(string biography)
        {
            if (string.IsNullOrWhiteSpace(biography))
            {
                Description = string.Empty;
                return;
            }

       
            string sanitized = Regex.Replace(biography.Trim(), @"\s+", " ");
            
      
            sanitized = Regex.Replace(sanitized, @"<[^>]+>", string.Empty);
            
       
            sanitized = Regex.Replace(sanitized, "[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", "");

            
            if (sanitized.Length > 150)
                throw new BusinessRulesException("Description cannot exceed 150 characters.");

            
            if (!IsValidBiography(sanitized))
                throw new BusinessRulesException("Biography contains invalid characters. Only letters, numbers, and basic punctuation are allowed.");
            
            if (ContainsXssPatterns(sanitized))
                throw new BusinessRulesException("Invalid input detected.");

            Description = sanitized;
        }

        public string Description { get; }

        private static bool IsValidBiography(string input)
        {
            return input.All(c => 
                char.IsLetterOrDigit(c) || 
                char.IsWhiteSpace(c) || 
                AllowedSpecialChars.Contains(c));
        }

        private static bool ContainsXssPatterns(string input)
        {
          
            string lowerInput = input.ToLowerInvariant();
            
      
            var xssPatterns = new[]
            {
                "<script", 
                "javascript:", 
                "onload=", 
                "onerror=", 
                "onclick=",
                "onmouseover=",
                "document.cookie",
                "eval(",
                "alert(",
                "fromcharcode(",
                "document.domain",
                "window.location",
                "document.write",
                "innerhtml",
                "settimeout(",
                "setinterval("
            };

            return xssPatterns.Any(pattern => lowerInput.Contains(pattern));
        }

        public object Clone()
        {
            return new Biography(Description);
        }

        public override string ToString()
        {
            return Description;
        }

        public override bool Equals(object? obj)
        {
            if (this == obj) return true;
            if (obj is not Biography other) return false;
            return string.Equals(Description, other.Description, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(Description ?? string.Empty);
        }
    }
}