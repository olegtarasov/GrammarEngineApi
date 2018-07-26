namespace GrammarEngineApi
{
    public class CoordPair
    {
        public CoordPair(int coordId, int stateId, string coordCode, string stateCode)
        {
            CoordId = coordId;
            StateId = stateId;
            CoordCode = coordCode;
            StateCode = stateCode;
        }

        public int CoordId { get; }

        public int StateId { get; }

        public string CoordCode { get; }

        public string StateCode { get; }

        protected bool Equals(CoordPair other)
        {
            return CoordId == other.CoordId && StateId == other.StateId && string.Equals(CoordCode, other.CoordCode) && string.Equals(StateCode, other.StateCode);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((CoordPair)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = CoordId;
                hashCode = (hashCode * 397) ^ StateId;
                hashCode = (hashCode * 397) ^ (CoordCode != null ? CoordCode.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (StateCode != null ? StateCode.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"[{CoordCode}] {StateCode}";
        }
    }
}