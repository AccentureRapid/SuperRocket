
namespace Orchard.OData
{
    using Logging;
    using Localization;
    using Orchard.OData.Services;
    using System;
    using System.Collections.Specialized;
    using System.Data.Services;
    using System.Data.Services.Common;
    using System.Data.Services.Providers;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.Web;

    public interface IODataService : IDependency
    {
    }

    [JSONPSupportBehavior]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Single, IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ODataService : DataService<IODataServiceContext>, IODataService, IServiceProvider
    {
        private readonly IODataServiceMetadata _oDataServiceMetadata;
        private readonly IODataServiceQuery _oDataServiceQuery;
        public ODataService(
            IODataServiceMetadata oDataServiceMetadata,
            IODataServiceQuery oDataServiceQuery)
        {
            this._oDataServiceMetadata = oDataServiceMetadata;
            this._oDataServiceQuery = oDataServiceQuery;

            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
            config.DataServiceBehavior.AcceptProjectionRequests = true;
            config.DataServiceBehavior.AcceptCountRequests = true;
            config.DataServiceBehavior.AcceptAnyAllRequests = true;
            config.UseVerboseErrors = true;
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            if (serviceType == typeof(IDataServiceMetadataProvider))
            {
                return this._oDataServiceMetadata;
            }
            if (serviceType == typeof(IDataServiceQueryProvider))
            {
                return this._oDataServiceQuery;
            }
            return null;
        }

        protected override void HandleException(HandleExceptionArgs args)
        {
            if (args.Exception != null)
            {
                Logger.Error("Error occurs when execute the odata service : " + args.Exception);
                args.UseVerboseErrors = false;
            }
        }

        protected override void OnStartProcessingRequest(ProcessRequestArgs args)
        {
            var lastFragment = args.RequestUri.AbsolutePath.Split('/').Last();
            if (lastFragment == string.Empty ||
                lastFragment.EndsWith("metadata")) {
                return;
            }

            NameValueCollection queryArgs = HttpUtility.ParseQueryString(args.RequestUri.Query);
            IODataServiceContext context = this._oDataServiceQuery.CurrentDataSource as IODataServiceContext;
            context.CustomState = context.GetAnnotation(args.ServiceUri, args.RequestUri, this._oDataServiceMetadata);
            int parsedValue = 0;

            var expand = queryArgs["$expand"];
            if (!string.IsNullOrEmpty(expand)) {
                context.Expand = expand;
            }

            var filter = queryArgs["$filter"];
            if (!string.IsNullOrEmpty(filter)) {
                context.Filter = filter;
            }
            
            var orderBy = queryArgs["$orderby"];
            if (!string.IsNullOrEmpty(orderBy)){
                context.OrderBy = orderBy;
            }

            var top = queryArgs["$top"];
            if (!string.IsNullOrEmpty(top) && true == int.TryParse(top, out parsedValue)) {
                context.MaxResults = parsedValue;
            }

            var skip = queryArgs["$skip"];
            if (!string.IsNullOrEmpty(skip) && true == int.TryParse(skip, out parsedValue)){
                context.FirstResults = parsedValue;
            }
        }

        protected override IODataServiceContext CreateDataSource()
        {
            return (IODataServiceContext)this._oDataServiceQuery.CurrentDataSource;
        }
    }
}