using auth_service.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace auth_service.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthoController : ControllerBase
    {
        private FoodAuthContext _db;

        public AuthoController(FoodAuthContext context)
        {
            _db = context;
        }

        [HttpGet]
        public IActionResult Test()
        {
            var listRole = _db.Roles.ToList();

            return Ok(listRole);
        }

    }
}
