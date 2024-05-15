using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using MS.Core.Authorization;
using MS.Core.Core;
using MS.Core.Entities;
using MS.Core.Helpers;
using MS.Core.Quartz;
using MS.Core.Utilities;
using MS.CenterApp.Api;
using MS.CenterApp.Api.Middware;
using Newtonsoft.Json.Serialization;
using Quartz;
using Quartz.AspNetCore;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var _specificOrigins = "MSpecificOrigins";
ConfigurationManager configuration = builder.Configuration;
CommonConst.ServerFileUrl = String.Format("{0}/{1}", configuration["ServerFiles"], configuration["ServerFilePath"]);

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
// Adding Jwt Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,

        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/NotificationHub")))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});



//SqlMapper.AddTypeHandler(new MySqlGuidTypeHandler());
//SqlMapper.AddTypeHandler(new MySqlGuidWithNullTypeHandler());
//SqlMapper.RemoveTypeMap(typeof(Guid));
//SqlMapper.RemoveTypeMap(typeof(Guid?));
// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(_specificOrigins,
                          builder =>
                          {
                              builder
                               .SetIsOriginAllowedToAllowWildcardSubdomains()
                               .WithOrigins("http://localhost:8866",
                                            "http://localhost:8666",
                                            "https://*.taphoa.site",
                                            "https://*.taphoa.site",
                                            "http://*.manhhao.com",
                                            "https://*.manhhao.com")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .AllowCredentials();
                          });
});
builder.Services
    .AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.PropertyNamingPolicy = null;
    });
builder.Services.AddApplication();
// Add services to the container.

builder.Services.AddControllers(options =>
{
    //options.Filters.Add<HttpResponseExceptionFilter>();
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    options.InputFormatters.Add(new BypassFormDataInputFormatter());
})
.AddNewtonsoftJson(o =>
{
    o.SerializerSettings.ContractResolver = new DefaultContractResolver();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

//builder.Services.AddMvc(options =>
//{
//    options.InputFormatters.Add(new BypassFormDataInputFormatter());
//});

// Cấu hình liên quan đến Files:
builder.Services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
builder.Services.Configure<FormOptions>(x =>
{
    x.ValueCountLimit = int.MaxValue;
    x.ValueLengthLimit = 1024 * 1024 * 100;// int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
});

builder.Services.Configure<KestrelServerOptions>(x =>
{
    x.Limits.MaxRequestBufferSize = int.MaxValue;
    x.Limits.MaxRequestBodySize = int.MaxValue;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API for Center Application",
        Description = "Các Api phục vụ cho ứng dụng Center",
        TermsOfService = new Uri("https://manhhao.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Liên hệ",
            Url = new Uri("https://manhhao.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Thông tin giấy phép sử dụng",
            Url = new Uri("https://example.com/license")
        }
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Sử dụng api đăng nhập lấy Token và nhập xuống ô dưới đây. 
                      Sau đó nhấn Authozire.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header,

            },
            new List<string>()
          }
        });
    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

//Quartz
builder.Services.AddQuartz(q =>
{
    // base Quartz scheduler, job and trigger configuration
    //q.UseMicrosoftDependencyInjectionScopedJobFactory();
    // Just use the name of your job that you created in the Jobs folder.
    var jobKeyYesterday = new JobKey("SendEmailJob");
    //var jobKeyToday = new JobKey("SendEmailJobToday");

    var jobKeyBackup = new JobKey("BackupDatabaseJob");

    q.AddJob<SendEmailJob>(opts => opts.WithIdentity(jobKeyYesterday));
    //q.AddJob<SendEmailJobTodayStatistic>(opts => opts.WithIdentity(jobKeyToday));
    q.AddJob<BackUpDatabaseJob>(opts => opts.WithIdentity(jobKeyBackup));

    q.AddTrigger(opts => opts
       .ForJob(jobKeyYesterday)
       .WithIdentity("SendEmailJob2-trigger")
       //This Cron interval can be described as "run every minute" (when second is zero)
       .WithCronSchedule("0 0 7 * * ?")
   );

    //q.AddTrigger(opts => opts
    //    .ForJob(jobKeyToday)
    //    .WithIdentity("SendEmailJob-trigger")
    //    //This Cron interval can be described as "run every minute" (when second is zero)
    //    .WithCronSchedule("0 0 21 * * ?")
    //);

    q.AddTrigger(opts => opts
       .ForJob(jobKeyBackup)
       .WithIdentity("BackupDatabaseJob-trigger")
       //This Cron interval can be described as "run every minute" (when second is zero)
       .WithCronSchedule("0 0 1 * * ?")
   );

});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// ASP.NET Core hosting
builder.Services.AddQuartzServer(options =>
{
    // when shutting down we want jobs to complete gracefully
    options.WaitForJobsToComplete = true;
});

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            c.RoutePrefix = "swagger";
        });
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(
        c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            c.RoutePrefix = "swagger";
        });
}


// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// global cors policy
app.UseCors(_specificOrigins);
app.UseMiddleware<ErrorHandlerMiddleware>();

// custom jwt auth middleware
app.UseMiddleware<JwtMiddleware>();
app.UseRouting();

app.UseAuthorization();
//app.UseFileServer();
//app.UseStaticFiles();
//app.UseDefaultFiles(new DefaultFilesOptions { DefaultFileNames = new List<string> { "index.html" } });

app.UseWebSockets();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHub<NotificationHub>("NotificationHub");
app.MapControllers();

app.Run();
