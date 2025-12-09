using GenerativeAI;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Telegram.Bot;
using Microsoft.Extensions.DependencyInjection;
using ChatBotAssistenteJuridico.Domain.Interfaces;
using ChatBotAssistenteJuridico.Infrastructure.Data;
using ChatBotAssistenteJuridico.Infrastructure.Interface;
using ChatBotAssistenteJuridico.Domain.Application;
using ChatBotAssistenteJuridico.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddScoped<ISendRequestApplication, SendRequestApplication>();
builder.Services.AddScoped<ISendRequestRepository, SendRequestRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IHistoryRepository, HistoryRepository>();

var telegraToken = builder.Configuration["TelegramBot:Token"];
var geminiToken = builder.Configuration["GeminiKey:Token"];

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(telegraToken))
{
    throw new InvalidOperationException("Token do Telegram não encontrado! Verifique a chave 'TelegramBot:Token'.");
}
if (string.IsNullOrEmpty(geminiToken))
{
    throw new InvalidOperationException("Token do Gemini não encontrado! Verifique a chave 'GeminiKey:Token'.");
}


builder.Services.AddHttpClient("Telegram_bot_client").AddTypedClient<ITelegramBotClient>((HttpClient, sp) =>
{
    TelegramBotClientOptions options = new(telegraToken!);
    return new TelegramBotClient(options, HttpClient);
}); 

builder.Services.AddDbContext<RequestContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddSingleton(new GenerativeModel(geminiToken!, model: "gemini-2.5-flash"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen()
;
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatBot API V1");
        c.RoutePrefix = string.Empty;
    });
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
