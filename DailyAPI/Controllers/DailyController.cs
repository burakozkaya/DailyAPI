using DailyAPI.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DailyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DailyController : ControllerBase
    {
        private readonly AppDbContext _context;
        public DailyController(AppDbContext context)
        {
            _context = context;
        }

    }
}
