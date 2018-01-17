using System.Collections.Generic;
using System.Threading.Tasks;

namespace AglKitties
{
	public interface IPetOwnerSource
	{
		Task<List<PetOwner>> GetOwnersAsync();
	}
}
