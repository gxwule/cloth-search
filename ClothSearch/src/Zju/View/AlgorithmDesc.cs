namespace Zju.View
{
    public enum AlgorithmType
    {
        Color1, Texture1, Texture2, Texture3,
    };

    public enum RecallLevel
    {
        Default, Recall1, Recall2, Recall3,
    };

    public sealed class AlgorithmDesc
    {
        public AlgorithmType AType { get; set; }
        public RecallLevel RLevel { get; set; }

        public AlgorithmDesc()
        {
            AType = AlgorithmType.Texture3;
            RLevel = RecallLevel.Default;
        }
    }
}
