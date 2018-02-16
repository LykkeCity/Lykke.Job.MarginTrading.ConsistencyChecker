using Lykke.Job.MarginTrading.ConsistencyChecker.Contract;
using Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Models;
using Lykke.Job.MarginTrading.ConsistencyChecker.Core;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Services
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
            // for each TradePosition based on TakerPositionID (Closed Positions Table)
            foreach (var tradingPosition in tradingPositions.OrderBy(x => x.Date))
            {
                // there should exist one and only one order for OpenDate and another for CloseDate (if closed)
                var tradingPositionOrders = tradingOrders
                    .Where(t => t.TakerPositionId == tradingPosition.TakerPositionId);
                if (tradingPosition.CloseDate != null && tradingPositionOrders.Count() != 2)
                {
                    result.Add(new OrdersReportAndOrderClosedOpenedCheckResult
                    {
                        OrderReport = tradingPositionOrders,
                        Position = tradingPosition,
                        Error = "Position should have 2 Order Reports (Open and Close)"
                    });
                }
                if (tradingPosition.CloseDate == null && tradingPositionOrders.Count() != 1)
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
            // for each TradePosition based on TakerPositionID (Closed Positions Table)
            foreach (var tradingPosition in tradingPositions.OrderBy(x => x.Date))
            {
                var tradingPositionOrders = tradingOrders
                    .Where(t => t.TakerPositionId == tradingPosition.TakerPositionId);
                var openOrderReport = tradingPositionOrders.FirstOrDefault(x => x.TakerAction == "Open");
                var closeOrderReport = tradingPosition.CloseDate != null ? tradingPositionOrders.FirstOrDefault(x => x.TakerAction == "Close") : null;

                // Date should exactly match
                var msg = new List<string>();

                if (openOrderReport != null && openOrderReport.Date.CompareTo(tradingPosition.OpenDate) != 0)
                    msg.Add($"Open date doesn't match: openOrderReport.Date=[{openOrderReport.Date.ToDateTimeCustomString()}] tradingPosition.OpenDate=[{tradingPosition.OpenDate?.ToDateTimeCustomString()}]");
                if (closeOrderReport != null && closeOrderReport.Date.CompareTo(tradingPosition.CloseDate) != 0)
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
        /// OrderType should match the CloseReason for close orders
        /// </summary>
        /// <param name="tradingOrders"></param>
        /// <param name="tradingPositions"></param>
        /// <returns></returns>
        internal static IEnumerable<OrdersReportAndOrderClosedOpenedCheckResult> CheckOrderTypes(this IEnumerable<ITradingOrder> tradingOrders, IEnumerable<ITradingPosition> tradingPositions)
        {
            // OrderType should match the CloseReason for close orders
            // TODO:
            // - order type?????
            return new OrdersReportAndOrderClosedOpenedCheckResult[0];
        }

        /// <summary>
        /// AccountID should match
        /// </summary>
        /// <param name="tradingOrders"></param>
        /// <param name="tradingPositions"></param>
        /// <returns></returns>
        internal static IEnumerable<OrdersReportAndOrderClosedOpenedCheckResult> CheckOrderAccountID(this IEnumerable<ITradingOrder> tradingOrders, IEnumerable<ITradingPosition> tradingPositions)
        {
            // AccountID should match
            // TODO:
            // - TradingOrderReport doesn't have Account Id
            return new OrdersReportAndOrderClosedOpenedCheckResult[0];
        }


        /// <summary>
        /// ClientID should match
        /// </summary>
        /// <param name="tradingOrders"></param>
        /// <param name="tradingPositions"></param>
        /// <returns></returns>
        internal static IEnumerable<OrdersReportAndOrderClosedOpenedCheckResult> CheckOrderClientID(this IEnumerable<ITradingOrder> tradingOrders, IEnumerable<ITradingPosition> tradingPositions)
        {
            var result = new List<OrdersReportAndOrderClosedOpenedCheckResult>();
            // for each TradePosition based on TakerPositionID (Closed Positions Table)
            foreach (var tradingPosition in tradingPositions.OrderBy(x => x.Date))
            {
                var tradingPositionOrders = tradingOrders
                    .Where(t => t.TakerPositionId == tradingPosition.TakerPositionId);


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
