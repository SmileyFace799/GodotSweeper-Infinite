using System;

namespace SmileyFace799.RogueSweeper.model
{
    public interface immutableNumberSquare : IImmutableSquare
    {
        new NumberSquareType Type {get;}
        int Number {get;}
    }

    public class NumberSquare : Square, immutableNumberSquare
    {
        private int _number = -1;

        public override bool Opened {get {
            lock (_lock) {
                return _number != -1;
            }
        }}

        /// <summary>
        /// The actual number of this square. This must be assigned before it can be read,
        /// and it can be assigned only once. Assigning this is what "opens" the square.
        /// If this square is not revealable, assigning to this does nothing.
        /// </summary>
        public int Number { get {
            lock (_lock) {
                if (!Opened) {
                    throw new InvalidOperationException("This square is not opened & does not have a number yet");
                }
                return _number;
            }
        } set {
            lock (_lock) {
                if (value < 0 || value > Type.RelativeCoverage.Length) {
                    throw new ArgumentOutOfRangeException($"The number must be between 0 & the square type's max coverage ({Type.RelativeCoverage.Length})");
                } else if (Revealable) {
                    _number = value;
                }
            }
        }}

        public override NumberSquareType Type {get;}

        public NumberSquare(NumberSquareType type) => Type = type;
    }
}