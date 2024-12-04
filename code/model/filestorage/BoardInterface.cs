using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class BoardInterface : FileInterface<BoardModel>, BoardUpdateListener {
    private static readonly SquareType[] ALL_SQUARE_TYPES = ((SquareType[]) NumberSquareType.ALL).Union(SpecialSquareType.ALL).ToArray();

    protected override BoardModel Default => new();

    public BoardInterface(string path) : base(path) {}

    public void OnSquareUpdated(Position position, SquareModel square) {
        throw new NotImplementedException();
        //TODO: Utilize this to "remember" what squares to save, erasing the need to re-write the entire save file when saving
    }

    public void SetBoardGuiListener(BoardUpdateListener guiListener) {
        Value.GuiListener = guiListener;
    }

    private byte NextByte(IEnumerator<byte> bytes) {
        if (!bytes.MoveNext()) {
            throw new InvalidDataException("Out of bytes!");
        }
        return bytes.Current;
    }

    private byte[] NextBytes(IEnumerator<byte> bytes, int amount) {
        byte[] next = new byte[amount];
        for (int i = 0; i < amount; ++i) {
            next[i] = NextByte(bytes);
        }
        return next;
    }



    private bool[] DecompressBools(byte boolByte) {
        bool[] bools = new bool[8];
        for (int i = 0; i < bools.Length; ++i) {
            bools[i] = (boolByte & 1) == 1;
            boolByte >>>= 1;
        }
        return bools;
    }

    private byte CompressBools(params bool[] bools) {
        if (bools.Length > 8) {
            throw new InvalidOperationException("Cannot compress more than 8 bools into a single byte");
        }

        byte boolByte = 0;
        int shift = 0;
        while (shift < bools.Length) {
            boolByte |= (byte) ((bools[shift] ? 1 : 0) << shift);
            ++shift;
        }
        return boolByte;
    }



    private SquareModel BytesToSquare(IEnumerator<byte> bytes) {
        bool[] bools = DecompressBools(NextByte(bytes));
        SquareType type = ALL_SQUARE_TYPES[NextByte(bytes)];
        SquareModel square;
        switch (type) {
            case NumberSquareType numberType:
                NumberSquareModel numberSquare = new(numberType);
                if (bools[1]) { // Opened
                    numberSquare.Number = NextByte(bytes);
                }
                square = numberSquare;
                break;
            case SpecialSquareType specialType:
                SpecialSquareModel specialSquare = new SpecialSquareModel(specialType);
                if (bools[1]) {
                    specialSquare.Open();
                }
                square = specialSquare;
                break;
            default:
                throw new InvalidDataException($"Unknown Square type: \"{type}\"");
        }
        if (bools[0]) { // Flagged
            square.ToggleFlagged();
        }
        return square;
    }

    private byte[] SquareToBytes(SquareModel square) {
        List<byte> bytes = new() {
            CompressBools(square.Flagged, square.Opened),
            (byte) Array.IndexOf(ALL_SQUARE_TYPES, square.Type)
        };
        if (square is NumberSquareModel numberSquare && numberSquare.Opened) {
            bytes.Add((byte) numberSquare.Number);
        }
        return bytes.ToArray();
    }



    private Dictionary<long, SquareModel> BytesToColumn(IEnumerator<byte> bytes) {
        int squareCount = BitConverter.ToInt32(NextBytes(bytes, 4));
        Dictionary<long, SquareModel> column = new();
        for (int i = 0; i < squareCount; ++i) {
            column.Add(BitConverter.ToInt64(NextBytes(bytes, 8)), BytesToSquare(bytes));
        }
        return column;
    }

    private byte[] ColumnToBytes(Dictionary<long, SquareModel> column) {
        List<byte> bytes = column.SelectMany(kvp => BitConverter.GetBytes(kvp.Key).Concat(SquareToBytes(kvp.Value))).ToList();
        return BitConverter.GetBytes(column.Count()).Concat(bytes).ToArray();
    }



    public override BoardModel FromBytes(byte[] byteArray) {
        IEnumerator<byte> bytes = byteArray.AsEnumerable().GetEnumerator();
        int columnCount = BitConverter.ToInt32(NextBytes(bytes, 4));
        Dictionary<long, Dictionary<long, SquareModel>> boardSquares = new();
        for (int i = 0; i < columnCount; ++i) {
            boardSquares.Add(BitConverter.ToInt64(NextBytes(bytes, 8)), BytesToColumn(bytes));
        }
        return new(boardSquares);
    }

    public override byte[] ToBytes(BoardModel value) {
        List<byte> bytes = value.GetSquares().SelectMany(kvp => BitConverter.GetBytes(kvp.Key).Concat(ColumnToBytes(kvp.Value))).ToList();
        return BitConverter.GetBytes(value.GetSquares().Count()).Concat(bytes).ToArray();
    }
}