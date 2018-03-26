using System.Collections.Generic;
using System.Linq;
using Lykke.Job.MtConsistencyChecker.Contract;
using Lykke.Job.MtConsistencyChecker.Contract.Results;
using Lykke.Job.MtConsistencyChecker.Core.Extensions;

namespace Lykke.Job.MtConsistencyChecker.Services.Extensions
{
    internal static class OrdersReportAndOrderClosedOpenedExtensions
    {
        /// <summary>
        /// for each TradePosition based on TakerPositionID (Closed Positions Table)
        /// there should exist one and only one order for OpenDate and another for CloseDate (if closed)
        /// </summary>
        /// <param name="tradingOrders"></param>
        /// <param name="tradingPositions"></param>
        /// <returns></returns>
        internal static IEnumerable<OrdersReportAndOrderClosedOpenedCheckResult> CheckNumberOfOrders(this IEnumerable<ITradingOrder> tradingOrders, IEnumerable<ITradingPosition> tradingPositions)
        {
            var result = new List<OrdersReportAndOrderClosedOpenedCheckResult>();
            var orders = tradingOrders.ToList();
            // for each TradePosition based on TakerPositionID (Closed Positions Table)
            foreach (var tradingPosition in tradingPositions.OrderBy(x => x.Date))
            {
                // there should exist one and only one order for OpenDate and another for CloseDate (if closed)
                
                var tradingPositionOrders = orders
                    .Where(t => t.TakerPositionId == tradingPosition.TakerPositionId)
                    .ToList();
                if (tradingPosition.CloseDate != null && tradingPositionOrders.Count != 2)
                {
                    result.Add(new OrdersReportAndOrderClosedOpenedCheckResult
                    {
                        OrderReport = tradingPositionOrders,
                        Position = tradingPosition,
                        Error = "Position should have 2 Order Reports (Open and Close)"
                    });
                }
                if (tradingPosition.CloseDate == null && tradingPositionOrders.Count != 1)
                {
                    result.Add(new OrdersReportAndOrderClosedOpenedCheckResult
                    {
                        OrderReport = tradingPositionOrders,
                        Position = tradingPosition,
                        Error = "Position should have 1 Order Report (Open only)"
                    });
                }

                var openOrderReport = tradingPositionOrders.FirstOrDefault(x => x.TakerAction == "Open");
                var closeOrderReport = tradingPosition.CloseDate != null ? tradingPositionOrders.FirstOrDefault(x => x.TakerAction == "Close") : null;

                if (openOrderReport == null)
                {
                    result.Add(new OrdersReportAndOrderClosedOpenedCheckResult
                    {
                        OrderReport = tradingPositionOrders,
                        Position = tradingPosition,
                        Error = "Open Order Report doesn't exist"
                    });
                }
                if (tradingPosition.CloseDate != null && closeOrderReport == null)
                {
                    result.Add(new OrdersReportAndOrderClosedOpenedCheckResult
                    {
                        OrderReport = tradingPositionOrders,
                        Position = tradingPosition,
                        Error = "Close Order Report doesn't exist"
                    });
                }
            }
            return result;
        }

        /// <summary>
        /// Date should exactly match
        /// </summary>
        /// <param name="tradingOrders"></param>
        /// <param name="tradingPositions"></param>
        /// <returns></returns>
        internal static IEnumerable<OrdersReportAndOrderClosedOpenedCheckResult> CheckOrdersDate(this IEnumerable<ITradingOrder> tradingOrders, IEnumerable<ITradingPosition> tradingPositions)
        {
            var result = new List<OrdersReportAndOrderClosedOpenedCheckResult>();
            var orders = tradingOrders.ToList();
            // for each TradePosition based on TakerPositionID
            foreach (var tradingPosition in tradingPositions.OrderBy(x => x.Date))
            {
                var tradingPositionOrders = orders
                    .Where(t => t.TakerPositionId == tradingPosition.TakerPositionId)
                    .ToList();
                var openOrderReport = tradingPositionOrders.FirstOrDefault(x => x.TakerAction == "Open");
                var closeOrderReport = tradingPosition.CloseDate != null ? tradingPositionOrders.FirstOrDefault(x => x.TakerAction == "Close") : null;

                // Date should exactly match
                var msg = new List<string>();

                if (openOrderReport != null && 
                    openOrderReport.ExecutionTimestamp?.CompareTo(tradingPosition.OpenDate) != 0)
                    msg.Add($"Open date doesn't match: openOrderReport.Date=[{openOrderReport.Date.ToDateTimeCustomString()}] tradingPosition.OpenDate=[{tradingPosition.OpenDate?.ToDateTimeCustomString()}]");
                if (closeOrderReport != null && 
                    closeOrderReport.ExecutionTimestamp?.CompareTo(tradingPosition.CloseDate) != 0)
                    msg.Add($"Close date doesn't match: closeOrderReport.Date=[{closeOrderReport.Date.ToDateTimeCustomString()}] tradingPosition.CloseDate=[{tradingPosition.CloseDate?.ToDateTimeCustomString()}]");
                if (msg.Count > 0)
                    result.Add(new OrdersReportAndOrderClosedOpenedCheckResult
                    {
                        OrderReport = tradingPositionOrders,
                        Position = tradingPosition,
                        Error = string.Join(";", msg)
                    });
            }
            return result;
        }
        
        /// <summary>
        /// ClientID should match
        /// </summary>
        /// <param name="tradingOrders"></param>
        /// <param name="tradingPositions"></param>
        /// <returns></returns>
        internal static IEnumerable<OrdersReportAndOrderClosedOpenedCheckResult> CheckOrderClientId(this IEnumerable<ITradingOrder> tradingOrders, IEnumerable<ITradingPosition> tradingPositions)
        {
            var result = new List<OrdersReportAndOrderClosedOpenedCheckResult>();
            var orders = tradingOrders.ToList();
            // for each TradePosition based on TakerPositionID (Closed Positions Table)
            foreach (var tradingPosition in tradingPositions.OrderBy(x => x.Date))
            {   
                var tradingPositionOrders = orders
                    .Where(t => t.TakerPositionId == tradingPosition.TakerPositionId)
                    .ToList();

                var noMatch = tradingPositionOrders.Where(m => m.TakerCounterpartyId != tradingPosition.TakerCounterpartyId)
                    .Select(x => new OrdersReportAndOrderClosedOpenedCheckResult
                    {
                        OrderReport = tradingPositionOrders,
                        Position = tradingPosition,
                        Error = $"OrderReport.TakerCounterpartyId=[{x.TakerCounterpartyId}] tradingPosition.TakerCounterpartyId=[{tradingPosition.TakerCounterpartyId}]"
                    });
                result.AddRange(noMatch);
            }
            return result;
        }
    }
}
