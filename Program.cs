var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("APICorsPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7217", "http://localhost:5215")
        .AllowAnyHeader()
        .AllowAnyMethod();
        
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("APICorsPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();
 