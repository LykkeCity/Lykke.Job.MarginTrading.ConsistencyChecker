﻿using System;

namespace Lykke.Job.MarginTrading.ConsistencyChecker.Contract
{
    public interface ICandle
    {
        DateTime DateTime { get; }
        double Open { get; }
        double Close { get; }
        double High { get; }
        double Low { get; }
        double TradingVolume { get; }
        double TradingOppositeVolume { get; }
        double LastTradePrice { get; }
    }
}
