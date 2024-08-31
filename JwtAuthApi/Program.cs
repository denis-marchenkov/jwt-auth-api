using JwtAuthApi;
using JwtAuthApi.Db;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

bool disableHttps = false;
#if DEBUG
disableHttps = true;
#endif

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(x => x.UseInMemoryDatabase("Users"));
builder.Services.AddSingleton<JwtTokenProvider>();

builder.Services.AddEndpointsApiExplorer();


# region swagger auth

builder.Services.AddSwaggerGen(x =>
{
    x.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "JWT Auth",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT"
    });

    x.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            },
            []
        }
    });
});

#endregion


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = disableHttps;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwt:secret"])),
            ValidIssuer = builder.Configuration["jwt:iss"],
            ValidAudience = builder.Configuration["jwt:aud"]
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

ApiEndpoints.Map(app);

app.UseAuthentication();
app.UseAuthorization();

app.Run();
