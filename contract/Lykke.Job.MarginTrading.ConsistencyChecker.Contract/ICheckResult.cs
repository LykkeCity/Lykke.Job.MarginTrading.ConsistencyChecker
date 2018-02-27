using System;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract
{
    public interface ICheckResult
    {
        DateTime Date { get; }
        DateTime DateFrom { get; }
        DateTime DateTo { get; }
        string Comments { get; }
    }
}
