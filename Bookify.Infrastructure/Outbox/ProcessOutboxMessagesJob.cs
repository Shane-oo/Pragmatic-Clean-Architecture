using Bookify.Application.Abstractions.Clock;
using Bookify.Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;

namespace Bookify.Infrastructure.Outbox;

[DisallowConcurrentExecution]
internal sealed class ProcessOutboxMessagesJob: IJob
{
    #region Fields

    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ApplicationDbContext _dbContext;

    private static readonly JsonSerializerSettings _jsonSerializerSettings = new()
                                                                             {
                                                                                 TypeNameHandling = TypeNameHandling.All
                                                                             };

    private readonly ILogger<ProcessOutboxMessagesJob> _logger;
    private readonly OutboxOptions _outboxOptions;
    private readonly IPublisher _publisher;

    #endregion

    #region Construction

    public ProcessOutboxMessagesJob(ApplicationDbContext dbContext,
                                    IPublisher publisher,
                                    IDateTimeProvider dateTimeProvider,
                                    IOptions<OutboxOptions> outboxOptions,
                                    ILogger<ProcessOutboxMessagesJob> logger)
    {
        _dbContext = dbContext;
        _publisher = publisher;
        _dateTimeProvider = dateTimeProvider;
        _outboxOptions = outboxOptions.Value;
        _logger = logger;
    }

    #endregion

    #region Private Methods

    private async Task<IReadOnlyList<OutboxMessage>> GetOutboxMessagesAsync(ApplicationDbContext dbContext, IDbContextTransaction transaction)
    {
        // would prefer to use a query 
        var outboxMessages = await dbContext.OutboxMessages.Where(o => o.ProcessedOnUtc == null)
                                            .OrderBy(o => o.OccuredOnUtc)
                                            .Take(_outboxOptions.BatchSize)
                                            .ToListAsync();

        return outboxMessages;
    }

    private void UpdateOutboxMessage(OutboxMessage outboxMessage, Exception? exception)
    {
        outboxMessage.ProcessedOnUtc = _dateTimeProvider.UtcNow;
        outboxMessage.Error = exception?.ToString();
    }

    #endregion

    #region Public Methods

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Beginning to process outbox messages");

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(context.CancellationToken);

        var outboxMessages = await GetOutboxMessagesAsync(_dbContext, transaction);

        foreach(var outboxMessage in outboxMessages)
        {
            Exception? exception = null;

            try
            {
                var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(outboxMessage.Content, _jsonSerializerSettings);
                if (domainEvent != null)
                {
                    await _publisher.Publish(domainEvent, context.CancellationToken);
                }
            }
            catch(Exception caughtException)
            {
                _logger.LogError(caughtException, "Exception while processing outbox message {MessageId}", outboxMessage.Id);
                exception = caughtException;
            }

            UpdateOutboxMessage(outboxMessage, exception);
        }

        await transaction.CommitAsync();

        _logger.LogInformation("Completed processing outbox messages");
    }

    #endregion
}
