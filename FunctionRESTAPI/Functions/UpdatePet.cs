using FunctionRESTAPI.Data;
using FunctionRESTAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionRESTAPI.Functions
{
    public class UpdatePet
    {
        private readonly ILogger<UpdatePet> _logger;
        private readonly AppDbContext _context;

        public UpdatePet(ILogger<UpdatePet> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("UpdatePet")]
        public async Task<IActionResult> PutPet([HttpTrigger(AuthorizationLevel.Function, "put", Route = "pet/{id}")] HttpRequest req, int id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string functionKey = req.Headers["x-functions-key"];
            string expectedKey = Environment.GetEnvironmentVariable("Labb3SecretKey");

            if (functionKey != expectedKey)
            {
                return new UnauthorizedResult();
            }

            var selectedPet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == id);

            if (selectedPet == null) return new NotFoundResult();
            else
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var updatedPet = JsonConvert.DeserializeObject<Pet>(requestBody);
                selectedPet.Name = updatedPet.Name;
                selectedPet.Owner = updatedPet.Owner;
                selectedPet.Age = updatedPet.Age;
                selectedPet.Type = updatedPet.Type;
                _context.Pets.Update(selectedPet);
                await _context.SaveChangesAsync();
                return new OkObjectResult(selectedPet);
            }

            
        }
    }
}
