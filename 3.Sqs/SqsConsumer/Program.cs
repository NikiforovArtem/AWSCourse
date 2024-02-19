// See https://aka.ms/new-console-template for more information

using Amazon.SQS;
using Amazon.SQS.Model;

var cts = new CancellationTokenSource();

Console.WriteLine("Hello, World!");

var sqsClient = new AmazonSQSClient();

var queueUrlResponse = await sqsClient.GetQueueUrlAsync("customers");

var receiveMessageRequest = new ReceiveMessageRequest(queueUrlResponse.QueueUrl)
{
    AttributeNames = new List<string> { "All" },
    MessageAttributeNames = new List<string> { "All" }
};

while (!cts.IsCancellationRequested)
{
    var response = await sqsClient.ReceiveMessageAsync(receiveMessageRequest, cts.Token);

    foreach (var message in response.Messages)
    {
        try
        {
            Console.WriteLine($"MessageId={message.MessageId}");
            Console.WriteLine($"MessageBody={message.Body}");
        }
        catch (Exception e)
        {
            Console.WriteLine("Message doesnt handled");
            throw;
        }

        await sqsClient.DeleteMessageAsync(queueUrlResponse.QueueUrl, message.ReceiptHandle);

    }
    
    await Task.Delay(1000);
}