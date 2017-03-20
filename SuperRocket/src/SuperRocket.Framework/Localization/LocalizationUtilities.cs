using Autofac;

namespace SuperRocket.Framework.Localization
{
    internal sealed class LocalizationUtilities
    {
        public static Localizer Resolve(IComponentContext context, string scope)
        {
            var text = context.Resolve<IText>(new NamedParameter("scope", scope));
            return text.Get;
        }
    }
}