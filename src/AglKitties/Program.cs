using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;

namespace AglKitties
{
	class Program
    {
        static async Task Main(string[] args)
        {
            var log = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            if(args.Length != 1)
            {
                log.Error($"Usage: {AppDomain.CurrentDomain.FriendlyName} <people.json url>");
                return;
            }

            var peopleUrl = new Uri(args[0]);

            var ownerSrc = new HttpPetOwnerSource(peopleUrl, new HttpClientHandler(), log);
            var petProvider = new PetProvider(ownerSrc);

            Dictionary<Gender, List<string>> catNamesByOwnerGender = await petProvider.GetCatNamesByOwnerGenderAsync();

            log.Information("All cats grouped by owner gender: {cats}", catNamesByOwnerGender);
        }
    }
}
