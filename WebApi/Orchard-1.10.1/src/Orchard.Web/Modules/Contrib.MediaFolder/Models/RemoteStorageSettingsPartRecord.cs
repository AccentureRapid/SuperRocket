using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;

namespace Contrib.MediaFolder.Models {
    public class RemoteStorageSettingsPartRecord : ContentPartRecord {
        [StringLengthMax, Required]
        public virtual string MediaLocation { get; set; }
        public virtual bool EnableDirectRoute { get; set; }
        [StringLengthMax]
        public virtual string DirectRoute { get; set; }
    }
}