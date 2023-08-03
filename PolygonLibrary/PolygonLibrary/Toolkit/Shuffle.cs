using System;
using System.Collections.Generic;

namespace PolygonLibrary.Toolkit;

public abstract partial class Tools
{

  /// <summary>
  /// Used in Shuffle(T).
  /// </summary>
  private static readonly RandomLC _random = new RandomLC();

  /// <summary>
  /// Shuffle the array using the Durstenfeld's algorithm
  /// </summary>
  /// <typeparam name="T">Array element type</typeparam>
  /// <param name="array">Array to shuffle</param>
  /// <param name="ownRnd">A random generator. If is not passed, some internal generator will be used</param>
  public static void Shuffle<T>(List<T> array, RandomLC? ownRnd = null)
  {
    int n = array.Count;
    RandomLC rnd = ownRnd ?? _random;
    for (int i = 0; i < n; i++)
    {
      // int r = i + (int)(rnd.NextDouble() * (n - i));
      int r = rnd.NextInt(0, i);
      (array[r], array[i]) = (array[i], array[r]);
    }
  }
}