using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webmap.Models;

namespace Webmap.Controllers
{
    public class ElasticController : Controller
    {
        // GET: Elastic
        public ActionResult Index()
        {
            return View("Elastic");
        }

        public ActionResult GetAllData(int clusterId)
        {
            var client = new ElasticClient();
            string indexName = "linestring1";
            
            var settings = new ConnectionSettings(new Uri("http://ec2-18-221-165-53.us-east-2.compute.amazonaws.com:9200")).DefaultIndex(indexName);

            client = new ElasticClient(settings);

            ISearchResponse<LineStringModel> searchResponse = client.Search<LineStringModel>(s => s
                                                    .Query(q => q
                                                        .MatchAll()
                                                    )
                                                );

            return Json(searchResponse.Documents.ToList<LineStringModel>(), JsonRequestBehavior.AllowGet);

        }
    }
}