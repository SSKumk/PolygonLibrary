using System;
using System.Numerics;

namespace CGLibrary;

/// <summary>
/// Represents a linear congruential random number generator.
/// </summary>
public class RandomLC {

  /// <summary>
  /// Multiplier
  /// </summary>
  private const uint a = 1664525;

  /// <summary>
  /// Increment
  /// </summary>
  private const uint c = 1013904223;

  /// <summary>
  /// Gets current value of generator.
  /// </summary>
  /// <returns>The current state-value.</returns>
  public uint Seed { get; private set; }

  /// <summary>
  /// Generates the next random number based on the current seed value.
  /// </summary>
  /// <returns>The generated random number.</returns>
  internal uint Rand() {
    Seed = Seed * a + c;

    return Seed;
  }

  /// <summary>
  /// Initializes a new instance of the RandomLC class with an optional seed value.
  /// </summary>
  /// <param name="initSeed">The initial seed value. If not provided, the current system time is used.</param>
  public RandomLC(uint? initSeed = null) {
    if (initSeed is null) {
      Seed = (uint)DateTime.Now.Ticks;
    } else {
      Seed = (uint)initSeed;
    }
  }

  /// <summary>
  /// Generates the next random integer within the specified range [a = 0, b = int.MaxValue].
  /// </summary>
  /// <param name="lb">The lower bound of the range (inclusive).</param>
  /// <param name="rb">The upper bound of the range (inclusive).</param>
  /// <returns>The generated random integer.</returns>
  public int NextInt(int lb = 0, int rb = int.MaxValue) => (int)(Rand() % int.MaxValue) % (rb - lb + 1) + lb;

  /// <summary>
  /// Generates the next random double within the specified range [a = 0, b = 1).
  /// </summary>
  /// <param name="lb">The lower bound of the range (inclusive).</param>
  /// <param name="rb">The upper bound of the range (exclusive).</param>
  /// <returns>The generated random double.</returns>
  public double NextDouble(double lb = 0, double rb = 1) => Rand() * (rb - lb) / uint.MaxValue + lb;

}

public partial class Geometry<TNum, TConv> where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Represents a linear congruential random precise-number generator.
  /// </summary>
  public class GRandomLC : RandomLC {

    /// <summary>
    /// Initializes a new instance of the GRandomLC class.
    /// </summary>
    /// <param name="initSeed">The initial seed value for the random number generator. If null, a random seed will be used.</param>
    public GRandomLC(uint? initSeed = null) : base(initSeed) { }

    /// <summary>
    /// Maximal value of the UInt expressed in TNum-type.
    /// </summary>
    private readonly TNum MaxValue = TConv.FromUInt(uint.MaxValue);

    /// <summary>
    /// Generates the next random precise-number within the [0,1) range.
    /// </summary>
    /// <returns>The generated random precise number.</returns>
    public TNum NextPrecise() => NextPrecise(Tools.Zero, Tools.One);

    /// <summary>
    /// Generates the next random precise-number within the specified range.
    /// </summary>
    /// <param name="lb">The lower bound of the range (inclusive).</param>
    /// <param name="rb">The upper bound of the range (exclusive).</param>
    /// <returns>The generated random precise number.</returns>
    public TNum NextPrecise(TNum lb, TNum rb) => TConv.FromUInt(Rand()) / MaxValue * (rb - lb) + lb;

  }

}
