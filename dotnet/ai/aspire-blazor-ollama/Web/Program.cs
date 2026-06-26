using Microsoft.Extensions.AI;
using Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Aspire service defaults: OpenTelemetry, health checks, service discovery.
builder.AddServiceDefaults();

// Register an IOllamaApiClient bound to the "chat" resource declared in the AppHost, then
// build an IChatClient (and the full Microsoft.Extensions.AI pipeline) on top of it. No URL,
// no port, no hard-coded config: Aspire supplies the connection string via service discovery.
builder.AddOllamaApiClient("chat")
    .AddChatClient()
    .UseFunctionInvocation()
    // EnableSensitiveData logs prompts/responses in traces — handy in dev, avoid in prod.
    .UseOpenTelemetry(configure: t => t.EnableSensitiveData = builder.Environment.IsDevelopment())
    .UseLogging();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
