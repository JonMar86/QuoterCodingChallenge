namespace Quoter.Interfaces
{
    using System;

    public interface IQuote
    {
        Guid Id { get; set; }

        string Symbol { get; set; }

        double Price { get; set; }

        uint AvailableVolume { get; set; }

        DateTime ExpirationDate { get; set; }
    }
}
