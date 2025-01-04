using System;

namespace SmileyFace799.RogueSweeper.model
{
    public class PowerUp : Switchable
    {
        public static readonly PowerUp SOLVER_SMALL = new(SolverAction, new Position(0, 0));
        public static readonly PowerUp SOLVER_MEDIUM = new(SolverAction, Position.Array2D(-1, -1, 3, 3));
        public static readonly PowerUp SOLVER_LARGE = new(SolverAction, Position.Array2D(-2, -2, 5, 5));
        public static readonly PowerUp DEFUSER = new((board, affectedTiles, usePos, genData) => {
            foreach(Position p in Position.Shift(usePos, affectedTiles)) {
                if (board.GetOrGenerateSquare(p, genData).Type.Level == TypeLevel.BAD) {
                    board.placeSquare(p, new NumberSquare(NumberSquareType.DEFUSED_BOMB), true, false);
                    board.RevealSquare(p, genData);
                }
            }
        }, new Position(0, 0));

        public Action<Board, Position, SquareGenData> Action {get;}

        private static void SolverAction(Board board, Position[] affectedTiles, Position usePos, SquareGenData genData) {
            foreach(Position p in Position.Shift(usePos, affectedTiles)) {
                Square square = board.GetOrGenerateSquare(p, genData);
                if (!square.Opened) {
                    if (square.Type.Level == TypeLevel.BAD) {
                        if (!square.Flagged) {
                            board.FlagSquare(p);
                        }
                    } else {
                        if (square.Flagged) {
                            board.FlagSquare(p);
                        }
                        board.RevealSquare(p, genData);
                    }
                }
            }
        }

        private PowerUp(Action<Board, Position[], Position, SquareGenData> action, params Position[] affectedTiles)
        {
            Action = (b, p, g) => action(b, affectedTiles, p, g);
        }
    }
}