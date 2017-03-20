using Orchard.DoTNetX.Common;
using Orchard.DoTNetX.Interfaces;
using Orchard.DoTNetX.Models;
using Orchard.DoTNetX.ServiceModels;
using Orchard.DoTNetX.ViewModels;
using Orchard;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using Orchard.Data;
using RaisingStudio.Contents.RepositoryFactory.Services;

namespace Orchard.DoTNetX.Services
{
    public class BlogPostService : IBlogPostService
    {
        private const string SignalName = CacheAndSignals.BlogPostSignal;
        private const string CacheName = CacheAndSignals.BlogPostCache;

        public const string BlogPostContentTypeName = "BlogPost";

        private readonly IContentManager _contentManager;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        private readonly IOrchardServices _orchardServices;

        public BlogPostService(
            IContentManager contentManager,
            ICacheManager cacheManager,
            ISignals signals,
            IOrchardServices orchardServices,
            IContentsRepositoryFactory repositoryFactory)
        {
            _contentManager = contentManager;
            _orchardServices = orchardServices;
            _cacheManager = cacheManager;
            _signals = signals;

            SetupRepository(repositoryFactory.GetRepository<BlogPostPartRecord>(BlogPostContentTypeName));

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        private IRepository<BlogPostPartRecord> _BlogPostRepository;
        public void SetupRepository(
            IRepository<BlogPostPartRecord> BlogPostRepository)
        {
            _BlogPostRepository = BlogPostRepository;
        }


        private void MonitorSignal(AcquireContext<string> ctx)
        {
            ctx.Monitor(_signals.When(SignalName));
        }

        public void TriggerSignal()
        {
            _signals.Trigger(SignalName);
        }


        private BlogPostModel Convert(BlogPostPartRecord record)
        {
            if (record != null)
            {
                return new BlogPostModel
                {
                    Id = record.Id,
                };
            }
            return null;
        }

        private BlogPostPartRecord Convert(BlogPostModel model, BlogPostPartRecord record)
        {
            if (model != null && record != null)
            {
              return record;
            }
            return null;
        }


        private IEnumerable<BlogPostModel> InternalGetBlogPostList()
        {
            return from r in _BlogPostRepository.Table
                    select Convert(r);
        }

        private IEnumerable<BlogPostModel> InternalGetBlogPostList(bool allowCache)
        {
            try
            {
                if (!allowCache)
                {
                    return InternalGetBlogPostList();
                }
                else
                {
                    return _cacheManager.Get(CacheName, ctx =>
                    {
                        MonitorSignal(ctx);
                        return InternalGetBlogPostList().ToList();
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Get BlogPost List failed.");
            }
            return null;
        }

        public IEnumerable<BlogPostModel> GetBlogPostList(BlogPostSearch search = null, BlogPostSort sort = null, bool allowCache = true)
        {

            try
            {
                var q = InternalGetBlogPostList(allowCache);

                #region Search
                if (search != null)
                {
                    q = q.Where
                        (
                            p =>
                                (search.Id == null || p.Id == search.Id)
                        );
                }
                #endregion
                #region Sort
                if (sort != null)
                {
                    if (sort.Id != null)
                    {
                        q = (sort.Id == OrderByDirection.Ascending) ? q.OrderBy(t => t.Id) : q.OrderByDescending(t => t.Id);
                    }
                }
                #endregion
                return q;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Get BlogPost List failed.");
            }
            return null;
        }

        public BlogPostModel GetBlogPost(int id, bool allowCache = true)
        {
            var q = InternalGetBlogPostList(allowCache);
            return q.SingleOrDefault(p => p.Id == id);
        }


        public int Create(BlogPostModel entity)
        {
            try
            {
                if (entity != null)
                {
                    var record = new BlogPostPartRecord();
                    _BlogPostRepository.Create(Convert(entity, record));
                    int id = record.Id;
                    if (id > 0)
                    {
                        TriggerSignal();
                    }
                    return id;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Create BlogPost failed, entity: {0}", JObject.FromObject(entity));
            }
            return -1;
        }

        public int Update(BlogPostModel entity)
        {
            try
            {
                if (entity != null)
                {
                    int id = entity.Id;
                    var record = _BlogPostRepository.Get(id);
                    _BlogPostRepository.Update(Convert(entity, record));
                    TriggerSignal();
                    return id;
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Update BlogPost failed, entity: {0}", JObject.FromObject(entity));
            }
            return -1;
        }

        public int Delete(int id)
        {
            try
            {
                var record = _BlogPostRepository.Get(id);
                if (record != null)
                {
                    _BlogPostRepository.Delete(record);
                    TriggerSignal();
                    return id;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Delete BlogPost failed, Id: {0}", id);
            }
            return -1;
        }
    }
}