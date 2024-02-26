// See https://aka.ms/new-console-template for more information

using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

Console.WriteLine("Hello, World!");

var secretsManager = new AmazonSecretsManagerClient();

var getSecretRequest = new GetSecretValueRequest
{
    SecretId = "ApiKey"
};

var response = await secretsManager.GetSecretValueAsync(getSecretRequest);

Console.WriteLine(response.SecretString);