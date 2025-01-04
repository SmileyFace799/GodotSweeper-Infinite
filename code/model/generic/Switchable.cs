using System;
using System.Collections.Generic;

namespace SmileyFace799.RogueSweeper.model
{
    public abstract class Switchable {
         public void Switch(Dictionary<Switchable, Action> actions) => Switch(actions, () => {});

        /// <summary>
        /// A "switch case" for square types, since they're technically not actual enums & therefore can't be used in regular switch cases.
        /// Only one action will be executed, fall-through is not supported.
        /// </summary>
        /// <param name="actions">A dictionary of switch cases</param>
        public void Switch(Dictionary<Switchable, Action> actions, Action @default) => actions.GetValueOrDefault(this, @default)();

        public R Switch<R>(Dictionary<Switchable, R> values, R @default) => values.GetValueOrDefault(this, @default);
    }
}