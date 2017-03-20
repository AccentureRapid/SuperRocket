using Orchard.DoTNetX.Models;
using Orchard.DoTNetX.ServiceModels;
using Orchard.DoTNetX.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.DoTNetX.Interfaces
{
    public interface IBlogPostService : IDependency
    {
        IEnumerable<BlogPostModel> GetBlogPostList(BlogPostSearch search = null, BlogPostSort sort = null, bool allowCache = true);

        BlogPostModel GetBlogPost(int id, bool allowCache = true);

        int Delete(int id);

        int Create(BlogPostModel model);

        int Update(BlogPostModel model);

        void TriggerSignal();
    }
}
