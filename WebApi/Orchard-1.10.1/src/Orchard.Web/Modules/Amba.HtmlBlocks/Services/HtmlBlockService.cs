using System.Collections.Generic;
using System.Linq;
using Amba.HtmlBlocks.Models;
using Orchard;
using Orchard.Data;

namespace Amba.HtmlBlocks.Services
{
    public interface IHtmlBlockService : IDependency
    {
        string GetHtmlBlockByKey(string blockKey);
        void SetHtmlBlock(string blockKey, string html);

        IEnumerable<HtmlBlockRecord> GetAllBlocks();
        HtmlBlockRecord GetHtmlBlock(int id);
        void SaveHtmlBlock(HtmlBlockRecord record);
        void Delete(int id);
        bool BlockExists(string key);
    }

    public class HtmlBlockService : IHtmlBlockService
    {
        private readonly IRepository<HtmlBlockRecord> _htmlBlockRepo;
        public HtmlBlockService(IRepository<HtmlBlockRecord> htmlBlockRepo)
        {
            _htmlBlockRepo = htmlBlockRepo;
        }

        public string GetHtmlBlockByKey(string blockKey)
        {
            return _htmlBlockRepo.Table
                .Where(x => x.BlockKey == blockKey)
                .Select(x => x.HTML)
                .FirstOrDefault();
        }

        public void SetHtmlBlock(string blockKey, string html)
        {
            var block = _htmlBlockRepo
                .Table
                .FirstOrDefault(x => x.BlockKey == blockKey);
            if (block == null)
            {
                block = new HtmlBlockRecord();
                block.BlockKey = blockKey;
                _htmlBlockRepo.Create(block);
            }
            block.HTML = html;
            _htmlBlockRepo.Update(block);
        }

        public IEnumerable<HtmlBlockRecord> GetAllBlocks()
        {
            return _htmlBlockRepo
                .Table
                .OrderBy(x => x.BlockKey)
                .ToList();
        }

        public HtmlBlockRecord GetHtmlBlock(int id)
        {
            return _htmlBlockRepo.Get(id);
        }

        public void SaveHtmlBlock(HtmlBlockRecord record)
        {
            if (record.Id > 0)
            {
                _htmlBlockRepo.Update(record);
            }
            else
            {
                _htmlBlockRepo.Create(record);    
            }
        }

        public bool BlockExists(string key)
        {
            return _htmlBlockRepo.Table
               .Any(x => x.BlockKey == key);
        }

        public void Delete(int id)
        {
            var block = GetHtmlBlock(id);
            _htmlBlockRepo.Delete(block);
        }
    }
}