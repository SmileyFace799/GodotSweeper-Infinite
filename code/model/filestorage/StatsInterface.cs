using System;
using System.Linq;
using SmileyFace799.RogueSweeper.model;

namespace SmileyFace799.RogueSweeper.filestorage {
public class StatsInterface : FileInterface<Stats> {
    public StatsInterface(string path) : base(path) {}

    protected override Stats Default => new();

    public ImmutableStats StatsView => Value;

    public override Stats FromBytes(ByteEnumerator bytes) {
        return new(
            BitConverter.ToInt32(bytes.Next(4)),
            BitConverter.ToInt32(bytes.Next(4)),
            BitConverter.ToDouble(bytes.Next(8)),
            BitConverter.ToUInt64(bytes.Next(8)),
            BitConverter.ToUInt32(bytes.Next(4)),
            BitConverter.ToUInt32(bytes.Next(4)),
            BitConverter.ToUInt32(bytes.Next(4)),
            BitConverter.ToUInt32(bytes.Next(4))
        );
    }

    public override byte[] ToBytes(Stats value) {
        return BitConverter.GetBytes(Value.LivesGained)
            .Concat(BitConverter.GetBytes(Value.LivesLost))
            .Concat(BitConverter.GetBytes(Value.BadChanceModifier))
            .Concat(BitConverter.GetBytes(Value.OpenedSquares))
            .Concat(BitConverter.GetBytes(Value.SmallSolvers))
            .Concat(BitConverter.GetBytes(Value.MediumSolvers))
            .Concat(BitConverter.GetBytes(Value.LargeSolvers))
            .Concat(BitConverter.GetBytes(Value.Defusers))
            .ToArray();
    }
}
}