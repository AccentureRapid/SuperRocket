using Orchard;
using Orchard.ContentManagement.Records;

namespace Amba.AmazonS3.Models {
	
    public class AmazonS3SettingsRecord  {
        public virtual int Id { get; set; }
        public virtual string AWSAccessKey{ get; set; }
        public virtual string AWSSecretKey{ get; set; }
        public virtual string AWSFileBucket{ get; set; }
        public virtual string RootFolder{ get; set; }
        public virtual string AWSS3PublicUrl{ get; set; }
        public virtual int AWSExpireMinutes { get; set; }
    }
}