using Orchard.CRM.Core.Models;
using Orchard.CRM.Core.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.Mvc;
using System.Linq;
using System.Globalization;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;

namespace Orchard.CRM.Core.Services
{
    public static class Converter
    {
        public static void Fill(Collection<SelectListItem> collection, int? selectedValue, IEnumerable<IBasicDataRecord> items)
        {
            foreach (var item in items)
            {
                bool isSelected = selectedValue.HasValue && selectedValue.Value == item.Id;
                collection.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString(),
                    Selected = isSelected
                });
            }
        }

        public static void Fill(Collection<SelectListItem> target, IEnumerable<ProjectPart> projects)
        {
            target.AddRange(projects.Select(c =>
                new SelectListItem
                {
                    Text = c.Record.Title,
                    Value = c.Record.Id.ToString(CultureInfo.InvariantCulture)
                }));

            target.Insert(0, new SelectListItem());
        }

        public static void Fill(Collection<SelectListItem> target, IEnumerable<ContentItem> contentItems)
        {
            Fill(target, contentItems, true);
        }

        public static void Fill(Collection<SelectListItem> target, IEnumerable<ContentItem> contentItems, bool addEmptyItem)
        {
            target.AddRange(contentItems.Select(c =>
                new SelectListItem
                {
                    Text = c.Is<TitlePart>() ? c.As<TitlePart>().Title : c.Id.ToString(),
                    Value = c.Record.Id.ToString(CultureInfo.InvariantCulture)
                }));

            if (addEmptyItem)
            {
                target.Insert(0, new SelectListItem());
            }
        }

        public static TargetContentItemPermissionViewModel DecodeGroupId(string groupId)
        {
            if (string.IsNullOrEmpty(groupId))
            {
                return null;
            }

            string[] parts = groupId.Split(new[] { ':' });
            if (parts.Length > 0)
            {
                int id = int.Parse(parts[1]);
                TargetContentItemPermissionViewModel targetContentItemPermissionViewModel = new TargetContentItemPermissionViewModel { Checked = true };
                if (parts[0] == "Team")
                {
                    targetContentItemPermissionViewModel.TeamId = id;
                }
                else
                {
                    targetContentItemPermissionViewModel.BusinessUnitId = id;
                }

                return targetContentItemPermissionViewModel;
            }
            else
            {
                return null;
            }
        }
    }
}