using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using dcp.Routing.Models;
using Orchard;
using Orchard.Alias.Records;
using Orchard.Data;

namespace dcp.Routing.Services
{
    public interface IExtendedAliasService : IDependency
    {
        IEnumerable<ExtendedAliasRecord> GetAll();
        ExtendedAliasRecord Get(int aliasRecordId);
        void Update(int aliasRecordId, string routeName);
    }

    public class ExtendedAliasService : IExtendedAliasService
    {
        private readonly IRepository<ExtendedAliasRecord> _repository;
        private readonly IRepository<AliasRecord> _aliasRepository;
        
        private readonly Expression<Func<AliasRecord, bool>> _predicate;
        private readonly Func<AliasRecord, bool> _predicateCompiled;

        public ExtendedAliasService(IRepository<ExtendedAliasRecord> repository, IRepository<AliasRecord> aliasRepository)
        {
            _repository = repository;
            _aliasRepository = aliasRepository;
            _predicate = x => x.Path.Contains("{") && x.Path.Contains("}");
            _predicateCompiled = _predicate.Compile();
        }
              
        public IEnumerable<ExtendedAliasRecord> GetAll()
        {
            var aliasRecords = _aliasRepository.Fetch(_predicate).ToList();
            var records = new List<ExtendedAliasRecord>();
            foreach (var aliasRecord in aliasRecords)
            {
                var record = _repository.Get(x => x.AliasRecord.Id == aliasRecord.Id);
                if (record == null)
                {
                    record = new ExtendedAliasRecord
                    {
                        AliasRecord = aliasRecord
                    };
                }
                records.Add(record);
            }
            return records;
        }

        public ExtendedAliasRecord Get(int aliasRecordId)
        { 
            var record = _repository.Get(x => x.AliasRecord.Id == aliasRecordId);
            if (record == null)
            {
                var aliasRecord = _aliasRepository.Get(aliasRecordId);
                if (!IsSatisfiedPath(aliasRecord.Path))
                    throw new ApplicationException("Alias is not supported");

                record = new ExtendedAliasRecord 
                { 
                    AliasRecord = aliasRecord
                };
            }
            return record;
        }

        public void Update(int aliasRecordId, string routeName)
        {
            var record = _repository.Get(x => x.AliasRecord.Id == aliasRecordId);
            if (record == null)
            {
                var aliasRecord = _aliasRepository.Get(aliasRecordId);
                if (!IsSatisfiedPath(aliasRecord.Path))
                    throw new ApplicationException("Alias is not supported");

                record = new ExtendedAliasRecord
                {
                    AliasRecord = aliasRecord,
                    RouteName = routeName
                };
                _repository.Create(record);
                return;
            }

            record.RouteName = routeName;
            _repository.Update(record);
        }

        
        private bool IsSatisfiedPath(string path)
        {
            var dummyAliasRecord = new AliasRecord { Path = path };
            return _predicateCompiled(dummyAliasRecord);
        }
    }

    
}