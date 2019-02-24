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
    public class FR_LangController : ApiController
    {
        private PricingDB_Entities db = new PricingDB_Entities();

        // GET: api/FR_Lang
        public IQueryable<FR_Lang> GetFR_Lang()
        {
            return db.FR_Lang;
        }


        /**GET LANGUAGE BY LANG CODE     * **/

        [Route("api/FRLangByCode/{langCode}")]
        [HttpGet, HttpPost]
        public IHttpActionResult GetFRLangByCode(string LangCode)
        {

            //string role = "en";
            var propertyInfo = typeof(FR_Lang).GetProperty("Lang");
            var result = (
                    from record in db.FR_Lang.AsEnumerable().Where(a =>
                    propertyInfo.GetValue(a, null).ToString().ToLower().Contains(LangCode.ToLower())
                )
                                select record

            ).AsQueryable();
            //ONLY RETURN THE FIRST VALUE IN THE RESULT LIST (IN CASE THERE HAPPENS TO BE TWO OF THE SAME RECORD
            return Ok(result.First());
        }




        // GET: api/FR_Lang/5
        [ResponseType(typeof(FR_Lang))]
        public async Task<IHttpActionResult> GetFR_Lang(int id)
        {
            FR_Lang fR_Lang = await db.FR_Lang.FindAsync(id);
            if (fR_Lang == null)
            {
                return NotFound();
            }

            return Ok(fR_Lang);
        }

        // PUT: api/FR_Lang/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutFR_Lang(int id, FR_Lang fR_Lang)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != fR_Lang.lfzid)
            {
                return BadRequest();
            }

            db.Entry(fR_Lang).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FR_LangExists(id))
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

        // POST: api/FR_Lang
        [ResponseType(typeof(FR_Lang))]
        public async Task<IHttpActionResult> PostFR_Lang(FR_Lang fR_Lang)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.FR_Lang.Add(fR_Lang);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = fR_Lang.lfzid }, fR_Lang);
        }

        // DELETE: api/FR_Lang/5
        [ResponseType(typeof(FR_Lang))]
        public async Task<IHttpActionResult> DeleteFR_Lang(int id)
        {
            FR_Lang fR_Lang = await db.FR_Lang.FindAsync(id);
            if (fR_Lang == null)
            {
                return NotFound();
            }

            db.FR_Lang.Remove(fR_Lang);
            await db.SaveChangesAsync();

            return Ok(fR_Lang);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FR_LangExists(int id)
        {
            return db.FR_Lang.Count(e => e.lfzid == id) > 0;
        }
    }
}