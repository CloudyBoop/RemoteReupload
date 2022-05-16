using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteReupload.Model
{
    public class Avatar
    {
        public string Id { get; set; }
        public string Group { get; set; }
        public string Name { get; set; }
        public string AssetUrl { get; set; }
        public string LocalAssetPath { get; set; }
        public string LocalImagePath { get; set; }
        public string Thumbnail { get; set; }
    }
}
