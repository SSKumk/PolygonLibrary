using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AVLUtils
{
  /// <summary>
  /// Basic class for all set classes of the library
  /// </summary>
  /// <typeparam name="TValue">Type of data to be stored in the container</typeparam>
  /// <typeparam name="TTree">The tree type on which the container is based</typeparam>
  public abstract class AVLBaseSet<TValue, TTree> : ISet<TValue>, IMultiEnumerable<TValue>
    where TValue : new ()
    where TTree : AVLTree<TValue>
  {
    /// <summary>
    /// The internal container
    /// </summary>
    protected TTree _tree;

    /// <summary>
    /// Default constructor
    /// </summary>
    protected AVLBaseSet() => _tree = null;

    /// <summary>
    /// Indexer
    /// </summary>
    /// <param name="i">Index</param>
    /// <returns>The value with the index i</returns>
    public TValue this[int i] { get => _tree[i]; }

    /// <summary>
    /// Getting the set comparer
    /// </summary>
    public IComparer<TValue> comparer { get => _tree.comparer; }

    #region IMultiEnumerable<TValue> and related methods
    /// <summary>
    /// Returns an enumerator that directly iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<TValue> GetEnumerator () => _tree.GetEnumerator ();

    /// <summary>
    /// Returns an untyped enumerator that directly iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();

    /// <summary>
    /// Returns an enumerator that iterates through the collection put at the given value or after it 
    /// (if there is no such a value in the collection)
    /// </summary>
    /// <param name="v">The value the enumerator to be put on</param>
    /// <returns>An enumerator that can be used to iterate through the collection</returns>
    public IEnumerator<TValue> GetEnumerator (TValue v) => _tree.GetEnumerator (v);

    /// <summary>
    /// Returns an enumerator that reversely iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<TValue> GetReverseEnumerator () => _tree.GetReverseEnumerator ();

    /// <summary>
    /// Returns an enumerator that reversely iterates through the collection put at the given value or before it 
    /// (if there is no such a value in the collection)
    /// </summary>
    /// <param name="v">The value the enumerator to be put on</param>
    /// <returns>An enumerator that can be used to iterate through the collection</returns>
    public IEnumerator<TValue> GetReverseEnumerator (TValue v) => _tree.GetReverseEnumerator (v);

    /// <summary>
    /// Returns an enumerator that directly iterates through the collection regarding it as a cycled one
    /// </summary>
    /// <returns>An enumerator that directly iterates through the collection regarding it as a cycled one</returns>
    public IEnumerator<TValue> GetCyclicEnumerator () => _tree.GetCyclicEnumerator();

    /// <summary>
    /// Returns an enumerator that directly iterates through the collection regarding it as a cycled one;
    /// initially the enumerator is put to the given value or (if it is absent) to minimal value cyclically 
    /// greater than the given one
    /// </summary>
    /// <param name="v">The value the enumerator to be put on</param>
    /// <returns>An enumerator that directly iterates through the collection regarding it as a cycled one</returns>
    public IEnumerator<TValue> GetCyclicEnumerator (TValue v) => _tree.GetCyclicEnumerator(v);

    /// <summary>
    /// Returns an enumerator that reversely iterates through the collection regarding it as a cycled one
    /// </summary>
    /// <returns>An enumerator that reversely iterates through the collection regarding it as a cycled one</returns>
    public IEnumerator<TValue> GetCyclicReverseEnumerator () => _tree.GetCyclicReverseEnumerator ();

    /// <summary>
    /// Returns an enumerator that reversely iterates through the collection regarding it as a cycled one;
    /// initially the enumerator is put to the given value or (if it is absent) to maximal value cyclically 
    /// less than the given one
    /// </summary>
    /// <param name="v">The value the enumerator to be put on</param>
    /// <returns>An enumerator that iterates reversely through the collection regarding it as a cycled one</returns>
    public IEnumerator<TValue> GetCyclicReverseEnumerator (TValue v) => _tree.GetCyclicReverseEnumerator (v);
    #endregion

    #region ICollection<TValue> methods
    /// <summary>
    /// Read-only property (permanently false)
    /// </summary>
    public bool IsReadOnly { get => false; }

    /// <summary>
    /// Number of elements in the tree
    /// </summary>
    public int Count { get => _tree.Count; }

    /// <summary>
    /// Adds an item to the collection
    /// </summary>
    /// <param name="v">The value to be added</param>
    void ICollection<TValue>.Add (TValue v) { _tree.Add (v); }

    /// <summary>
    /// Adding a collection to the tree
    /// </summary>
    /// <param name="other">The collection to be added</param>
    public void AddRange (IEnumerable<TValue> other) { _tree.AddRange (other); }

    /// <summary>
    /// Removes all items from the collection
    /// </summary>
    public void Clear () { _tree.Clear (); }

    /// <summary>
    /// Determines whether the collection contains a specific value
    /// </summary>
    /// <param name="v">The object to locate in the collection</param>
    /// <returns>true if item is found in the collection; otherwise, false</returns>
    public bool Contains (TValue v) => _tree.Contains (v);

    /// <summary>
    /// Copies the elements of the collection to an array, starting at a particular index
    /// </summary>
    /// <param name="array">The one-dimensional array that is the destination of the elements copied from collection. The array must have zero-based indexing</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins</param>
    public void CopyTo (TValue[] array, int arrayIndex)
    {
      _tree.CopyTo (array, arrayIndex);
    }

    /// <summary>
    /// Removes the first occurrence of a specific object from the collection
    /// </summary>
    /// <param name="v">The value to be removed form the collection</param>
    /// <returns>true if item was successfully removed from the collection; otherwise, false. 
    /// This method also returns false if item is not found in the collection</returns>
    public bool Remove (TValue v) => _tree.Remove (v);
    #endregion

    #region ISet methods
    /// <summary>
    /// Adds an element to the current set and returns a value to indicate if the element was successfully added.
    /// </summary>
    /// <param name="v">The element to add to the set.</param>
    /// <returns>true if the element is added to the set; false if the element is already in the set. </returns>
    //    bool ISet<TValue>.Add (TValue v)
    public bool Add (TValue v)
    {
      int oldQnt = Count;
      _tree.Add (v);
      return oldQnt != Count;
    }

    /// <summary>
    /// Removes all elements in the specified collection from the current set.
    /// </summary>
    /// <param name="other">The collection of items to remove from the set.</param>
    public void ExceptWith (IEnumerable<TValue> other)
    {
      foreach (TValue v in other) {
        Remove (v);
      }
    }

    /// <summary>
    /// Modifies the current set so that it contains only elements that are also in a specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    public void IntersectWith (IEnumerable<TValue> other)
    {
      TValue v;
      IEnumerable<TValue> enumerable = other as TValue[] ?? other.ToArray();
      for (int i = Count - 1; i >= 0; i++)
      {
        v = _tree[i];
        if (!enumerable.Contains (v)) {
          Remove (v);
        }
      }
    }

    /// <summary>
    /// Determines whether the current set is a proper (strict) subset of a specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns>true if the current set is a proper subset of other; otherwise, false.</returns>
    public bool IsProperSubsetOf (IEnumerable<TValue> other)
    {
      SortedSet<TValue> processed = new SortedSet<TValue> ();
      bool hasNewElems = false;
      int qntContained = 0;

      // Loop over the given collection
      foreach (TValue v in other)
      {
        // Skip duplicates
        if (processed.Contains (v)) {
          continue;
        }

        if (Contains (v))
          // If there is an element that belongs to our collection, count it
        {
          qntContained++;
        } else
          // If there is an element that does not belong to our collection, remember this fact
        {
          hasNewElems = true;
        }

        // Remember the processed element for duplicate counting
        processed.Add (v);
      }

      // Our collection is a proper subset iff all our elements are in the other collection and
      // the other collection has some additional elements
      return qntContained == Count && hasNewElems;
    }

    /// <summary>
    /// Determines whether the current set is a proper (strict) superset of a specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns>true if the current set is a proper superset of other; otherwise, false.</returns>
    public bool IsProperSupersetOf (IEnumerable<TValue> other)
    {
      SortedSet<TValue> processed = new SortedSet<TValue> ();
      int qntContained = 0;

      // Loop over the given collection
      foreach (TValue v in other)
      {
        // Skip duplicates
        if (processed.Contains (v)) {
          continue;
        }

        if (Contains (v))
          // If there is an element that belongs to our collection, count it
        {
          qntContained++;
        }

        // Remember the processed element for duplicate counting
        processed.Add (v);
      }

      // Our collection is a proper superset iff all our elements of the other collection are in our one and
      // our collection has some additional elements
      return qntContained < Count;
    }

    /// <summary>
    /// Determines whether a set is a subset of a specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns>true if the current set is a subset of other; otherwise, false. </returns>
    public bool IsSubsetOf (IEnumerable<TValue> other)
    {
      IEnumerable<TValue> enumerable = other as TValue[] ?? other.ToArray();
      for (int i = 0; i < Count; i++) {
        if (!enumerable.Contains (_tree[i])) {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Determines whether a set is a subset of a specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns>true if the current set is a superset of other; otherwise, false. </returns>
    public bool IsSupersetOf (IEnumerable<TValue> other)
    {
      foreach (TValue v in other)
      {
        if (Contains (v)) {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Determines whether the current set overlaps with the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns>true if the current set and other share at least one common element; otherwise, false. </returns>
    public bool Overlaps (IEnumerable<TValue> other)
    {
      foreach (TValue v in other)
      {
        if (Contains (v)) {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Determines whether the current set and the specified collection contain the same elements.
    /// This method ignores the order of elements and any duplicate elements in other.
    /// </summary>
    /// <param name="other">The collection to be compared with the current set</param>
    /// <returns>true if the current set is equal to the given; otherwise, false.</returns>
    public bool SetEquals (IEnumerable<TValue> other)
    {
      SortedSet<TValue> processed = new SortedSet<TValue> ();

      // Loop over the given collection
      foreach (TValue v in other)
      {
        // Skip duplicates
        if (processed.Contains (v)) {
          continue;
        }

        // If there is an element that does not belong to our collection, there is no equivalence
        if (!Contains (v)) {
          return false;
        }

        // Remember the processed element for duplicate counting
        processed.Add (v);
      }

      // The equivalence is iff number of processed elements equals to the number of elements in our collection
      return processed.Count == Count;
    }

    /// <summary>
    /// Modifies the current set so that it contains only elements that are present 
    /// either in the current set or in the specified collection, but not both.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    public void SymmetricExceptWith (IEnumerable<TValue> other)
    {
      // Temporary storage for suitable objects
      List<TValue> temp = new List<TValue> ();
      IEnumerable<TValue> enumerable = other as TValue[] ?? other.ToArray();

      // Search for the elements of our collection that are not in the other one
      for (int i = 0; i < Count; i++)
      {
        TValue v = _tree[i];
        if (!enumerable.Contains (v)) {
          temp.Add (v);
        }
      }

      // Search for the elements of the other collection that are not in our one
      foreach (TValue v in enumerable)
      {
        if (!Contains (v)) {
          temp.Add (v);
        }
      }

      // Clearing our storage and fill it with the found elements
      _tree.Clear ();
      _tree.AddRange (temp);
    }

    /// <summary>
    /// Modifies the current set so that it contains all elements that are present in the current set, 
    /// in the specified collection, or in both.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    public void UnionWith (IEnumerable<TValue> other)
    {
      foreach (TValue v in other) {
        Add (v);
      }
    }
    #endregion

    #region My functions
    /// <summary>
    /// Take the minimal value in the tree
    /// </summary>
    /// <returns>The minimal value</returns>
    public TValue Min () => _tree.Min ();

    /// <summary>
    /// Take the maximal value in the tree
    /// </summary>
    /// <returns>The maximal value</returns>
    public TValue Max () => _tree.Max ();

    /// <summary>
    /// Take the minimal value in the tree and remove it
    /// </summary>
    /// <returns>The value</returns>
    public TValue Pop () => _tree.Pop ();

    /// <summary>
    /// Take the maximal value in the tree and remove it
    /// </summary>
    /// <returns>The value</returns>
    public TValue Pop_Back () => _tree.Pop_Back ();

    /// <summary>
    /// Take the value following after the given one
    /// </summary>
    /// <param name="v">The given value</param>
    /// <param name="next">The next value</param>
    /// <returns>true, if the next value is taken successfully; 
    /// false, otherwise (the given value is maximal)</returns>
    public bool Next (TValue v, out TValue next) => _tree.Next (v, out next);

    /// <summary>
    /// Take the value following after the given one in the cyclic order
    /// </summary>
    /// <param name="v">The given value</param>
    /// <param name="next">The next value</param>
    /// <returns>true, if the next value is taken successfully; false, otherwise</returns>
    public bool CyclicNext (TValue v, out TValue next) => _tree.CyclicNext (v, out next);

    /// <summary>
    /// Take the value previous to the given one
    /// </summary>
    /// <param name="v">The given value</param>
    /// <param name="prev">The previous value</param>
    /// <returns>true, if the previous value is taken successfully; 
    /// false, otherwise (the given value is minimal)</returns>
    public bool Prev (TValue v, out TValue prev) => _tree.Prev (v, out prev);

    /// <summary>
    /// Take the value previous to the given one in the cyclic order
    /// </summary>
    /// <param name="v">The given value</param>
    /// <param name="prev">The previous value</param>
    /// <returns>true, if the previous value is taken successfully; false, otherwise</returns>
    public bool CyclicPrev (TValue v, out TValue prev) => _tree.CyclicPrev (v, out prev);
    #endregion
  }

  /// <summary>
  /// Simple set class on the basis of AVL-tree
  /// </summary>
  /// <typeparam name="TValue"></typeparam>
  public class AVLSet<TValue> : AVLBaseSet<TValue, AVLTree<TValue>>
    where TValue : new()
  {
    /// <summary>
    /// Default constructor that involves the default order
    /// </summary>
    public AVLSet() => _tree = new AVLTree<TValue> ();

    /// <summary>
    /// Constructor that involves the given order
    /// </summary>
    public AVLSet (IComparer<TValue> newComp) => _tree = new AVLTree<TValue> (newComp);
  }

  /// <summary>
  /// Class of set with changeable comparer on the basis of AVL-tree
  /// </summary>
  /// <typeparam name="TValue"></typeparam>
  public class AVLSetUnsafe<TValue> : AVLBaseSet<TValue, AVLTreeUnsafe<TValue>>, IUnsafeContainer<TValue>
    where TValue : new ()
  {
    #region Constructors
    /// <summary>
    /// Default constructor that involves the default order
    /// </summary>
    public AVLSetUnsafe () => _tree = new AVLTreeUnsafe<TValue> ();

    /// <summary>
    /// Constructor that involves the given order
    /// </summary>
    public AVLSetUnsafe (IComparer<TValue> newComp) => _tree = new AVLTreeUnsafe<TValue> (newComp);
    #endregion

    #region Methods for working with comparer
    /// <summary>
    /// Checks consistency of the current structure of the tree
    /// </summary>
    /// <returns>true, if the order</returns>
    public bool CheckConsistency () => _tree.CheckConsistency ();

    /// <summary>
    /// Rebuild the tree according to the current comparer
    /// </summary>
    public void Rebuild ()
    {
      _tree.Rebuild ();
    }

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
    public void SetComparer (IComparer<TValue> newComp, bool checkAfter = false)
    {
      _tree.SetComparer (newComp, checkAfter);
    }
    #endregion
  }
}
