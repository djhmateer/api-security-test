using System.Diagnostics;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// remove default logging providers
builder.Logging.ClearProviders();

var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/information.log")
    .CreateLogger();

builder.Logging.AddSerilog(logger);

var app = builder.Build();

logger.Information("****Starting API");

app.MapGet("/textget", () =>
    // returns a 200
    "hello from textget2"
);

app.MapGet("/jsonget", () =>
   // this will serialise the object and returns json by default as is of type T
   // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-6.0#responses
   new { Message = "hello world from jsonget" }
);

app.MapPost("/hs", Handler3);
async Task<IResult> Handler3(HSDto hsdtoIn)
{
    // csv helper to write inbound hsdto to a csv
    var recordsToWrite = new List<HSDto>();
    recordsToWrite.Add(hsdtoIn);


    var guid = Guid.NewGuid();

    //using (var writer = new StreamWriter("/home/dave/hatespeech/temp/input.csv"))
    //using (var writer = new StreamWriter("/home/dave/hatespeech/input/002.csv"))
    using (var writer = new StreamWriter($"/home/dave/hatespeech/input/{guid}.csv"))
    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
    {
        csv.WriteRecords(recordsToWrite);
    }

    // call the python script
    // python3 PreBERT.py -m xlm-roberta-base -d all_train -s TE1.csv -fn hate_speech_results
    //var start = new ProcessStartInfo();
    //start.FileName = "bash";
    //start.WorkingDirectory = "/home/dave/hatespeech";

    //var command = "python3 PreBERT.py -m xlm-roberta-base -d all_train -s temp/input.csv -fn temp/output";

    //// process running as www-data, but we want to run Python script as dave
    //start.Arguments = $"-c \"sudo -u dave {command}\"";

    //start.UseShellExecute = false;
    //start.RedirectStandardOutput = true;
    //start.RedirectStandardError = true;

    //logger.Information(" Starting Python");
    //using (Process process = Process.Start(start))
    //{
    //    //using (StreamReader reader = process.StandardOutput)
    //    using (StreamReader reader = process.StandardError)
    //    {
    //        string result = reader.ReadToEnd();
    //        logger.Information($" inside: {result}");
    //    }
    //}
    //logger.Information(" Ending Python");


    // **HERE**
    // poll the output directory

    var hsdto = new HSDto { };

    //// read temp/output.csv
    //using (var reader = new StreamReader("/home/dave/hatespeech/temp/output.csv"))
    //using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
    //{
    //    var records = csv.GetRecords<PythonDTO>();
    //    foreach (var record in records)
    //    {
    //        logger.Information($"record Text: {record.Text} ");
    //        logger.Information($"record Predi: {record.Prediction} ");
    //        logger.Information($"record HS: {record.HateScore} ");

    //        hsdto.Text = record.Text;
    //        hsdto.Score = record.HateScore;
    //        hsdto.Prediction = record.Prediction;
    //    }
    //}

    return Results.Json(hsdto);
}


app.Run();

class PythonDTO
{
    public string Text { get; set; }
    public string Prediction { get; set; }
    [Name("Hate score")]
    public string HateScore { get; set; }
}

class HSDto
{
    public string Text { get; set; }
    public string? Score { get; set; }
    public string? Prediction { get; set; }
}

//class Todo
//{
//    public int Id { get; set; }
//    public string? Name { get; set; }
//    public bool IsComplete { get; set; }
//}

//class TodoDb : DbContext
//{
//    public TodoDb(DbContextOptions<TodoDb> options)
//        : base(options) { }

//    public DbSet<Todo> Todos => Set<Todo>();
//}









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


