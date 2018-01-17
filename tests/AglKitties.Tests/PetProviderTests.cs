using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace AglKitties.Tests
{
	public class PetProviderTests
	{
		readonly ILogger _log;
		readonly Mock<IPetOwnerSource> _ownerSrc;
		private readonly PetProvider _petProvider;

		public PetProviderTests(ITestOutputHelper output)
		{
			_log = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.WriteTo.TestOutput(output)
				.CreateLogger();

			_ownerSrc = new Mock<IPetOwnerSource>(MockBehavior.Strict);
			_petProvider = new PetProvider(_ownerSrc.Object);
		}

		[Fact]
		public async Task GivenNoOwners_WhenGetCatNamesByOwnerGender_ThenReturnsEmptyDictionary()
		{
			_ownerSrc.Setup(p => p.GetOwnersAsync()).ReturnsAsync(new List<PetOwner>());
			Assert.Empty(await _petProvider.GetCatNamesByOwnerGenderAsync());
		}

		[Fact]
		public async Task GivenOwnerWithPetsOtherThanCats_WhenGetCatNamesByOwnerGender_ThenGenderExcludedFromResults()
		{
			_ownerSrc.Setup(p => p.GetOwnersAsync()).ReturnsAsync(new List<PetOwner>
			{
				new PetOwner
				{
					Gender = Gender.Male,
					Pets = new List<Pet>
					{
						new Pet
						{
							Type = PetType.Dog,
							Name = "foo"
						},
						new Pet
						{
							Type = PetType.Fish,
							Name = "bar"
						}
					}
				}
			});

			Assert.Empty(await _petProvider.GetCatNamesByOwnerGenderAsync());
		}

		[Fact]
		public async Task GivenCatOwnersOfDifferentGenders_WhenGetCatNamesByOwnerGender_ThenCatNamesAreGroupedByOwnerGender()
		{
			_ownerSrc.Setup(p => p.GetOwnersAsync()).ReturnsAsync(new List<PetOwner>
			{
				new PetOwner
				{
					Gender = Gender.Male,
					Pets = new List<Pet>
					{
						new Pet
						{
							Type = PetType.Cat,
							Name = "foo"
						}
					}
				},
				new PetOwner
				{
					Gender = Gender.Female,
					Pets = new List<Pet>
					{
						new Pet
						{
							Type = PetType.Cat,
							Name = "bar"
						}
					}
				}
			});

			var catNamesByOwnerGender = await _petProvider.GetCatNamesByOwnerGenderAsync();
			Assert.Equal(2, catNamesByOwnerGender.Count);

			Assert.Collection(catNamesByOwnerGender[Gender.Male],
				catName => Assert.Equal("foo", catName));

			Assert.Collection(catNamesByOwnerGender[Gender.Female],
				catName => Assert.Equal("bar", catName));
		}

		[Fact]
		public async Task GivenOwnersWithCatsAndOtherPets_WhenGetCatNamesByOwnerGender_ThenOnlyCatNamesReturned()
		{
			_ownerSrc.Setup(p => p.GetOwnersAsync()).ReturnsAsync(new List<PetOwner>
			{
				new PetOwner
				{
					Gender = Gender.Male,
					Pets = new List<Pet>
					{
						new Pet
						{
							Type = PetType.Dog,
							Name = "foo"
						},
						new Pet
						{
							Type = PetType.Cat,
							Name = "bar"
						}
					}
				}
			});

			Assert.Collection(await _petProvider.GetCatNamesByOwnerGenderAsync(), c =>
			{
				Assert.Equal(Gender.Male, c.Key);
				Assert.Collection(c.Value, catName => Assert.Equal("bar", catName));
			});
		}

		[Fact]
		public async Task GivenCatNamesOutOfOrder_WhenGetCatNamesByOwnerGender_ThenCatNamesAreOrdered()
		{
			_ownerSrc.Setup(p => p.GetOwnersAsync()).ReturnsAsync(new List<PetOwner>
			{
				new PetOwner
				{
					Gender = Gender.Male,
					Pets = new List<Pet>
					{
						new Pet
						{
							Type = PetType.Cat,
							Name = "beta"
						},
						new Pet
						{
							Type = PetType.Cat,
							Name = "gamma"
						}
					}
				},
				new PetOwner
				{
					Gender = Gender.Male,
					Pets = new List<Pet>
					{
						new Pet
						{
							Type = PetType.Cat,
							Name = "alpha"
						},
						new Pet
						{
							Type = PetType.Cat,
							Name = "omega"
						}
					}
				}
			});

			Assert.Collection(await _petProvider.GetCatNamesByOwnerGenderAsync(), c =>
			{
				Assert.Equal(Gender.Male, c.Key);
				Assert.Collection(c.Value,
					catName => Assert.Equal("alpha", catName),
					catName => Assert.Equal("beta", catName),
					catName => Assert.Equal("gamma", catName),
					catName => Assert.Equal("omega", catName));
			});
		}
	}
}