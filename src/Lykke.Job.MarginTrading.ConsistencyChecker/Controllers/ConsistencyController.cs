using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Models;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Results;
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
        private readonly ILog _log;

        public ConsistencyController(IConsistencyService consistencyService, ILog log)
        {            
            _consistencyService = consistencyService;
            _log = log;
        }

        /// <summary>
        /// Performs a Balance and transaction amount consistency Check
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("CheckBalanceAndTransactionAmount")]
        [SwaggerOperation("CheckBalanceAndTransactionAmount/{dateFrom}/{dateTo}")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(List<BalanceAndTransactionAmountCheckResult>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CheckBalanceAndTransactionAmount(DateTime? dateFrom, DateTime? dateTo)
        {
            var res = await _consistencyService.CheckBalanceAndTransactionAmount(true, dateFrom, dateTo);
            if (res.Count() == 0)
                return Ok("OK");
            else
                return Ok(res);
        }

        /// <summary>
        /// Performs a Balance Transaction and OrderClosed consistency check
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("CheckBalanceAndOrderClosed")]
        [SwaggerOperation("CheckBalanceAndOrderClosed/{dateFrom}/{dateTo}")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(List<BalanceAndOrderClosedCheckResult>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CheckBalanceAndOrderClosed(DateTime? dateFrom, DateTime? dateTo)
        {
            var res = await _consistencyService. CheckBalanceAndOrderClosed(true, dateFrom, dateTo);
            if (res.Count() == 0)
                return Ok("OK");
            else
                return Ok(res);
        }

        /// <summary>
        /// Performs OrdersReport to TradePositionReportClosed & TradePositionReportOpened consistency check
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("CheckOrdersReportAndOrderClosedOpened")]
        [SwaggerOperation("CheckOrdersReportAndOrderClosedOpened/{dateFrom}/{dateTo}")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(List<OrdersReportAndOrderClosedOpenedCheckResult>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CheckOrdersReportAndOrderClosedOpened(DateTime? dateFrom, DateTime? dateTo)
        {
            var res = await _consistencyService.CheckOrdersReportAndOrderClosedOpened(true, dateFrom, dateTo);
            if (res.Count() == 0)
                return Ok("OK");
            else
                return Ok(res);
        }

        /// <summary>
        /// Performs an OpenPrice & ClosePrice consistency check with Price Candles data 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("CheckCandlesPriceConsistency")]
        [SwaggerOperation("CheckCandlesPriceConsistency/{dateFrom}/{dateTo}")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(List<OrdersReportAndOrderClosedOpenedCheckResult>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CheckCandlesPriceConsistency(DateTime? dateFrom, DateTime? dateTo)
        {
            var res = await _consistencyService.CheckCandlesPriceConsistency(true, dateFrom, dateTo);
            if (res.Count() == 0)
                return Ok("OK");
            else
                return Ok(res);
        }

  
        /// <summary>
        /// Performs a MarginEvents account status consistency with balance transactions check
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("CheckMarginEventsAccountStatus")]
        [SwaggerOperation("CheckMarginEventsAccountStatus/{dateFrom}/{dateTo}")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(List<MarginEventsAccountStatusCheckResult>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CheckMarginEventsAccountStatus(DateTime? dateFrom, DateTime? dateTo)
        {
            var res = await _consistencyService.CheckMarginEventsAccountStatus(true, dateFrom, dateTo);
            if (res.Count() == 0)
                return Ok("OK");
            else
                return Ok(res);
        }


        /// <summary>
        /// Performs a MarginEvents account status consistency with balance transactions check
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("CheckHedgingService")]
        [SwaggerOperation("CheckHedgingService/{dateFrom}/{dateTo}")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(List<HedgingServiceCheckResult>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CheckHedgingService(DateTime? dateFrom, DateTime? dateTo)
        {
            var res = await _consistencyService.CheckHedgingService(true, dateFrom, dateTo);
            if (res.Count() == 0)
                return Ok("OK");
            else
                return Ok(res);
        }
    }
}
