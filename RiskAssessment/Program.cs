using Refit;
using RiskAssessment.Domain;
using RiskAssessment.Services;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = InvoiceOption.Scheme;
})
    .AddCookie("Cookies")
    .AddOpenIdConnect(InvoiceOption.Scheme, options =>
    {
        options.Authority = InvoiceOption.AuthorizationUrl;

        options.ClientId = InvoiceOption.ClientId;
        options.ResponseType = "code";

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("invoice");
        //options.GetClaimsFromUserInfoEndpoint = true;

        options.SaveTokens = true;
    });

builder.Services.AddScoped<IFinancialAssessmentRepository, FinancialAssessmentRepository>();

builder.Services
    .AddRefitClient<IFinancialAssessmentService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(InvoiceOption.ApiUrl));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
