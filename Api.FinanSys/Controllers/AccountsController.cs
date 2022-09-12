using System.Collections.Generic;
using System.Threading.Tasks;
using Api.FinanSys.Models.ViewModels;
using FinansysControl.Models;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace FinansysControl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //Authorize]
    public class AccountsController : Controller
    {
        private readonly AccountRepository _repository;
        public AccountsController(AccountRepository repository) 
        {
            this._repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountViewModel>>> Get()
        {
            return _repository.GetAll();

        }
        

        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> Get(int id)
        {
            var entity = await _repository.Get(id);
            if (entity == null)
            {
                return NotFound();
            }
            return entity;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Account entity)
        {
            if (id != entity.Id)
            {
                return BadRequest();
            }

            await _repository.Update(entity);

            return CreatedAtAction("Get", new { id = entity.Id }, entity);
        }

        [HttpPost]
        public async Task<ActionResult<Account>> Post(Account entity)
        {
            await _repository.Add(entity);
            return CreatedAtAction("Get", new { id = entity.Id }, entity);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Account>> Delete(int id)
        {
            var entity = await _repository.Delete(id);
            if (entity == null)
            {
                return NotFound();
            }
            return entity;
        }

    }
}