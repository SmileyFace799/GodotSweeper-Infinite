using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class BoardModel {
    public const double SPECIAL_BAD_CHANCE = 0.25;
    public const double SPECIAL_GOOD_CHANCE = 0.00;
    public const int CASCADE_LIMIT = 50;

    private readonly Random _random = new();
    private readonly Dictionary<long, Dictionary<long, SquareModel>> _squares;

    public BoardUpdateListener GuiListener {get; set;}

    public BoardModel() {
        _squares = new();
    }

    public BoardModel(Dictionary<long, Dictionary<long, SquareModel>> squares) {
        _squares = squares;
    }

    public HashSet<SquareModel> Squares() {
        return _squares.Values.SelectMany(v => v.Values).ToHashSet();
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

    public bool HasSquare(Position position) {
        return _squares.ContainsKey(position.X) && _squares[position.X].ContainsKey(position.Y);
    }

    /// <summary>
	/// Utility function to check if a square at a specified position satisfies a specified condition.
	/// </summary>
	/// <param name="position">The position of the square to check</param>
	/// <param name="ifNotFound">The default result if the square doesn't exist</param>
	/// <param name="checker">The condition to check if the square exists</param>
	/// <returns>If the condition is satisfied or not, or the default result if the square didn't exist</returns>
	private bool SquareState(Position position, bool ifNotFound, Func<SquareModel, bool> checker) {
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
	public bool IsSmartRevealable(Position position) => SquareState(position, false, s => s.Opened && s is NumberSquareModel);

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

    public ReadOnlyDictionary<long, Dictionary<long, SquareModel>> GetSquares() {
        return new(_squares);
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
        GuiListener.OnSquareUpdated(position, squareToFlag);
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
                GuiListener.OnSquareUpdated(position, squareToReveal);
            } else if (squareToReveal is NumberSquareModel numberSquare) {
                Dictionary<Position, SquareModel> coveredSquares = GetOrGenerateCoveredSquares(position, numberSquare.Type, specialBadChance, specialGoodChance);
                int number = coveredSquares.Values.Where(s => s is SpecialSquareModel specialSquare && specialSquare.Type.IsBad).Count();
                numberSquare.Number = number;
                GuiListener.OnSquareUpdated(position, squareToReveal);

                if (number == 0 && cascadeLimit > 0) {
                    foreach ((Position p, SquareModel coveredSquare) in coveredSquares) {
                        if (!squaresToReveal.ContainsKey(p) && !cascadingSquares.ContainsKey(p) && !coveredSquare.Opened && !coveredSquare.Flagged) {
                            cascadingSquares.Add(p, coveredSquare);
                        }
                    }
                } else {
                    foreach ((Position p, SquareModel coveredSquare) in coveredSquares) {
                        GuiListener.OnSquareUpdated(p, coveredSquare);
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
}