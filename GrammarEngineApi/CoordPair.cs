namespace GrammarEngineApi
{
    public struct CoordPair
    {
        public readonly int CoordId, StateId;

        public CoordPair(int coordId, int stateId, string coordCode, string stateCode)
        {
            CoordId = coordId;
            StateId = stateId;
            CoordCode = coordCode;
            StateCode = stateCode;
        }

        public readonly string CoordCode;

        public readonly string StateCode;

        public override bool Equals(object obj)
        {
            CoordPair y = (CoordPair)obj;
            return CoordId == y.CoordId && StateId == y.StateId;
        }

        public override int GetHashCode()
        {
            return CoordId ^ (StateId << 10);
        }

        public override string ToString()
        {
            return $"[{CoordCode}] {StateCode}";
        }
    }
}