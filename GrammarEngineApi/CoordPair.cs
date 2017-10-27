namespace GrammarEngineApi
{
    public class CoordPair
    {
        private readonly GrammarEngine _engine;

        public readonly int CoordId, StateId;

        public CoordPair(GrammarEngine engine, int coordId, int stateId)
        {
            CoordId = coordId;
            StateId = stateId;
            _engine = engine;
        }

        private string _coordCode;
        public string CoordCode => _coordCode ?? (_coordCode = _engine.GetCoordName(CoordId));

        private string _stateCode;
        public string StateCode => _stateCode ?? (_stateCode = _engine.GetCoordStateName(CoordId, StateId));

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