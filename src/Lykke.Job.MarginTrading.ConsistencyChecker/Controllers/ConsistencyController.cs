using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Models;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core.Services;
using Lykke.Job.MarginTrading.ConsistencyChecker.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Controllers
{
    // NOTE: See https://lykkex.atlassian.net/wiki/spaces/LKEWALLET/pages/35755585/Add+your+app+to+Monitoring
    [Route("api/[controller]")]
    public class ConsistencyController : Controller
    {
        private readonly IConsistencyService _consistencyService;

        public ConsistencyController(IConsistencyService consistencyService)
        {
            _consistencyService = consistencyService;
        }

        /// <summary>
        /// Checks service is alive
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("checkbalanceandtransactionamount")]
        [SwaggerOperation("CheckBalanceAndTransactionAmount/{dateFrom}/{dateTo}")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(List<BalanceAndTransactionAmountCheckResult>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CheckBalanceAndTransactionAmount(DateTime? dateFrom, DateTime? dateTo)
        {
            var res = await _consistencyService.CheckBalanceAndTransactionAmount(false, dateFrom, dateTo);
            if (res.Count() == 0)
                return Ok("OK");
            else
                return Ok(res);
        }
    }
}
