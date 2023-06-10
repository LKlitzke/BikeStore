using BikeStore.Models;
using BikeStore.Dal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BikeStore.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BikesController : ControllerBase
    {
        private readonly ILogger<BikesController> logger;
        IBikeUtils bikeUtils;
        //IRedisHelper redis;

        public BikesController(ILogger<BikesController> logger, IBikeUtils _bikeUtils)
        {
            this.logger = logger;
            bikeUtils = _bikeUtils;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bike>>> GetBikes()
        {
            var list = await bikeUtils.GetBikes();
            if(list == null)
            {
                return NotFound("Bikes not found or database is not accessible.");
            }
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        [ActionName("GetBikeById")]
        public async Task<ActionResult<IEnumerable<Bike>>> GetBikeById(int id)
        {
            if(id <= 0)
            {
                return BadRequest("Specify an valid Id value.");
            }
            var bike = await bikeUtils.GetBikeById(id);

            if(bike == null)
            {
                return NotFound($"Bike with ID {id} could not be found.");
            }
            return Ok(bike);
        }

        [HttpPost("{quant:int}", Name = "GenerateData")]
        [ActionName("GerenateData")]
        public async Task<ActionResult> GerenateData(int quant)
        {
            if(quant <= 0)
            {
                return BadRequest("Specify a quantity value that is greater than 0.");
            }
            await bikeUtils.GenerateData(quant);
            return Ok($"{quant} rows were successfully generated.");
        }

        [HttpPost(Name = "CreateTable")]
        [ActionName("CreateTable")]
        public async Task<ActionResult> CreateTable()
        {
            await bikeUtils.CreateTable();
            return Ok();
        }

        [HttpDelete]
        [ActionName("ClearTable")]
        public async Task<ActionResult> ClearTable()
        {
            await bikeUtils.ClearTable();
            return Ok();
        }
    }
}
