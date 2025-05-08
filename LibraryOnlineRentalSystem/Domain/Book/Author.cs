namespace LibraryOnlineRentalSystem.Domain.Book;

public class Author
{
   
        public string BookAuthor { get;  private set; } 
        

        public Author(string author)
        {
                if (string.IsNullOrEmpty(author))
                {
                        throw new ArgumentException("Author cannot be null or empty");
                }

                this.BookAuthor = author.Trim();
        }


        public string GetBookAuthor()
        {
                return BookAuthor;
        }


        public Author ValueOf(string author)
        {
                return new Author(author);
        }
    
        public override bool Equals(object? obj)
        {
                if (this == obj) return true;

                if (obj == null || obj.GetType() != GetType()) return false;

                var that = (Author) obj;

                return BookAuthor.ToUpper().Equals(that.BookAuthor.ToUpper());
        }

        public override string ToString()
        {
                return $"{BookAuthor}";
        }
}