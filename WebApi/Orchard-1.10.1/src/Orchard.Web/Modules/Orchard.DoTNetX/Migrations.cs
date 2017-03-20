using Orchard.Data.Migration;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;

namespace Orchard.DoTNetX
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
           #region BlogPost
           ContentDefinitionManager.AlterTypeDefinition("BlogPost",
                cfg => cfg
                  .WithPart("BlogPost")
                  .WithPart("CommonPart" )
           );

           #endregion
            
          return 1;
        }
    }
}