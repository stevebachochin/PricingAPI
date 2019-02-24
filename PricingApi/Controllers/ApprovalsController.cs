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
    public class ApprovalsController : ApiController
    {
        private PricingDB_Entities db = new PricingDB_Entities();

#pragma warning disable 1998
        //  GET ALL APPROVALS - SORTED BY ID
        //  GET: api/Approvals
        [HttpGet]
        [Route("api/Approvals")]
        
        public async Task<IHttpActionResult> GetApprovals()
        {

            var source = Enumerable.Empty<Approval>().AsQueryable();
            source = db.Approvals.AsEnumerable().AsQueryable();

            var items = source.ToList();

            if (items.Count == 0)
            {
                return NotFound();
            }

            return Ok(items);
        }


        //  GET ALL APPROVALS - SORTED BY 'FormTypeName'
        //  GET: api/approvalsbytype

        [HttpGet]
        [Route("api/ApprovalsByTerritoryAndType")]
        [ResponseType(typeof(Approval))]
        public async Task<IHttpActionResult> GetApprovalsByFormTypeName()
        {
            var propertyInfoTerritory = typeof(Approval).GetProperty("Territory");
            var propertyInfoType = typeof(Approval).GetProperty("FormTypeName");
            var source = Enumerable.Empty<Approval>().AsQueryable();

            source = (
                from frmfield in db.Approvals.AsEnumerable().OrderBy(a => propertyInfoTerritory.GetValue(a, null)
            ).ThenBy(a => propertyInfoType.GetValue(a, null))
            select frmfield
            ).AsQueryable();

            var items = source.ToList();

            if (items.Count == 0)
            {
                return NotFound();
            }

            return Ok(items);
        }


        /**GET APPROVAL BY 2 KEYWORDS Territory and Form Type 
         * http://localhost:51789/api/approvals/filter?territory=n/a&type=FR
         * **/

        [HttpGet]
        [Route("api/Approvals/filter/{territory?}/{type?}")]
        public async Task<IHttpActionResult> GetApproval(string territory = null, string type = null)
        {

            if (!string.IsNullOrEmpty(territory) && !string.IsNullOrEmpty(type))
            {
                var source = Enumerable.Empty<Approval>().AsQueryable();
                source = (
                    from approver2 in db.Approvals.AsEnumerable()
                    where approver2.Territory.ToLower() == territory.ToLower() && approver2.FormType.ToLower() == type.ToLower()
                    select approver2
                    ).AsQueryable();

                return Ok(source.ToList().FirstOrDefault());

            }
            
            return NotFound();

        }


        [HttpGet]
        [Route("api/Approvals/{id}")]
        // GET: api/Approvals/5
        [ResponseType(typeof(Approval))]
        public async Task<IHttpActionResult> GetApproval(int id)
        {
            Approval approval = await db.Approvals.FindAsync(id);
            if (approval == null)
            {
                return NotFound();
            }

            return Ok(approval);
        }


        [HttpPut]
        [Route("api/Approvals/{id}")]
        [ResponseType(typeof(void))]
        // PUT: api/Approvals/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutApproval(int id, Approval approval)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != approval.aid)
            {
                return BadRequest();
            }

            db.Entry(approval).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApprovalExists(id))
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


        [HttpPost]
        [Route("api/Approvers")]
        // POST: api/Approvals
        [ResponseType(typeof(Approval))]
        public async Task<IHttpActionResult> PostApproval(Approval approval)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Approvals.Add(approval);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = approval.aid }, approval);
        }


        [HttpDelete]
        [Route("api/Approvals/{id}")]
        // DELETE: api/Approvals/5
        [ResponseType(typeof(Approval))]
        public async Task<IHttpActionResult> DeleteApproval(int id)
        {
            Approval approval = await db.Approvals.FindAsync(id);
            if (approval == null)
            {
                return NotFound();
            }

            db.Approvals.Remove(approval);
            await db.SaveChangesAsync();

            return Ok(approval);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ApprovalExists(int id)
        {
            return db.Approvals.Count(e => e.aid == id) > 0;
        }
    }
}