using System.Text;
using System.Text.Json.Serialization;
using BankApi.Services;
using BankRDMS.Data;
using BankRDMS.Service.Abstract;
using BankRDMS.Service.Concrete;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAccountService, AccountManager>();
builder.Services.AddScoped<IAccountCardService, AccountCardManager>();
builder.Services.AddScoped<ICreditCardService, CreditCardManager>();
builder.Services.AddScoped<ITransactionService, TransactionManager>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
  
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});


builder.Services.AddScoped<ICustomerService, CustomerManager>();
builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
     .UseSnakeCaseNamingConvention());

// JWT Konfigürasyonu
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = builder.Configuration["Jwt:Issuer"],
          ValidAudience = builder.Configuration["Jwt:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
      };
  });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run(); 