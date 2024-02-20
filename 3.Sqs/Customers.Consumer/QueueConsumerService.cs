using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Customers.Consumer.Messages;
using MediatR;
using Microsoft.Extensions.Options;

namespace Customers.Consumer;

public class QueueConsumerService : BackgroundService
{
    private readonly IAmazonSQS _sqs;
    private readonly IOptions<QueueSettings> _settings;
    private readonly IMediator _mediator;
    private readonly ILogger<QueueConsumerService> _logger;
    
    private string? _queueUrl;

    public QueueConsumerService(IAmazonSQS sqs, IOptions<QueueSettings> settings, IMediator mediator, ILogger<QueueConsumerService> logger)
    {
        _sqs = sqs;
        _settings = settings;
        _mediator = mediator;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueUrlResponse = await GetQueueUrl();

        var receiveMessageRequest = new ReceiveMessageRequest(queueUrlResponse)
        {
            AttributeNames = new List<string> { "All" },
            MessageAttributeNames = new List<string> { "All" }
        };
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _sqs.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);
            
            foreach (var message in response.Messages)
            {
                var messageType = message.MessageAttributes["MessageType"].StringValue;
                var type = Type.GetType($"Customers.Consumer.Messages.{messageType}");

                if (type is null)
                {
                    _logger.LogWarning("Unknown message type: {MessageType}", messageType);
                    continue;
                }

                var typedMessage = (ISqsMessage)JsonSerializer.Deserialize(message.Body, type)!;
                
                try
                {
                    await _mediator.Send(typedMessage, stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Message doesnt handled");
                    continue;
                }

                await _sqs.DeleteMessageAsync(new DeleteMessageRequest(queueUrlResponse, message.ReceiptHandle), stoppingToken);
            }
        }
    }
    
    private async Task<string> GetQueueUrl()
    {
        //Not a problem for multithreads. If a few threads will here the result of response will the same.
        // And even a few additional requests to AWS wont be a problem.
        
        if (_queueUrl is not null)
        {
            return _queueUrl;
        }

        var queueUrlResponse = await _sqs.GetQueueUrlAsync(_settings.Value.QueueName);

        _queueUrl = queueUrlResponse.QueueUrl;

        return _queueUrl;
    }
}