using System;
using System.Collections.Generic;

namespace SmileyFace799.RogueSweeper.model
{
    public abstract class SquareType : Switchable
    {
        public abstract double Weight {get;}
        public TypeLevel Level {get;}

        protected SquareType(TypeLevel level) => Level = level;
    }

    /// <summary>
    /// Represents the level of "severity" of a square.
    /// </summary>
    public enum TypeLevel
    {
        BAD = -1,
        NEUTRAL = 0,
        GOOD = 1
    }
}