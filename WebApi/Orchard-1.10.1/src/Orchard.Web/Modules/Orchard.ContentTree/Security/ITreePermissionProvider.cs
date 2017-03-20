using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.Core.Contents;
using Orchard.Security;

namespace Orchard.ContentTree.Security {
    public interface ITreePermissionProvider : IDependency {
        bool Editable(IContent content);
    }

    public class DefaultTreePermissionProvider : ITreePermissionProvider {
        private readonly IAuthorizer _authorizer;
        public DefaultTreePermissionProvider(IAuthorizer authorizer) {
            _authorizer = authorizer;
        }

        public bool Editable(IContent content) {
            return _authorizer.Authorize(Permissions.EditContent, content);
        }
    }
}