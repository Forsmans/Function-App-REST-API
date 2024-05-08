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
            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == id);
            if (pet == null)
            {
                _logger.LogInformation("The requested pet could not be found!");
                return new NotFoundResult();
            }

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();
            _logger.LogInformation("The pet has been successfully removed!");
            return new NoContentResult();
        }
    }
}
