using Microsoft.AspNetCore.Mvc;

namespace LibraryOnlineRentalSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}