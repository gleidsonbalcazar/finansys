using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.FinanSys.Models.Entities;
using Api.FinanSys.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace Api.FinanSys.Controllers.Launchs
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LaunchsController : Controller
    {
        private readonly LaunchRepository _repository;
        public LaunchsController(LaunchRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Launch>>> Get()
        {
            return await _repository.GetAll();

        }

        [HttpGet("{month}/{year}/{accountID}")]
        public ActionResult<IEnumerable<Launch>> Get(int month, int year, int accountID)
        {
            var entity = _repository.GetAllOrdered(month, year, accountID);
            if (entity == null)
            {
                return NotFound();
            }
            return entity;
        }

        [HttpPost("checkDuplicate")]
        public ActionResult<bool> CheckDuplicate(Launch launch)
        {
            return _repository.CheckDuplicate(launch);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Launch entity)
        {

            if (id != entity.Id)
            {
                return BadRequest();
            }
            await _repository.Update(entity);
            return CreatedAtAction("Get", new { id = entity.Id }, entity);
        }

        [HttpPost]
        public async Task<ActionResult<Launch>> Post(Launch entity)
        {
            entity.UserCreated = User.Identity.Name;
            entity.DateCreated = DateTime.Now;

            await _repository.Add(entity);
            return CreatedAtAction("Get", new { id = entity.Id }, entity);
        }

        [HttpPost("import/{accountId}")]
        public async Task<ActionResult<ImportRequest>> Import(ImportRequest importRequest, int accountId)
        {

            await _repository.ProcessImport(importRequest, accountId);
            return CreatedAtAction("Get", new { id = accountId });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Launch>> Delete(int id)
        {
            var entity = await _repository.Delete(id);
            if (entity == null)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}