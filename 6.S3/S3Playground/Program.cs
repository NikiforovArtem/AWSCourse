using Amazon.S3;
using Amazon.S3.Model;

Console.WriteLine();
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