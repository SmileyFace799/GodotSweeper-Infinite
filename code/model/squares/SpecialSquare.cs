using System;

namespace SmileyFace799.RogueSweeper.model
{
    public interface ImmutableSpecialSquare : IImmutableSquare
    {
        new SpecialSquareType Type {get;}
    }

    public class SpecialSquare : Square, ImmutableSpecialSquare
    {
        private bool _opened = false;

        public override bool Opened {get {
            lock (_lock) {
                return _opened;
            }
        }}
        public override SpecialSquareType Type {get;}

        public SpecialSquare(SpecialSquareType type) => Type = type;
        
        /// <summary>
        /// Opens the square if it is revealabe, otherwise does nothing.
        /// </summary>
        public void Open() {
            lock (_lock) {
                if (Revealable) {
                    _opened = true;
                }
            }
        }
    }
}