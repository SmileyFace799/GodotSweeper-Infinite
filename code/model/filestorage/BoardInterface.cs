using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SmileyFace799.RogueSweeper.model;

namespace SmileyFace799.RogueSweeper.filestorage
{
    public class BoardInterface : FileInterface<Board>
    {
        private static readonly SquareType[] ALL_SQUARE_TYPES = SquareFactory.ALL;

        protected override Board Default => new();

        public ImmutableBoard BoardView => Value;

        public BoardInterface(string path) : base(path) {}

        private void OnSquareUpdated(Position position, Square square)
        {
            throw new NotImplementedException();
            //TODO: Utilize this to "remember" what squares to save, erasing the need to re-write the entire save file when saving
        }

        private bool[] DecompressBools(byte boolByte)
        {
            bool[] bools = new bool[8];
            for (int i = 0; i < bools.Length; ++i) {
                bools[i] = (boolByte & 1) == 1;
                boolByte >>>= 1;
            }
            return bools;
        }

        private byte CompressBools(params bool[] bools)
        {
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



        private Square BytesToSquare(ByteEnumerator bytes)
        {
            bool[] bools = DecompressBools(bytes.Next());
            SquareType type = ALL_SQUARE_TYPES[bytes.Next()];
            Square square;
            switch (type) {
                case NumberSquareType numberType:
                    NumberSquare numberSquare = new(numberType);
                    if (bools[1]) { // Opened
                        numberSquare.Number = bytes.Next();
                    }
                    square = numberSquare;
                    break;
                case SpecialSquareType specialType:
                    SpecialSquare specialSquare = new SpecialSquare(specialType);
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

        private byte[] SquareToBytes(Square square)
        {
            List<byte> bytes = new() {
                CompressBools(square.Flagged, square.Opened),
                (byte) Array.IndexOf(ALL_SQUARE_TYPES, square.Type)
            };
            if (square is NumberSquare numberSquare && numberSquare.Opened) {
                bytes.Add((byte) numberSquare.Number);
            }
            return bytes.ToArray();
        }



        private Dictionary<long, Square> BytesToColumn(ByteEnumerator bytes)
        {
            int squareCount = BitConverter.ToInt32(bytes.Next(4));
            Dictionary<long, Square> column = new();
            for (int i = 0; i < squareCount; ++i) {
                column.Add(BitConverter.ToInt64(bytes.Next(8)), BytesToSquare(bytes));
            }
            return column;
        }

        private byte[] ColumnToBytes(Dictionary<long, Square> column)
        {
            List<byte> bytes = column.SelectMany(kvp => BitConverter.GetBytes(kvp.Key).Concat(SquareToBytes(kvp.Value))).ToList();
            return BitConverter.GetBytes(column.Count()).Concat(bytes).ToArray();
        }



        public override Board FromBytes(ByteEnumerator bytes)
        {
            int columnCount = BitConverter.ToInt32(bytes.Next(4));
            Dictionary<long, Dictionary<long, Square>> boardSquares = new();
            for (int i = 0; i < columnCount; ++i) {
                boardSquares.Add(BitConverter.ToInt64(bytes.Next(8)), BytesToColumn(bytes));
            }
            return new(boardSquares);
        }

        public override byte[] ToBytes(Board value)
        {
            List<byte> bytes = value.GetSquares().SelectMany(kvp => BitConverter.GetBytes(kvp.Key).Concat(ColumnToBytes(kvp.Value))).ToList();
            return BitConverter.GetBytes(value.GetSquares().Count()).Concat(bytes).ToArray();
        }
    }
}