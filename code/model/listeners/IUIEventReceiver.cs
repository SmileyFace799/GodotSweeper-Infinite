namespace SmileyFace799.RogueSweeper.events {
/// <summary>
/// An interface for any object that wants to receive UI events.
/// </summary>
public interface IUIEventReceiver {
    void OnUpdateUI(IUIUpdateEvent @event);

    public static IUIEventReceiver Null => new UINullReceiver();
}

class UINullReceiver : IUIEventReceiver {
    public void OnUpdateUI(IUIUpdateEvent @event) {}
}

public interface UIEventAdapter : IUIEventReceiver {
    new public sealed void OnUpdateUI(IUIUpdateEvent @event) {
        switch (@event) {
            case NewGameLoadedEvent nglEvent:
                OnNewGameLoaded(nglEvent);
                break;
            case SquareUpdatedEvent suEvent:
                OnSquareUpdated(suEvent);
                break;
            case LivesUpdatedEvent luEvent:
                OnLivesUpdated(luEvent);
                break;
            case BadChanceModifierUpdatedEvent bcmuEvent:
                OnBadChanceModifierUpdated(bcmuEvent);
                break;
            case OpenedSquaresUpdatedEvent osuEvent:
                OnOpenedSquaresUpdated(osuEvent);
                break;
        }
    }

    public abstract void OnNewGameLoaded(NewGameLoadedEvent @event);
    public abstract void OnSquareUpdated(SquareUpdatedEvent @event);
    public abstract void OnLivesUpdated(LivesUpdatedEvent @event);
    public abstract void OnBadChanceModifierUpdated(BadChanceModifierUpdatedEvent @event);
    public abstract void OnOpenedSquaresUpdated(OpenedSquaresUpdatedEvent @event);
}
}