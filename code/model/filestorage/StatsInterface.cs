using System;
using System.Linq;

public class StatsInterface : FileInterface<StatsModel> {
    public StatsInterface(string path) : base(path) {}

    protected override StatsModel Default => StatsModel.GetStartingStats();

    public int Lives => Value.Lives;
    public double MinechanceReduction => Value.MinechanceReduction;
    public bool Alive => Value.Alive;

    public override StatsModel FromBytes(ByteEnumerator bytes){
        return new(BitConverter.ToInt32(bytes.Next(4)), BitConverter.ToDouble(bytes.Next(8)));
    }

    public override byte[] ToBytes(StatsModel value){
        return BitConverter.GetBytes(Value.Lives)
            .Concat(BitConverter.GetBytes(Value.MinechanceReduction))
            .ToArray();
    }
}