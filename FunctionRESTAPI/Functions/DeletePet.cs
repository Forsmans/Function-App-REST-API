using FunctionRESTAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FunctionRESTAPI.Functions
{
    public class DeletePet
    {
        private readonly ILogger<DeletePet> _logger;
        private readonly AppDbContext _context;

        public DeletePet(ILogger<DeletePet> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("DeletePet")]
        public async Task<IActionResult> RemovePet([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "pet/{id}")] HttpRequest req, int id)
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

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}
