// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using SqsPublisher;

Console.WriteLine("Hello, World!");

var sqsClient = new AmazonSQSClient();

var customer = new CustomerCreated
{
    Id = Guid.NewGuid(),
    Email = "nikart28@gmail.com",
    FullName = "Artem Nikiforov",
    DateOfBirth = new DateTime(1995, 8, 3),
    GitHubUsername = "NikiforovArtem"
};

var queueUrlResponse = await sqsClient.GetQueueUrlAsync("customers");

var sendMessageRequset = new SendMessageRequest
{
    QueueUrl = queueUrlResponse.QueueUrl,
    MessageBody = JsonSerializer.Serialize(customer),
    MessageAttributes = new Dictionary<string, MessageAttributeValue>
    {
        { "MessageType", new MessageAttributeValue { DataType = nameof(String), StringValue = nameof(CustomerCreated) } }
    }
};

var response = await sqsClient.SendMessageAsync(sendMessageRequset);

Console.WriteLine();
