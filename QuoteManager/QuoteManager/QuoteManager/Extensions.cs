namespace Quoter
{
    using Quoter.Core;
    using Quoter.Interfaces;

    internal static class Extensions
    {
        public static IQuote Copy(this IQuote original)
        {
            return new Quote
            {
                Id = original.Id,
                Symbol = original.Symbol,
                Price = original.Price,
                AvailableVolume = original.AvailableVolume,
                ExpirationDate = original.ExpirationDate
            };
        }
    }
}
