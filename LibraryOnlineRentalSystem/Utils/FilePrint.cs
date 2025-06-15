namespace LibraryOnlineRentalSystem.Utils;

public class FilePrint
{
    public static bool RentalFilePrint(string content)
    {
        try
        {
            // Caminho relativo à raiz do projeto
            string folderPath = ("Files/Rentals/");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // Nome do ficheiro único usando timestamp
            string fileName = $"rental_{DateTime.Now:yyyyMMdd_HHmmssfff}.txt";
            string filePath = Path.Combine(folderPath, fileName);

            File.WriteAllText(filePath, content);

            return true;
        }
        catch
        {
            // Pode adicionar logging aqui se quiser
            return false;
        }
    }
}