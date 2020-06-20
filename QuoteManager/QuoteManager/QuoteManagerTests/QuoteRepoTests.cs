namespace QuoteManagerTests
{
    using NUnit.Framework;
    using Quoter;
    using Quoter.Interfaces;
    using Quoter.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    internal class QuoteRepoTests
    {
        private Dictionary<Guid, IQuote> _quotes;
        private Dictionary<string, SortedQuotes> _books;
        private QuoteRepo _quoteRepo;

        [SetUp]
        public void InitializeFixture()
        {
            _quotes = new Dictionary<Guid, IQuote>();
            _books = new Dictionary<string, SortedQuotes>();
            _quoteRepo = new QuoteRepo(_quotes, _books);
        }

        [Test]
        public void AddOrUpdate_WhenNewQuoteNewBook_Adds()
        {
            // Arrange
            var quote = new Quote("TestSymbol", 2d, 20, DateTime.Now.AddDays(2));

            // Act
            _quoteRepo.AddOrUpdate(quote);

            // Assert
            Assert.AreEqual(1, _quotes.Count);
            Assert.IsTrue(_quotes.ContainsKey(quote.Id));
            Assert.AreSame(quote, _quotes[quote.Id]);

            Assert.AreEqual(1, _books.Count);
            Assert.IsTrue(_books.ContainsKey(quote.Symbol));
            Assert.AreSame(quote, _books[quote.Symbol].First());
        }

        [Test]
        public void AddOrUpdate_WhenNewQuoteExistingBook_BetterPrice_AddsFirst()
        {
            // Arrange
            const string symbol = "TestSymbol";

            var existingQuote = new Quote(symbol, 3d, 20, DateTime.Now.AddDays(1));

            _quotes.Add(existingQuote.Id, existingQuote);
            _books.Add(symbol, new SortedQuotes { existingQuote });

            var quote = new Quote(symbol, 2d, 20, DateTime.Now.AddDays(2));

            // Act
            _quoteRepo.AddOrUpdate(quote);

            // Assert
            Assert.AreEqual(2, _quotes.Count);
            Assert.IsTrue(_quotes.ContainsKey(quote.Id));
            Assert.AreSame(quote, _quotes[quote.Id]);

            Assert.AreEqual(1, _books.Count);
            Assert.IsTrue(_books.ContainsKey(quote.Symbol));
            Assert.AreSame(quote, _books[quote.Symbol].First());
        }

        [Test]
        public void AddOrUpdate_WhenNewQuoteExistingBook_WorsePrice_AddsLast()
        {
            // Arrange
            const string symbol = "TestSymbol";

            var existingQuote = new Quote(symbol, 2d, 20, DateTime.Now.AddDays(1));

            _quotes.Add(existingQuote.Id, existingQuote);
            _books.Add(symbol, new SortedQuotes { existingQuote });

            var quote = new Quote(symbol, 3d, 20, DateTime.Now.AddDays(2));

            // Act
            _quoteRepo.AddOrUpdate(quote);

            // Assert
            Assert.AreEqual(2, _quotes.Count);
            Assert.IsTrue(_quotes.ContainsKey(quote.Id));
            Assert.AreSame(quote, _quotes[quote.Id]);

            Assert.AreEqual(1, _books.Count);
            Assert.IsTrue(_books.ContainsKey(quote.Symbol));
            Assert.AreSame(quote, _books[quote.Symbol].Last());
        }

        [Test]
        public void AddOrUpdate_WhenUpdatingExistingQuote_SameBook_UpdatesValues()
        {
            // Arrange
            const string symbol = "TestSymbol";

            var existingQuote = new Quote(symbol, 3d, 20, DateTime.Now.AddDays(1));

            _quotes.Add(existingQuote.Id, existingQuote);
            _books.Add(symbol, new SortedQuotes { existingQuote });

            var updatingQuote = new Quote(existingQuote.Id, symbol, 2d, 25, DateTime.Now.AddDays(2));

            // Act
            _quoteRepo.AddOrUpdate(updatingQuote);

            // Assert
            Assert.AreEqual(1, _quotes.Count);
            Assert.IsTrue(_quotes.ContainsKey(updatingQuote.Id));
            Assert.AreSame(existingQuote, _quotes[updatingQuote.Id]);

            Assert.AreEqual(1, _books.Count);
            Assert.IsTrue(_books.ContainsKey(updatingQuote.Symbol));
            Assert.AreSame(existingQuote, _books[updatingQuote.Symbol].First());

            Assert.AreEqual(updatingQuote.Price, existingQuote.Price);
            Assert.AreEqual(updatingQuote.Symbol, existingQuote.Symbol);
            Assert.AreEqual(updatingQuote.AvailableVolume, existingQuote.AvailableVolume);
            Assert.AreEqual(updatingQuote.ExpirationDate, existingQuote.ExpirationDate);
        }

        [Test]
        public void AddOrUpdate_WhenUpdatingExistingQuote_ChangingBook_UpdatesValues()
        {
            // Arrange
            var existingQuote = new Quote("TestSymbol1", 3d, 20, DateTime.Now.AddDays(1));

            _quotes.Add(existingQuote.Id, existingQuote);
            _books.Add("TestSymbol1", new SortedQuotes { existingQuote });

            var updatingQuote = new Quote(existingQuote.Id, "TestSymbol2", 2d, 25, DateTime.Now.AddDays(2));

            // Act
            _quoteRepo.AddOrUpdate(updatingQuote);

            // Assert
            Assert.AreEqual(1, _quotes.Count);
            Assert.IsTrue(_quotes.ContainsKey(updatingQuote.Id));
            Assert.AreSame(existingQuote, _quotes[updatingQuote.Id]);

            Assert.AreEqual(2, _books.Count);
            Assert.IsTrue(_books.ContainsKey(updatingQuote.Symbol));
            Assert.AreSame(existingQuote, _books["TestSymbol2"].First());

            Assert.AreEqual(updatingQuote.Price, existingQuote.Price);
            Assert.AreEqual(updatingQuote.Symbol, existingQuote.Symbol);
            Assert.AreEqual(updatingQuote.AvailableVolume, existingQuote.AvailableVolume);
            Assert.AreEqual(updatingQuote.ExpirationDate, existingQuote.ExpirationDate);
        }
    }
}
