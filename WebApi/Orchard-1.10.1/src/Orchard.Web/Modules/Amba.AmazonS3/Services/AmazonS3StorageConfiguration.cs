using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Amba.AmazonS3.Models;
using Orchard;
using Orchard.Data;

namespace Amba.AmazonS3.Services
{
    public interface IAmazonS3StorageConfiguration : IDependency
    {
        string AWSAccessKey { get; set; }
        string AWSSecretKey { get; set; }
        string AWSFileBucket { get; set; }
        string AWSS3PublicUrl { get; set; }
        string RootFolder { get; set; }
        int AWSExpireMinutes { get; set; }

        void Save();
    }

    public class AmazonS3StorageConfiguration : IAmazonS3StorageConfiguration
    {
        private readonly IRepository<AmazonS3SettingsRecord> _amazonS3SettingsRepository;
        private AmazonS3SettingsRecord _record = null;

        public AmazonS3StorageConfiguration(IRepository<AmazonS3SettingsRecord> amazonS3SettingsRepository)
        {
            _amazonS3SettingsRepository = amazonS3SettingsRepository;
        }

        private AmazonS3SettingsRecord GetSettingsRecord()
        {
            var record = _amazonS3SettingsRepository.Table.FirstOrDefault();
            if (record == null)
            {
                record = new AmazonS3SettingsRecord()
                {
                    AWSS3PublicUrl = "https://s3.amazonaws.com/",
                    AWSAccessKey = "***",
                    AWSSecretKey = "***",
                    AWSFileBucket = "***",
                    AWSExpireMinutes = 300
                };
                _amazonS3SettingsRepository.Create(record);
            }
            return record;
        }


        private AmazonS3SettingsRecord Record
        {
            get
            {
                if (_record == null)
                {
                    _record = GetSettingsRecord();
                }
                return _record;
            }
        }

        public string RootFolder
        {
            get { return _record.RootFolder; }
            set { _record.RootFolder = value; }
        }

        public void Save()
        {
            _amazonS3SettingsRepository.Update(_record);
        }

        public string AWSAccessKey
        {
            get
            {
                return Record.AWSAccessKey;
            }
            set { Record.AWSAccessKey = value; }
        }

        public string AWSSecretKey
        {
            get
            {
                return Record.AWSSecretKey;
            }
            set { Record.AWSSecretKey = value; }
        }

        public string AWSFileBucket
        {
            get
            {
                return Record.AWSFileBucket;
            }
            set { Record.AWSFileBucket = value; }
        }

        public string AWSS3PublicUrl
        {
            get
            {
                return Record.AWSS3PublicUrl;
            }
            set { Record.AWSS3PublicUrl = value; }            
        }

        public int AWSExpireMinutes
        {
            get
            {
                return Record.AWSExpireMinutes;
            }
            set { Record.AWSExpireMinutes = value; }
        }

    }
}