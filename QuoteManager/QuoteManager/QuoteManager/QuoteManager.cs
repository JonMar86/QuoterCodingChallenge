using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("QuoteManagerTests")]
namespace Quoter
{
    using System;
    using System.Collections.Generic;
    using Quoter.Core;
    using Quoter.Interfaces;

    internal class QuoteManager : IQuoteManager
    {
        private readonly IQuoteRepo<IQuote> _quoteRepo;

        public QuoteManager()
            : this(new QuoteRepo())
        {
        }

        public QuoteManager(IQuoteRepo<IQuote> quoteRepo)
        {
            _quoteRepo = quoteRepo;
        }

        public void AddOrUpdateQuote(IQuote quote)
        {
            _quoteRepo.AddOrUpdate(quote);
        }

        public void RemoveQuote(Guid id)
        {
            _quoteRepo.Remove(id);
        }

        public void RemoveAllQuotes(string symbol)
        {
            _quoteRepo.Remove(symbol);
        }

        public IQuote GetBestQuoteWithAvailableVolume(string symbol)
        {
            // The book list is already sorted by this time
            foreach (IQuote quote in _quoteRepo[symbol])
            {
                if (quote.AvailableVolume > 0 && quote.ExpirationDate > DateTime.Now)
                {
                    return quote.Copy();
                }
            }

            return null;
        }

        public ITradeResult ExecuteTrade(string symbol, uint volumeRequested)
        {
            var tradeResults = new TradeResult
            {
                Id = Guid.NewGuid(),
                Symbol = symbol,
                VolumeExecuted = 0,
                VolumeRequested = volumeRequested,
                VolumeWeightedAveragePrice = 0d
            };

            var prices = new List<double>();
            var obtained = new List<uint>();

            foreach (var quote in _quoteRepo[symbol])
            {
                if (tradeResults.VolumeExecuted == volumeRequested)
                    break;

                if (quote.AvailableVolume == 0 || quote.ExpirationDate < DateTime.Now)
                    continue;

                uint volumedObtained = Math.Min(quote.AvailableVolume, volumeRequested);

                tradeResults.VolumeExecuted += volumedObtained;
                quote.AvailableVolume -= volumedObtained;

                prices.Add(quote.Price);
                obtained.Add(volumedObtained);
            }

            if (tradeResults.VolumeExecuted > 0)
            {
                double weightedTotal = 0d;

                for (int i = 0; i < prices.Count; i++)
                {
                    weightedTotal += (prices[i] * obtained[i]) / tradeResults.VolumeExecuted;
                }

                tradeResults.VolumeWeightedAveragePrice = weightedTotal;
            }

            return tradeResults;
        }
    }
}
