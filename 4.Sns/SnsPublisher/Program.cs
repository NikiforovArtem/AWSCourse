// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using SnsPublisher;

var snsClient = new AmazonSimpleNotificationServiceClient();

var customer = new CustomerCreated
{
    Id = Guid.NewGuid(),
    Email = "nikart28@gmail.com",
    FullName = "Artem Nikiforov",
    DateOfBirth = new DateTime(1995, 8, 3),
    GitHubUsername = "NikiforovArtem"
};

var customerTopic = await snsClient.FindTopicAsync("customers");

var publishRequest = new PublishRequest
{
    TopicArn = customerTopic.TopicArn,
    Message = JsonSerializer.Serialize(customer),
    MessageAttributes = new Dictionary<string, MessageAttributeValue>
    {
        { "MessageType", new MessageAttributeValue { DataType = nameof(String), StringValue = nameof(CustomerCreated) } }
    }
};

var response = await snsClient.PublishAsync(publishRequest);
