using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using PricingApi.Models;

namespace PricingApi.Controllers
{
    public class RegionsController : ApiController
    {
        private PricingDB_Entities db = new PricingDB_Entities();

        // GET: api/Regions
        [HttpGet]
        [Route("api/Regions")]
        public IQueryable<Region> GetRegions()
        {
            return db.Regions;
        }

        // GET: api/Regions/5
        [HttpGet]
        [Route("api/Regions/{id}")]
        [ResponseType(typeof(Region))]
        public async Task<IHttpActionResult> GetRegion(int id)
        {
            Region region = await db.Regions.FindAsync(id);
            if (region == null)
            {
                return NotFound();
            }

            return Ok(region);
        }

        //GET REGION BY REGION NAME
        // POST: api/Keywords
        [HttpGet]
        [Route("api/Regions/filter/{RegionName?}")]
        public async Task<IHttpActionResult> GetRegionByName(string RegionName = null)
        {

            if (!string.IsNullOrEmpty(RegionName))
            {
                var source = Enumerable.Empty<Region>().AsQueryable();
                source = (
                    from regionList in db.Regions.AsEnumerable()
                    where regionList.Region1.ToLower() == RegionName.ToLower()
                    select regionList
                    ).AsQueryable();

                return Ok(source.ToList().FirstOrDefault());

            }
            return NotFound();

        }

        // PUT: api/Regions/5
        [HttpPut]
        [Route("api/Regions/{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRegion(int id, Region region)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != region.rid)
            {
                return BadRequest();
            }

            db.Entry(region).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Regions
        [HttpPost]
        [Route("api/Regions")]
        [ResponseType(typeof(Region))]
        public async Task<IHttpActionResult> PostRegion(Region region)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Regions.Add(region);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = region.rid }, region);
        }

        // DELETE: api/Regions/5
        [HttpDelete]
        [Route("api/Regions/{id}")]
        [ResponseType(typeof(Region))]
        public async Task<IHttpActionResult> DeleteRegion(int id)
        {
            Region region = await db.Regions.FindAsync(id);
            if (region == null)
            {
                return NotFound();
            }

            db.Regions.Remove(region);
            await db.SaveChangesAsync();

            return Ok(region);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RegionExists(int id)
        {
            return db.Regions.Count(e => e.rid == id) > 0;
        }
    }
}