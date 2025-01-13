using System.Collections.Generic;
using SmileyFace799.RogueSweeper.model;

namespace SmileyFace799.RogueSweeper.events {
    public interface IBoardUpdateEvent {}
    public record SquaresUpdatedEvent(Dictionary<Position, IImmutableSquare> Squares, byte Priority) : IBoardUpdateEvent {
        public const byte FLAGGED_PRIORITY = byte.MaxValue;
        public const byte OPENED_PRIORITY = byte.MaxValue - 1;
    }
    // SquaresUpdatedEvent is a both a board event & a UI event, and is defined in the UI events file

    public interface IBoardUpdateListener
    {
        public static IBoardUpdateListener Null => new BoardNullListener();
        void OnBoardUpdate(IBoardUpdateEvent @event);
    }

    class BoardNullListener : IBoardUpdateListener
    {
        public void OnBoardUpdate(IBoardUpdateEvent @event) {}
    }

    public abstract class BoardUpdateAdapter : IBoardUpdateListener
    {
        public void OnBoardUpdate(IBoardUpdateEvent @event)
        {
            if (@event is SquaresUpdatedEvent sEvent) {
                OnSquareUpdated(sEvent);
            }
        }

        public abstract void OnSquareUpdated(SquaresUpdatedEvent @event);
    }
}