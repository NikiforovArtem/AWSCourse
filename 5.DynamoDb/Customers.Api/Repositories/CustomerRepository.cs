using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Customers.Api.Contracts.Data;

namespace Customers.Api.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly string _tableItem = "customers";
    
    public CustomerRepository(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb;
    }
    
    public async Task<bool> CreateAsync(CustomerDto customer)
    {
        customer.UpdatedAt = DateTime.UtcNow;
        var customerAsJson = JsonSerializer.Serialize(customer);
        var customerAsAttribute = Document.FromJson(customerAsJson).ToAttributeMap();

        var createItemRequest = new PutItemRequest
        {
            TableName = _tableItem,
            Item = customerAsAttribute
        };

        var response = await _dynamoDb.PutItemAsync(createItemRequest);

        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<CustomerDto?> GetAsync(Guid id)
    {
        var getItemRequest = new GetItemRequest
        {
            TableName = _tableItem,
            Key = new Dictionary<string, AttributeValue>
            {
                { "sk", new AttributeValue { S = id.ToString() } },
                { "pk", new AttributeValue { S = id.ToString() } }
            }
        };

        var response = await _dynamoDb.GetItemAsync(getItemRequest);

        if (!response.Item.Any())
        {
            return null;
        }

        var customerAsDocument = Document.FromAttributeMap(response.Item);

        return JsonSerializer.Deserialize<CustomerDto>(customerAsDocument.ToJson());
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateAsync(CustomerDto customer)
    {
        customer.UpdatedAt = DateTime.UtcNow;
        var customerAsJson = JsonSerializer.Serialize(customer);
        var customerAsAttribute = Document.FromJson(customerAsJson).ToAttributeMap();

        var putItemRequest = new PutItemRequest
        {
            TableName = _tableItem,
            Item = customerAsAttribute
        };

        var response = await _dynamoDb.PutItemAsync(putItemRequest);

        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var deleteItemRequest = new DeleteItemRequest
        {
            TableName = _tableItem,
            Key = new Dictionary<string, AttributeValue>
            {
                { "sk", new AttributeValue { S = id.ToString() } },
                { "pk", new AttributeValue { S = id.ToString() } }
            }
        };

        var response = await _dynamoDb.DeleteItemAsync(deleteItemRequest);

        return response.HttpStatusCode == HttpStatusCode.OK;
    }
}
