﻿using System;

namespace Lykke.Job.MtConsistencyChecker.Contract
{
    public interface IAccountTransactionsReport
    {
        string AccountId { get; }
        double Amount { get; }
        double Balance { get; }
        string ClientId { get; }
        string Comment { get; }
        DateTime Date { get; }
        string Id { get; }
        string PositionId { get; }
        string Type { get; }
        double WithdrawTransferLimit { get; }
    }
}
