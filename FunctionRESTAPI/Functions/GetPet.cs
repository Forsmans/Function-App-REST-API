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
            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == id);
            if (pet == null)
            {
                _logger.LogInformation("The requested pet could not be found!");
                return new NotFoundResult();
            }

            _logger.LogInformation("The pet has been successfully received!");
            return new OkObjectResult(pet);
        }
    }
}
