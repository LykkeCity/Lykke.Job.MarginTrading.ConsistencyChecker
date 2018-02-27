﻿using System;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract.Models
{
    public class CheckResult : ICheckResult
    {
        public DateTime Date { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string Comments { get; set; }
    }
}
