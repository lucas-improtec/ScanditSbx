using System;
using System.Collections.Generic;
using System.Text;

namespace ScanditSbx.Models
{
    public class ScanResult : IEquatable<ScanResult>
    {
        public int Id { get; private set; }
        public string Symbology { get; set; }
        public string Data { get; set; }

        public ScanResult(int id)
        {
            this.Id = id;
        }

        public bool Equals(ScanResult other)
        {
            if (other == null)
            {
                return false;
            }

            return this.GetHashCode() == other.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (obj as ScanResult) != null && this.Equals((ScanResult)obj);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^ this.Symbology.GetHashCode() ^ this.Data.GetHashCode();
        }
    }
}
