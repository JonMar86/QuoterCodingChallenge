namespace QuoteManagerTests
{
    using NUnit.Framework;
    using Quoter;
    using Quoter.Core;
    using System;

    [TestFixture]
    internal class QuoteComparerTests
    {
        [Test]
        public void Compare_SameObject_ReturnsEqual()
        {
            // Arrange
            var quote = new Quote("TestSymbol", 5d, 10, DateTime.Now.AddDays(1));
            var comparer = new QuoteComparer();

            // Act
            int result = comparer.Compare(quote, quote);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Compare_SamePrice_SameVolume_ReturnsEqual()
        {
            // Arrange
            var quoteA = new Quote("TestSymbol", 5d, 10, DateTime.Now.AddDays(1));
            var quoteB = new Quote("TestSymbol", 5d, 10, DateTime.Now.AddDays(1));
            var comparer = new QuoteComparer();

            // Act
            int result = comparer.Compare(quoteA, quoteB);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Compare_BetterPrice_ReturnsOrdered()
        {
            // Arrange
            var quoteA = new Quote("TestSymbol", 2d, 10, DateTime.Now.AddDays(1));
            var quoteB = new Quote("TestSymbol", 5d, 10, DateTime.Now.AddDays(1));
            var comparer = new QuoteComparer();

            // Act
            int result = comparer.Compare(quoteA, quoteB);

            // Assert
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void Compare_WorsePrice_ReturnsUnordered()
        {
            // Arrange
            var quoteA = new Quote("TestSymbol", 5d, 10, DateTime.Now.AddDays(1));
            var quoteB = new Quote("TestSymbol", 2d, 10, DateTime.Now.AddDays(1));
            var comparer = new QuoteComparer();

            // Act
            int result = comparer.Compare(quoteA, quoteB);

            // Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void Compare_SamePrice_HigherVolume_ReturnsOrdered()
        {
            // Arrange
            var quoteA = new Quote("TestSymbol", 5d, 20, DateTime.Now.AddDays(1));
            var quoteB = new Quote("TestSymbol", 5d, 10, DateTime.Now.AddDays(1));
            var comparer = new QuoteComparer();

            // Act
            int result = comparer.Compare(quoteA, quoteB);

            // Assert
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void Compare_SamePrice_LowerVolume_ReturnsUnordered()
        {
            // Arrange
            var quoteA = new Quote("TestSymbol", 5d, 10, DateTime.Now.AddDays(1));
            var quoteB = new Quote("TestSymbol", 5d, 20, DateTime.Now.AddDays(1));
            var comparer = new QuoteComparer();

            // Act
            int result = comparer.Compare(quoteA, quoteB);

            // Assert
            Assert.AreEqual(1, result);
        }
    }
}
