using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webmap.Models
{
    public class LineStringModel
    {
        [Nest.Keyword(Store = true)]
        public string name { get; set; }

        [Nest.GeoShape(Store = true)]
        public Nest.LineStringGeoShape location { get; set; }

        public string timestamp { get; set; }
    }
}