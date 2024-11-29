using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class BoardModel {
    public const double SPECIAL_BAD_CHANCE = 0.2;
    public const double SPECIAL_GOOD_CHANCE = 0.002;
    public const int CASCADE_LIMIT = 50;

    private readonly Random _random = new();
    private Dictionary<long, Dictionary<long, SquareModel>> _squares = new();

    public BoardUpdateListener Listener {get; set;}

    public HashSet<SquareModel> Squares() {
        return _squares.Values.SelectMany(v => v.Values).ToHashSet();
    }

    public bool HasSquare(Position position) {
        return _squares.ContainsKey(position.X) && _squares[position.X].ContainsKey(position.Y);
    }

    public SquareModel GetSquare(Position position) {
        SquareModel square = null;
        if (_squares.ContainsKey(position.X)) {
            Dictionary<long, SquareModel> column = _squares[position.X];
            if (column.ContainsKey(position.Y)) {
                square = column[position.Y];
            }
        }
        return square;
    }

    private SquareModel GenerateSquare(double specialBadChance, double specialGoodChance) {
        SquareModel generatedSquare;
        double randomNum = _random.NextDouble();
        if (randomNum < specialBadChance) {
            generatedSquare = new SpecialSquareModel(SpecialSquareType.GetRandomBad());
        } else if (randomNum - specialBadChance < specialGoodChance) {
            generatedSquare = new SpecialSquareModel(SpecialSquareType.GetRandomGood());
        } else {
            generatedSquare = new NumberSquareModel(NumberSquareType.GetRandom());
        }
        return generatedSquare;
    }

    private SquareModel GetOrGenerateSquare(Position position, double specialBadChance, double specialGoodChance) {
        SquareModel square;
        if (HasSquare(position)) {
            square = GetSquare(position);
        } else {
            square = GenerateSquare(specialBadChance, specialGoodChance);
            placeSquare(position, square);
        }
        return square;
    }

    public Dictionary<Position, SquareModel> GetOrGenerateCoveredSquares(Position position, NumberSquareType numberSquareType) {
        return GetOrGenerateCoveredSquares(position, numberSquareType, SPECIAL_BAD_CHANCE, SPECIAL_GOOD_CHANCE);
    }

    private Dictionary<Position, SquareModel> GetOrGenerateCoveredSquares(
        Position position,
        NumberSquareType numberSquareType,
        double specialBadChance,
        double specialGoodChance
    ) {
        return numberSquareType.RelativeCoverage.ToDictionary(p => position + p, p => GetOrGenerateSquare(position + p, specialBadChance, specialGoodChance));
    }

    public void FlagSquare(Position position) {
        if (!HasSquare(position)) {
            throw new InvalidOperationException($"There is no generated square at ({position.X}, {position.Y}), can't place flag");
        }
        SquareModel squareToFlag = GetSquare(position);
        if (squareToFlag.Opened) {
            throw new InvalidOperationException($"Square at ({position.X}, {position.Y}) is already opened, can't place flag");
        }
        squareToFlag.ToggleFlagged();
        Listener.OnSquareUpdated(position, squareToFlag);
    }

    public void RevealSquare(Position position) {
        RevealSquare(position, SPECIAL_BAD_CHANCE, SPECIAL_GOOD_CHANCE);
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
    public void RevealSquare(Position position, double specialBadChance, double specialGoodChance) {
        SquareModel squareToReveal = GetOrGenerateSquare(position, specialBadChance, specialGoodChance);
        if (squareToReveal.Opened || squareToReveal.Flagged) {
            throw new InvalidOperationException($"Square at ({position.X}, {position.Y}) is already {(squareToReveal.Opened ? "opened" : "flagged")}");
        }
        RevealSquares(new Dictionary<Position, SquareModel>{{position, squareToReveal}}, CASCADE_LIMIT, specialBadChance, specialGoodChance);
    }

    public void RevealSquares(Dictionary<Position, SquareModel> squaresToReveal, int cascadeLimit, double specialBadChance, double specialGoodChance) {
        Dictionary<Position, SquareModel> cascadingSquares = new();
        foreach ((Position position, SquareModel squareToReveal) in squaresToReveal) {
            if (squareToReveal is SpecialSquareModel specialSquare) {
                specialSquare.Open();
                Listener.OnSquareUpdated(position, squareToReveal);
            } else if (squareToReveal is NumberSquareModel numberSquare) {
                Dictionary<Position, SquareModel> coveredSquares = GetOrGenerateCoveredSquares(position, numberSquare.Type, specialBadChance, specialGoodChance);
                int number = coveredSquares.Values.Where(s => s is SpecialSquareModel specialSquare && specialSquare.Type.IsBad).Count();
                numberSquare.Number = number;
                Listener.OnSquareUpdated(position, squareToReveal);

                if (number == 0 && cascadeLimit > 0) {
                    foreach ((Position p, SquareModel coveredSquare) in coveredSquares) {
                        if (!squaresToReveal.ContainsKey(p) && !cascadingSquares.ContainsKey(p) && !coveredSquare.Opened && !coveredSquare.Flagged) {
                            cascadingSquares.Add(p, coveredSquare);
                        }
                    }
                } else {
                    foreach ((Position p, SquareModel coveredSquare) in coveredSquares) {
                        Listener.OnSquareUpdated(p, coveredSquare);
                    }
                }

            } else {
                throw new InvalidOperationException($"Square at ({position.X}, {position.Y}) is of an unknown type, cannot open it");
            }
        }
        if (cascadingSquares.Any()) {
            RevealSquares(cascadingSquares, --cascadeLimit, SPECIAL_BAD_CHANCE, SPECIAL_GOOD_CHANCE);
        }
    }

    private void placeSquare(Position position, SquareModel square) {
        Dictionary<long, SquareModel> column;
        if (_squares.ContainsKey(position.X)) {
            column = _squares[position.X];
        } else {
            column = new();
            _squares[position.X] = column;
        }
        if (column.ContainsKey(position.Y)) {
            throw new ArgumentException($"There is already a square at ({position.X}, {position.Y})");
        }
        column[position.Y] = square;
    }
}