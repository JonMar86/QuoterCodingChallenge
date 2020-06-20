namespace QuoteManagerTests
{
    using NUnit.Framework;
    using Quoter;
    using Quoter.Interfaces;
    using NSubstitute;
    using Quoter.Core;
    using System;
    using System.Collections.Generic;

    [TestFixture]
    internal class QuoteManagerTests
    {
        private IQuoteRepo<IQuote> _quoteRepoMock;
        private QuoteManager _quoteManager;

        [SetUp]
        public void InitializeFixture()
        {
            _quoteRepoMock = Substitute.For<IQuoteRepo<IQuote>>();
            _quoteManager = new QuoteManager(_quoteRepoMock);
        }

        [Test]
        public void AddOrUpdateQuote_QuoteReceived()
        {
            // Arrange
            var enteredQuote = new Quote();

            // Act
            _quoteManager.AddOrUpdateQuote(enteredQuote);

            // Assert            
            _quoteRepoMock.Received(1).AddOrUpdate(enteredQuote);
        }

        [Test]
        public void RemoveQuote_GuidReceived()
        {
            // Arrange
            var enteredId = Guid.NewGuid();

            // Act
            _quoteManager.RemoveQuote(enteredId);

            // Assert            
            _quoteRepoMock.Received(1).Remove(enteredId);
        }

        [Test]
        public void RemoveAllQuotes_SymbolReceived()
        {
            // Arrange
            var enteredSymbol = "TestSymbol";

            // Act
            _quoteManager.RemoveAllQuotes(enteredSymbol);

            // Assert            
            _quoteRepoMock.Received(1).Remove(enteredSymbol);
        }

        [Test]
        public void GetBestQuoteWithAvailableVolume_ReturnsFirstAvailableQuote()
        {
            // Arrange
            const string symbol = "TestSymbol";
            var expectedQuote = new Quote(symbol, 6.78d, 1234, DateTime.Now.AddDays(2));
            var expiredQuote = new Quote(symbol, 5.67d, 123, DateTime.Now.AddDays(-2));
            var emptyVolumeQuote = new Quote(symbol, 3.45d, 0, DateTime.Now.AddDays(2));

            var quotes = new List<IQuote>
            {
                emptyVolumeQuote,
                expiredQuote,
                expectedQuote
            };

            _quoteRepoMock[symbol].Returns(quotes);

            // Act
            var obtainedQuote = _quoteManager.GetBestQuoteWithAvailableVolume(symbol);

            // Assert
            Assert.IsNotNull(obtainedQuote);
            Assert.AreEqual(expectedQuote.Id, obtainedQuote.Id);
            Assert.AreEqual(expectedQuote.AvailableVolume, obtainedQuote.AvailableVolume);
            Assert.AreEqual(expectedQuote.ExpirationDate, obtainedQuote.ExpirationDate);
            Assert.AreEqual(expectedQuote.Price, obtainedQuote.Price);
            Assert.AreEqual(expectedQuote.Symbol, obtainedQuote.Symbol);
            Assert.AreNotSame(expectedQuote, obtainedQuote);
        }

        [Test]
        public void ExecuteTrade_WhenMetWithOneQuote_JustEnoughVolume_ReturnsTradeResult()
        {
            // Arrange
            const string symbol = "TestSymbol";
            const uint volumeRequested = 100;
            const uint uneededVolume = 500;
            const double price = 10.00d;
            const double expectedWeightedPrice = 10.00d;// (price * volumeRequested) / volumeRequested

            var sufficientQuote = new Quote(symbol, price, volumeRequested, DateTime.Now.AddDays(1));
            var uneededQuote = new Quote(symbol, price + 0.15d, uneededVolume, DateTime.Now.AddDays(2));

            var quotes = new List<IQuote>
            {
                sufficientQuote,
                uneededQuote
            };

            _quoteRepoMock[symbol].Returns(quotes);

            // Act
            var tradeResult = _quoteManager.ExecuteTrade(symbol, volumeRequested);

            // Assert
            Assert.IsNotNull(tradeResult);
            Assert.AreNotEqual(Guid.Empty, tradeResult.Id);
            Assert.AreEqual(symbol, tradeResult.Symbol);
            Assert.AreEqual(volumeRequested, tradeResult.VolumeRequested);
            Assert.AreEqual(volumeRequested, tradeResult.VolumeExecuted);
            Assert.AreEqual(expectedWeightedPrice, tradeResult.VolumeWeightedAveragePrice);
            Assert.AreEqual(0, sufficientQuote.AvailableVolume);
            Assert.AreEqual(uneededVolume, uneededQuote.AvailableVolume);
        }

        [Test]
        public void ExecuteTrade_CannotBeMetFully_ObtainsAsMuchAsPossible_ReturnsTradeResult()
        {
            // Arrange
            const string symbol = "TestSymbol";
            const uint volumeRequested = 100;
            const uint volumeAvailable = 50;
            const double price = 10.00d;
            const double expectedWeightedPrice = 10.00d;// (price * volumnAvailable) / volumnAvailable

            var availableQuote = new Quote(symbol, price, volumeAvailable, DateTime.Now.AddDays(1));
            var expiredQuote = new Quote(symbol, 5.67d, 123, DateTime.Now.AddDays(-2));
            var emptyVolumeQuote = new Quote(symbol, 3.45d, 0, DateTime.Now.AddDays(2));

            var quotes = new List<IQuote>
            {
                expiredQuote, // will be ignored
                emptyVolumeQuote, // will be ignored
                availableQuote
            };

            _quoteRepoMock[symbol].Returns(quotes);

            // Act
            var tradeResult = _quoteManager.ExecuteTrade(symbol, volumeRequested);

            // Assert
            Assert.IsNotNull(tradeResult);
            Assert.AreNotEqual(Guid.Empty, tradeResult.Id);
            Assert.AreEqual(symbol, tradeResult.Symbol);
            Assert.AreEqual(volumeRequested, tradeResult.VolumeRequested);
            Assert.AreEqual(volumeAvailable, tradeResult.VolumeExecuted);
            Assert.AreEqual(expectedWeightedPrice, tradeResult.VolumeWeightedAveragePrice);
            Assert.AreEqual(0, availableQuote.AvailableVolume);
        }

        [Test]
        public void ExecuteTrade_MetUsingMoreThanOneQuote_ReturnsTradeResult()
        {
            // Arrange
            const string symbol = "TestSymbol";
            const uint partialVolume = 50;
            const uint volumeRequested = partialVolume * 2;
            const int unneededVolume = 80;
            const double bestPrice = 40d;
            const double secondBestPrice = 60d;
            const double expectedWeightedPrice = 50.00d;// ((bestPrice * partialVolume)+(secondBestPrice * partialVolume)) / volumnAvailable

            var bestQuote = new Quote(symbol, bestPrice, partialVolume, DateTime.Now.AddDays(1));            
            var secondBestQuote = new Quote(symbol, secondBestPrice, partialVolume, DateTime.Now.AddDays(2));
            var uneededQuote = new Quote(symbol, 70d, unneededVolume, DateTime.Now.AddDays(3));

            var quotes = new List<IQuote>
            {
                bestQuote,
                secondBestQuote,
                uneededQuote // will be ignored
            };

            _quoteRepoMock[symbol].Returns(quotes);

            // Act
            var tradeResult = _quoteManager.ExecuteTrade(symbol, volumeRequested);

            Assert.IsNotNull(tradeResult);
            Assert.AreNotEqual(Guid.Empty, tradeResult.Id);
            Assert.AreEqual(symbol, tradeResult.Symbol);
            Assert.AreEqual(volumeRequested, tradeResult.VolumeRequested);
            Assert.AreEqual(volumeRequested, tradeResult.VolumeExecuted);
            Assert.AreEqual(expectedWeightedPrice, tradeResult.VolumeWeightedAveragePrice);

            Assert.AreEqual(0, bestQuote.AvailableVolume);
            Assert.AreEqual(0, secondBestQuote.AvailableVolume);
            Assert.AreEqual(unneededVolume, uneededQuote.AvailableVolume);
        }

        [Test]
        public void ExecuteTrade_UnableToMeetAny_ReturnsTradeResult()
        {
            const string symbol = "TestSymbol";
            const uint volumeRequested = 100;

            var expiredQuote = new Quote(symbol, 5.67d, 123, DateTime.Now.AddDays(-2));
            var emptyVolumeQuote = new Quote(symbol, 3.45d, 0, DateTime.Now.AddDays(2));

            var quotes = new List<IQuote>
            {
                expiredQuote, // will be ignored
                emptyVolumeQuote // will be ignored
            };

            _quoteRepoMock[symbol].Returns(quotes);

            // Act
            var tradeResult = _quoteManager.ExecuteTrade(symbol, volumeRequested);

            Assert.IsNotNull(tradeResult);
            Assert.AreNotEqual(Guid.Empty, tradeResult.Id);
            Assert.AreEqual(symbol, tradeResult.Symbol);
            Assert.AreEqual(volumeRequested, tradeResult.VolumeRequested);
            Assert.AreEqual(0, tradeResult.VolumeExecuted);
            Assert.AreEqual(0d, tradeResult.VolumeWeightedAveragePrice);
        }
    }
}
