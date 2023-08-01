using System;
using System.Collections.Generic;

namespace PolygonLibrary.Toolkit;

public abstract partial class Tools
{

  /// <summary>
  /// Used in Shuffle(T).
  /// </summary>
  private static readonly Random _random = new Random();

  /// <summary>
  /// Shuffle the array using the Durstenfeld's algorithm
  /// </summary>
  /// <typeparam name="T">Array element type</typeparam>
  /// <param name="array">Array to shuffle</param>
  /// <param name="ownRnd">A random generator. If is not passed, some internal generator will be used</param>
  public static void Shuffle<T>(List<T> array, Random? ownRnd = null)
  {
    int n = array.Count;
    Random rnd = ownRnd ?? _random;
    for (int i = 0; i < n; i++)
    {
      // int r = i + (int)(rnd.NextDouble() * (n - i));
      int r = rnd.Next(0, i + 1);
      (array[r], array[i]) = (array[i], array[r]);
    }
  }
}