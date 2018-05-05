using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IVehicleRepository repository;

        //Constructor
        public VehiclesController(IMapper mapper, VegaDbContext context, IVehicleRepository repository )
        {
            this.mapper = mapper;
            this.context = context;
            this.repository = repository;
        }
        /*
         *this method for update vehicle information
         */
       
        [HttpPut("{id}")]
        #region UpdateVehicle method
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody]SaveVehicleResource vehicleResource)
        {
            //  server side validation
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var vehicle = await repository.GetVehicle(id);

            if (vehicle == null)
                return NotFound();

            mapper.Map<SaveVehicleResource, Vehicle>(vehicleResource, vehicle);
            vehicle.LastUpdate = DateTime.Now;

            
            await context.SaveChangesAsync();

            var result = mapper.Map<Vehicle, VehicleResource>(vehicle);
            return Ok(result);
        }
        #endregion

        //This method for adding new vehicle
       
        [HttpPost]
        #region CreateVehicle method
        public async Task<IActionResult> CreateVehicle([FromBody]SaveVehicleResource vehicleResource)
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

            var vehicle = mapper.Map<SaveVehicleResource, Vehicle>(vehicleResource);
            vehicle.LastUpdate = DateTime.Now;

            context.Vehicles.Add(vehicle);
            await context.SaveChangesAsync();

            vehicle = await repository.GetVehicle(vehicle.Id);

            var result = mapper.Map<Vehicle, VehicleResource>(vehicle);
            return Ok(result);
        }
        #endregion

        //This mehtod for Delete vehicle
        #region DeleteVehicle method
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var vehicle = await context.Vehicles.FindAsync(id);

            if (vehicle == null)
                return NotFound();

            context.Remove(vehicle);
            await context.SaveChangesAsync();

            return Ok(id);
        }
        #endregion

        // This method for Grt vehicle informathion
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicle(int id)
        {
            var vehicle = await repository.GetVehicle(id);

            if (vehicle == null)
                return NotFound();

            var vehicleResource = mapper.Map<Vehicle, VehicleResource>(vehicle);

            return Ok(vehicleResource);


        }
    }
}