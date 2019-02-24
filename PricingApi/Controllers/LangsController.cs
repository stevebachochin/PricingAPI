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
    public class LangsController : ApiController
    {
        private PricingDB_Entities db = new PricingDB_Entities();

        // GET: api/Langs
        public IQueryable<Lang> GetLangs()
        {
            return db.Langs;
        }


        /**GET LANGUAGE BY LANG CODE     * **/

        [Route("api/LangsByCode/{langCode}")]
        [HttpGet, HttpPost]
        public IHttpActionResult GetFRLangByCode(string LangCode)
        {

            //string role = "en";
            var propertyInfo = typeof(Lang).GetProperty("Lang1");
            var result = (
                    from record in db.Langs.AsEnumerable().Where(a =>
                    propertyInfo.GetValue(a, null).ToString().ToLower().Contains(LangCode.ToLower())
                )
                    select record

            ).AsQueryable();
            //ONLY RETURN THE FIRST VALUE IN THE RESULT LIST (IN CASE THERE HAPPENS TO BE TWO OF THE SAME RECORD
            return Ok(result.First());
        }


        // GET: api/Langs/5
        [ResponseType(typeof(Lang))]
        public async Task<IHttpActionResult> GetLang(int id)
        {
            Lang lang = await db.Langs.FindAsync(id);
            if (lang == null)
            {
                return NotFound();
            }

            return Ok(lang);
        }

        // PUT: api/Langs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLang(int id, Lang lang)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != lang.lid)
            {
                return BadRequest();
            }

            db.Entry(lang).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LangExists(id))
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

        // POST: api/Langs
        [ResponseType(typeof(Lang))]
        public async Task<IHttpActionResult> PostLang(Lang lang)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Langs.Add(lang);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = lang.lid }, lang);
        }

        // DELETE: api/Langs/5
        [ResponseType(typeof(Lang))]
        public async Task<IHttpActionResult> DeleteLang(int id)
        {
            Lang lang = await db.Langs.FindAsync(id);
            if (lang == null)
            {
                return NotFound();
            }

            db.Langs.Remove(lang);
            await db.SaveChangesAsync();

            return Ok(lang);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LangExists(int id)
        {
            return db.Langs.Count(e => e.lid == id) > 0;
        }
    }
}