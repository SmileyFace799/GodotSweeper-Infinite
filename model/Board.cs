using System;
using System.Collections.Generic;
using System.Linq;

public class Board {
    public static readonly double SPECIAL_BAD_CHANCE = 0.2;
    public static readonly double SPECIAL_GOOD_CHANCE = 0.002;

    private readonly Random _random = new();
    private Dictionary<long, Dictionary<long, Square>> _squares = new();

    public HashSet<Square> Squares() {
        return _squares.Values.SelectMany(v => v.Values).ToHashSet();
    }

    public Square GetSquare(Position position) {
        Square square = null;
        if (_squares.Keys.Contains(position.X)) {
            Dictionary<long, Square> column = _squares[position.X];
            if (column.Keys.Contains(position.Y)) {
                square = column[position.Y];
            }
        }
        return square;
    }

    /// <summary>
    /// Reveals a square, and generates new squares in the area it covers.<br/>
    /// If the revealed square is a number that covers no mines,
    /// the squares it covers will also be revealed,
    /// and this process will continue cascading.
    /// </summary>
    /// <param name="position">The position of the square to reveal</param>
    /// <param name="specialBadOverride">If the chance of generating a bad special square should be overriden for this reveal.
    /// Will not apply to revealed cascading squares</param>
    /// <param name="specialGoodOverride">If the chance of generating a good special square should be overriden for this reveal.
    /// Will not apply to revealed cascading squares</param>
    public void RevealSquare(Position position, double specialBadOverride=-1, double specialGoodOverride=-1) {
        RevealSquare(position, 50, specialBadOverride);
    }

    private void RevealSquare(Position position, int cascadeLimit, double mineChanceOverride=-1, double specialGoodOverride=-1) {
        if (mineChanceOverride == -1) {
            mineChanceOverride = SPECIAL_BAD_CHANCE;
        }

        Square squareToReveal = GetSquare(position);
        if (squareToReveal.Opened || squareToReveal.Flagged) {
            throw new InvalidOperationException($"Square at ({position.X}, {position.Y}) is already {(squareToReveal.Opened ? "opened" : "flagged")}");
        }
        if (squareToReveal is SpecialSquare specialSquare) {
            specialSquare.Open();
        } else if (squareToReveal is NumberSquare numberSquare) {
            HashSet<Square> coveredSquares = numberSquare.Type.RelativeCoverage.Select(p => {
                Position coveredPosition = position + p;
                Square coveredSquare = GetSquare(coveredPosition);
                if (coveredSquare == null) {
                    coveredSquare = GenerateSquare();
                    placeSquare(coveredPosition, coveredSquare);
                }
                return coveredSquare;
            }).ToHashSet();
            int number = coveredSquares.Where(s => s is SpecialSquare specialSquare && specialSquare.Type.IsBad).Count();
            numberSquare.Number = number;


        } else {
            throw new InvalidOperationException($"Square at ({position.X}, {position.Y}) is of an unknown type, cannot open it");
        }
    }

    public Square GenerateSquare() {
        Square generatedSquare;
        double randomNum = _random.NextDouble();
        if (randomNum < SPECIAL_BAD_CHANCE) {
            generatedSquare = new SpecialSquare(SpecialSquareType.GetRandomBad());
        } else if (randomNum - SPECIAL_BAD_CHANCE < SPECIAL_GOOD_CHANCE) {
            generatedSquare = new SpecialSquare(SpecialSquareType.GetRandomGood());
        } else {
            generatedSquare = new NumberSquare(NumberSquareType.GetRandom());
        }
        return generatedSquare;
    }

    public void placeSquare(Position position, Square square) {
        Dictionary<long, Square> column;
        if (_squares.Keys.Contains(position.X)) {
            column = _squares[position.X];
        } else {
            column = new();
            _squares[position.X] = column;
        }
        if (column.Keys.Contains(position.Y)) {
            throw new ArgumentException($"There is already a square at ({position.X}, {position.Y})");
        }
        column[position.Y] = square;
    }
}