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
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string functionKey = req.Headers["x-functions-key"];
            string expectedKey = Environment.GetEnvironmentVariable("Labb3SecretKey");

            if (functionKey != expectedKey)
            {
                return new UnauthorizedResult();
            }

            var pets = await _context.Pets.ToListAsync();
            return new OkObjectResult(pets);
        }
    }
}
