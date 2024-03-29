using Api.FinanSys.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.FinanSys.Controllers.Budgets
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public  class BudgetsPlanController : Controller
    {
        private readonly BudgetPlanRepository budgetPlanRepository;

        public BudgetsPlanController(BudgetPlanRepository budgetPlanRepository)
        {
            this.budgetPlanRepository = budgetPlanRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BudgetPlan>>> Get(bool allFields = false)
        {
            return await budgetPlanRepository.GetAllFilter(x => !allFields ? x.Active == true : x.Active == true || x.Active == false, o => o.OrderBy(a => a));
        }

        [HttpPost]
        public async Task<ActionResult<BudgetPlan>> Post(BudgetPlan entity)
        {
            entity.UserCreated = User.Identity.Name;
            entity.DateCreated = DateTime.Now;
            await budgetPlanRepository.Add(entity);
            return CreatedAtAction("Get", new { id = entity.Id }, entity);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, BudgetPlan entity)
        {
            if (id != entity.Id)
            {
                return BadRequest();
            }

            await budgetPlanRepository.Update(entity);

            return CreatedAtAction("Get", new { id = entity.Id }, entity);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<BudgetPlan>> Delete(int id)
        {
            var entity = await budgetPlanRepository.Delete(id);
            if (entity == null)
            {
                return NotFound();
            }
            return entity;
        }

    }
}