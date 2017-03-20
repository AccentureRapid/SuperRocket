using Orchard;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RaisingStudio.Contents.RepositoryFactory.Services
{
    public interface IContentsRepository<T> : IRepository<T>
    {
    }
}
