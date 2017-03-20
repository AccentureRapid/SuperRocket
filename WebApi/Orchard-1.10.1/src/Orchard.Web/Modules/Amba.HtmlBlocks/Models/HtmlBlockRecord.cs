using System.ComponentModel.DataAnnotations;
using Orchard.Data.Conventions;

namespace Amba.HtmlBlocks.Models
{
    public class HtmlBlockRecord
    {
        public virtual int Id { get; set; }
        [Required]
        public virtual string BlockKey { get; set; }
        [StringLengthMax]
        public virtual string HTML { get; set; }
        public virtual string HelpText { get; set; }
    }
}