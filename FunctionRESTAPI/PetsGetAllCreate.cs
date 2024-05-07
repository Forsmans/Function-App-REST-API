using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionRESTAPI
{
    public class PetsGetAllCreate
    {
        private readonly AppDbContext _context;


        public PetsGetAllCreate(AppDbContext context)
        {
            _context = context;
        }

        [Function("PetsGetAllCreate")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "pets")] HttpRequest req)
        {
            string functionKey = req.Headers["x-functions-key"];
            string expectedKey = Environment.GetEnvironmentVariable("Labb3SecretKey");

            if (functionKey != expectedKey)
            {
                return new UnauthorizedResult();
            }

            if (req.Method == HttpMethods.Post)
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var pet = JsonConvert.DeserializeObject<Pet>(requestBody);
                _context.Pets.Add(pet);
                await _context.SaveChangesAsync();
                return new CreatedResult("/pets", pet);
            }

            var pets = await _context.Pets.ToListAsync();
            return new OkObjectResult(pets);
        }

    }
}
