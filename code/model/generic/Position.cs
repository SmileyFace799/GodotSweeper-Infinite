using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;

/// <summary>
/// A 2D position, with a read-only X & Y value.
/// </summary>
public struct Position {
    [SetsRequiredMembers]
    public Position(long x, long y) {
        X = x;
        Y = y;
    }
    public required long X {get; init;}
    public required long Y {get; init;}

    public static Position operator + (Position p1, Position p2) {
        return new(p1.X + p2.X, p1.Y + p2.Y);
    }

    public static Position operator - (Position p1, Position p2) {
        return new(p1.X - p2.X, p1.Y - p2.Y);
    }

    public static Position operator * (Position p, long n) {
        return new(p.X * n, p.Y * n);
    }

    public static Position operator * (long n, Position p) {
        return p * n;
    }

    public static Position operator / (Position p, long n) {
        return new(p.X / n, p.Y / n);
    }

    public override string ToString(){
        return $"Position{{X={X}, Y={Y}}}";
    }
};