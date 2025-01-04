using System.Collections.Generic;
using SmileyFace799.RogueSweeper.model;

namespace SmileyFace799.RogueSweeper.events
{
    /// <summary>
    /// A generic event meant to inform any UI event receivers about a change in the model.
    /// 
    /// </summary>
    public interface IUIUpdateEvent {}

    /// <summary>
    /// <para>Fired when a new game is loaded.</para>
    /// <para><b>NOTE: Since the square dictionary can be really big,
    /// it is not made immutable to save loading time.
    /// It is technically safe to modify the dictionary,
    /// although it still should not be modified, as that breaks cohesion</b></para>
    /// </summary>
    /// <param name="stats">THe stats loaded for the game</param>
    /// <param name="squares">The squares present in the loaded board</param>
    public record NewGameLoadedEvent(ImmutableStats Stats, Dictionary<long, Dictionary<long, Square>> Squares) : IUIUpdateEvent {}
    public record SquareUpdatedEvent(Position Position, ImmutableSquare Square) : IUIUpdateEvent {}
    public record LivesUpdatedEvent(int Lives, int LivesGained, int LivesLost) : IUIUpdateEvent {}
    public record BadChanceModifierUpdatedEvent(double BadChanceModifier) : IUIUpdateEvent {}
    public record OpenedSquaresUpdatedEvent(ulong OpenedSquares) : IUIUpdateEvent {}
    public record SmallSolversUpdatedEvent(uint SmallSolvers) : IUIUpdateEvent {}
    public record MediumSolversUpdatedEvent(uint MediumSolvers) : IUIUpdateEvent {}
    public record LargeSolversUpdatedEvent(uint LargeSolvers) : IUIUpdateEvent {}
    public record DefusersUpdatedEvent(uint Defusers) : IUIUpdateEvent {}
    public record PowerUpDeselectedEvent() : IUIUpdateEvent {}
}