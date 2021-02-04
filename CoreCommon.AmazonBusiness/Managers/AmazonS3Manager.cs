using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using CoreCommon.AmazonBusiness.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace CoreCommon.AmazonBusiness.helpers
{
    public class AmazonS3Manager
    {
        private IAmazonS3 s3Client;

        public AmazonConfig Config { get; private set; }

        public AmazonS3Manager(AmazonConfig config)
        {
            Config = config;
            var region = RegionEndpoint.GetBySystemName(config.Region);

            s3Client = new AmazonS3Client(Config.AccessKey, Config.SecretKey, new AmazonS3Config()
            {
                RegionEndpoint = region
            });
        }

        public async Task<bool> UploadFileAsync(string bucketName, string keyName, Stream stream)
        {
            try
            {
                var fileTransferUtility = new TransferUtility(s3Client);

                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    InputStream = stream,
                    BucketName = bucketName,
                    Key = keyName,
                    CannedACL = S3CannedACL.PublicRead
                };
                //fileTransferUtilityRequest.Metadata.Add("param1", "Value1");

                await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
                // result string.Format("http://{0}.s3.amazonaws.com/{1}", bucketName, keyName);
                return true;
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            return false;
        }
    }
}
