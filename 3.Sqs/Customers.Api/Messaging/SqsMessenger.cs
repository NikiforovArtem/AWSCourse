using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;

namespace Customers.Api.Messaging;

public class SqsMessenger : ISqsMessenger
{
    private readonly IAmazonSQS _sqs;
    private readonly IOptions<QueueSettings> _queueSettings;
    private string? _queueUrl;

    public SqsMessenger(IAmazonSQS sqs, IOptions<QueueSettings> queueSettings)
    {
        _sqs = sqs;
        _queueSettings = queueSettings;
    }

    public async Task<SendMessageResponse> SendMessageAsync<T>(T message)
    {
        var queueUrlResponse = await GetQueueUrl();

        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = queueUrlResponse,
            MessageBody = JsonSerializer.Serialize(message),
            MessageAttributes = new Dictionary<string, MessageAttributeValue>()
            {
                {
                    "MessageType", new MessageAttributeValue
                    {
                        DataType = "String",
                        StringValue = typeof(T).Name,
                    }
                }
            }
        };

        var response = await _sqs.SendMessageAsync(sendMessageRequest);

        return response;
    }
    
    private async Task<string> GetQueueUrl()
    {
        //Not a problem for multithreads. If a few threads will here the result of response will the same.
        // And even a few additional requests to AWS wont be a problem.
        
        if (_queueUrl is not null)
        {
            return _queueUrl;
        }

        var queueUrlResponse = await _sqs.GetQueueUrlAsync(_queueSettings.Value.QueueName);

        _queueUrl = queueUrlResponse.QueueUrl;

        return _queueUrl;
    }
}