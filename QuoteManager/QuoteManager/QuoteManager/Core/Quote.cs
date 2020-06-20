namespace Quoter.Core
{
    using System;
    using Interfaces;

    internal class Quote : IQuote
    {
        public Guid Id { get; set; }

        public string Symbol { get; set; }

        public double Price { get; set; }

        public uint AvailableVolume { get; set; }

        public DateTime ExpirationDate { get; set; }

        public Quote()
        {
        }

        public Quote(string symbol, double price, uint availableVolume, DateTime expirationDate)
        {
            this.Id = Guid.NewGuid();
            this.Symbol = symbol;
            this.Price = price;
            this.AvailableVolume = availableVolume;
            this.ExpirationDate = expirationDate;
        }
    }
}
