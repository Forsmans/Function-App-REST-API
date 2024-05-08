using FunctionRESTAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FunctionRESTAPI.Functions
{
    public class GetPet
    {
        private readonly ILogger<GetPet> _logger;
        private readonly AppDbContext _context;

        public GetPet(ILogger<GetPet> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("GetPet")]
        public async Task<IActionResult> GetPetId([HttpTrigger(AuthorizationLevel.Function, "get", Route = "pet/{id}")] HttpRequest req, int id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string functionKey = req.Headers["x-functions-key"];
            string expectedKey = Environment.GetEnvironmentVariable("Labb3SecretKey");

            if (functionKey != expectedKey)
            {
                return new UnauthorizedResult();
            }

            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == id);
            if (pet == null) return new NotFoundResult();

            return new OkObjectResult(pet);
        }
    }
}
