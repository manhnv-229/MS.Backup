using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using MS.ApplicationCore.Authorization;
using MS.ApplicationCore.Core;
using MS.ApplicationCore.Entities;
using MS.ApplicationCore.Helpers;
using MS.ApplicationCore.Quartz;
using MS.ApplicationCore.Utilities;
using MS.ThaiSalonV2.Api;
using MS.ThaiSalonV2.Api.Middware;
using Quartz;
using System.Text;

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
                              builder.WithOrigins("http://localhost:8006",
                                                   "http://localhost:8007",
                                                  "http://nmanh.com",
                                                  "http://wwww.nmanh.com",
                                                  "https://nmanh.com",
                                                  "https://wwww.nmanh.com",
                                                  "http://nmanh.com",
                                                  "https://wwww.nmanh.com",
                                                  "https://api.nmanh.com",
                                                  "http://shop.nmanh.com",
                                                  "https://shop.nmanh.com",
                                                  "https://apishop.nmanh.com",
                                                  "https://shop.manhhao.com",
                                                  "http://shop.manhhao.com")
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
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
}); ;

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
builder.Services.AddSwaggerGen();

//Quartz
builder.Services.AddQuartz(q =>
{
    // base Quartz scheduler, job and trigger configuration
    q.UseMicrosoftDependencyInjectionScopedJobFactory();
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
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

app.MapHub<NotificationHub> ("NotificationHub");
app.MapControllers();

app.Run();
