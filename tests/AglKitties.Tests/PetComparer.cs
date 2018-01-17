using System;
using System.Collections.Generic;

namespace AglKitties.Tests
{
	internal class PetComparer : IEqualityComparer<Pet>
    {
        bool IEqualityComparer<Pet>.Equals(Pet x, Pet y)
        {
            if(x == null && y == null) return true;
            if((x == null) != (y == null)) return false;
            if(x.Type != y.Type) return false;
            if(!string.Equals(x.Name, y.Name, StringComparison.Ordinal)) return false;

            return true;
        }

        int IEqualityComparer<Pet>.GetHashCode(Pet obj) => throw new NotImplementedException();
    }
}