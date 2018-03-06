using Entities.DTO;
using Entities.DTO.Static;
using Microsoft.Practices.Unity;
using ProductTracking.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProductTracking.Controllers.Core
{
    [AllowAnonymous]
    public class OpenController : ApiController
    {
        [Dependency("Tanservice")]
        public TanService TanService { get; set; }

        [HttpGet]
        public AppStaticDTO StaticData()
        {
            AppStaticDTO dto = new AppStaticDTO();

            #region Regular Expressions
            var dbRegularExpressions = TanService.GetRegulerExpressions();
            foreach (var rex in dbRegularExpressions)
                dto.RegulerExpressions.Add(new RegulerExpressionDTO
                {
                    Expression = rex.Expression,
                    Id = rex.Id,
                    RegulerExpressionFor = rex.RegulerExpressionFor
                });
            #endregion

            var boilingPoints = TanService.GetSolventBoilingPoints();
            foreach (var bp in boilingPoints)
                dto.SolventBoilingPoints.Add(new SolventBoilingPointDTO
                {
                    DegreesBoilingPoint = bp.DegreesBoilingPoint,
                    fahrenheitBoilingPoint = bp.fahrenheitBoilingPoint,
                    Id = bp.Id,
                    KelvinBoilingPoint = bp.KelvinBoilingPoint,
                    Name = bp.Name,
                    RankineBoilingPoint = bp.RankineBoilingPoint,
                    RegNo = bp.RegNo
                });
            var cvts = TanService.GetCVTs();
            var freetexts = TanService.GetFreetexts();
            dto.CommentDictionary.CVTs = cvts;
            dto.CommentDictionary.Freetexts = freetexts;
            dto.NamePriorities = TanService.GetNamePriorities();
            return dto;
        }
    }
}
