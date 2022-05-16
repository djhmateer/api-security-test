using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/textget", () =>
    // returns a 200
    "hello from textget"
);

app.MapGet("/jsonget", () =>
   // this will serialise the object and returns json by default as is of type T
   // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-6.0#responses
   new { Message = "hello world from jsonget" }
);


// https://docs.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-6.0&tabs=visual-studio
// accepts a POST
// and will deserialise a Todo item
app.MapPost("/todoitems", Handler2);
async Task<IResult> Handler2(Todo todo, TodoDb db)
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    // Newtonsoft.Json
    // System.Text.Json.Serialization

    var foo = new Todo() { Id = 7, IsComplete = true, Name = "Foo" };

    // Decides the IResult implementation
    // returns a 201 Created
    return Results.Created($"/todoitems/{todo.Id}", todo);
}

app.MapPost("/hs", Handler3);
async Task<IResult> Handler3(HSDto hsdto)
{
    var rnd = new Random().Next(1, 10);

    // write a text file
    // Text
    // hatespeech sample text (need to escape commas)
    //string path = @"c:\temp\MyTest.txt";
    string path = @"/home/dave/hatespeech/temp/input.csv";

    // Create a file to write to, or overwrite
    await using (var sw = File.CreateText(path))
    {
        sw.WriteLine("Text");
        sw.WriteLine(hsdto.HSText);
    }

    // call the python script here
    // python3 PreBERT.py -m xlm-roberta-base -d all_train -s TE1.csv -fn hate_speech_results
    ProcessStartInfo start = new ProcessStartInfo();
    //start.FileName = "/usr/bin/python3"; // full path to python
    start.FileName = "/bin/bash"; // 
    //start.Arguments = string.Format("{0} {1}", cmd, args);

    // want to run as dave: sudo -u dave
    //start.Arguments = "/home/dave/hatespeech/PreBERT.py -m xlm-roberta-base -d all_train -s TE1.csv -fn hate_speech_results";
    start.Arguments = "sudo python3 /home/dave/hatespeech/PreBERT.py -m xlm-roberta-base -d all_train -s TE1.csv -fn hate_speech_results";
    start.UseShellExecute = false;
    start.RedirectStandardOutput = true;
    using (Process process = Process.Start(start))
    {
        using (StreamReader reader = process.StandardOutput)
        {
            string result = reader.ReadToEnd();
            Console.Write(result);
        }
    }



    //var foo = new Todo() { Id = 9, IsComplete = true, Name = "From HS2" };
    var foo = new HSDto
    {
        Id = rnd, //hsdto.Id,
        HSText = hsdto.HSText,
        Score = hsdto.Score,
        IsHS = hsdto.IsHS
    };

    return Results.Json(foo);
}


app.Run();

class HSDto
{
    public int Id { get; set; }
    public string HSText { get; set; }
    public string? Score { get; set; }
    public bool IsHS { get; set; }
}

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

//app.MapGet("/todoitems", async (TodoDb db) =>
//    await db.Todos.ToListAsync());

//app.MapGet("/todoitems/complete", async (TodoDb db) =>
//    await db.Todos.Where(t => t.IsComplete).ToListAsync());

//app.MapGet("/todoitems/{id}", async (int id, TodoDb db) =>
//    await db.Todos.FindAsync(id)
//        is Todo todo
//        ? Results.Ok(todo)
//        : Results.NotFound());


