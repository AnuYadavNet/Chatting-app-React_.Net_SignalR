using ChattingApp.API.Hubs;
using ChattingApp.Application.BackgroundServices;
using ChattingApp.Application.Interfaces;
using ChattingApp.Application.Services;
using ChattingApp.Infrastructure.Data;
using ChattingApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// SERVICE REGISTRATION (Dependency Injection Container)
// ============================================================================

// --- Controllers ---
builder.Services.AddControllers();

// --- Swagger / OpenAPI ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title   = "ChattingApp API",
        Version = "v1",
        Description = "Real-time chat API powered by SignalR + SQL Server"
    });
});

// --- CORS: Allow React dev server (http://localhost:3000) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactDevPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",   // React dev server
                "http://localhost:5173")   // Vite dev server (if used)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Required for SignalR WebSocket negotiation
    });
});

// --- SignalR ---
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.MaximumReceiveMessageSize = 32 * 1024; // 32 KB per message
    options.ClientTimeoutInterval     = TimeSpan.FromSeconds(60);
    options.KeepAliveInterval         = TimeSpan.FromSeconds(15);
});

// --- Infrastructure Layer ---
builder.Services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();

// --- Application Layer ---
builder.Services.AddScoped<IChatService, ChatService>();

// --- Background Service: Message Cleanup (runs every hour) ---
builder.Services.AddHostedService<MessageCleanupService>();

// ============================================================================
// BUILD THE APPLICATION
// ============================================================================
var app = builder.Build();

// --- Development-only middleware ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChattingApp API v1");
        c.RoutePrefix = "swagger";
    });
}

// --- HTTPS redirection ---
app.UseHttpsRedirection();

// --- CORS must come BEFORE UseRouting and MapHub ---
app.UseCors("ReactDevPolicy");

// --- Auth middleware (placeholder — extend for JWT in production) ---
app.UseAuthentication();
app.UseAuthorization();

// --- Map REST Controllers ---
app.MapControllers();

// --- Map SignalR Hub ---
// The client connects to: https://localhost:PORT/hubs/chat
app.MapHub<ChatHub>("/hubs/chat");

app.Run();
