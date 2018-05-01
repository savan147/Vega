using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Vega.Controllers.Resources;
using Vega.Models;
using Vega.Persistence;

namespace Vega.Controllers
{
    // base route
    [Route("/api/vehicles")]    // its apply to all action in this controller
    public class VehiclesController : Controller
    {
        private readonly IMapper mapper;
        private readonly VegaDbContext context;
        //Constructor
        public VehiclesController(IMapper mapper, VegaDbContext context )
        {
            this.mapper = mapper;
            this.context = context;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateVehicle([FromBody]VehicleResource vehicleResource)
        {
            //  server side validation
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            #region
            // this code is extra bcz user going to choose modelId 
            /* checking ModelId valid ot not
            var model = await context.Models.FindAsync(vehicleResource.ModelId);
            if(model == null)
            {
                ModelState.AddModelError("ModelId", "Invalid modelId");
                return BadRequest(ModelState);
            } */
            #endregion

            var vehicle = mapper.Map<VehicleResource, Vehicle>(vehicleResource);
            vehicle.LastUpdate = DateTime.Now;

            context.Vehicles.Add(vehicle);
            await context.SaveChangesAsync();

            var result = mapper.Map<Vehicle, VehicleResource>(vehicle);
            return Ok(result);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}