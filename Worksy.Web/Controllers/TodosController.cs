using Microsoft.AspNetCore.Mvc;
using Worksy.Web.Models;

namespace Worksy.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // -> /api/todos
    public class TodosController : ControllerBase
    {
        // Lista en memoria (solo demo)
        private static readonly List<TodoItem> _todos = new()
        {
            new TodoItem { Id = 1, Title = "Aprender ASP.NET Core", IsDone = false },
            new TodoItem { Id = 2, Title = "Integrar API REST en Worksy", IsDone = true }
        };

        private static int _nextId = 3;

        // GET /api/todos
        [HttpGet]
        public ActionResult<IEnumerable<TodoItem>> GetAll()
        {
            return Ok(_todos);
        }

        // GET /api/todos/5
        [HttpGet("{id:int}")]
        public ActionResult<TodoItem> GetById(int id)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo is null)
                return NotFound();

            return Ok(todo);
        }

        // POST /api/todos
        [HttpPost]
        public ActionResult<TodoItem> Create([FromBody] TodoItem dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var todo = new TodoItem
            {
                Id = _nextId++,
                Title = dto.Title,
                IsDone = dto.IsDone
            };

            _todos.Add(todo);

            // Devuelve 201 Created con la URL del recurso
            return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
        }

        // PUT /api/todos/5
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] TodoItem dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo is null)
                return NotFound();

            todo.Title = dto.Title;
            todo.IsDone = dto.IsDone;

            // 204 No Content
            return NoContent();
        }

        // DELETE /api/todos/5
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo is null)
                return NotFound();

            _todos.Remove(todo);
            return NoContent();
        }
    }
}
