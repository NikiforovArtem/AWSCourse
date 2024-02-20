namespace Customers.Consumer;

public class QueueSettings
{
    public const string SettingsKey = "QueueSettings";
    
    public required string QueueName { get; init; }
}