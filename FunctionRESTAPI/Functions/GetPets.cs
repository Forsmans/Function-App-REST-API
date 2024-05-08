using FunctionRESTAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FunctionRESTAPI.Functions
{
    public class GetPets
    {
        private readonly ILogger<GetPets> _logger;
        private readonly AppDbContext _context;

        public GetPets(ILogger<GetPets> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("GetPets")]
        public async Task<IActionResult> GetAllPets([HttpTrigger(AuthorizationLevel.Function, "get", Route = "pet")] HttpRequest req)
        {
            var pets = await _context.Pets.ToListAsync();
            _logger.LogInformation("All pets has been successfully received!");
            return new OkObjectResult(pets);
        }
    }
}
