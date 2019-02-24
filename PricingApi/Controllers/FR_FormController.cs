using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json;
using PricingApi.Models;

namespace PricingApi.Controllers
{
    public class FR_FormController : ApiController
    {
        private PricingDB_Entities db = new PricingDB_Entities();

        // GET: api/FR_Form
        // public IQueryable<FR_Form> GetFR_Form()
        // {
        //     return db.FR_Form;
        // }
        // GET: api/Fields


        //public IEnumerable<FR_Form> GetFields([FromUri]PagingParameterModel pagingparametermodel)
        public async Task<IHttpActionResult> GetFields([FromUri]PagingParameterModel pagingparametermodel)
        {

            // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
            string SortOrder = pagingparametermodel.sortOrder;

            string ColumnName = pagingparametermodel.columnName;
            //WHAT FIELD IS BEING SEARCHED
            string QuerySearchName = pagingparametermodel.querySearchName;    //.ToLower();

            var source = Enumerable.Empty<FR_Form>().AsQueryable();

            var propertyInfo = typeof(FR_Form).GetProperty(ColumnName);
            // SORT Order
            if (SortOrder == "desc")
            {
                source = (
                from field in db.FR_Form.AsEnumerable().OrderByDescending(a =>
                    propertyInfo.GetValue(a, null)
                )
                select field
            ).AsQueryable();
            }
            else
            {
                source = (
                from field in db.FR_Form.AsEnumerable().OrderBy(a =>
                    propertyInfo.GetValue(a, null)
                )
                select field
            ).AsQueryable();
            }


            if (!string.IsNullOrEmpty(pagingparametermodel.querySearch))
            {
                var propertyQueryInfo = typeof(FR_Form).GetProperty(pagingparametermodel.querySearchName);
                //CHECK FOR AND EXCLUDE NULL SEARCH VALUES
                source = (from record in source.AsEnumerable().Where(
                        a => 
                        propertyQueryInfo.GetValue(a, null) != null && 
                        propertyQueryInfo.GetValue(a, null).ToString().ToLower().Contains(pagingparametermodel.querySearch.ToLower())
                        )
                          select record
                    ).AsQueryable();
            }
            //return Ok(source);
            //int count = db.Products.Count();
            int count = source.Count();

            // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
            int CurrentPage = pagingparametermodel.pageNumber;

            // Parameter is passed from Query string if it is null then it default Value will be pageSize:20  
            int PageSize = pagingparametermodel.pageSize;

            // Display TotalCount to Records to User  
            int TotalCount = count;

            // Calculating Totalpage by Dividing (No of Records / Pagesize)  
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

            // Returns List of Customer after applying Paging   
            var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

            // if CurrentPage is greater than 1 means it has previousPage  
            var previousPage = CurrentPage > 1 ? "Yes" : "No";

            // if TotalPages is greater than CurrentPage means it has nextPage  
            var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

            // Object which we are going to send in header   
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage
            };

            // Setting Header  

            HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
            // Returing List of Details
            return Ok(items);

        }


        // GET: api/FR_Form/5
        [ResponseType(typeof(FR_Form))]
        public async Task<IHttpActionResult> GetFR_Form(int id)
        {
            FR_Form fR_Form = await db.FR_Form.FindAsync(id);
            if (fR_Form == null)
            {
                return NotFound();
            }

            return Ok(fR_Form);
        }

        // PUT: api/FR_Form/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutFR_Form(int id, FR_Form fR_Form)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != fR_Form.fzid)
            {
                return BadRequest();
            }

            db.Entry(fR_Form).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FR_FormExists(id))
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

        // POST: api/FR_Form
        [ResponseType(typeof(FR_Form))]
        public async Task<IHttpActionResult> PostFR_Form(FR_Form fR_Form)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.FR_Form.Add(fR_Form);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = fR_Form.fzid }, fR_Form);
        }

        // DELETE: api/FR_Form/5
        [ResponseType(typeof(FR_Form))]
        public async Task<IHttpActionResult> DeleteFR_Form(int id)
        {
            FR_Form fR_Form = await db.FR_Form.FindAsync(id);
            if (fR_Form == null)
            {
                return NotFound();
            }

            db.FR_Form.Remove(fR_Form);
            await db.SaveChangesAsync();

            return Ok(fR_Form);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FR_FormExists(int id)
        {
            return db.FR_Form.Count(e => e.fzid == id) > 0;
        }
    }
}