using ContentFactory.Services;
using Quartz;
namespace ContentFactory.Jobs;

public class QuartzJobRunner : IJob
{
    private readonly IServiceProvider _serviceProvider;

    public QuartzJobRunner(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var jobType = context.JobDetail.JobType;
            var job = scope.ServiceProvider.GetRequiredService(jobType) as IJob;

            var docService = _serviceProvider.GetRequiredService<DocService>();
            var cft = _serviceProvider.GetRequiredService<CFTelegramService>();

            await job.Execute(context);

            // job completed, save dbContext changes
            //  await _docService.UpdateDocs();

            // db transaction succeeded, send messages
            //await messageBus.DispatchAsync();
        }
    }
}
public class JobSchedule
{
    public JobSchedule(Type jobType, string cronExpression)
    {
        JobType = jobType;
        CronExpression = cronExpression;
    }

    public Type JobType { get; }
    public string CronExpression { get; }
}