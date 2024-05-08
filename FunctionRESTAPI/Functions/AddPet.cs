using FunctionRESTAPI.Data;
using FunctionRESTAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionRESTAPI.Functions
{
    public class AddPet
    {
        private readonly ILogger<AddPet> _logger;
        private readonly AppDbContext _context;

        public AddPet(ILogger<AddPet> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("AddPet")]
        public async Task<IActionResult> PostPet([HttpTrigger(AuthorizationLevel.Function, "post", Route = "pet")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string functionKey = req.Headers["x-functions-key"];
            string expectedKey = Environment.GetEnvironmentVariable("Labb3SecretKey");

            if (functionKey != expectedKey)
            {
                return new UnauthorizedResult();
            }


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var pet = JsonConvert.DeserializeObject<Pet>(requestBody);
            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();
            return new CreatedResult("/pets", pet);
        }
    }
}
