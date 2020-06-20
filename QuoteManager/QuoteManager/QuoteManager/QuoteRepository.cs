namespace Quoter
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Metadata.Ecma335;
    using Quoter.Core;
    using Quoter.Interfaces;

    internal class QuoteRepository : IQuoteManager
    {
        private static readonly IComparer<IQuote> _quoteSorter = new QuoteComparer();

        private readonly IDictionary<Guid, IQuote> _quotes;
        private readonly IDictionary<string, SortedSet<IQuote>> _books;

        public QuoteRepository()
            : this(new Dictionary<Guid, IQuote>(),
                   new Dictionary<string, SortedSet<IQuote>>())
        {
        }

        public QuoteRepository(IDictionary<Guid, IQuote> quotes,
                               IDictionary<string, SortedSet<IQuote>> books)
        {
            _quotes = quotes;
            _books = books;
        }

        public void AddOrUpdateQuote(IQuote quote)
        {
            if (_quotes.TryGetValue(quote.Id, out IQuote foundQuote))
            {
                foundQuote.Price = quote.Price;
                foundQuote.AvailableVolume = quote.AvailableVolume;
                foundQuote.ExpirationDate = quote.ExpirationDate;

                if (foundQuote.Symbol != quote.Symbol)
                {
                    _books[foundQuote.Symbol].Remove(foundQuote);
                    _books[quote.Symbol].Add(foundQuote);
                    foundQuote.Symbol = quote.Symbol;
                }
            }
            else
            {
                _quotes.Add(quote.Id, quote);

                if (_books.TryGetValue(quote.Symbol, out SortedSet<IQuote> list))
                {
                    list.Add(quote);
                }
                else
                {
                    _books.Add(quote.Symbol, new SortedSet<IQuote>(_quoteSorter) { quote });
                }
            }
        }

        public void RemoveQuote(Guid id)
        {
            if (_quotes.TryGetValue(id, out IQuote quote))
            {
                _books[quote.Symbol].Remove(quote);
                _quotes.Remove(id);
            }
        }

        public void RemoveAllQuotes(string symbol)
        {
            if (_books.TryGetValue(symbol, out var book))
            {
                foreach (IQuote quote in book)
                {
                    _quotes.Remove(quote.Id);
                }

                book.Clear();
                _books.Remove(symbol);
            }
        }

        public IQuote GetBestQuoteWithAvailableVolume(string symbol)
        {
            if (_books.TryGetValue(symbol, out var book))
            {
                foreach (var quote in book)
                {
                    if (quote.AvailableVolume > 0 && quote.ExpirationDate > DateTime.Now)
                    {
                        return quote.Copy();
                    }
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

            if (_books.TryGetValue(symbol, out var book))
            {
                foreach (var quote in book)
                {
                    if (volumeRequested == 0)
                        break;

                    if (quote.AvailableVolume == 0 || quote.ExpirationDate < DateTime.Now)
                        continue;

                    uint volumedObtained = Math.Min(quote.AvailableVolume, volumeRequested);

                    tradeResults.VolumeExecuted += volumedObtained;
                    quote.AvailableVolume -= volumedObtained;

                    prices.Add(quote.Price);
                    obtained.Add(volumedObtained);
                }
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
