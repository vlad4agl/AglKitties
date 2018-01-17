using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AglKitties.Tests
{
	/// <remarks>
	/// Could have used Moq, but setting up protected SendAsync requires quite a bit more code..
	/// Alternatively, could have wrapped HttpClient, but that also would have been lengthier
	/// </remarks>
	internal class OwnersHttpHandler : HttpMessageHandler
	{
		private readonly IEnumerable<PetOwner> _owners;

		public OwnersHttpHandler(IEnumerable<PetOwner> owners)
		{
			_owners = owners;
		}

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return Task.FromResult(new HttpResponseMessage()
			{
				Content = new StringContent(JsonConvert.SerializeObject(_owners), Encoding.UTF8, "application/json")
			});
		}
	}
}