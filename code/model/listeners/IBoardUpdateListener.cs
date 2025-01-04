
namespace SmileyFace799.RogueSweeper.events
{
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
            if (@event is SquareEvent sEvent) {
                OnSquareUpdated(sEvent);
            }
        }

        public abstract void OnSquareUpdated(SquareEvent @event);
    }
}