﻿namespace Quoter.Core
{
    using System;
    using Interfaces;

    internal class TradeResult : ITradeResult
    {
        public Guid Id { get; set; }

        public string Symbol { get; set; }

        public double VolumeWeightedAveragePrice { get; set; }

        public uint VolumeRequested { get; set; }

        public uint VolumeExecuted { get; set; }
    }
}
