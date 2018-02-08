using Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Models;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract
{
    public static class Extensions
    {
        public static AccountMarginEventReport ToDto(this IAccountMarginEventReport src)
        {
            return new AccountMarginEventReport
            {
                AccountId = src.AccountId,
                Balance = src.Balance,
                BaseAssetId = src.BaseAssetId,
                ClientId = src.ClientId,
                EventId = src.EventId,
                EventTime = src.EventTime,
                FreeMargin = src.FreeMargin,
                Id = src.Id,
                IsEventStopout = src.IsEventStopout,
                MarginAvailable= src.MarginAvailable,
                MarginCall = src.MarginCall,
                MarginInit = src.MarginInit,
                MarginUsageLevel = src.MarginUsageLevel,
                OpenPositionsCount = src.OpenPositionsCount,
                PnL = src.PnL,
                StopOut = src.StopOut,
                TotalCapital = src.TotalCapital,
                TradingConditionId = src.TradingConditionId,
                UsedMargin = src.UsedMargin,
                WithdrawTransferLimit = src.WithdrawTransferLimit
            };
        }

        public static AccountsReport ToDto(this IAccountsReport src)
        {
            return new AccountsReport
            {
                BaseAssetId = src.BaseAssetId,
                Date = src.Date,
                Id = src.Id,
                IsLive = src.IsLive,
                TakerAccountId = src.TakerAccountId,
                TakerCounterpartyId = src.TakerCounterpartyId
            };
        }

        public static AccountsStatReport ToDto(this IAccountsStatReport src)
        {
            return new AccountsStatReport
            {
                AccountId = src.AccountId,
                Balance = src.Balance,
                BaseAssetId = src.BaseAssetId,
                ClientId = src.ClientId,
                Date = src.Date,
                FreeMargin = src.FreeMargin,
                Id = src.Id,
                IsLive = src.IsLive,
                MarginAvailable = src.MarginAvailable,
                MarginCall = src.MarginCall,
                MarginInit = src.MarginInit,
                MarginUsageLevel = src.MarginUsageLevel,
                OpenPositionsCount = src.OpenPositionsCount,
                PnL = src.PnL,
                StopOut = src.StopOut,
                TotalCapital = src.TotalCapital,
                TradingConditionId = src.TradingConditionId,
                UsedMargin = src.UsedMargin,
                WithdrawTransferLimit = src.WithdrawTransferLimit
            };
        }
        public static AccountTransactionsReport ToDto(this IAccountTransactionsReport src)
        {
            return new AccountTransactionsReport
            {
                AccountId = src.AccountId,
                Amount = src.Amount,
                Balance = src.Balance,
                ClientId = src.ClientId,
                Comment = src.Comment,
                Date = src.Date,
                Id = src.Id,
                PositionId = src.PositionId,
                Type = src.Type,
                WithdrawTransferLimit = src.WithdrawTransferLimit
            };
        }
    }
}
