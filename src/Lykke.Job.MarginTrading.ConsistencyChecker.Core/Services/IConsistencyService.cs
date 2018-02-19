﻿using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Core.Services
{
    public interface IConsistencyService
    {
        Task<IEnumerable<IBalanceAndTransactionAmountCheckResult>> CheckBalanceAndTransactionAmount(bool isSql, DateTime? from, DateTime? to);
        Task<IEnumerable<IBalanceAndOrderClosedCheckResult>> CheckBalanceAndOrderClosed(bool isSql, DateTime? from, DateTime? to);
        Task<IEnumerable<IOrdersReportAndOrderClosedOpenedCheckResult>> CheckOrdersReportAndOrderClosedOpened(bool isSql, DateTime? from, DateTime? to);
        Task<IEnumerable<IOrdersReportAndOrderClosedOpenedCheckResult>> CheckCandlesPriceConsistency(bool isSql, DateTime? from, DateTime? to);
        Task<IEnumerable<IOrdersReportAndOrderClosedOpenedCheckResult>> CheckTradePnLConsistency(bool isSql, DateTime? from, DateTime? to);
        Task<IEnumerable<IOrdersReportAndOrderClosedOpenedCheckResult>> CheckMarginEventsAccountStatus(bool isSql, DateTime? from, DateTime? to);
        Task<IEnumerable<IBalanceAndOrderClosedCheckResult>> CheckHedgingServiceBalance(bool isSql, DateTime? from, DateTime? to);
    }
}
