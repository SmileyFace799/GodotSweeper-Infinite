using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using SmileyFace799.RogueSweeper.events;

namespace SmileyFace799.RogueSweeper.model
{
    public interface ImmutableBoard
    {
        public IBoardUpdateListener Listener {get; set;}

        bool HasSquare(Position position);
        ImmutableSquare GetSquare(Position position);
        bool IsRevealable(Position position);
        bool IsFlaggable(Position position);
        bool IsSmartRevealable(Position position);
    }

    public class Board : ImmutableBoard
    {
        public const int CASCADE_LIMIT = 50;

        private readonly Random _random = new();
        private readonly Dictionary<long, Dictionary<long, Square>> _squares;

        private IBoardUpdateListener _listener = null;
        public IBoardUpdateListener Listener {get => _listener == null ? IBoardUpdateListener.Null : _listener; set {_listener = value;}}

        public Board()
        {
            _squares = new();
        }

        public Board(Dictionary<long, Dictionary<long, Square>> squares)
        {
            _squares = squares;
        }

        public void placeSquare(Position position, Square square, bool replaceSquare=false, bool sendUpdateEvent=true)
        {
            Dictionary<long, Square> column;
            if (_squares.ContainsKey(position.X)) {
                column = _squares[position.X];
            } else {
                column = new();
                _squares[position.X] = column;
            }
            if (replaceSquare != column.ContainsKey(position.Y)) {
                if (replaceSquare) {
                    throw new ArgumentException($"There is no square at ({position.X}, {position.Y}) to replace");
                } else {
                    throw new ArgumentException($"There is already a square at ({position.X}, {position.Y})");
                }
            }
            column[position.Y] = square;
            if (sendUpdateEvent) {
                Listener.OnBoardUpdate(new SquareEvent(position, square));
            }
        }

        public bool HasSquare(Position position)
        {
            return _squares.ContainsKey(position.X) && _squares[position.X].ContainsKey(position.Y);
        }

        /// <summary>
        /// Utility function to check if a square at a specified position satisfies a specified condition.
        /// </summary>
        /// <param name="position">The position of the square to check</param>
        /// <param name="ifNotFound">The default result if the square doesn't exist</param>
        /// <param name="checker">The condition to check if the square exists</param>
        /// <returns>If the condition is satisfied or not, or the default result if the square didn't exist</returns>
        private bool SquareState(Position position, bool ifNotFound, Func<Square, bool> checker)
        {
            bool state = HasSquare(position) ^ ifNotFound;
            if (state ^ ifNotFound) { // Can only enter if HasSquare returns true
                state = checker(GetSquare(position));
            }
            return state;
        }

        /// <summary>
        /// Checks if a square can be revealed at a specified position.
        /// </summary>
        /// <param name="position">The position to check</param>
        /// <returns>If a square can be revealed at the specified position</returns>
        public bool IsRevealable(Position position) => SquareState(position, true, s => !s.Flagged && !s.Opened);
        /// <summary>
        /// Checks if a square can be flagged at a specified position.
        /// </summary>
        /// <param name="position">The position to check</param>
        /// <returns>If a square can be flagged at the specified position</returns>
        public bool IsFlaggable(Position position) => SquareState(position, false, s => !s.Opened);
        /// <summary>
        /// Checks if a square can be "smart revealed" at a specified position.<br/>
        /// "Smart revealing" refers to revealing a number square's covered squares if its number equals the number of covered squares that are flagged.
        /// </summary>
        /// <param name="position">The position to check</param>
        /// <returns>If a square can be "smart revealed" at the specified position</returns>
        public bool IsSmartRevealable(Position position) => SquareState(position, false, s => s.Opened && s is NumberSquare);

        ImmutableSquare ImmutableBoard.GetSquare(Position position) => GetSquare(position);

        public Square GetSquare(Position position)
        {
            Square square = null;
            if (_squares.ContainsKey(position.X)) {
                Dictionary<long, Square> column = _squares[position.X];
                if (column.ContainsKey(position.Y)) {
                    square = column[position.Y];
                }
            }
            return square;
        }

        public Dictionary<long, Dictionary<long, Square>> GetSquares() => _squares;

        public Square GetOrGenerateSquare(Position position, SquareGenData squareGenData, bool sendUpdateEvent=true)
        {
            Square square;
            if (HasSquare(position)) {
                square = GetSquare(position);
            } else {
                square = SquareFactory.MakeRandom(position, squareGenData);
                placeSquare(position, square, false, sendUpdateEvent);
            }
            return square;
        }

        public Dictionary<Position, Square> GetOrGenerateCoveredSquares(
            Position position,
            NumberSquareType numberSquareType,
            SquareGenData squareGenData,
            bool sendUpdateEvent=true
        ) {
            return numberSquareType.RelativeCoverage.ToDictionary(p => position + p, p => GetOrGenerateSquare(position + p, squareGenData, sendUpdateEvent));
        }

        public void FlagSquare(Position position)
        {
            if (!HasSquare(position)) {
                throw new InvalidOperationException($"There is no generated square at ({position.X}, {position.Y}), can't place flag");
            }
            Square squareToFlag = GetSquare(position);
            if (squareToFlag.Opened) {
                throw new InvalidOperationException($"Square at ({position.X}, {position.Y}) is already opened, can't place flag");
            }
            squareToFlag.ToggleFlagged();
            Listener.OnBoardUpdate(new SquareEvent(position, squareToFlag));
        }

        public void RevealSquare(Position position, SquareGenData cascadeGenData)
        {
            RevealSquare(position, cascadeGenData, cascadeGenData);
        }

        /// <summary>
        /// Reveals a square, and generates new squares in the area it covers.<br/>
        /// If the revealed square is a number that covers no mines,
        /// the squares it covers will also be revealed,
        /// and this process will continue cascading.
        /// </summary>
        /// <param name="position">The position of the square to reveal</param>
        /// <param name="specialBadChance">If the chance of generating a bad special square should be overriden for this reveal.
        /// Will not apply to revealed cascading squares</param>
        /// <param name="specialGoodChance">If the chance of generating a good special square should be overriden for this reveal.
        /// Will not apply to revealed cascading squares</param>
        public void RevealSquare(Position position, SquareGenData squareGenData, SquareGenData cascadeGenData)
        {
            Square squareToReveal = GetOrGenerateSquare(position, squareGenData);
            if (squareToReveal.Opened || squareToReveal.Flagged) {
                throw new InvalidOperationException($"Square at ({position.X}, {position.Y}) is already {(squareToReveal.Opened ? "opened" : "flagged")}");
            }
            RevealSquares(new Dictionary<Position, Square>{{position, squareToReveal}}, CASCADE_LIMIT, squareGenData, cascadeGenData);
        }

        public void RevealSquares(Dictionary<Position, Square> squaresToReveal, int cascadeLimit, SquareGenData cascadeGenData)
        {
            RevealSquares(squaresToReveal, cascadeLimit, cascadeGenData, cascadeGenData);
        }

        private void RevealSquares(Dictionary<Position, Square> squaresToReveal, int cascadeLimit, SquareGenData squareGenData, SquareGenData cascadeGenData)
        {
            Dictionary<Position, Square> cascadingSquares = new();
            foreach ((Position position, Square squareToReveal) in squaresToReveal) {
                if (squareToReveal is SpecialSquare specialSquare) {
                    specialSquare.Open();
                    Listener.OnBoardUpdate(new SquareEvent(position, squareToReveal));
                } else if (squareToReveal is NumberSquare numberSquare) {
                    // Events will be sent manually for slightly increased performance
                    Dictionary<Position, Square> coveredSquares = GetOrGenerateCoveredSquares(position, numberSquare.Type, squareGenData, false);
                    int number = coveredSquares.Values.Where(s => s.Type.Level == TypeLevel.BAD).Count();
                    numberSquare.Number = number;
                    Listener.OnBoardUpdate(new SquareEvent(position, squareToReveal));

                    foreach ((Position p, Square coveredSquare) in coveredSquares) {
                        if (!squaresToReveal.ContainsKey(p) && !cascadingSquares.ContainsKey(p) && !coveredSquare.Opened && !coveredSquare.Flagged) {
                            if (number == 0 && cascadeLimit > 0) {
                                cascadingSquares.Add(p, coveredSquare);
                            } else {
                                // Cascading squares will be updated on next iteration, so only the squares that don't cascade need to be updated
                                Listener.OnBoardUpdate(new SquareEvent(p, coveredSquare));
                            }
                        }
                    }

                } else {
                    throw new InvalidOperationException($"Square at ({position.X}, {position.Y}) is of an unknown type, cannot open it");
                }
            }
            if (cascadingSquares.Any()) {
                RevealSquares(cascadingSquares, --cascadeLimit, cascadeGenData);
            }
        }
    }

    public interface SquareGenData
    {
        protected static double GetRawBadChance(Position position)
        {
            double x = position.Abs;
            return Math.Log(x / 6 + 1) / 8 + Math.Sin(x / 5 + 1.25 * Math.PI) / 30 + 80 / ((x + 4) * (x + 4) + 240) + Math.Pow(1.001, 3e-3 * x * x) + 5e-4 * x - 1.1;
        }

        protected static double GetRawGoodChance(Position position) => 1 / (position.Abs + 100);

        double GetBadChance(Position position);
        double GetGoodChance(Position position);
    }

    public record RelativeGenData(double BadChanceModifier=0, double GoodChanceModifier=0) : SquareGenData
    {
        public double GetBadChance(Position position) => SquareGenData.GetRawBadChance(position) + BadChanceModifier;
        public double GetGoodChance(Position position) => SquareGenData.GetRawGoodChance(position) + GoodChanceModifier;
    };

    public record StaticGenData(double? BadChance=null, double? GoodChance=null) : SquareGenData
    {
        public double GetBadChance(Position position) => BadChance == null ? SquareGenData.GetRawBadChance(position) : (double) BadChance;
        public double GetGoodChance(Position position) => GoodChance == null ? SquareGenData.GetRawGoodChance(position) : (double) GoodChance;
    };
}