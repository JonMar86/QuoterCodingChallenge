namespace Quoter
{
    using System.Collections.Generic;
    using Quoter.Interfaces;

    /// <summary>
    /// Comparer for use with sorting.
    /// Prefers lower prices before higher prices.
    /// If prices are equal, the quote with the greater available volume comes first.
    /// </summary>
    internal class QuoteComparer : IComparer<IQuote>
    {
        public int Compare(IQuote x, IQuote y)
        {
            const int bothAreEqual = 0;
            const int xIsBetter = -1;
            const int yIsBetter = 1;

            if (ReferenceEquals(x, y))
                return bothAreEqual;

            if (x.Price == y.Price)
            {
                if (x.AvailableVolume == y.AvailableVolume)
                    return bothAreEqual;

                if (x.AvailableVolume > y.AvailableVolume)
                    return xIsBetter;

                return yIsBetter;
            }

            if (x.Price < y.Price)
                return xIsBetter;

            return yIsBetter;
        }
    }
}
