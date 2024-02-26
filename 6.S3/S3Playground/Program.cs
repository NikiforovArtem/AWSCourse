using System.Text;
using Amazon.S3;
using Amazon.S3.Model;

Console.WriteLine();

//await PutObjectToS3();
await GetObjectFromS3();
return;

async Task PutObjectToS3()
{
    var s3Client = new AmazonS3Client();

    using var inputStream = new FileStream("./movies.csv", FileMode.Open, FileAccess.Read);

    var putObjectRequest = new PutObjectRequest
    {
        BucketName = "awscourseartemnik",
        Key = "csv/movies.csv",
        ContentType = "text/csv",
        InputStream = inputStream
    };

    await s3Client.PutObjectAsync(putObjectRequest);
}

async Task GetObjectFromS3()
{
    var s3Client = new AmazonS3Client();
    
    var getObjectRequest = new GetObjectRequest
    {
        BucketName = "awscourseartemnik",
        Key = "csv/movies.csv",
    };

    var response = await s3Client.GetObjectAsync(getObjectRequest);

    using var memoryStream = new MemoryStream();

    response.ResponseStream.CopyTo(memoryStream);

    var text = Encoding.Default.GetString(memoryStream.ToArray());
    
    Console.WriteLine(text);
}

