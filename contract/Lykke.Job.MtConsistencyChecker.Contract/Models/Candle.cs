using System;

namespace Lykke.Job.MtConsistencyChecker.Contract.Models
{
    public class Candle : ICandle
    {
        public DateTime DateTime { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double TradingVolume { get; set; }
        public double TradingOppositeVolume { get; set; }
        public double LastTradePrice { get; set; }
    }
}
