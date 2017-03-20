using AgileUploaderField.Settings;

namespace AgileUploaderField.ViewModels
{
    public class AgileUploaderFieldViewModel
    {
        public string AgileUploaderMediaFolder { get; set; }
        public AgileUploaderFieldSettings Settings { get; set; }
        public Fields.AgileUploaderField Field { get; set; }
    }
}
