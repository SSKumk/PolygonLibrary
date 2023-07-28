using System;
using System.Diagnostics;

namespace PolygonLibrary.Toolkit;

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
  /// Current value
  /// </summary>
  private uint seed;

  /// <summary>
  /// Gets current value of generator.
  /// </summary>
  /// <returns>The current state-value.</returns>
  public uint GetSeed() { return seed; }

  /// <summary>
  /// Generates the next random number based on the current seed value.
  /// </summary>
  /// <returns>The generated random number.</returns>
  private uint Rand() {
    seed = seed * a + c;

    return seed;
  }

  /// <summary>
  /// Initializes a new instance of the RandomLC class with an optional seed value.
  /// </summary>
  /// <param name="initSeed">The initial seed value. If not provided, the current system time is used.</param>
  public RandomLC(uint? initSeed = null) {
    if (initSeed is null) {
      seed = (uint)DateTime.Now.Ticks;
    } else {
      seed = (uint)initSeed;
    }
  }

  /// <summary>
  /// Generates the next random integer within the specified range [a = 0, b = int.MaxValue].
  /// </summary>
  /// <param name="lb">The lower bound of the range (inclusive).</param>
  /// <param name="rb">The upper bound of the range (inclusive).</param>
  /// <returns>The generated random integer.</returns>
  public int NextInt(int lb = 0, int rb = int.MaxValue) { return (int)(Rand() % int.MaxValue) % (rb - lb + 1) + lb; }

  /// <summary>
  /// Generates the next random double within the specified range [a = 0, b = 1).
  /// </summary>
  /// <param name="lb">The lower bound of the range (inclusive).</param>
  /// <param name="rb">The upper bound of the range (exclusive).</param>
  /// <returns>The generated random double.</returns>
  public double NextDouble(double lb = 0, double rb = 1) { return Rand() / (double)uint.MaxValue * (rb - lb) + lb; }

}

