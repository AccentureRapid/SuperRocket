using Orchard;

namespace RM.QuickLogOn.Providers
{
    public interface IQuickLogOnProvider : IDependency
    {
        string Name { get; }
        string Description { get; }

        string GetLogOnUrl(WorkContext context);
    }
}
