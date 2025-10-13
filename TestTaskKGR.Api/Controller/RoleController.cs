using Microsoft.AspNetCore.Mvc;
using Serilog;
using Microsoft.EntityFrameworkCore;
using TestTaskKGR.ViewModel;
using TestTaskKGR.DAL;

namespace TestTaskKGR.Api
{
    [Route("[controller]")]
    public class RoleController : Controller
    {
        private readonly IRepository<RoleViewModel> _repo;
        public RoleController(IRepository<RoleViewModel> repo)
        {
            _repo = repo;
        }

        [HttpPost]
        public ActionResult Create([FromBody] RoleViewModel client)
        {
            var response = _repo.Create(client);
            _repo.Save();
            return StatusCode(200,response);
        }

        [HttpGet("{id}")]
        public ActionResult Get([FromRoute] int id)
        {
            return StatusCode(200, _repo.GetItem(id));
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _repo.Delete(id);
            _repo.Save();
            return StatusCode(201,null);
        }
    }
}