using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/", () => "Hello World5!");

//app.MapGet("/todoitems", async (TodoDb db) =>
//    await db.Todos.ToListAsync());

//app.MapGet("/todoitems/complete", async (TodoDb db) =>
//    await db.Todos.Where(t => t.IsComplete).ToListAsync());

//app.MapGet("/todoitems/{id}", async (int id, TodoDb db) =>
//    await db.Todos.FindAsync(id)
//        is Todo todo
//        ? Results.Ok(todo)
//        : Results.NotFound());

app.MapPost("/todoitems", async (Todo todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todoitems/{todo.Id}", todo);
});

//app.MapPut("/todoitems/{id}", async (int id, Todo inputTodo, TodoDb db) =>
//{
//    var todo = await db.Todos.FindAsync(id);

//    if (todo is null) return Results.NotFound();

//    todo.Name = inputTodo.Name;
//    todo.IsComplete = inputTodo.IsComplete;

//    await db.SaveChangesAsync();

//    return Results.NoContent();
//});

//app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
//{
//    if (await db.Todos.FindAsync(id) is Todo todo)
//    {
//        db.Todos.Remove(todo);
//        await db.SaveChangesAsync();
//        return Results.Ok(todo);
//    }

//    return Results.NotFound();
//});

app.Run();

class Todo
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
}

class TodoDb : DbContext
{
    public TodoDb(DbContextOptions<TodoDb> options)
        : base(options) { }

    public DbSet<Todo> Todos => Set<Todo>();
}









//var builder = WebApplication.CreateBuilder(args);
//var app = builder.Build();

////app.UseAuthentication();

//app.MapGet("/api/", () => "Hello World - api!");

//app.MapGet("/api/anon", () => "Anon endpoint2").AllowAnonymous();

//app.MapGet("/api/auth", () => "This endpoint requires authorization").RequireAuthorization();


//app.MapPost("/todoitems", async (Todo todo, TodoDb db) =>
//{
//    db.Todos.Add(todo);
//    await db.SaveChangesAsync();

//    return Results.Created($"/todoitems/{todo.Id}", todo);
//});

//app.Run();

//class TodoDb : DbContext
//{
//    public TodoDb(DbContextOptions<TodoDb> options)
//        : base(options) { }

//    public DbSet<Todo> Todos => Set<Todo>();
//}

//class Todo
//{
//    public int Id { get; set; }
//    public string? Name { get; set; }
//    public bool IsComplete { get; set; }
//}





//builder.Services.Configure<KestrelServerOptions>(options =>
//{
//    options.ConfigureHttpsDefaults(options =>
//        options.ClientCertificateMode = ClientCertificateMode.RequireCertificate);
//});

//builder.Services.AddAuthentication(
//        CertificateAuthenticationDefaults.AuthenticationScheme)
//    .AddCertificate(options =>
//    {
//        options.Events = new CertificateAuthenticationEvents
//        {
//            OnAuthenticationFailed = context =>
//              {
//                  return Task.CompletedTask;
//              },

//            OnCertificateValidated = context =>
//            {
//                var claims = new[]
//                {
//                    new Claim(
//                        ClaimTypes.NameIdentifier,
//                        context.ClientCertificate.Subject,
//                        ClaimValueTypes.String, context.Options.ClaimsIssuer),
//                    new Claim(
//                        ClaimTypes.Name,
//                        context.ClientCertificate.Subject,
//                        ClaimValueTypes.String, context.Options.ClaimsIssuer)
//                };

//                context.Principal = new ClaimsPrincipal(
//                    new ClaimsIdentity(claims, context.Scheme.Name));
//                context.Success();

//                return Task.CompletedTask;
//            }
//        };
//    });
