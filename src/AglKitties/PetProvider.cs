using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AglKitties
{
	public class PetProvider
	{
		readonly IPetOwnerSource _ownerSrc;

		public PetProvider(IPetOwnerSource ownerSrc)
		{
			_ownerSrc = ownerSrc;
		}

		public async Task<Dictionary<Gender, List<string>>> GetCatNamesByOwnerGenderAsync()
		{
            var owners = await _ownerSrc.GetOwnersAsync();

            return owners
				.Where(owner => owner.Pets != null && owner.Pets.Any(p => p.Type == PetType.Cat))
                .GroupBy(owner => owner.Gender)
                .ToDictionary(g => g.Key, g => g
					.SelectMany(owner => owner.Pets
                        .Where(pet => pet.Type == PetType.Cat)
                        .Select(cat => cat.Name))
					.OrderBy(catName => catName)
					.ToList());
		}
	}
}
