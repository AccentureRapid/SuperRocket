using Orchard.Alias.Records;

namespace dcp.Routing.Models
{
    public class ExtendedAliasRecord
    {
        public virtual int Id { get; set; }
        public virtual string RouteName { get; set; }
        public virtual AliasRecord AliasRecord { get; set; }
    }
}