using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;

namespace AglKitties
{
	public class HttpPetOwnerSource : IPetOwnerSource
	{
		readonly Uri _peopleUrl;
		readonly HttpClient _client;
		readonly ILogger _log;

		public HttpPetOwnerSource(Uri peopleUrl, HttpMessageHandler handler, ILogger log)
		{
			_peopleUrl = peopleUrl;
			_client = new HttpClient(handler);
			_log = log;
		}

		public async Task<List<PetOwner>> GetOwnersAsync()
		{
			_log.Information("Retrieving a list of pet owners from {peopleUrl}", _peopleUrl);
			string ownersJson = await _client.GetStringAsync(_peopleUrl);

			_log.Debug("Retrieved pet owners json: {ownersJson}", ownersJson);
			var owners = JsonConvert.DeserializeObject<List<PetOwner>>(ownersJson);
			_log.Debug("Parsed {ownersCount} pet owners", owners.Count);

			return owners;
		}
	}
}