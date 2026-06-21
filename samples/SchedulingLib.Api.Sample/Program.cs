using SchedulingLib.Persistence.PostgreSQL;
using SchedulingLib.Services.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddServiceScheduling()
    .AddPostgreSqlPersistence(builder.Configuration.GetConnectionString("Default")!);

var app = builder.Build();

app.Run();
