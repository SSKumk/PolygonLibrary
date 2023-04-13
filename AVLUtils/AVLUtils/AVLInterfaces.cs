using System.Collections.Generic;

namespace AVLUtils
{
  /// <summary>
  /// Interface that expands capabilities of an enumerable object by adding methods
  /// for getting reverse enumerator and cyclic enumerators (for both direct and reverse
  /// traversing)
  /// </summary>
  /// <typeparam name="T">The type of the stored objects</typeparam>
  public interface IMultiEnumerable<T> : IEnumerable<T>
  {
    /// <summary>
    /// Returns an enumerator that iterates directly through the collection put at the given value or after it 
    /// (if there is no such a value in the collection)
    /// </summary>
    /// <param name="v">The value the enumerator to be put on</param>
    /// <returns>An enumerator that can be used to iterate directly through the collection</returns>
    IEnumerator<T> GetEnumerator (T v);

    /// <summary>
    /// Returns an enumerator that reversely iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that reversely iterates through the collection</returns>
    IEnumerator<T> GetReverseEnumerator ();

    /// <summary>
    /// Returns an enumerator that iterates reversely through the collection put at the given value or before it 
    /// (if there is no such a value in the collection)
    /// </summary>
    /// <param name="v">The value the enumerator to be put on</param>
    /// <returns>An enumerator that can be used to iterate reversely through the collection</returns>
    IEnumerator<T> GetReverseEnumerator (T v);

    /// <summary>
    /// Returns an enumerator that directly iterates through the collection regarding it as a cycled one
    /// </summary>
    /// <returns>An enumerator that directly iterates through the collection regarding it as a cycled one</returns>
    IEnumerator<T> GetCyclicEnumerator ();

    /// <summary>
    /// Returns an enumerator that directly iterates through the collection regarding it as a cycled one;
    /// initially the enumerator is put to the given value or (if it is absent) to minimal value cyclically 
    /// greater than the given one
    /// </summary>
    /// <param name="v">The value the enumerator to be put on</param>
    /// <returns>An enumerator that directly iterates through the collection regarding it as a cycled one</returns>
    IEnumerator<T> GetCyclicEnumerator (T v);

    /// <summary>
    /// Returns an enumerator that reversely iterates through the collection regarding it as a cycled one
    /// </summary>
    /// <returns>An enumerator that reversely iterates through the collection regarding it as a cycled one</returns>
    IEnumerator<T> GetCyclicReverseEnumerator ();

    /// <summary>
    /// Returns an enumerator that reversely iterates through the collection regarding it as a cycled one;
    /// initially the enumerator is put to the given value or (if it is absent) to maximal value cyclically 
    /// less than the given one
    /// </summary>
    /// <param name="v">The value the enumerator to be put on</param>
    /// <returns>An enumerator that iterates reversely through the collection regarding it as a cycled one</returns>
    IEnumerator<T> GetCyclicReverseEnumerator (T v);
  }

  /// <summary>
  /// Interface defining capabilities of a tree class
  /// </summary>
  /// <typeparam name="T">The type of the stored objects</typeparam>
  public interface IAVLTree<T> : IMultiEnumerable<T>, ICollection<T>
  {
    /// <summary>
    /// Comparer defining the order in the tree
    /// </summary>
    IComparer<T> comparer { get; }

    /// <summary>
    /// Gets a value indicating whether this tree is empty.
    /// </summary>
    /// <value><c>true</c> if empty; otherwise, <c>false</c>.</value>
    bool IsEmpty { get; }

    /// <summary>
    /// Indexer
    /// </summary>
    /// <param name="i">Index</param>
    /// <returns>The value with the index i</returns>
    T this[int i] { get; }

    /// <summary>
    /// Get a reference to the object by its value
    /// </summary>
    /// <param name="v">The value to be found</param>
    /// <param name="res">Reference to the found object: the object itself if found, default value otherwise</param>
    /// <returns>true if the object has been found, false otherwise</returns>
    bool Find (T v, out T? res);

    /// <summary>
    /// Take the minimal value in the tree
    /// </summary>
    /// <returns>The minimal value</returns>
    T Min ();

    /// <summary>
    /// Take the maximal value in the tree
    /// </summary>
    /// <returns>The maximal value</returns>
    T Max ();

    /// <summary>
    /// Take the minimal value in the tree and remove it
    /// </summary>
    /// <returns>The value</returns>
    T Pop ();

    /// <summary>
    /// Take the maximal value in the tree and remove it
    /// </summary>
    /// <returns>The value</returns>
    T Pop_Back ();

    /// <summary>
    /// Take the value following after the given one
    /// </summary>
    /// <param name="v">The given value</param>
    /// <param name="next">The next value</param>
    /// <returns>true, if the next value is taken successfully; 
    /// false, otherwise (the given value is maximal)</returns>
    bool Next (T v, out T? next);

    /// <summary>
    /// Take the value cyclically following after the given one
    /// </summary>
    /// <param name="v">The given value</param>
    /// <param name="next">The next value</param>
    /// <returns>true, if the next value is taken successfully; 
    /// false, otherwise (the given value is maximal)</returns>
    bool CyclicNext (T v, out T? next);

    /// <summary>
    /// Take the value previous to the given one
    /// </summary>
    /// <param name="v">The given value</param>
    /// <param name="prev">The previous value</param>
    /// <returns>true, if the previous value is taken successfully; false, otherwise</returns>
    bool Prev (T v, out T? prev);

    /// <summary>
    /// Take the value cyclically previous to the given one
    /// </summary>
    /// <param name="v">The given value</param>
    /// <param name="prev">The previous value</param>
    /// <returns>true, if the previous value is taken successfully; false, otherwise</returns>
    bool CyclicPrev (T v, out T? prev);
  }

  /// <summary>
  /// Interface introducing methods for a sorted container with changeable comparer
  /// </summary>
  /// <typeparam name="T">The type of the stored objects</typeparam>
  public interface IUnsafeContainer<out T>
  {
    /// <summary>
    /// Checks consistency of the current structure of the tree
    /// </summary>
    /// <returns>true, if the order</returns>
    bool CheckConsistency ();

    /// <summary>
    /// Rebuild the tree according to the current comparer
    /// </summary>
    void Rebuild ();

    /// <summary>
    /// Set a new comparer for the tree. It can be unsafe. 
    /// In the debug version the consistency is checked
    /// after setting a new comparer; if it is violated, an exception is raised.
    /// In release version, a flag is taken into account that shows whether to check
    /// the consistency.
    /// </summary>
    /// <param name="newComp">The new comparer to be set</param>
    /// <param name="checkAfter">Flag showing whether to check the consistency of the tree
    /// with the new comparer (in release mode only!)</param>
    void SetComparer (IComparer<T> newComp, bool checkAfter = false);
  }
}