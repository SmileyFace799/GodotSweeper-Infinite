using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Godot;
using SmileyFace799.RogueSweeper.model;

namespace SmileyFace799.RogueSweeper.Godot
{
    /// <summary>
    /// Will take tasks from other threads & queue them for execution on the main Godot thread.
    /// </summary>
    public partial class GDThread : Node
    {
        private const ulong PER_FRAME_TIME_LIMIT = 50; // ms (Rxuivalent to 40fps at worst)
        private static readonly ConcurrentDictionary<int, ConcurrentQueue<Action>> TASKS = new();
        private static bool _quitSignalReceived;

        public override void _Ready()
        {
            ProcessMode = ProcessModeEnum.Always;
            GetTree().AutoAcceptQuit = false;
        }

        public override void _Notification(int what)
        {
            if (what == NotificationWMCloseRequest) {
                _quitSignalReceived = true;
            }
        }

        public override void _Process(double delta)
        {
            if (Game.Instance.CanQuitSafely && _quitSignalReceived) {
                GetTree().Quit();
            } else {
                ulong timeNow = Time.GetTicksMsec();
                while (TASKS.Count > 0 && Time.GetTicksMsec() - timeNow < PER_FRAME_TIME_LIMIT) {
                    int mostImportantIndex = TASKS.Keys.Max();
                    ConcurrentQueue<Action> mostImportantQueue = TASKS.GetValueOrDefault(mostImportantIndex, new());
                    while (mostImportantQueue.Count > 0 && Time.GetTicksMsec() - timeNow < PER_FRAME_TIME_LIMIT) {
                        Action a;
                        if (mostImportantQueue.TryDequeue(out a)) {
                            try {
                                a();
                            } catch (ObjectDisposedException) {
                                // Ignore the task
                            }
                        }
                    }
                    if (mostImportantQueue.Count == 0) {
                        TASKS.Remove(mostImportantIndex, out _);
                    }
                }
            }
        }

        public override void _ExitTree()
        {
            
        }

        /// <summary>
        /// Adds a task to be executed by the Godot main thread as soon as possible. This method is threadsafe.
        /// </summary>
        /// <param name="action">The actuon to be executed by the Godot main thread</param>
        public static void QueueTask(int priority, Action action) {
            if (priority != TaskPriority.PRINT) {
                //Print(priority);
            }
            TASKS.GetOrAdd(priority, p => new()).Enqueue(action);
        }

        public static void Print(params object[] objs) {
            QueueTask(TaskPriority.PRINT, () => GD.Print(string.Join(" | ", objs)));
        }

        public static void ClearTasks() => TASKS.Clear();
    }

    public static class TaskPriority
    {
        public const int PRINT = int.MaxValue;
        public const int SCENE_CHANGE = 100;
        public const int UI_UPDATE = 99;
        public const int SQUARE_LOADED = 1;
        public const int SQUARE_UPDATE_BASE = -byte.MaxValue;
    }
}