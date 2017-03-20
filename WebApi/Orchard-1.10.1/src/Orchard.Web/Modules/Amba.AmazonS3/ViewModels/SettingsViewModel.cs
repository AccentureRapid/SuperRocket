using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amba.AmazonS3.ViewModels
{
    public class SettingsViewModel
    {
        public SettingsViewModel()
        {
            
        }
        public SettingsViewModel(Services.IAmazonS3StorageConfiguration _settingsService)
        {
            this.AWSAccessKey = _settingsService.AWSAccessKey;
            this.AWSFileBucket = _settingsService.AWSFileBucket;
            this.AWSS3PublicUrl = _settingsService.AWSS3PublicUrl;
            this.AWSSecretKey = _settingsService.AWSSecretKey;
            this.AWSExpireMinutes = _settingsService.AWSExpireMinutes;
            RootFolder = _settingsService.RootFolder;
        }
        public string AWSAccessKey { get; set; }
        public string AWSSecretKey { get; set; }
        public string AWSFileBucket { get; set; }
        public string RootFolder { get; set; }
        public string AWSS3PublicUrl { get; set; }
        public int AWSExpireMinutes { get; set; }
    }
}
