using SmileyFace799.RogueSweeper.model;

namespace SmileyFace799.RogueSweeper.events {
public interface IBoardUpdateEvent {}
public record SquareEvent(Position Position, ImmutableSquare Square) : IBoardUpdateEvent {}
public class ResetEvent : IBoardUpdateEvent {}
}