using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RaisingStudio.Contents.RepositoryFactory.Services
{
    public interface IContentsRepositoryFactory : IDependency
    {
        IContentsRepository<T> GetRepository<T>(string contentTypeName = null) where T : class, new();
    }
}