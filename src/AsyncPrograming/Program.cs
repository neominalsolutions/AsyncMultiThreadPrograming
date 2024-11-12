using AsyncPrograming.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAsyncService, AsyncServiceSample>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}


// middleware Main Thread Id log iþlemi

app.Use(async (context, next) =>
{
  await Console.Out.WriteLineAsync($"Main Thread {Thread.CurrentThread.ManagedThreadId} \n");

  await next();
});



// exception middleware yapýsý uyguluyoruz.


app.Use(async (context, next) =>
{
  try
  {
    await next(); // request de bir exception olmadýðý durumda response döndürmek için kullanýlacak
  }
  catch (Exception ex) // tüm uygulama genelinde tüm exceptionlarý loglayýp json result verecek.
  {
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    await context.Response.WriteAsJsonAsync(new { message = ex.Message });

  }
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();



