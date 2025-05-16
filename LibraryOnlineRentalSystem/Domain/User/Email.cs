using System.Text.RegularExpressions;

namespace LibraryOnlineRentalSystem.Domain.User;

    public class Email : ICloneable, IValueObject
    {
        public Email(string email)
        {
            ValidateEmail(email);
            this.email = email;
        }

        private void ValidateEmail(string email)
        {
            string detectPattern =
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            MatchCollection m = Regex.Matches(email, detectPattern,RegexOptions.IgnoreCase);
            if (m.Count != 1)
            {
                throw new BusinessRuleValidationException("The email is not valid");
            }
        }

        public void ChangeEmail(string email)
        {
            ValidateEmail(email);
            this.email = email;
        }

        public string email { get; private set; }
        
        public object Clone()
        {
            Email email = new Email(this.email);
            return email;
        }
    }
