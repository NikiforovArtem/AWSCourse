namespace SnsPublisher;

public class CustomerCreated
{
    public required Guid Id { get; init; }
    
    public required string FullName { get; init; }
    
    public required string Email { get; set; }
    
    public required string GitHubUsername { get; set; }
    
    public required DateTime DateOfBirth { get; set; }
}