using Microsoft.AspNetCore.Mvc;

namespace LibraryOnlineRentalSystem;

public class Controller1 : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}