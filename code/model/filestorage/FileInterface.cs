using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmileyFace799.RogueSweeper.filestorage {

public abstract class FileInterface {
    private const string APP_DIR_NAME = "GodotSweeperInfinite";
    private static readonly string DIR_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), APP_DIR_NAME);

    protected static string FullPath(string path) => Path.Combine(DIR_PATH, path);

    protected static void MakeDir() {
        if (!Directory.Exists(DIR_PATH)) {
            Directory.CreateDirectory(DIR_PATH);
        }
    }

    public static bool Exists(string path) => File.Exists(FullPath(path));

    public static bool ExistsAll(params string[] paths) {
        return paths.All(Exists);
    }

    public static void Delete(string path) {
        string fullPath = FullPath(path);
        if (File.Exists(fullPath)) {
            File.Delete(fullPath);
        }
    }

    public static void Delete(params string[] paths) {
        foreach (string path in paths) {
            Delete(path);
        }
    }
}

public abstract class FileInterface<T> : FileInterface {
    private readonly string _path;

    private bool _loaded;
    private T _value;

    /// <summary>
    /// <para>The value managed by this file interface.</para>
    /// <para><b>NOTE: Do not modify the value returned by this, doing so will not save the new value. Instead, use <see cref="With"/></b></para>
    /// </summary>
    protected virtual T Value { get {
        if (!_loaded) {
            Load();
        }
        return _value;
    } set {
        _value = value;
        _loaded = true;
        Save();
    }}

    /// <summary>
    /// The default value to produce if there is nothing stored.
    /// </summary>
    protected virtual T Default => default;

    /// <summary>
    /// Creates a new interface to a file at the specified path. The file is not immediately created, only being made the first time the value is saved.
    /// If the value is accessed before being saved, <see cref="Default"/> is returned.
    /// </summary>
    /// <param name="path">The path to the file used by this interface for persistent storage</param>
    protected FileInterface(string path) {
        _path = path;
    }

    /// <summary>
    /// Perform an action with the saved value. It is safe to modify the value here, as the value will be saved after modification.
    /// </summary>
    /// <param name="modification"></param>
    public void With(Action<T> modification) {
        modification(Value);
        Save();
    }

    /// <summary>
    /// Sets the stored value, and saves it.
    /// </summary>
    /// <param name="value">The new value</param>
    public void SetValue(T value) {
        Value = value;
    }

    public void ResetValue() {
        Value = Default;
    }

    private void Load() {
        _loaded = true;
        if (Exists(_path)) {
            _value = FromBytes(new ByteEnumerator(File.ReadAllBytes(FullPath(_path))));
            try {
            } catch (Exception e) {
                _value = Default;
                Console.WriteLine("Failed to load value, resorting to default. Exception: ", e);
            }
        } else {
            _value = Default;
        }
    }

    private void Save() {
        if (!Exists(_path)) {
            MakeDir();
            File.Create(FullPath(_path)).Close();
        }
        T value = Value;
        File.WriteAllBytes(FullPath(_path), ToBytes(value));
    }

    /// <summary>
    /// Serializes the stored value into bytes.
    /// </summary>
    /// <param name="value">The stored value to serialize</param>
    /// <returns>A byte array, with the store value serialized</returns>
    public abstract byte[] ToBytes(T value);

    /// <summary>
    /// Creates a value from the saved bytes.
    /// </summary>
    /// <param name="bytes">The saved bytes to create a value from, provided in a <see cref="ByteEnumerator"/></param>
    /// <returns>The created value</returns>
    public abstract T FromBytes(ByteEnumerator bytes);

    /// <summary>
    /// <para> An enumerator made specifically for bytes, able to select batches of bytes instead of going one-by-one.</para>
    /// <para>This is functionally a wrapper class for <c>IEnumerator&lt;byte&gt;</c>,
    /// and it uses the enumerator returned from <c>byte[].AsEnumerable().GetEnumerator()</c>.</para>
    /// </summary>
    public class ByteEnumerator : IEnumerator<byte> {
        private readonly IEnumerator<byte> _baseEnumerator;

        /// <summary>
        /// The current byte the enumerator is on.
        /// </summary>
        public byte Current => _baseEnumerator.Current;

        object IEnumerator.Current => ((IEnumerator) _baseEnumerator).Current;

        public ByteEnumerator(byte[] bytes) {
            _baseEnumerator = bytes.AsEnumerable().GetEnumerator();
        }

        /// <summary>
        /// Moves the current position of the enumerator to the next byte, and returns it.
        /// </summary>
        /// <returns>The next stored byte</returns>
        /// <exception cref="InvalidDataException">If the enumerator is already on the last byte</exception>
        public byte Next() {
            if (!MoveNext()) {
                throw new InvalidDataException("Out of bytes!");
            }
            return Current;
        }

        /// <summary>
        /// Moves the current position of the enumerator forward a specified amount of times,
        /// returning an array of all the bytes passed, plus the last byte it lands on.
        /// </summary>
        /// <param name="amount">The amount of bytes to move forward</param>
        /// <returns>A byte array with the bytes passed, plus the new current byte, so that <c>byte[].Length == amount</c></returns>
        /// <exception cref="InvalidDataException">If there are less bytes left than the amount to return</exception>
        public byte[] Next(int amount) {
            byte[] next = new byte[amount];
            for (int i = 0; i < amount; ++i) {
                next[i] = Next();
            }
            return next;
        }

        public void Dispose() {
            _baseEnumerator.Dispose();
        }

        private bool MoveNext() {
            return _baseEnumerator.MoveNext();
        }

        bool IEnumerator.MoveNext() {
            return MoveNext();
        }

        public void Reset() {
            _baseEnumerator.Reset();
        }
    }
}
}