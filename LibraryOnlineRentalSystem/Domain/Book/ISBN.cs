using LibraryOnlineRentalSystem.Domain.Common;
using System.Text.RegularExpressions;

namespace LibraryOnlineRentalSystem.Domain.Book;

public class ISBN : IValueObject
{
    public ISBN()
    {
    } 
    public ISBN(string isbn)
    {
        if (string.IsNullOrEmpty(isbn)) throw new BusinessRulesException("ISBN cannot be null or empty");

        if (!IsISBNValid(isbn)) throw new BusinessRulesException("Invalid ISBN");

        BookISBN = isbn.Trim();
    }

    public string BookISBN { get; }

    public override int GetHashCode()
    {
        return BookISBN.GetHashCode();
    }


    public string GetISBN()
    {
        return BookISBN;
    }


    public ISBN ValueOf(string isbn)
    {
        return new ISBN(isbn);
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;

        if (obj == null || obj.GetType() != GetType()) return false;

        var that = (ISBN)obj;

        return BookISBN.ToUpper().Equals(that.BookISBN.ToUpper());
    }

    public override string ToString()
    {
        return $"{BookISBN}";
    }

    public static bool IsISBNValid(string isbnToTest)
    {
        isbnToTest = isbnToTest.Replace("-", "").Replace(" ", "").ToUpper();

        var regex10 = new Regex(@"^\d{9}[\dX]$");
        var regex13 = new Regex(@"^\d{13}$");

        if (regex10.IsMatch(isbnToTest))
        {
            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += (isbnToTest[i] - '0') * (10 - i);
            }

            int lastCharValue = (isbnToTest[9] == 'X') ? 10 : (isbnToTest[9] - '0');
            sum += lastCharValue;

            return sum % 11 == 0;
        }
        else if (regex13.IsMatch(isbnToTest))
        {
            int sum = 0;
            for (int i = 0; i < 12; i++)
            {
                sum += (isbnToTest[i] - '0') * ((i % 2 == 0) ? 1 : 3);
            }

            int checkDigit = (isbnToTest[12] - '0');
            int calculatedCheckDigit = (10 - (sum % 10)) % 10;

            return checkDigit == calculatedCheckDigit;
        }

        return false;
    }
}