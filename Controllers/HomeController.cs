using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webmap.Models;

namespace Webmap.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult GetAllData(string hurricaneName)
        {
            var client = new ElasticClient();
            string indexName = "linestring1";

            var settings = new ConnectionSettings(new Uri("http://ec2-54-218-10-53.us-west-2.compute.amazonaws.com:9200")).DefaultIndex(indexName);

            client = new ElasticClient(settings);




            ISearchResponse<LineStringModel> searchResponse = client.Search<LineStringModel>(s => s.Size(2000)
                                                    .Query(q => q
                                                        .MatchAll()
                                                    ));
            

            JsonResult result = Json(searchResponse.Documents.ToList<LineStringModel>(), JsonRequestBehavior.AllowGet);

            return result;

        }

        public ActionResult GetAllDataByHurricaneName(string hurricaneName)
        {
            var client = new ElasticClient();
            string indexName = "linestring1";

            var settings = new ConnectionSettings(new Uri("http://ec2-54-218-10-53.us-west-2.compute.amazonaws.com:9200")).DefaultIndex(indexName);

            client = new ElasticClient(settings);
            
            ISearchResponse<LineStringModel> searchResponse = client.Search<LineStringModel>(s => s.Size(2000)
                                                    .Query(q => q
                                                        .MatchAll()
                                                    )
                                                    .Aggregations(
                                                            m => m.TopHits("hurricanes",
                                                            n => n.Field(f => f.name.Suffix("keyword")).Size(0))));

            TopHitsAggregate topHits = searchResponse.Aggs.TopHits("hurricanes");
            List<LineStringModel> documents = (List<LineStringModel>)topHits.Documents<LineStringModel>();

            documents = documents.Where(s => s.name == hurricaneName).ToList<LineStringModel>();
            
            JsonResult result = Json(searchResponse.Documents.ToList<LineStringModel>(), JsonRequestBehavior.AllowGet);

            return result;

        }

        public ActionResult GetHurricanes(string hurricaneNames)
        {
            var client = new ElasticClient();
            string indexName = "linestring1";

            var settings = new ConnectionSettings(new Uri("http://ec2-54-218-10-53.us-west-2.compute.amazonaws.com:9200")).DefaultIndex(indexName);

            client = new ElasticClient(settings);

            List<string> names = hurricaneNames.Split(',').ToList<string>();

            ISearchResponse<LineStringModel> searchResponse = client.Search<LineStringModel>(s => s.Size(2000)
                                                    .Query(q => q
                                                        .MatchAll()
                                                        ));



            /*BucketAggregate bucket = (BucketAggregate)searchResponse.Aggregations["hurricanes"];

            KeyedBucket<LineStringModel> keyedB = (KeyedBucket<LineStringModel>)bucket.Items;
            List<LineStringModel> documents = new List<LineStringModel>();
            
            List<LineStringModel> filteredList = new List<LineStringModel>();

            foreach (LineStringModel model in documents)
            {
                if (names.Contains(model.name))
                {
                    filteredList.Add(model);
                }

            }*/
            //List<LineStringModel> filteredList = searchResponse.Documents.ToList<LineStringModel>().Where(s => s.name == names[0].ToString()).ToList<LineStringModel>();

            //var allowedStatus = new[] { "A", "B", "C" };
            List<LineStringModel> filteredList = searchResponse.Documents.ToList<LineStringModel>().Where(o => names.Contains(o.name)).ToList<LineStringModel>();

            JsonResult result = Json(filteredList, JsonRequestBehavior.AllowGet);

            return result;

        }
    }
}