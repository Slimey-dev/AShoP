using Microsoft.AspNetCore.Mvc;

namespace AShoP.Controllers;

public class DetailsController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}