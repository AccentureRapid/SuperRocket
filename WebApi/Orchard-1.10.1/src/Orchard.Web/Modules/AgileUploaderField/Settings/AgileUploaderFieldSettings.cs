using System.ComponentModel.DataAnnotations;

namespace AgileUploaderField.Settings
{
    public class AgileUploaderFieldSettings
    {
        public const int DefaultMaxWidth = 500;
        public const int DefaultMaxHeight = 500;
        public const int DefaultFileLimit = 10;

        private int _maxWidth;
        private int _maxHeight;
        private int _fileLimit;

        public string Hint { get; set; }

        public string MediaFolder { get; set; }

        public bool AuthorCanSetAlternateText { get; set; }

        [Range(0, 2048)]
        public int MaxWidth
        {
            get { return _maxWidth != 0 ? _maxWidth : DefaultMaxWidth; }
            set { _maxWidth = value; }
        }

        [Range(0, 2048)]
        public int MaxHeight
        {
            get { return _maxHeight != 0 ? _maxHeight : DefaultMaxHeight; }
            set { _maxHeight = value; }
        }

        [Range(0, 50)]
        public int FileLimit
        {
            get { return _fileLimit != 0 ? _fileLimit : DefaultFileLimit; }
            set { _fileLimit = value; }
        }

    }
}
