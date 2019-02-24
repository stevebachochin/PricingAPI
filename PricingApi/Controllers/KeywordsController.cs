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
    public class KeywordsController : ApiController
    {
        private PricingDB_Entities db = new PricingDB_Entities();

        // GET: api/Keywords
       [HttpPost]
       [Route("api/keywords")]
       public IHttpActionResult GetKeywords(HttpRequestMessage request)
        {
            
            IEnumerable<string> headerValues = request.Headers.GetValues("App");
            var id = headerValues.FirstOrDefault();
            if (id == "Pricing")
            {
                // return db.Keywords;
                var propertyInfo = typeof(Keyword).GetProperty("Keyword1");
                {
                    var source = Enumerable.Empty<Keyword>().AsQueryable();
                    source = (
                    from field in db.Keywords.AsEnumerable().OrderBy(a =>
                        propertyInfo.GetValue(a, null)
                    )
                    select field
                    ).AsQueryable();
                    return Ok(source);
                }
            }
            return NotFound();
        }

        // GET: api/Keywords/5
        [HttpPost]
        [Route("api/keywords/{kid}")]
        [ResponseType(typeof(Keyword))]
        //public async Task<IHttpActionResult> GetKeyword(int id)
        public IHttpActionResult GetKeyword(HttpRequestMessage request, int kid)
        {
            Keyword keyword = db.Keywords.Find(kid);
            if (keyword == null)
            {
                return NotFound();
            }

            IEnumerable<string> headerValues = request.Headers.GetValues("App");
            var id = headerValues.FirstOrDefault();
            if (id == "Pricing")
            {
                return Ok(keyword);
            }

            return NotFound();
        }

        // PUT: api/Keywords/5
        [HttpPut]
        [Route("api/keywords/{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutKeyword(int id, Keyword keyword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != keyword.kid)
            {
                return BadRequest();
            }

            db.Entry(keyword).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KeywordExists(id))
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

        // POST: api/Keywords
        [HttpPost]
        [Route("api/keyword")]
        [ResponseType(typeof(Keyword))]
        public async Task<IHttpActionResult> PostKeyword(Keyword keyword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Keywords.Add(keyword);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = keyword.kid }, keyword);
        }

        // DELETE: api/Keywords/5
        [HttpDelete]
        [Route("api/keywords/{id}")]
        [ResponseType(typeof(Keyword))]
        public async Task<IHttpActionResult> DeleteKeyword(int id)
        {
            Keyword keyword = await db.Keywords.FindAsync(id);
            if (keyword == null)
            {
                return NotFound();
            }

            db.Keywords.Remove(keyword);
            await db.SaveChangesAsync();

            return Ok(keyword);
        }

        /**GET KEYWORD BY KEYWORD VALUE
 * http://localhost:51789/api/approvals/filter?territory=n/a&type=FR
 * **/

        [HttpPost]
        [Route("api/keywords/filter/{Keyword1?}")]
        //public async Task<IHttpActionResult> GetApproval(string territory = null, string type = null)
        public async Task<IHttpActionResult> GetKeywordValues(string Keyword1 = null)
        {

            if (!string.IsNullOrEmpty(Keyword1))
            {
                var source = Enumerable.Empty<Keyword>().AsQueryable();
                source = (
                    from keywordList in db.Keywords.AsEnumerable()
                    where keywordList.Keyword1.ToLower() == Keyword1.ToLower()
                    select keywordList
                    ).AsQueryable();

                return Ok(source.ToList().FirstOrDefault());

            }

            return NotFound();

        }










        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool KeywordExists(int id)
        {
            return db.Keywords.Count(e => e.kid == id) > 0;
        }
    }
}