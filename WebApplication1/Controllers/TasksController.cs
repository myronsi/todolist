@using Microsoft.AspNetCore.Authorization;
@using Microsoft.AspNetCore.Mvc;
@using Microsoft.Extensions.Logging;
@using System.Collections.Generic;
@using System.IO;
@using System.Linq;
@using System.Security.Claims;
@using System.Text.Json;
@using System.Threading.Tasks;
< p > @namespace TodoAPI.Controllers
@{
public class TodoTask
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Status { get; set; } = "open";
    public int UserId { get; set; }
}</ p >
< p > [Authorize]
[ApiController]
[Route("tasks")]
public class TasksController : ControllerBase
{
    private readonly ILogger<taskscontroller> _logger;
    private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "./db/tasks/tasks.json");</taskscontroller></p>
<p>public TasksController(ILogger<taskscontroller> logger)
    {
        _logger = logger;
    }</taskscontroller></p>
<p>private async Task&#x3C;List<todotask>> ReadTasksAsync()
{
try
{
_logger.LogInformation("Reading tasks from file {FilePath}", _filePath);
if (!System.IO.File.Exists(_filePath))
{
_logger.LogInformation("Tasks file does not exist, returning empty list");
return new List<todotask>();
}</ todotask ></ todotask ></ p >
< p > var json = await System.IO.File.ReadAllTextAsync(_filePath);
var tasks = JsonSerializer.Deserialize &#x3C;List<todotask>>(json) ?? new List<todotask>();
_logger.LogInformation("Successfully read {TaskCount} tasks", tasks.Count);
return tasks;
}
catch (Exception ex)
{
_logger.LogError(ex, "Error reading tasks");
return new List<todotask>();
}
}</ todotask ></ todotask ></ todotask ></ p >
< p >private async Task WriteTasksAsync(List<todotask> tasks)
{
    try
    {
        _logger.LogInformation("Writing {TaskCount} tasks to file {FilePath}", tasks.Count, _filePath);
        var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
        await System.IO.File.WriteAllTextAsync(_filePath, json);
        _logger.LogInformation("Successfully wrote tasks to file");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error writing tasks");
        throw;
    }
}</ todotask ></ p >
< p >private int GetCurrentUserId()
{
    _logger.LogDebug("Retrieving current user ID from claims");
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    return int.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User ID not found."));
}</ p >
< p > [HttpGet("list/{userId}")]
public async Task<iactionresult> GetAll()
{
    try
    {
        var userId = GetCurrentUserId();
        _logger.LogInformation("Getting all tasks for user {UserId}", userId);
        var tasks = await ReadTasksAsync();
        var userTasks = tasks.Where(t => t.UserId == userId).ToList();
        _logger.LogInformation("Retrieved {TaskCount} tasks for user {UserId}", userTasks.Count, userId);
        return Ok(userTasks);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error in GetAll");
        return StatusCode(500, "Internal server error");
    }
}</ iactionresult ></ p >
< p > [HttpPost("add")]
public async Task<iactionresult> Add([FromBody] TodoTask newTask)
{
    try
    {
        var userId = GetCurrentUserId();
        _logger.LogInformation("Adding new task for user {UserId}: {TaskText}", userId, newTask.Text);
        var tasks = await ReadTasksAsync();
        newTask.Id = tasks.Count > 0 ? tasks.Max(t => t.Id) + 1 : 1;
        newTask.UserId = userId;
        tasks.Add(newTask);
        await WriteTasksAsync(tasks);
        _logger.LogInformation("Successfully added task with ID {TaskId} for user {UserId}", newTask.Id, userId);
        return Ok(newTask);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error in Add");
        return StatusCode(500, "Internal server error");
    }
}</ iactionresult ></ p >
< p > [HttpPut("edit/{id}")]
public async Task<iactionresult> Edit(int id, [FromBody] TodoTask updatedTask)
{
    try
    {
        var userId = GetCurrentUserId();
        _logger.LogInformation("Editing task {TaskId} for user {UserId}", id, userId);
        var tasks = await ReadTasksAsync();
        var task = tasks.FirstOrDefault(t => t.Id == id &#x26;&#x26; t.UserId == userId);
if (task == null)
        {
            _logger.LogWarning("Task {TaskId} not found for user {UserId}", id, userId);
            return NotFound();
        }</ iactionresult ></ p >
        < p > task.Text = updatedTask.Text;
        task.Status = updatedTask.Status;
        await WriteTasksAsync(tasks);
        _logger.LogInformation("Successfully edited task {TaskId} for user {UserId}", id, userId);
        return Ok(task);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error in Edit");
        return StatusCode(500, "Internal server error");
    }
}</ p >
< p > [HttpDelete("delete/{id}")]
public async Task<iactionresult> Delete(int id)
{
    try
    {
        var userId = GetCurrentUserId();
        _logger.LogInformation("Deleting task {TaskId} for user {UserId}", id, userId);
        var tasks = await ReadTasksAsync();
        var task = tasks.FirstOrDefault(t => t.Id == id &#x26;&#x26; t.UserId == userId);
if (task == null)
        {
            _logger.LogWarning("Task {TaskId} not found for user {UserId}", id, userId);
            return NotFound();
        }</ iactionresult ></ p >
        < p > tasks.Remove(task);
        await WriteTasksAsync(tasks);
        _logger.LogInformation("Successfully deleted task {TaskId} for user {UserId}", id, userId);
        return Ok();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error in Delete");
        return StatusCode(500, "Internal server error");
    }
}</ p >
< p > [HttpPut("toggle/{id}")]
public async Task<iactionresult> ToggleStatus(int id)
{
    try
    {
        var userId = GetCurrentUserId();
        _logger.LogInformation("Toggling status of task {TaskId} for user {UserId}", id, userId);
        var tasks = await ReadTasksAsync();
        var task = tasks.FirstOrDefault(t => t.Id == id &#x26;&#x26; t.UserId == userId);
if (task == null)
        {
            _logger.LogWarning("Task {TaskId} not found for user {UserId}", id, userId);
            return NotFound();
        }</ iactionresult ></ p >
        < p > task.Status = task.Status switch
        {
            "open" => "in-progress",
            "in-progress" => "completed",
            "completed" => "open",
            _ => task.Status
        };
        await WriteTasksAsync(tasks);
        _logger.LogInformation("Successfully toggled status of task {TaskId} to {NewStatus} for user {UserId}", id, task.Status, userId);
        return Ok(task);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error in ToggleStatus");
        return StatusCode(500, "Internal server error");
    }
}
}
}
</ p >
< document filename = "UsersController.cs" >
@using Microsoft.AspNetCore.Mvc;
@using Microsoft.Extensions.Logging;
@using System.Collections.Generic;
@using System.IO;
@using System.Security.Cryptography;
@using System.Text;
@using System.Text.Json;
@using System.Threading.Tasks;
@using Microsoft.IdentityModel.Tokens;
@using System.IdentityModel.Tokens.Jwt;
@using System.Security.Claims;
< p > @namespace TodoAPI.Controllers
@{
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}</ p >
< p >public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}</ p >
< p >public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}</ p >
< p > [ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly ILogger<userscontroller> _logger;
    private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "./db/users/users.json");</userscontroller></p>
<p>public UsersController(ILogger<userscontroller> logger)
    {
        _logger = logger;
    }</userscontroller></p>
<p>private async Task&#x3C;List<user>> ReadUsersAsync()
{
try
{
_logger.LogInformation("Reading users from file {FilePath}", _filePath);
if (!System.IO.File.Exists(_filePath))
{
_logger.LogInformation("Users file does not exist, returning empty list");
return new List<user>();
}</ user ></ user ></ p >
< p > var json = await System.IO.File.ReadAllTextAsync(_filePath);
var users = JsonSerializer.Deserialize &#x3C;List<user>>(json) ?? new List<user>();
_logger.LogInformation("Successfully read {UserCount} users", users.Count);
return users;
}
catch (Exception ex)
{
_logger.LogError(ex, "Error reading users");
return new List<user>();
}
}</ user ></ user ></ user ></ p >
< p >private async Task WriteUsersAsync(List<user> users)
{
    try
    {
        _logger.LogInformation("Writing {UserCount} users to file {FilePath}", users.Count, _filePath);
        var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
        await System.IO.File.WriteAllTextAsync(_filePath, json);
        _logger.LogInformation("Successfully wrote users to file");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error writing users");
        throw;
    }
}</ user ></ p >
< p >private string HashPassword(string password)
{
    using var sha256 = SHA256.Create();
    var bytes = Encoding.UTF8.GetBytes(password);
    var hash = sha256.ComputeHash(bytes);
    return Convert.ToBase64String(hash);
}</ p >
< p >private bool VerifyPassword(string password, string hash)
{
    var computedHash = HashPassword(password);
    return computedHash == hash;
}</ p >
< p > [HttpPost("register")]
public async Task<iactionresult> Register([FromBody] RegisterRequest request)
{
    try
    {
        _logger.LogInformation("Registering new user with username {Username}", request.Username);
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            _logger.LogWarning("Registration attempt with missing username or password");
            return BadRequest("Username and password are required.");
        }</ iactionresult ></ p >
        < p > var users = await ReadUsersAsync();
        if (users.Any(u => u.Username == request.Username))
        {
            _logger.LogWarning("Username {Username} already exists", request.Username);
            return Conflict("Username already exists.");
        }</ p >
        < p > var newUser = new User
        {
            Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1,
            Username = request.Username,
            Password = HashPassword(request.Password)
        };</ p >
< p > users.Add(newUser);
        await WriteUsersAsync(users);
        _logger.LogInformation("Successfully registered user {Username} with ID {UserId}", newUser.Username, newUser.Id);
        return Ok(new { newUser.Id, newUser.Username });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error in Register");
        return StatusCode(500, "Internal server error");
    }
}</ p >
< p > [HttpPost("login")]
public async Task<iactionresult> Login([FromBody] LoginRequest request)
{
    try
    {
        _logger.LogInformation("Login attempt for username {Username}", request.Username);
        var users = await ReadUsersAsync();
        var user = users.FirstOrDefault(u => u.Username == request.Username);</ iactionresult ></ p >
        < p >if (user == null || !VerifyPassword(request.Password, user.Password))
        {
            _logger.LogWarning("Invalid login attempt for username {Username}", request.Username);
            return Unauthorized("Invalid username or password.");
        }</ p >
        < p > var token = GenerateJwtToken(user);
        _logger.LogInformation("Successful login for user {Username} with ID {UserId}", user.Username, user.Id);
        return Ok(new { Token = token });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error in Login");
        return StatusCode(500, "Internal server error");
    }
}</ p >
< p >private string GenerateJwtToken(User user)
{
    _logger.LogDebug("Generating JWT token for user {Username}", user.Username);
    var claims = new[]
    {
new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
new Claim(ClaimTypes.Name, user.Username)
};</ p >
< p > var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("fWb1ZOo3kk4lgOWr+tk+WOYlV5BFDaXVpvjtkSQwy1C6ni27OaakAvrMFs+lkIgY\r\n"));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);</ p >
    < p > var token = new JwtSecurityToken(
    issuer: "TodoApi",
    audience: "TodoApi",
    claims: claims,
    expires: DateTime.Now.AddDays(1),
    signingCredentials: creds);</ p >
    < p >return new JwtSecurityTokenHandler().WriteToken(token);
}