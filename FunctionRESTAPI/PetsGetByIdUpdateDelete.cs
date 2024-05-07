using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionRESTAPI
{
    public class PetsGetByIdUpdateDelete
    {
        private readonly AppDbContext _context;

        public PetsGetByIdUpdateDelete(AppDbContext context)
        {
            _context = context;
        }

        [Function("PetsGetByIdUpdateDelete")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, 
            "get", "put", "delete", Route = "pets/{id}")] HttpRequest req,
            int id)
        {

            string functionKey = req.Headers["x-functions-key"];
            string expectedKey = Environment.GetEnvironmentVariable("Labb3SecretKey");

            if (functionKey != expectedKey)
            {
                return new UnauthorizedResult();
            }

            if (req.Method == HttpMethods.Get)
            {
                var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == id);
                if (pet == null) return new NotFoundResult();

                return new OkObjectResult(pet);
            }

            else if (req.Method == HttpMethods.Put)
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var pet = JsonConvert.DeserializeObject<Pet>(requestBody);
                pet.Id = id;
                _context.Pets.Update(pet);
                await _context.SaveChangesAsync();

                return new OkObjectResult(pet);
            }

            else
            {
                var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == id);
                if (pet == null) return new NotFoundResult();

                _context.Pets.Remove(pet);
                await _context.SaveChangesAsync();

                return new NoContentResult();
            }
        }
    }
}
