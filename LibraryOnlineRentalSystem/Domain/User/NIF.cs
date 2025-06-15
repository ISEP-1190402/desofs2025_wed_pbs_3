using System;
using System.Linq;
using LibraryOnlineRentalSystem.Domain.Common;

namespace LibraryOnlineRentalSystem.Domain.User
{
    public class NIF : ICloneable, IValueObject
    {
        public NIF() { } 
        
        public NIF(string nif)
        {
            if (string.IsNullOrWhiteSpace(nif))
                throw new ArgumentNullException(nameof(nif), "NIF cannot be null or empty.");

            nif = nif.Trim();

            if (nif.Length != 9 || !nif.All(char.IsDigit))
                throw new BusinessRulesException("NIF must contain exactly 9 digits.");

            TaxID = nif;
        }


        public string TaxID { get; }


        public object Clone()
        {
            return new NIF(TaxID);
        }

        public override string ToString()
        {
            return TaxID;
        }

        public override bool Equals(object? obj)
        {
            if (this == obj) return true;
            if (obj is not NIF other) return false;
            return TaxID == other.TaxID;
        }

        public override int GetHashCode()
        {
            return TaxID?.GetHashCode() ?? 0;
        }
    }
}