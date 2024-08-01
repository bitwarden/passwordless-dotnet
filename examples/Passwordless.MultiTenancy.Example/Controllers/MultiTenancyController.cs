using Microsoft.AspNetCore.Mvc;

namespace Passwordless.MultiTenancy.Example.Controllers;

[ApiController]
[Route("[controller]")]
public class MultiTenancyController : ControllerBase
{
    [HttpGet("Users")]
    public async Task<IActionResult> Get([FromServices] IPasswordlessClient client)
    {
        var users = await client.ListUsersAsync();

        return Ok(users);
    }
}