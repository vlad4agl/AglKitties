using System;
using System.Collections.Generic;
using System.Linq;

namespace AglKitties.Tests
{
	internal class PetOwnerComparer : IEqualityComparer<PetOwner>
    {
        bool IEqualityComparer<PetOwner>.Equals(PetOwner x, PetOwner y)
        {
            if(x == null && y == null) return true;
            if((x == null) != (y == null)) return false;
            if(x.Age != y.Age) return false;
            if(!string.Equals(x.Name, y.Name, StringComparison.Ordinal)) return false;

            if(x.Pets == null && y.Pets == null) return true;
            if((x.Pets == null) && (y.Pets == null)) return true;
            if(!Enumerable.SequenceEqual(x.Pets, y.Pets, new PetComparer())) return false;

            return true;
        }

        int IEqualityComparer<PetOwner>.GetHashCode(PetOwner obj) => throw new NotImplementedException();
    }
}