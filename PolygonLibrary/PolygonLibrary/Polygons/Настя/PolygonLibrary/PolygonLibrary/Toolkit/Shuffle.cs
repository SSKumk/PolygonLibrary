using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolygonLibrary.Toolkit
{
  public abstract partial class Tools
  {

    /// <summary>
    /// Used in Shuffle(T).
    /// </summary>
    static Random _random = new Random();

    /// <summary>
    /// Shuffle the array using the Durstenfeld's algorithm
    /// </summary>
    /// <typeparam name="T">Array element type</typeparam>
    /// <param name="array">Array to shuffle</param>
    /// <param name="ownRnd">A random generator. If is not passed, some internal generator will be used</param>
    static public void Shuffle<T>(List<T> array, Random ownRnd = null)
    {
      int n = array.Count;
      Random rnd = ownRnd == null ? _random : ownRnd;
      for (int i = 0; i < n; i++)
      {
        int r = i + (int)(rnd.NextDouble() * (n - i));
        T t = array[r];
        array[r] = array[i];
        array[i] = t;
      }
    }
  }
}