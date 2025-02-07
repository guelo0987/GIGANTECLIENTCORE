using GIGANTECLIENTCORE.Context;
using GIGANTECLIENTCORE.Models;

namespace GIGANTECLIENTCORE.Utils;

public class AdminMultiMedia
{
    
    
    private readonly MyDbContext _context;

    public AdminMultiMedia(MyDbContext context)
    {
        _context = context;
    }
    
    
    public List<Banner> GetImages()
    {
        return _context.Banners
            .OrderBy(b => b.OrderIndex)
            .ToList();
    }

}