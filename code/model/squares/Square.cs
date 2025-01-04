using System;
using System.Linq;

namespace SmileyFace799.RogueSweeper.model
{
    public interface ImmutableSquare
    {
        bool Flagged {get;}
        bool Opened {get;}
        SquareType Type {get;}
    }

    public abstract class Square : ImmutableSquare
    {
        private bool _flagged;

        public bool Flagged {get {return _flagged;}}
        public abstract bool Opened {get;}
        public abstract SquareType Type {get;}

        public void ToggleFlagged()
        {
            _flagged = !_flagged;
        }
    }
}