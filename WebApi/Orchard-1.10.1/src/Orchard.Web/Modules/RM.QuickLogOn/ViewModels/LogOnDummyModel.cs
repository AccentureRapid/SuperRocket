using System.ComponentModel.DataAnnotations;
using Orchard.Environment.Extensions;

namespace RM.QuickLogOn.ViewModels
{
    [OrchardFeature("RM.QuickLogOn.TestForm")]
    public class LogOnDummyModel
    {
        [Required(ErrorMessage = "Login is required")]
        public string Login { get; set; }

        public string ReturnUrl { get; set; }
    }
}
