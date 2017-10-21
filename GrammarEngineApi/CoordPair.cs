namespace GrammarEngineApi
{
    public struct CoordPair
    {
        public readonly int CoordId, StateId;

        public CoordPair(int coordId, int stateId)
        {
            CoordId = coordId;
            StateId = stateId;
        }

        public override bool Equals(object obj)
        {
            CoordPair y = (CoordPair)obj;
            return CoordId == y.CoordId && StateId == y.StateId;
        }

        public override int GetHashCode()
        {
            return CoordId ^ (StateId << 10);
        }
    }
}