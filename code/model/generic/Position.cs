using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;

namespace SmileyFace799.RogueSweeper.model
{
    /// <summary>
    /// A 2D position, with a read-only X & Y value.
    /// </summary>
    public struct Position
    {
        [SetsRequiredMembers]
        public Position(long x, long y) {
            X = x;
            Y = y;
        }
        public required long X {get; init;}
        public required long Y {get; init;}

        public readonly double Abs => Math.Sqrt(X * X + Y * Y);

        public static Position operator + (Position p1, Position p2) => new(p1.X + p2.X, p1.Y + p2.Y);
        public static Position operator - (Position p1, Position p2) => new(p1.X - p2.X, p1.Y - p2.Y);
        public static Position operator * (Position p, long n) => new(p.X * n, p.Y * n);
        public static Position operator * (long n, Position p) => p * n;
        public static Position operator / (Position p, long n) => new(p.X / n, p.Y / n);
        public static bool operator == (Position p1, Position p2) => p1.X == p2.X && p1.Y == p2.Y;
        public static bool operator != (Position p1, Position p2) => !(p1 == p2);
        public override string ToString() => $"Position{{X={X}, Y={Y}}}";

        public static Position[] Array2D(long left, long top, long width, long height, bool includeZero=true)
        {
            Position[] positions = new Position[width * height];
            for (long x = 0; x < width; ++x) {
                for (long y = 0; y < height; ++y) {
                    positions[x + (y * width)] = new Position(left + x, top + y);
                }
            }
            return includeZero ? positions : positions.Where(p => p.X != 0 || p.Y != 0).ToArray();
        }

        public static Position[] Shift(Position shift, Position[] positions) => Shift(shift.X, shift.Y, positions);
        public static Position[] Shift(long shiftX, long shiftY, Position[] positions) => positions.Select(p => new Position(p.X + shiftX, p.Y + shiftY)).ToArray();
    };
}