namespace Quoter.Core
{
    using System;
    using Interfaces;
    using Newtonsoft.Json;

    [JsonObject]
    internal class Quote : IQuote
    {
        [JsonProperty]
        public Guid Id { get; set; }

        [JsonProperty]
        public string Symbol { get; set; }

        [JsonProperty]
        public double Price { get; set; }

        [JsonProperty]
        public uint AvailableVolume { get; set; }

        [JsonProperty]
        public DateTime ExpirationDate { get; set; }

        public Quote()
        {
        }

        public Quote(string symbol, double price, uint availableVolume, DateTime expirationDate)
            : this(Guid.NewGuid(), symbol, price, availableVolume, expirationDate)
        {
        }

        public Quote(Guid id, string symbol, double price, uint availableVolume, DateTime expirationDate)
        {
            this.Id = id;
            this.Symbol = symbol;
            this.Price = price;
            this.AvailableVolume = availableVolume;
            this.ExpirationDate = expirationDate;
        }
    }
}
