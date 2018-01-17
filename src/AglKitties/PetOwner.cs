using System.Collections.Generic;

namespace AglKitties
{
	public class PetOwner
    {
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public int Age { get; set; }
        public List<Pet> Pets { get; set; }
    }
}
