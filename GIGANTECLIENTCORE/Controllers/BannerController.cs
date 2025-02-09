using GIGANTECLIENTCORE.Context;
using GIGANTECLIENTCORE.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GIGANTECLIENTCORE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BannerController : ControllerBase
{
    private readonly MyDbContext _db;
    private readonly ILogger<BannerController> _logger;
    
    public BannerController(MyDbContext db, ILogger<BannerController> logger)
    {
        _logger = logger;
        _db = db;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetBanner()
    {
        _logger.LogInformation("Obteniendo Imagenes...");
        var adminMultiMedia = new AdminMultiMedia(_db);
        var images = await Task.Run(() => adminMultiMedia.GetImages());
        return Ok(images);
    }
}