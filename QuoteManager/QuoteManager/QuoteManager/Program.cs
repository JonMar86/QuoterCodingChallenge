namespace Quoter
{
    using System;
    using Newtonsoft.Json;
    using Quoter.Core;
    using Quoter.Interfaces;

    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var quoteMgr = new QuoteManager();

                string command = args[0];
                switch (command)
                {
                    case nameof(IQuoteManager.AddOrUpdateQuote):
                        if (args.Length == 2)
                        {
                            try
                            {
                                Quote receivedQuote = JsonConvert.DeserializeObject<Quote>(args[1]);
                                quoteMgr.AddOrUpdateQuote(receivedQuote);
                                return;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error: " + ex.Message);
                            }
                        }
                        break;
                    case nameof(IQuoteManager.RemoveQuote):
                        if (args.Length == 2)
                        {
                            try
                            {
                                var receivedId = Guid.Parse(args[1]);
                                quoteMgr.RemoveQuote(receivedId);
                                return;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error: " + ex.Message);
                            }
                        }
                        break;
                    case nameof(IQuoteManager.RemoveAllQuotes):
                        if (args.Length == 2)
                        {
                            quoteMgr.RemoveAllQuotes(args[1]);
                            return;
                        }
                        break;
                    case nameof(IQuoteManager.GetBestQuoteWithAvailableVolume):
                        if (args.Length == 2)
                        {
                            var quote = quoteMgr.GetBestQuoteWithAvailableVolume(args[1]);
                            Console.WriteLine(JsonConvert.SerializeObject(quote));
                            return;
                        }
                        break;
                    case nameof(IQuoteManager.ExecuteTrade):
                        if (args.Length == 3)
                        {
                            if (uint.TryParse(args[2], out uint volumeRequested))
                            {
                                var trade = quoteMgr.ExecuteTrade(args[1], volumeRequested);
                                Console.WriteLine(JsonConvert.SerializeObject(trade));
                            }
                            else
                            {
                                Console.WriteLine("Error: Invalid volume requested.");
                                return;
                            }
                        }
                        break;
                    default:
                        Console.WriteLine($"Error: '{command}' is not a recognized command.");
                        break;
                }
            }
            else
            {
                Console.WriteLine($"Error: Missing arguments");
            }

            Console.WriteLine($"Error: Invalid number of arguments.");
            return;
        }
    }
}
