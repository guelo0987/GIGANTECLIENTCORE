using GIGANTECLIENTCORE.Context;
using Microsoft.AspNetCore.Mvc;

namespace GIGANTECLIENTCORE.Controllers;





[ApiController]
[Route("api/[controller]")]
public class UserController:ControllerBase
{


    private readonly MyDbContext _db;
    private readonly ILogger<UserController> _logger;

    public UserController(MyDbContext db, ILogger<UserController> logger)
    {
        _db = db;
        _logger = logger;

    }
    
    
    
    
    
    
    
    
}