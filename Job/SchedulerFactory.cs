using ContentFactory.Models;
using ContentFactory.Services;
using Quartz;
using Quartz.Spi;
using Quartz.Impl;
using System.Runtime;
//using Microsoft.NET.Sdk.Worker;
namespace ContentFactory.Jobs;

[DisallowConcurrentExecution]
public class ConconcurrentJob : IJob
{
    //private readonly IServiceProvider _serviceProvider;
    private readonly DocService _docService;
    private readonly CFTelegramService _cft;
    public ConconcurrentJob(DocService docService, CFTelegramService cft)
    {
        _docService = docService;
        _cft = cft;
    }
    public async Task Execute(IJobExecutionContext context)
    {
        //GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        //GC.Collect();
        //using (var scope = _serviceProvider.CreateScope())
        ////{
        //    var _docService = scope.ServiceProvider.GetService<DocService>();
        //    var _cft = scope.ServiceProvider.GetService<CFTelegramService>();


            List<Order> orders = await _docService.GetSendingDocs();
            foreach (var item in orders)
            {
                List<string> files = new List<string>();
                var path = Path.Combine(item.FilePath, $"{item.FileName}_2.pdf");

                if (System.IO.File.Exists(path))
                {
                    files.Add(path);
                }
                path = Path.Combine(item.FilePath, $"{item.FileName}_2.xlsx");
                if (System.IO.File.Exists(path))
                {
                    files.Add(path);
                }
                User _user = await _docService.GetUser(item.Id);
                string vCard = await _docService.GetVcard(_user);
                files.Add(vCard);
                await _cft.SendDocAsync(files);
                await _docService.UpdateOrderAsync(item);

                if (_user.HashLink == null && _user.TelegramId.Length > 3)
                {
                    Models.User user = await _cft.SendConfirmAsync(_user, "Вы сделали заказ на сайте ContentFactory.store");
                    if (user.IsTelegramConfirmed == true)
                    {
                        await _docService.UpdateUser(user);

                    }
                }

            }
            await _docService.UpdateDoc();

        //}
    }
}
//[DisallowConcurrentExecution]
//public class NonConconcurrentJob : IJob
//{
//    private readonly IServiceProvider _serviceProvider;
//    //private readonly DocService _docService;

//    public NonConconcurrentJob(IServiceProvider serviceProvider)
//    {

//        this._serviceProvider = serviceProvider;

//    }
//    public async Task Execute(IJobExecutionContext context)
//    {
//        using (var scope = _serviceProvider.CreateScope())
//        {
//            var _docService = scope.ServiceProvider.GetService<DocService>();
//            await _docService.UpdateDocs();

//        }

//    }
//}
public class JobFactory : IJobFactory
{
    protected readonly IServiceScopeFactory serviceScopeFactory;
    public JobFactory(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        using (var scope = serviceScopeFactory.CreateScope())
        {
            var job = scope.ServiceProvider.GetService(bundle.JobDetail.JobType) as IJob;
            return job;
        }
    }

    public void ReturnJob(IJob job)
    {
        //Do something if need
    }
}
public static class DataScheduler
{

    public static async void Start(IServiceProvider serviceProvider)
    {
        IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        scheduler.JobFactory = serviceProvider.GetService<JobFactory>();
        await scheduler.Start();

        IJobDetail jobDetail = JobBuilder.Create<ConconcurrentJob>().Build();
        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("ConconcurrentJob-trigger", "default")
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(2)
            .RepeatForever())
            .Build();

        await scheduler.ScheduleJob(jobDetail, trigger);
    }
}