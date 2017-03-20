using System.Data;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace RM.QuickLogOn.OAuth
{
    [OrchardFeature("RM.QuickLogOn.OAuth")]
    public class GoogleMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(
                "GoogleSettingsPartRecord",
                table => table.ContentPartRecord()
                              .Column("ClientId", DbType.String, command => command.WithLength(255))
                              .Column("EncryptedClientSecret", DbType.String, command => command.WithLength(512)));
            return 1;
        }
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.Facebook")]
    public class FacebookMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(
                "FacebookSettingsPartRecord",
                table => table.ContentPartRecord()
                              .Column("ClientId", DbType.String, command => command.WithLength(255))
                              .Column("EncryptedClientSecret", DbType.String, command => command.WithLength(512)));
            return 1;
        }
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.LiveID")]
    public class LiveIDMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(
                "LiveIDSettingsPartRecord",
                table => table.ContentPartRecord()
                              .Column("ClientId", DbType.String, command => command.WithLength(255))
                              .Column("EncryptedClientSecret", DbType.String, command => command.WithLength(512)));
            return 1;
        }
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.Twitter")]
    public class TwitterMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(
                "TwitterSettingsPartRecord",
                table => table.ContentPartRecord()
                              .Column("ConsumerKey", DbType.String, command => command.WithLength(512))
                              .Column("AccessToken", DbType.String, command => command.WithLength(512))
                              .Column("EncryptedConsumerSecret", DbType.String, command => command.WithLength(512)));
            return 1;
        }
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.QQ")]
    public class QQMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(
                "QQSettingsPartRecord",
                table => table.ContentPartRecord()
                              .Column("ClientId", DbType.String, command => command.WithLength(512))
                              .Column("EncryptedClientSecret", DbType.String, command => command.WithLength(512))
                              .Column("Additional", DbType.String, command => command.WithLength(2000)));
            return 1;
        }
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.Sina")]
    public class SinaMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(
                "SinaSettingsPartRecord",
                table => table.ContentPartRecord()
                              .Column("ClientId", DbType.String, command => command.WithLength(512))
                              .Column("EncryptedClientSecret", DbType.String, command => command.WithLength(512))
                              .Column("Additional", DbType.String, command => command.WithLength(2000)));
            return 1;
        }
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.Renren")]
    public class RenrenMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(
                "RenrenSettingsPartRecord",
                table => table.ContentPartRecord()
                              .Column("ClientId", DbType.String, command => command.WithLength(512))
                              .Column("EncryptedClientSecret", DbType.String, command => command.WithLength(512))
                              .Column("Additional", DbType.String, command => command.WithLength(2000)));
            return 1;
        }
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.Kaixin")]
    public class KaixinMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(
                "KaixinSettingsPartRecord",
                table => table.ContentPartRecord()
                              .Column("ClientId", DbType.String, command => command.WithLength(512))
                              .Column("EncryptedClientSecret", DbType.String, command => command.WithLength(512))
                              .Column("Additional", DbType.String, command => command.WithLength(2000)));
            return 1;
        }
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.Douban")]
    public class DoubanMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(
                "DoubanSettingsPartRecord",
                table => table.ContentPartRecord()
                              .Column("ClientId", DbType.String, command => command.WithLength(512))
                              .Column("EncryptedClientSecret", DbType.String, command => command.WithLength(512))
                              .Column("Additional", DbType.String, command => command.WithLength(2000)));
            return 1;
        }
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.Baidu")]
    public class BaiduMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(
                "BaiduSettingsPartRecord",
                table => table.ContentPartRecord()
                              .Column("ClientId", DbType.String, command => command.WithLength(512))
                              .Column("EncryptedClientSecret", DbType.String, command => command.WithLength(512))
                              .Column("Additional", DbType.String, command => command.WithLength(2000)));
            return 1;
        }
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.Taobao")]
    public class TaobaoMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(
                "TaobaoSettingsPartRecord",
                table => table.ContentPartRecord()
                              .Column("ClientId", DbType.String, command => command.WithLength(512))
                              .Column("EncryptedClientSecret", DbType.String, command => command.WithLength(512))
                              .Column("Additional", DbType.String, command => command.WithLength(2000)));
            return 1;
        }
    }
}
