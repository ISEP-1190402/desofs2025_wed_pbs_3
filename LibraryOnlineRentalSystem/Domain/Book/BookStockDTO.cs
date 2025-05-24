namespace LibraryOnlineRentalSystem.Domain.Book;

public class BookStockDTO
{
    public string Id { get; private set; }
    public int AmountOfCopies { get; private set; }
    
    public BookStockDTO(string id, int amountOfCopies)
    {
        Id = id;
        AmountOfCopies = amountOfCopies;
    }
    
    
}