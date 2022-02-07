using credentialsPBR.Models.Dashboard;
using System.Collections.Generic;

namespace apiPBR.Models.Response.Dashboard
{
    public class ResponseInformationGlobal : BasicResponse
    {
        public List<InformationGlobal> InformationGlobalAdquisiciones { get; set; }
        public List<InformationGlobal> InformationGlobalObras { get; set; }
        public List<InformationGlobal> InformationGlobalObrasEjercido { get; set; }
        public List<InformationCake> InformationCakeAdjudicaciones { get; set; }
        public List<InformationCake> InformationCakeObras { get; set; }
        public List<InformationCake> InformationAdjudicaciones { get; set; }
        public List<InformationCake> InformationObras { get; set; }
    }
}