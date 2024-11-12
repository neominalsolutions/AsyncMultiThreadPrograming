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


// middleware Main Thread Id log i�lemi

app.Use(async (context, next) =>
{
  await Console.Out.WriteLineAsync($"Main Thread {Thread.CurrentThread.ManagedThreadId} \n");

  await next();
});



// exception middleware yap�s� uyguluyoruz.


app.Use(async (context, next) =>
{
  try
  {
    await next(); // request de bir exception olmad��� durumda response d�nd�rmek i�in kullan�lacak
  }
  catch (Exception ex) // t�m uygulama genelinde t�m exceptionlar� loglay�p json result verecek.
  {
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    await context.Response.WriteAsJsonAsync(new { message = ex.Message });

  }
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();



