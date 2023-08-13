using System.Text.RegularExpressions;

namespace Deerchao.War3Share.W3gParser
{
    public class MapInfo
    {
        private readonly string path;
        private readonly long hash;

        public MapInfo(long hash, string path)
        {
            this.hash = hash;
            this.path = path;
        }

        public string Path
        {
            get { return path; }
        }

        public long Hash
        {
            get { return hash; }
        }

        public string GetName()
        {
            if (path == null)
                return null;
            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            return Regex.Replace(fileName, @"\(\d+\)", "");
        }
    }
}