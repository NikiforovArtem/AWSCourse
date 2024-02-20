using Customers.Api.Domain;

namespace Customers.Api.Mapping;

public static class DomainToMessageMapper
{
    public static CustomerCreated ToCustomerCreatedMessage(this Customer customer)
    {
        return new CustomerCreated
        {
            Email = customer.Email,
            Id = customer.Id,
            GitHubUsername = customer.GitHubUsername,
            FullName = customer.FullName,
            DateOfBirth = customer.DateOfBirth
        };
    }   
    
    public static CustomerUpdated ToCustomerUpdatedMessage(this Customer customer)
    {
        return new CustomerUpdated
        {
            Email = customer.Email,
            Id = customer.Id,
            GitHubUsername = customer.GitHubUsername,
            FullName = customer.FullName,
            DateOfBirth = customer.DateOfBirth
        };
    }
}