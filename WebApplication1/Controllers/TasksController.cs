using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace TodoAPI.Controllers
{
    public class TodoTask
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Status { get; set; } = "open";
        public int UserId { get; set; }
    }

    [Authorize]
    [ApiController]
    [Route("tasks")]
    public class TasksController : ControllerBase
    {
        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "./db/tasks/tasks.json");

        private async Task<List<TodoTask>> ReadTasksAsync()
        {
            try
            {
                if (!System.IO.File.Exists(_filePath))
                {
                    return new List<TodoTask>();
                }

                var json = await System.IO.File.ReadAllTextAsync(_filePath);
                return JsonSerializer.Deserialize<List<TodoTask>>(json) ?? new List<TodoTask>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading tasks: {ex.Message}");
                return new List<TodoTask>();
            }
        }

        private async Task WriteTasksAsync(List<TodoTask> tasks)
        {
            try
            {
                var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
                await System.IO.File.WriteAllTextAsync(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing tasks: {ex.Message}");
                throw;
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User ID not found."));
        }

        [HttpGet("list/{userId}")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var userId = GetCurrentUserId();
                var tasks = await ReadTasksAsync();
                var userTasks = tasks.Where(t => t.UserId == userId).ToList();
                return Ok(userTasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAll: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] TodoTask newTask)
        {
            try
            {
                var userId = GetCurrentUserId();
                var tasks = await ReadTasksAsync();
                newTask.Id = tasks.Count > 0 ? tasks.Max(t => t.Id) + 1 : 1;
                newTask.UserId = userId;
                tasks.Add(newTask);
                await WriteTasksAsync(tasks);
                return Ok(newTask);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Add: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("edit/{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] TodoTask updatedTask)
        {
            try
            {
                var userId = GetCurrentUserId();
                var tasks = await ReadTasksAsync();
                var task = tasks.FirstOrDefault(t => t.Id == id && t.UserId == userId);
                if (task == null)
                {
                    return NotFound();
                }

                task.Text = updatedTask.Text;
                task.Status = updatedTask.Status;
                await WriteTasksAsync(tasks);
                return Ok(task);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Edit: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var tasks = await ReadTasksAsync();
                var task = tasks.FirstOrDefault(t => t.Id == id && t.UserId == userId);
                if (task == null)
                {
                    return NotFound();
                }

                tasks.Remove(task);
                await WriteTasksAsync(tasks);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Delete: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("toggle/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var tasks = await ReadTasksAsync();
                var task = tasks.FirstOrDefault(t => t.Id == id && t.UserId == userId);
                if (task == null)
                {
                    return NotFound();
                }

                task.Status = task.Status switch
                {
                    "open" => "in-progress",
                    "in-progress" => "completed",
                    "completed" => "open",
                    _ => task.Status
                };

                await WriteTasksAsync(tasks);
                return Ok(task);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ToggleStatus: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}