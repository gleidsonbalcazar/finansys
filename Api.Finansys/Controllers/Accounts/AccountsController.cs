using System.Collections.Generic;
using System.Threading.Tasks;
using Api.FinanSys.Models.Entities;
using Api.FinanSys.Models.Presentations.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace Api.FinanSys.Controllers.Accounts
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly AccountRepository _repository;
        public AccountsController(AccountRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("GetAllAccountsWithDetail")]
        public async Task<ActionResult<IEnumerable<AccountDetailsPresentation>>> GetAllAccountsWithDetail([System.Web.Http.FromUri] int month)
        {
            var userLogged = User.Identity.Name;
            return _repository.GetAllAccountsWithDetail(month, userLogged);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountsBasePresentation>>> GetAll()
        {
            return _repository.GetAll();

        }

        [HttpGet("user/{id}")]
        public async Task<ActionResult<IEnumerable<AccountsByUserPresentation>>> GetAccountByUser(int id)
        {
            return _repository.GetAccountByUser(id);
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