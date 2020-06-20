namespace Quoter.Core
{
    using System;
    using System.Collections.Generic;
    using Quoter.Interfaces;

    internal class QuoteRepo : IQuoteRepo<IQuote>
    {
        private readonly IDictionary<Guid, IQuote> _quotes;
        private readonly IDictionary<string, SortedQuotes> _books;

        public QuoteRepo()
            : this(new Dictionary<Guid, IQuote>(), new Dictionary<string, SortedQuotes>())
        {
        }

        public QuoteRepo(IDictionary<Guid, IQuote> quotes, IDictionary<string, SortedQuotes> books)
        {
            _quotes = quotes;
            _books = books;
        }

        public void AddOrUpdate(IQuote quote)
        {
            if (_quotes.TryGetValue(quote.Id, out IQuote foundQuote))
            {
                foundQuote.Price = quote.Price;
                foundQuote.AvailableVolume = quote.AvailableVolume;
                foundQuote.ExpirationDate = quote.ExpirationDate;

                if (foundQuote.Symbol != quote.Symbol)
                {
                    _books[foundQuote.Symbol].Remove(foundQuote);

                    if (_books.TryGetValue(quote.Symbol, out var quotes))
                    {
                        quotes.Add(foundQuote);
                    }
                    else
                    {
                        _books.Add(quote.Symbol, new SortedQuotes() { foundQuote });
                    }

                    _books[foundQuote.Symbol].Remove(foundQuote);
                    foundQuote.Symbol = quote.Symbol;
                }
            }
            else
            {
                _quotes.Add(quote.Id, quote);

                if (_books.TryGetValue(quote.Symbol, out SortedQuotes list))
                {
                    list.Add(quote);
                }
                else
                {
                    _books.Add(quote.Symbol, new SortedQuotes() { quote });
                }
            }
        }

        public bool Remove(Guid id)
        {
            if (_quotes.TryGetValue(id, out IQuote quote))
            {
                _books[quote.Symbol].Remove(quote);
                return _quotes.Remove(id);
            }

            return false;
        }

        public bool Remove(string symbol)
        {
            if (_books.TryGetValue(symbol, out var book))
            {
                foreach (IQuote quote in book)
                {
                    _quotes.Remove(quote.Id);
                }

                book.Clear();
            }

            return _books.Remove(symbol);
        }

        public IEnumerable<IQuote> this[string symbol]
        {
            get
            {
                if (_books.TryGetValue(symbol, out var quotes))
                {
                    foreach (var quote in quotes)
                    {
                        yield return quote;
                    }
                }
            }
        }
    }
}
