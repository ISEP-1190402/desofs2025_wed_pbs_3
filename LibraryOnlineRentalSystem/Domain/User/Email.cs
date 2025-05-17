using System.Text.RegularExpressions;
using LibraryOnlineRentalSystem.Domain.Shared;

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
                    @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            MatchCollection m = Regex.Matches(email, detectPattern,RegexOptions.IgnoreCase);
            if (!Regex.IsMatch(email, detectPattern, RegexOptions.IgnoreCase))
            {
                throw new BusinessRulesException("The email is not valid");
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
