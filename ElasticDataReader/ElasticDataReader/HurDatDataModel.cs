using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticDataReader
{
    public class HurDatDataModel
    {
        [Nest.Keyword(Store = true)]
        public string basin { get; set; }
        public int cyclone_number { get; set; }
        public int year { get; set; }
        [Nest.Keyword(Store = true)]
        public string name { get; set; }
        public int total_track_entries { get; set; }
        public int data_row_year { get; set; }
        public int data_row_month { get; set; }
        public int data_row_day { get; set; }
        public int data_row_hours { get; set; }
        public int data_row_minutes { get; set; }
        [Nest.Keyword(Store = true)]
        public string record_identifier { get; set; }
        [Nest.Keyword(Store = true)]
        public string system_status { get; set; }

        [Nest.GeoPoint(Store =true)]        
        public Nest.GeoLocation location { get; set; }
        
        public double Lat { get; set;}
        [Nest.Keyword(Store = true)]
        public string lat_hemisphere { get; set; }

        public double Lon { get; set; }
        [Nest.Keyword(Store = true)]
        public string long_hemisphere { get; set; }
        public int max_sustained_wind { get; set; }
        public int min_pressure { get; set; }
        public int max_ne_34_kt { get; set; }
        public int max_se_34_kt { get; set; }
        public int max_sw_34_kt { get; set; }
        public int max_nw_34_kt { get; set; }
        public int max_ne_50_kt { get; set; }
        public int max_se_50_kt { get; set; }
        public int max_sw_50_kt { get; set; }
        public int max_nw_50_kt { get; set; }
        public int max_ne_64_kt { get; set; }
        public int max_se_64_kt { get; set; }
        public int max_sw_64_kt { get; set; }
        public int max_nw_64_kt { get; set; }
        public string timestamp { get; set; }
        public double bearing_to_point { get; set; }
        public int star_calculus_value { get; set; }
        public double distance { get; set; }
        public int norm_star_value { get; set; }
    }

}



