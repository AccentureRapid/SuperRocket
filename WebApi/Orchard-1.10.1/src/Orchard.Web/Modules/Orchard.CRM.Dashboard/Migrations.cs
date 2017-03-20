using Orchard.Data.Migration;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;

namespace Orchard.CRM.Dashboard
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            ContentDefinitionManager.AlterPartDefinition("DashboardPortletPart", builder => builder.Attachable());
            ContentDefinitionManager.AlterPartDefinition("GenericDashboardPart", builder => builder.Attachable());
            ContentDefinitionManager.AlterPartDefinition("IMapEmailPortletPart", builder => builder.Attachable());
            ContentDefinitionManager.AlterPartDefinition("SmtpEmailPortletPart", builder => builder.Attachable());
            ContentDefinitionManager.AlterPartDefinition("BasicDataPortletPart", builder => builder.Attachable());
            ContentDefinitionManager.AlterPartDefinition("NavigationPortletPart", builder => builder.Attachable());
            ContentDefinitionManager.AlterPartDefinition("WorkflowPortletPart", builder => builder.Attachable());
            ContentDefinitionManager.AlterPartDefinition("QueriesPortletPart", builder => builder.Attachable());
            ContentDefinitionManager.AlterPartDefinition("SidebarDashboardPart", builder => builder.Attachable());
            ContentDefinitionManager.AlterPartDefinition("ContentManagementPortletPart", builder => builder.Attachable());
            ContentDefinitionManager.AlterPartDefinition("SidebarPart", builder => builder.Attachable());

            ContentDefinitionManager.AlterTypeDefinition(Consts.ContentManagementPortletContentType,
                cfg => cfg
                .WithPart("CommonPart")
                .WithPart("IdentityPart")
                .WithPart("ContentManagementPortletPart")
                .WithPart("ContainablePart")
                .WithPart("DashboardPortletPart")
                .DisplayedAs(Consts.SmtpPortletContentType)
                .Creatable(false).Listable(false));

            ContentDefinitionManager.AlterTypeDefinition(Consts.QueriesPortletContentType,
                cfg => cfg
                .WithPart("CommonPart")
                .WithPart("IdentityPart")
                .WithPart("QueriesPortletPart")
                .WithPart("ContainablePart")
                .WithPart("DashboardPortletPart")
                .DisplayedAs(Consts.SmtpPortletContentType)
                .Creatable(false).Listable(false));

            ContentDefinitionManager.AlterTypeDefinition(Consts.SmtpPortletContentType,
                cfg => cfg
                    .WithPart("CommonPart")
                    .WithPart("IdentityPart")
                    .WithPart("SmtpEmailPortletPart")
                    .WithPart("ContainablePart")
                    .WithPart("DashboardPortletPart")
                    .DisplayedAs(Consts.SmtpPortletContentType)
                    .Creatable(false).Listable(false));

            ContentDefinitionManager.AlterTypeDefinition(Consts.BasicDataPortletContentType,
                cfg => cfg
                    .WithPart("CommonPart")
                    .WithPart("IdentityPart")
                    .WithPart("BasicDataPortletPart")
                    .WithPart("ContainablePart")
                    .WithPart("DashboardPortletPart")
                    .DisplayedAs(Consts.BasicDataPortletContentType)
                    .Creatable(false).Listable(false));

            ContentDefinitionManager.AlterTypeDefinition(Consts.WorkflowPortletContentType,
                cfg => cfg
                    .WithPart("CommonPart")
                    .WithPart("IdentityPart")
                    .WithPart("WorkflowPortletPart")
                    .WithPart("ContainablePart")
                    .WithPart("DashboardPortletPart")
                    .DisplayedAs(Consts.BasicDataPortletContentType)
                    .Creatable(false).Listable(false));

            ContentDefinitionManager.AlterTypeDefinition(Consts.NavigationPortletContentType,
                cfg => cfg
                .WithPart("CommonPart")
                .WithPart("IdentityPart")
                .WithPart("NavigationPortletPart")
                .WithPart("ContainablePart")
                .WithPart("DashboardPortletPart")
                .DisplayedAs(Consts.BasicDataPortletContentType)
                .Creatable(false).Listable(false));

            ContentDefinitionManager.AlterTypeDefinition(Consts.IMAPTPortletContentType,
                cfg => cfg
                    .WithPart("CommonPart")
                    .WithPart("IMapEmailPortletPart")
                    .WithPart("IdentityPart")
                    .WithPart("ContainablePart")
                    .WithPart("DashboardPortletPart")
                    .DisplayedAs(Consts.IMAPTPortletContentType)
                    .Creatable(false).Listable(false));

            ContentDefinitionManager.AlterTypeDefinition(Consts.SidebarProjectionPortletType,
                cfg => cfg
                    .WithPart("CommonPart")
                    .WithPart("ProjectionWithDynamicSortPart")
                    .WithPart("IdentityPart")
                    .WithPart("TitlePart")
                    .WithPart("ContainablePart")
                    .WithPart("DashboardPortletPart")
                    .DisplayedAs(Consts.SidebarProjectionPortletType)
                    .Creatable(false).Listable(false));

            ContentDefinitionManager.AlterTypeDefinition(Consts.SidebarProjectionPortletTemplateType,
                cfg => cfg
                .WithPart("CommonPart")
                .WithPart("ProjectionWithDynamicSortPart")
                .WithPart("IdentityPart")
                .WithPart("TitlePart")
                .WithPart("ContainablePart")
                .WithPart("DashboardPortletPart")
                .DisplayedAs(Consts.SidebarProjectionPortletTemplateType)
                .Creatable(true).Listable(true));

            ContentDefinitionManager.AlterTypeDefinition(Consts.SidebarStaticPortletTemplateType,
                cfg => cfg
                    .WithPart("CommonPart")
                    .WithPart("BodyPart")
                    .WithPart("IdentityPart")
                    .WithPart("TitlePart")
                    .WithPart("ContainablePart")
                    .WithPart("DashboardPortletPart")
                    .DisplayedAs(Consts.SidebarStaticPortletTemplateType)
                    .Creatable(false).Listable(false));

            ContentDefinitionManager.AlterTypeDefinition(Consts.SidebarStaticPortletType,
                cfg => cfg
                .WithPart("CommonPart")
                .WithPart("BodyPart")
                .WithPart("IdentityPart")
                .WithPart("TitlePart")
                .WithPart("ContainablePart")
                .WithPart("DashboardPortletPart")
                .DisplayedAs(Consts.SidebarStaticPortletType)
                .Creatable(true).Listable(true));

            ContentDefinitionManager.AlterTypeDefinition(Consts.GenericDashboardContentType,
                cfg => cfg
                    .WithPart("CommonPart")
                    .WithPart("IdentityPart")
                    .WithPart("GenericDashboardPart")
                    .WithPart("AutoroutePart")
                    .WithPart("TitlePart")
                    .WithPart("ContainerPart")
                    .DisplayedAs(Consts.GenericDashboardContentType)
                    .Creatable(true).Listable(true));

            ContentDefinitionManager.AlterTypeDefinition(Consts.SidebarDashboardType,
                cfg => cfg
                    .WithPart("CommonPart")
                    .WithPart("IdentityPart")
                    .WithPart("SidebarDashboardPart")
                    .WithPart("TitlePart")
                    .WithPart("ContainerPart")
                    .DisplayedAs(Consts.SidebarDashboardType)
                    .Creatable(true).Listable(true));

            ContentDefinitionManager.AlterTypeDefinition(Consts.GenericDashboardWidgetContentType,
                cfg => cfg
                    .WithPart("WidgetPart")
                    .WithPart("CommonPart")
                    .WithPart("IdentityPart")
                    .WithPart("ContainerPart")
                    .WithPart("GenericDashboardPart")
                    .WithPart("TitlePart")
                    .WithSetting("Stereotype", "Widget")
                    .DisplayedAs(Consts.GenericDashboardWidgetContentType)
                );

            ContentDefinitionManager.AlterTypeDefinition(Consts.SidebarWidgetType,
                cfg => cfg
                    .WithPart("WidgetPart")
                    .WithPart("CommonPart")
                    .WithPart("IdentityPart")
                    .WithPart("SidebarPart")
                    .WithSetting("Stereotype", "Widget")
                    .DisplayedAs("Sidebar")
                );

            return 1;
        }
    }
}