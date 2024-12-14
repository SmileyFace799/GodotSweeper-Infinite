using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class BoardModel {
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

    private static double GetBadChance(Position position) {
        double x = position.Abs;
        return Math.Log(x / 6 + 1) / 16 + Math.Sin(x / 5 + Math.PI) / 25 + 1 / (x + 10) - 0.95 + Math.Pow(1.001, 0.005 * x * x);
    }
    private static double GetGoodChance(Position position) => 1 / (50 * position.Abs + 1000);

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

    private SquareModel GenerateSquare(Position position, double badChanceModifier, double goodChanceModifier) {
        return GenerateSquare(position, GetBadChance(position) + badChanceModifier, GetGoodChance(position) + goodChanceModifier);
    }

    private SquareModel GenerateSquare(
        Position position,
        SquareGenData squareGenData
    ) {
        SquareModel generatedSquare;
        double randomNum = _random.NextDouble();
        double badChance = squareGenData.GetBadChance(position);
        double goodChance = squareGenData.GetGoodChance(position);
        if (randomNum < badChance) {
            generatedSquare = new SpecialSquareModel(SpecialSquareType.GetRandomBad());
        } else if (randomNum - badChance < goodChance) {
            generatedSquare = new SpecialSquareModel(SpecialSquareType.GetRandomGood());
        } else {
            generatedSquare = new NumberSquareModel(NumberSquareType.GetRandom());
        }
        return generatedSquare;
    }

    private SquareModel GetOrGenerateSquare(Position position, SquareGenData squareGenData) {
        SquareModel square;
        if (HasSquare(position)) {
            square = GetSquare(position);
        } else {
            square = GenerateSquare(position, squareGenData);
            placeSquare(position, square);
        }
        return square;
    }

    public Dictionary<Position, SquareModel> GetOrGenerateCoveredSquares(
        Position position,
        NumberSquareType numberSquareType,
        SquareGenData squareGenData
    ) {
        return numberSquareType.RelativeCoverage.ToDictionary(p => position + p, p => GetOrGenerateSquare(position + p, squareGenData));
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

    public void RevealSquare(Position position, SquareGenData cascadeGenData) {
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
    public void RevealSquare(Position position, SquareGenData squareGenData, SquareGenData cascadeGenData) {
        SquareModel squareToReveal = GetOrGenerateSquare(position, squareGenData);
        if (squareToReveal.Opened || squareToReveal.Flagged) {
            throw new InvalidOperationException($"Square at ({position.X}, {position.Y}) is already {(squareToReveal.Opened ? "opened" : "flagged")}");
        }
        RevealSquares(new Dictionary<Position, SquareModel>{{position, squareToReveal}}, CASCADE_LIMIT, squareGenData, cascadeGenData);
    }

    public void RevealSquares(Dictionary<Position, SquareModel> squaresToReveal, int cascadeLimit, SquareGenData cascadeGenData) {
        RevealSquares(squaresToReveal, cascadeLimit, cascadeGenData, cascadeGenData);
    }

    private void RevealSquares(Dictionary<Position, SquareModel> squaresToReveal, int cascadeLimit, SquareGenData squareGenData, SquareGenData cascadeGenData) {
        Dictionary<Position, SquareModel> cascadingSquares = new();
        foreach ((Position position, SquareModel squareToReveal) in squaresToReveal) {
            if (squareToReveal is SpecialSquareModel specialSquare) {
                specialSquare.Open();
                GuiListener.OnSquareUpdated(position, squareToReveal);
            } else if (squareToReveal is NumberSquareModel numberSquare) {
                Dictionary<Position, SquareModel> coveredSquares = GetOrGenerateCoveredSquares(position, numberSquare.Type, squareGenData);
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
            RevealSquares(cascadingSquares, --cascadeLimit, cascadeGenData);
        }
    }

    public interface SquareGenData {
        protected static double GetRawBadChance(Position position) {
            double x = position.Abs;
            return Math.Log(x / 6 + 1) / 16 + Math.Sin(x / 5 + Math.PI) / 25 + 1 / (x + 10) - 0.95 + Math.Pow(1.001, 0.005 * x * x);
        }
        protected static double GetRawGoodChance(Position position) => 1 / (10 * position.Abs + 200) + 0.2;

        double GetBadChance(Position position);
        double GetGoodChance(Position position);
    }

    public record RelativeGenData(double BadChanceModifier=0, double GoodChanceModifier=0) : SquareGenData {
        public double GetBadChance(Position position) {
            return SquareGenData.GetRawBadChance(position) + BadChanceModifier;
        }

        public double GetGoodChance(Position position) {
            return SquareGenData.GetRawGoodChance(position) + GoodChanceModifier;
        }
    };

    public record StaticGenData(double? BadChance=null, double? GoodChance=null) : SquareGenData {
        public double GetBadChance(Position position) {
            return BadChance == null ? SquareGenData.GetRawBadChance(position) : (double) BadChance;
        }

        public double GetGoodChance(Position position) {
            return GoodChance == null ? SquareGenData.GetRawGoodChance(position) : (double) GoodChance;
        }
    };
}