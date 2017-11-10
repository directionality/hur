using Microsoft.VisualBasic.FileIO;
using Nest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticDataReader
{
    class Program
    {
        static void Main(string[] args)
        {

            using (TextFieldParser parser = new TextFieldParser(@"C:\Users\User\Desktop\HurricaneProject\hurdat2.csv"))
            {
                parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
                parser.SetDelimiters(",");

                List<HurDatDataModel> list = new List<HurDatDataModel>();
                
                while (!parser.EndOfData)
                {
                    //Process row
                    string[] fields = parser.ReadFields();

                    /*
                    AL092011,              IRENE,     39, 
                    1234567890123456789012345768901234567
                    AL(Spaces 1 and 2) – Basin – Atlantic
                    09(Spaces 3 and 4)
                    – ATCF cyclone number for that year
                    2011(Spaces 5 -
                    8, before first comma) – Year
                    IRENE(Spaces 19
                    - 28, before second comma) – Name, if available, or else “UNNAMED”  
                    39(Spaces 34 -
                    36) – Number of best track entries – rows – to follow
                    */
                    if (fields.Length == 4)
                    {
                        if (String.IsNullOrEmpty(fields[3]))
                        {
                            //line is a header-type row
                            
                            string cycloneNumber = fields[0].Substring(2, 2);
                            int intCycloneNumber = 0;
                            Int32.TryParse(cycloneNumber, out intCycloneNumber);
                            
                            string year = fields[0].Substring(4, 4);
                            int intYear = 0;
                            Int32.TryParse(year, out intYear);
                            
                            string entryCount = fields[2];
                            int intEntryCount = 0;
                            Int32.TryParse(entryCount, out intEntryCount);
                            
                            for (int i = 0; i < intEntryCount; i++)
                            {
                                HurDatDataModel model = new HurDatDataModel();
                                model.basin = fields[0].Substring(0, 2);
                                model.cyclone_number = intCycloneNumber;
                                if (fields[1] == "UNNAMED")
                                {
                                    model.name = fields[1] + "_" + cycloneNumber + "_" + year;
                                }
                                else
                                {
                                    model.name = fields[1] + "_" + cycloneNumber + "_" + year;
                                }
                                model.total_track_entries = intEntryCount;

                                string[] dataFields = parser.ReadFields();
                                if (dataFields.Length == 21 && !String.IsNullOrEmpty(dataFields[3]))
                                {
                                    model.data_row_year = Int32.Parse(dataFields[0].Substring(0, 4));
                                    model.data_row_month = Int32.Parse(dataFields[0].Substring(4, 2));
                                    model.data_row_day = Int32.Parse(dataFields[0].Substring(6, 2));

                                    model.data_row_hours = Int32.Parse(dataFields[1].Substring(0, 2));
                                    
                                    model.data_row_minutes = Int32.Parse(dataFields[1].Substring(2, 2));
                                    
                                    model.record_identifier = dataFields[2];

                                    model.system_status = dataFields[3];

                                    int decimalLocation4 = dataFields[4].IndexOf('.');
                                    model.Lat = Double.Parse(dataFields[4].Substring(0, decimalLocation4+2));
                                    model.lat_hemisphere = dataFields[4].Substring(decimalLocation4 + 2, 1);

                                    int decimalLocation5 = dataFields[5].IndexOf('.');
                                    model.Lon = Double.Parse(dataFields[5].Substring(0, decimalLocation5+2));
                                    model.long_hemisphere = dataFields[5].Substring(decimalLocation5+2, 1);
                                    if (model.long_hemisphere == "W")
                                    {
                                        model.Lon = -model.Lon;
                                    }

                                    try
                                    {
                                        model.location = new GeoLocation(model.Lat, model.Lon); //model.Lat.ToString() + "," + model.Lon.ToString(); //new GeoLocation(model.Lat, model.Lon);
                                    }
                                    catch
                                    {
                                        continue;
                                    }

                                    model.max_sustained_wind = Int32.Parse(dataFields[6]);
                                    model.max_ne_34_kt = Int32.Parse(dataFields[7]);
                                    model.max_se_34_kt = Int32.Parse(dataFields[8]);
                                    model.max_sw_34_kt = Int32.Parse(dataFields[9]);
                                    model.max_nw_34_kt = Int32.Parse(dataFields[10]);
                                    model.max_ne_50_kt = Int32.Parse(dataFields[11]);
                                    model.max_se_50_kt = Int32.Parse(dataFields[12]);
                                    model.max_sw_50_kt = Int32.Parse(dataFields[13]);
                                    model.max_nw_50_kt = Int32.Parse(dataFields[14]);
                                    model.max_ne_64_kt = Int32.Parse(dataFields[15]);
                                    model.max_se_64_kt = Int32.Parse(dataFields[16]);
                                    model.max_sw_64_kt = Int32.Parse(dataFields[17]);
                                    model.max_nw_64_kt = Int32.Parse(dataFields[18]);

                                    string hourFix = model.data_row_hours.ToString(); 
                                    string minuteFix = model.data_row_minutes.ToString();
                                    //yyyy-MM-dd'T'HH:mm
                                    if (model.data_row_hours == 0)
                                    {
                                        hourFix = "00";

                                    }

                                    if (hourFix.Length == 1)
                                    {
                                        hourFix = "0" + hourFix;
                                    }
                                   
                                    if (model.data_row_minutes == 0)
                                    {
                                        minuteFix = "00";
                                    }

                                    if (minuteFix.Length == 1)
                                    {
                                        minuteFix = "0" + minuteFix;
                                    }

                                    string dayFix = model.data_row_day.ToString();
                                    string monthFix = model.data_row_month.ToString();
                                    if (dayFix.Length == 1)
                                    {
                                        dayFix = "0" + dayFix;
                                    }
                                    if (monthFix.Length == 1)
                                    {
                                        monthFix = "0" + monthFix;
                                    }
                                    //"2015-01-01T12:10:30Z"
                                    string datetime = model.data_row_year + "-" + monthFix + "-" + dayFix + "T" + hourFix + ":" + minuteFix+ ":00Z";
                                    model.timestamp = datetime; // DateTime.ParseExact(datetime,"yyyy-MM-dd'T'HH:mm",CultureInfo.InvariantCulture);

                                    list.Add(model);
    }
                            }
                        }
                        else
                        {

                        }
                    }

                    foreach (string field in fields)
                    {
                        //TODO: Process field


                    }
                }


                SetupClientAndMapping(list);
            }
        }

        static void SetupClientAndMapping(List<HurDatDataModel> list) {
            var client = new ElasticClient();
            string indexName = "hurdat5";
            int granularity = 10;
            var settings = new ConnectionSettings(new Uri("http://ec2-18-221-165-53.us-east-2.compute.amazonaws.com:9200")).DefaultIndex("hurdat4");

            client = new ElasticClient(settings);

            var indexSettings = new IndexSettings { NumberOfReplicas = 1, NumberOfShards = 1 };
            var indexConfig = new IndexState
            {
                Settings = indexSettings
            };

            var createDescriptor = new CreateIndexDescriptor(indexName)
               .InitializeUsing(indexConfig);

            //var result = client.CreateIndex(createDescriptor);


            //client.Map<HurDatDataModel>(m => m
             //            .AutoMap().Properties(p => p
             //                .GeoPoint(ps => ps
             //                        .Name(pg => pg.location)
             //        )));

            //if (result.IsValid)
            {

                //var createMappingResult = client.Map<HurDatDataModel>(m => m.AutoMap());

                //if (createMappingResult.IsValid)
                //{
                //    Console.WriteLine("success creating index: " + result);
                //}
                for (int i = 0; i < list.Count; i++)
                {
                    if (i == 0)
                    {
                        continue;
                    }
                    else
                    {
                        HurDatDataModel model1 = list.ElementAt<HurDatDataModel>(i);
                        if (i < list.Count - 1)
                        {
                            double lat1 = model1.Lat;
                            double long1 = model1.Lon;

                            HurDatDataModel model2 = list.ElementAt<HurDatDataModel>(i+1);
                            if (model2 != null)
                            {
                                double lat2 = model2.Lat;
                                double long2 = model2.Lon;

                                model1.bearing_to_point = GetBearing(lat1, long1, lat2, long2);
                                model1.star_calculus_value = GetStarCalculusValue(granularity, model1.bearing_to_point);
                                model1.distance = GetDistance(lat1, long1, lat2, long2);
                                model1.norm_star_value = GetNormalizedValue(granularity, model1.star_calculus_value);
                                //update the list;
                                if (model1.norm_star_value < 0)
                                {
                                    int result3 = 0;
                                }
                                list.RemoveAt(i);

                                list.Insert(i, model1);
                            }
                            else
                            {
                                string alpha = "";
                            }

                        }
                        else
                        {
                            continue;
                        }

                        

                        
                    }

                }
               // foreach (HurDatDataModel row in list)
                {
                 //   IIndexResponse response = client.Index<HurDatDataModel>(row);

                }

                Dictionary<string, List<int>> sequenceDictionary = new Dictionary<string, List<int>>();
                Dictionary<string, List<HurDatDataModel>> dictionary = list.GroupBy(x => x.name).ToDictionary(x => x.Key, x => x.ToList());
                int result1 = dictionary.Count;

                Dictionary<string, string> finalDict = new Dictionary<string, string>();

                foreach (string key in dictionary.Keys)
                {
                    List<HurDatDataModel> tempList = new List<HurDatDataModel>();
                    tempList = dictionary[key];
                    string tempSequence = "";
                    foreach (HurDatDataModel modelItem in tempList)
                    {
                        tempSequence = tempSequence + modelItem.norm_star_value + ",";
                    }
                    finalDict.Add(key, tempSequence);

                }

                int result2 = finalDict.Count;

                WriteSequencesToFile(finalDict);
                //make sure to verify the sort order of the data!!!! by datetime stamp.
            }
            //else
            {
               // System.Console.WriteLine("error creating index: " + result);
            }
        }

        public static void WriteSequencesToFile(Dictionary<string,string> sequenceDictionary)
        {
            string path = @"C:\Users\User\Desktop\HurricaneProject\Sequences.txt";

            if (!File.Exists(path))
            {
                using (File.Create(path)) { }
                
            }
            
            TextWriter tw = new StreamWriter(path);

            foreach (string key in sequenceDictionary.Keys)
                {
                    // If the line doesn't contain the word 'Second', write the line to the file.
                    tw.WriteLine(key + "," + sequenceDictionary[key].ToString());
                    
                }
            tw.Close();

            Console.WriteLine("file complete, please check for format and content");
        }

        public static int GetNormalizedValue(int granularity, int star_calculus_value)
        {
            if (star_calculus_value == 35)
            {
                int result4 = 0;
            }
            int halfway_point = (360 / granularity)/2;

            if (star_calculus_value >= halfway_point)
            {
                int max = 360 / granularity;
                return -1 * (max - star_calculus_value);
            }
            else
            {
                return star_calculus_value;
            }

        }

        public static double GetBearing(double lat1, double long1, double lat2, double long2)
        {
            double angle = Math.Atan2(Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(long2 - long1), Math.Sin(long2 - long1) * Math.Cos(lat2));
            //= ATAN2(COS(lat1) * SIN(lat2) - SIN(lat1) * COS(lat2) * COS(lon2 - lon1), SIN(lon2 - lon1) * COS(lat2))
            //= ATAN2(COS(lat1)*SIN(lat2)-SIN(lat1)*COS(lat2)*COS(lon2-lon1), SIN(lon2-lon1)*COS(lat2)) 
            //convert to degrees
            angle = angle * (180 / Math.PI);
            if (angle < 0)
            {
                angle = 360 + angle;
            }
            return angle;
        }

        public static int GetStarCalculusValue(int granularity, double bearing) {
            
            //0: 0-10
            //1: 11-20
            //2: 21-30
            int star_calculus_value = (int)Math.Floor(bearing/granularity);
            
            return star_calculus_value;
        }

        public static double GetDistance(double lat1, double long1, double lat2, double long2)
        {
            double R = 6371000.0; // metres
            var phi1 = lat1 * (Math.PI / 180);
            var phi2 = lat2 * (Math.PI / 180);
            var deltaPhi = (lat2 - lat1) * (Math.PI / 180);
            var deltaLambda = (long2 - long1) * (Math.PI / 180);

            var a = Math.Sin(deltaLambda / 2) * Math.Sin(deltaPhi / 2) +
                    Math.Cos(phi1) * Math.Cos(phi2) *
                    Math.Sin(deltaLambda / 2) * Math.Sin(deltaLambda / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double d = R * c;

            return d;

        }

        /*
         * public static void LCS(char[] str1, char[] str2)
    {
        int[,] l = new int[str1.Length, str2.Length];
        int lcs = -1;
        string substr = string.Empty;
        int end = -1;

        for (int i = 0; i < str1.Length; i++)
        {
            for (int j = 0; j < str2.Length; j++)
            {
                if (str1[i] == str2[j])
                {
                    if (i == 0 || j == 0)
                    {
                        l[i, j] = 1;
                    }
                    else
                        l[i, j] = l[i - 1, j - 1] + 1;
                    if (l[i, j] > lcs)
                    {
                        lcs = l[i, j];
                        end = i;
                    }

                }
                else
                    l[i, j] = 0;
            }
        }

        for (int i = end - lcs + 1; i <= end; i++)
        {
            substr += str1[i];
        }

        Console.WriteLine("Longest Common SubString Length = {0}, Longest Common Substring = {1}", lcs, substr);
    } 
         */

    }

}


/*
public class VehicleRecords
{

    public string name { get; set; }
    public Coordinate Coordinate { get; set; }
    public double Distance { get; set; }
}

public class Coordinate
{
    public double Lat { get; set; }
    public double Lon { get; set; }
}

Step 2 :Insert some record using above class
Step 3 :Using below query to search....


Nest.ISearchResponse<VehicleRecords> Response = client.Search<VehicleRecords>(s => s.Sort(sort => sort.OnField(f => f.Year).Ascending()).From(0).Size(10).Filter(fil => fil.GeoDistance(n => n.Coordinate, d => d.Distance(Convert.ToDouble(100), GeoUnit.Miles).Location(73.1233252, 36.2566525))));
*/