using System;
using System.IO;

public abstract class FileInterface<T> {
    private const string APP_DIR_NAME = "GodotSweeperInfinite";
    private static readonly string DIR_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), APP_DIR_NAME);

    private readonly string _path;

    private bool _loaded;
    private T _value;

    /// <summary>
    /// <para>The value managed by this file interface.</para>
    /// <para><b>NOTE: Do not modify the value returned by this, doing so will not save the new value. Instead, use <see cref="Modify"/></b></para>
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

    protected FileInterface(string path) {
        _path = Path.Combine(DIR_PATH, path);
    }

    public void With(Action<T> modification) {
        modification(Value);
        Save();
    }

    private void Load() {
        _loaded = true;
        if (File.Exists(_path)) {
            _value = FromBytes(File.ReadAllBytes(_path));
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
        if (!File.Exists(_path)) {
            if (!Directory.Exists(DIR_PATH)) {
                Directory.CreateDirectory(DIR_PATH);
            }
            File.Create(_path).Close();
        }
        T value = Value;
        File.WriteAllBytes(_path, ToBytes(value));
    }

    public abstract byte[] ToBytes(T value);

    public abstract T FromBytes(byte[] bytes);
}