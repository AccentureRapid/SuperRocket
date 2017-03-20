using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;

namespace Orchard.Roles.Models {
    public class UserRolesPart : ContentPart, IUserRoles {

        internal LazyField<IList<string>> _roles = new LazyField<IList<string>>();

        //public IList<string> Roles {
        //    get { return _roles.Value; }
        //}

        public IList<string> Roles
        {
            get { return _roles.Value; }
            set { _roles.Value = value; }
        }

    }
}