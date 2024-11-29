using System;
using System.Collections.Generic;
using System.Linq;

public class BoardModel {
    public static readonly double SPECIAL_BAD_CHANCE = 0.15;
    public static readonly double SPECIAL_GOOD_CHANCE = 0.002;

    private readonly Random _random = new();
    private Dictionary<long, Dictionary<long, SquareModel>> _squares = new();

    public BoardUpdateListener Listener {get; set;}

    public HashSet<SquareModel> Squares() {
        return _squares.Values.SelectMany(v => v.Values).ToHashSet();
    }

    public bool hasSquare(Position position) {
        return _squares.Keys.Contains(position.X) && _squares[position.X].Keys.Contains(position.Y);
    }

    public SquareModel GetSquare(Position position) {
        SquareModel square = null;
        if (_squares.Keys.Contains(position.X)) {
            Dictionary<long, SquareModel> column = _squares[position.X];
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
        RevealSquare(position, 50, specialBadOverride, specialBadOverride);
    }

    private void RevealSquare(Position position, int cascadeLimit, double specialBadOverride=-1, double specialGoodOverride=-1) {
        if (specialBadOverride == -1) {
            specialBadOverride = SPECIAL_BAD_CHANCE;
        }

        if (specialGoodOverride == -1) {
            specialGoodOverride = SPECIAL_GOOD_CHANCE;
        }

        SquareModel squareToReveal = GetSquare(position);
        if (squareToReveal == null) {
            squareToReveal = GenerateSquare();
            placeSquare(position, squareToReveal);
        } else if (squareToReveal.Opened || squareToReveal.Flagged) {
            throw new InvalidOperationException($"Square at ({position.X}, {position.Y}) is already {(squareToReveal.Opened ? "opened" : "flagged")}");
        }
        if (squareToReveal is SpecialSquareModel specialSquare) {
            specialSquare.Open();
            Listener.OnSquareUpdated(position, squareToReveal);
        } else if (squareToReveal is NumberSquareModel numberSquare) {
            HashSet<SquareModel> coveredSquares = numberSquare.Type.RelativeCoverage.Select(p => {
                Position coveredPosition = position + p;
                SquareModel coveredSquare = GetSquare(coveredPosition);
                if (coveredSquare == null) {
                    coveredSquare = GenerateSquare();
                    placeSquare(coveredPosition, coveredSquare);
                    Listener.OnSquareUpdated(coveredPosition, coveredSquare);
                }
                return coveredSquare;
            }).ToHashSet();
            int number = coveredSquares.Where(s => s is SpecialSquareModel specialSquare && specialSquare.Type.IsBad).Count();
            numberSquare.Number = number;
            Listener.OnSquareUpdated(position, squareToReveal);

        } else {
            throw new InvalidOperationException($"Square at ({position.X}, {position.Y}) is of an unknown type, cannot open it");
        }
    }

    public SquareModel GenerateSquare() {
        SquareModel generatedSquare;
        double randomNum = _random.NextDouble();
        if (randomNum < SPECIAL_BAD_CHANCE) {
            generatedSquare = new SpecialSquareModel(SpecialSquareType.GetRandomBad());
        } else if (randomNum - SPECIAL_BAD_CHANCE < SPECIAL_GOOD_CHANCE) {
            generatedSquare = new SpecialSquareModel(SpecialSquareType.GetRandomGood());
        } else {
            generatedSquare = new NumberSquareModel(NumberSquareType.GetRandom());
        }
        return generatedSquare;
    }

    public void placeSquare(Position position, SquareModel square) {
        Dictionary<long, SquareModel> column;
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