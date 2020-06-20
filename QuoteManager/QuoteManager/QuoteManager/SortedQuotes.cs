namespace Quoter
{
    using System.Collections.Generic;
    using Quoter.Interfaces;

    internal class SortedQuotes : SortedSet<IQuote>
    {
        private static readonly IComparer<IQuote> _quoteSorter = new QuoteComparer();

        public SortedQuotes()
            : base(_quoteSorter)
        {
        }
    }
}
