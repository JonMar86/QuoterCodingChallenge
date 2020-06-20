namespace Quoter.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IQuoteRepo<T>
        where T : IQuote
    {
        void AddOrUpdate(T quote);

        bool Remove(Guid id);

        bool Remove(string symbol);

        IEnumerable<T> this[string symbol] { get; }
    }
}
