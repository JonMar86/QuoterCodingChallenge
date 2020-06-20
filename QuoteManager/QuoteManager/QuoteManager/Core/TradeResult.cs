namespace Quoter.Core
{
    using System;
    using Interfaces;
    using Newtonsoft.Json;

    [JsonObject]
    internal class TradeResult : ITradeResult
    {
        [JsonProperty]
        public Guid Id { get; set; }

        [JsonProperty]
        public string Symbol { get; set; }

        [JsonProperty]
        public double VolumeWeightedAveragePrice { get; set; }

        [JsonProperty]
        public uint VolumeRequested { get; set; }

        [JsonProperty]
        public uint VolumeExecuted { get; set; }
    }
}
