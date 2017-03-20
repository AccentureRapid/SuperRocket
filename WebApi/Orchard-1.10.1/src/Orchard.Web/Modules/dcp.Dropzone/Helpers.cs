using System.IO;
using System.Linq;

namespace dcp.Dropzone
{
    public static class Helpers
    {
        public static string ToDropzoneId(this string value)
        {
            var comps = value.Split(new[] {'_'}, System.StringSplitOptions.RemoveEmptyEntries);
            var compsN = comps.Take(1).Select(x => x.ToLower()).Union(comps.Skip(1).Select(x => x.Substring(0, 1).ToUpper() + x.Substring(1))).ToList();
            return string.Join("", compsN);
        }
    }

   
}