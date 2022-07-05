using ContentFactory.Data;
using ContentFactory.Jobs;
using ContentFactory.Models;
using ContentFactory.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

//var botConfig = builder.Configuration.GetSection("BotConfiguration").Get<BotConfiguration>();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true).AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
//builder.Services.AddScoped<ApplicationDbContext, ApplicationDbContext>();
builder.Services.Configure<QuartzOptions>(builder.Configuration.GetSection("Quartz"));
builder.Services.Configure<QuartzOptions>(options =>
{
    options.Scheduling.IgnoreDuplicates = true; // default: false
    options.Scheduling.OverWriteExistingData = true; // default: true
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IEmailSender, EmailService>();


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<ApplicationDbContext>();
builder.Services.AddMemoryCache();
var keysFolder = Path.Combine(builder.Environment.ContentRootPath, "Keys");
builder.Services.AddDataProtection()
     .PersistKeysToFileSystem(new DirectoryInfo(keysFolder))
     .SetDefaultKeyLifetime(TimeSpan.FromDays(14));

builder.Services.AddScoped<DocService>();
builder.Services.AddSingleton<CFTelegramService>();
builder.Services.AddHostedService(provider => provider.GetService<CFTelegramService>());
builder.Services.AddControllers();
//builder.Services.AddSingleton<DocService>();
//builder.Services.AddHostedService(provider => provider.GetService<DocService>());


builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Home";
});

builder.Services.AddTransient<JobFactory>();
//builder.Services.AddScoped<NonConconcurrentJob>();
builder.Services.AddTransient<ConconcurrentJob>();
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    var conconcurrentJobKey = new JobKey("ConconcurrentJob");
    q.AddJob<ConconcurrentJob>(opts => opts.WithIdentity(conconcurrentJobKey));
    q.AddTrigger(opts => opts
        .ForJob(conconcurrentJobKey)
        .WithIdentity("ConconcurrentJob-trigger")
        .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(15)
            .RepeatForever()));

    //var nonConconcurrentJobKey = new JobKey("NonConconcurrentJob");
    //q.AddJob<NonConconcurrentJob>(opts => opts.WithIdentity(nonConconcurrentJobKey));
    //q.AddTrigger(opts => opts
    //    .ForJob(nonConconcurrentJobKey)
    //    .WithIdentity("NonConconcurrentJob-trigger")
    //    .WithSimpleSchedule(x => x
    //        .WithIntervalInMinutes(15)
    //        .RepeatForever()));

});


builder.Services.AddQuartzHostedService(
    q => q.WaitForJobsToComplete = true);


var app = builder.Build();
//var serviceProvider = app.Services.CreateScope().ServiceProvider;
//DataScheduler.Start(serviceProvider);

IConfiguration configuration = app.Configuration;
IWebHostEnvironment environment = app.Environment;

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor |
            ForwardedHeaders.XForwardedProto
});


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<User>>();
        var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await RoleInitializer.InitializeAsync(userManager, rolesManager);

        var context = services.GetRequiredService<ApplicationDbContext>();
        await VideoInitializer.InitializeAsync(context);
        await TxtInitializer.InitializeAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();

    //app.UseSwaggerUI(c =>
    //{
    //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CF.Bot API V1");
    //});
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
//TBot.GetBotClientAsync().Wait();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
//app.MapControllers();
//app.UseEndpoints(endpoints =>
//{
//    // Configure custom endpoint per Telegram API recommendations:
//    // https://core.telegram.org/bots/api#setwebhook
//    // If you'd like to make sure that the Webhook request comes from Telegram, we recommend
//    // using a secret path in the URL, e.g. https://www.example.com/<token>.
//    // Since nobody else knows your bot's token, you can be pretty sure it's us.
//  //  var token = botConfig.BotToken;
//    endpoints.MapControllerRoute(name: "tgwebhook",
//                                 pattern: $"bot/5442355625:AAHqbSgD3OEi8g_qOkuT82zABx8BerRHRK4",
//                                 new { controller = "CFBot", action = "Post" });
//    endpoints.MapControllers();
//});
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});



app.Run();
