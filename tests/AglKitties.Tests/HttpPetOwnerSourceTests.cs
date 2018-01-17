using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace AglKitties.Tests
{
	public class HttpPetOwnerSourceTests
	{
		readonly ILogger _log;

		public HttpPetOwnerSourceTests(ITestOutputHelper output)
		{
			_log = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.WriteTo.TestOutput(output)
				.CreateLogger();

		}

		[Fact]
		public async Task GivenEmptyJsonAtUrl_WhenGetJson_ThenReturnsEmptyList()
		{
			// Arrange
			var httpHandler = new OwnersHttpHandler(Enumerable.Empty<PetOwner>());
			var ownerSrc = new HttpPetOwnerSource(new Uri("http://anything"), httpHandler, _log);

			// Act
			var actualOwners = await ownerSrc.GetOwnersAsync();

			// Assert
			Assert.Empty(actualOwners);
		}

		[Fact]
		public async Task GivenJsonWithSomeOwnersAtUrl_WhenGetJson_ThenReturnsListWithSameOwners()
		{
			// Arrange
            var expectedOwners = new List<PetOwner>
            {
                new PetOwner
                {
                    Name = "IHasNoPets",
                    Age = 22,
                    Gender = Gender.Female,
                },
                new PetOwner
                {
                    Name = "Foo",
                    Age = 100,
                    Gender = Gender.Male,
                    Pets = new List<Pet>
                    {
                        new Pet
                        {
                            Type = PetType.Dog,
                            Name = "dawg"
                        },
                        new Pet
                        {
                            Type = PetType.Fish,
                            Name = "Pepper"
                        }
                    }
                }
            };
			var httpHandler = new OwnersHttpHandler(expectedOwners);
			var ownerSrc = new HttpPetOwnerSource(new Uri("http://anything"), httpHandler, _log);

			// Act
			var actualOwners = await ownerSrc.GetOwnersAsync();

			// Assert
            Assert.Equal(expectedOwners, actualOwners, new PetOwnerComparer());
		}
	}
}