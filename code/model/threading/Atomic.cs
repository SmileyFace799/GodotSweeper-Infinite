namespace SmileyFace799.RogueSweeper.Threading
{
    /// <summary>
    /// Provides read & write access to an atomic variable.
    /// </summary>
    /// <typeparam name="T">An immutable type</typeparam>
    public class Atomic<T>
    {   
        protected readonly object _lock = new();
        protected T _value;
        
        public T Get() {
            lock (_lock) {
                return _value;
            }
        }

        public void Set(T value) {
            lock (_lock) {
                _value = value;
            }
        }

        public Atomic() {}
        public Atomic(T value) => Set(value);
    }
}