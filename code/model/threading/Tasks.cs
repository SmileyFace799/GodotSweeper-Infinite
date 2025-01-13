using System;
using System.Threading;

namespace SmileyFace799.RogueSweeper.Threading {
    /// <summary>
    /// Handles asynchroneous queuing of tasks using a threadpool.
    /// </summary>
    public static class Tasks {
        /// <summary>
        /// Queue a task for asynchroneous execution.
        /// </summary>
        /// <param name="action">The action to queue</param>
        public static void Queue(Action action) {
            ThreadPool.QueueUserWorkItem(_ => action());
        }
    }
}