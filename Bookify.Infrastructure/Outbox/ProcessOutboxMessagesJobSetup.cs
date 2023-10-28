using Microsoft.Extensions.Options;
using Quartz;

namespace Bookify.Infrastructure.Outbox;

public class ProcessOutboxMessagesJobSetup: IConfigureOptions<QuartzOptions>
{
    private readonly OutboxOptions _outboxOptions;

    public ProcessOutboxMessagesJobSetup(IOptions<OutboxOptions> outboxOptions)
    {
        _outboxOptions = outboxOptions.Value;
    }

    public void Configure(QuartzOptions options)
    {
        const string JOB_NAME = nameof(ProcessOutboxMessagesJob);

        options
            .AddJob<ProcessOutboxMessagesJob>(c => c.WithIdentity(JOB_NAME))
            .AddTrigger(c => c.ForJob(JOB_NAME)
                              .WithSimpleSchedule(s =>
                                                      s.WithIntervalInSeconds(_outboxOptions.IntervalInSeconds).RepeatForever())
                              );
    }
}
