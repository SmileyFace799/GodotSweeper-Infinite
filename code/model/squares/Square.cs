using System;

namespace SmileyFace799.RogueSweeper.model
{
    public interface IImmutableSquare
    {
        bool Flagged {get;}
        bool Opened {get;}
        bool Revealable {get;}
        SquareType Type {get;}
    }

    public abstract class Square : IImmutableSquare
    {
        private bool _flagged = false;
        protected readonly object _lock = new();

        public bool Flagged {get {
            lock (_lock) {
                return _flagged;
            }
        }}
        public abstract bool Opened {get;}
        public bool Revealable {get {
            lock (_lock) {
                return !Opened && !Flagged;
            }
        }}
        public abstract SquareType Type {get;}

        /// <summary>
        /// Toggles the flagged state of the square. If the square is opened, this does nothing.
        /// </summary>
        public void ToggleFlagged()
        {
            lock (_lock) {
                if (!Opened) {
                    _flagged = !_flagged;
                }
            }
        }

        public void WithLock(Action action) {
            lock (_lock) {
                action();
            }
        }
    }
}