using System;
using System.Collections;
using System.Collections.Generic;

namespace AVLUtils
{
  /// <summary>
  /// The class of AVL-tree
  /// </summary>
  /// <typeparam name="TValue">Type of the value to be stored in the tree</typeparam>
  public partial class AVLTree<TValue> : IAVLTree<TValue>
    where TValue : new ()
  {
    #region IMultiEnumerable<T> methods (methids of IEnumerable<T> and related)
    /// <summary>
    /// Returns an enumerator that iterates through the collection from the beginning
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection</returns>
    public IEnumerator<TValue> GetEnumerator () { return new AVLEnumerator (this); }

    /// <summary>
    /// Returns an enumerator that iterates through a collection form the beginning (typeless version)
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection</returns>
    IEnumerator IEnumerable.GetEnumerator () { return GetEnumerator (); }

    /// <summary>
    /// Returns an enumerator that iterates through the collection put at the given value or after it 
    /// (if there is no such a value in the collection)
    /// </summary>
    /// <param name="v">The value the enymerator to be put on</param>
    /// <returns>An enumerator that can be used to iterate through the collection</returns>
    public IEnumerator<TValue> GetEnumerator (TValue v) { return new AVLEnumerator (this, v); }

    /// <summary>
    /// Returns an enumerator that reversely iterates through the collection from the end
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection</returns>
    public IEnumerator<TValue> GetReverseEnumerator () { return new AVLReverseEnumerator (this); }

    /// <summary>
    /// Returns an enumerator that iterates reversely through the collection put at the given value or before it 
    /// (if there is no such a value in the collection)
    /// </summary>
    /// <param name="v">The value the enymerator to be put on</param>
    /// <returns>An enumerator that can be used to iterate reversely through the collection</returns>
    public IEnumerator<TValue> GetReverseEnumerator (TValue v) { return new AVLReverseEnumerator (this, v); }

    /// <summary>
    /// Returns an enumerator that directly iterates through the collection regarding it as a cycled one
    /// </summary>
    /// <returns>An enumerator that directly iterates through the collection regarding it as a cycled one</returns>
    public IEnumerator<TValue> GetCyclicEnumerator () { return new AVLCyclicEnumerator (this); }

    /// <summary>
    /// Returns an enumerator that directly iterates through the collection regarding it as a cycled one;
    /// initially the enumerator is put to the given value or (if it is absent) to minimal value cyclicly 
    /// greater than the given one
    /// </summary>
    /// <param name="v">The value the enymerator to be put on</param>
    /// <returns>An enumerator that directly iterates through the collection regarding it as a cycled one</returns>
    public IEnumerator<TValue> GetCyclicEnumerator (TValue v) { return new AVLCyclicEnumerator (this, v); }

    /// <summary>
    /// Returns an enumerator that reversely iterates through the collection regarding it as a cycled one
    /// </summary>
    /// <returns>An enumerator that reversely iterates through the collection regarding it as a cycled one</returns>
    public IEnumerator<TValue> GetCyclicReverseEnumerator () { return new AVLCyclicReverseEnumerator (this); }

    /// <summary>
    /// Returns an enumerator that reversely iterates through the collection regarding it as a cycled one;
    /// initially the enumerator is put to the given value or (if it is absent) to maximal value cyclicly 
    /// less than the given one
    /// </summary>
    /// <param name="v">The value the enymerator to be put on</param>
    /// <returns>An enumerator that iterates reversely through the collection regarding it as a cycled one</returns>
    public IEnumerator<TValue> GetCyclicReverseEnumerator (TValue v) { return new AVLCyclicReverseEnumerator (this, v); }
    #endregion

    #region ICollection<T> methods (patially here)
    /// <summary>
    /// Read-only property (permanently false)
    /// </summary>
    public bool IsReadOnly { get { return false; } }

    /// <summary>
    /// Number of elements in the tree
    /// </summary>
    public int Count
    {
      get
      {
        if (_top == null)
          return 0;
        else
          return _top.subtreeQnt;
      }
    }

    /// <summary>
    /// Copies the elements of the collection to an array, starting at a particular index
    /// </summary>
    /// <param name="array">The one-dimensional array that is the destination of the elements copied from collection. The array must have zero-based indexing</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins</param>
    public void CopyTo (TValue[] array, int arrayIndex)
    {
      if (array == null)
        throw new ArgumentNullException (nameof(array));
      if (arrayIndex < 0)
        throw new ArgumentOutOfRangeException (nameof(array));
      if (array.Length - arrayIndex < Count)
        throw new ArgumentException ("array too small");

      int qnt = this.Count;
      IEnumerator<TValue> en = GetEnumerator ();
      for (int i = 0; i < qnt; i++, arrayIndex++)
      {
        en.MoveNext ();
        array[arrayIndex] = en.Current;
      }

      en.Dispose();
    }
    #endregion

    /// <summary>
    /// The _top of the tree
    /// </summary>
    internal AVLNode _top;

    /// <summary>
    /// Comparer defining the order in the tree
    /// </summary>
    public IComparer<TValue> comparer { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether this tree is empty.
    /// </summary>
    /// <value><c>true</c> if empty; otherwise, <c>false</c>.</value>
    public bool IsEmpty { get { return Count == 0; } }

    /// <summary>
    /// Default constructor. Sets default comparer
    /// </summary>
    public AVLTree ()
    {
      _top = null;
      comparer = Comparer<TValue>.Default;
    }

    /// <summary>
    /// Constructor, which sets the comparer for the tree
    /// </summary>
    /// <param name="newComp">The comparer should be used in the tree</param>
    public AVLTree (IComparer<TValue> newComp)
    {
      _top = null;
      comparer = newComp;
    }

    /// <summary>
    /// Iterator for the indexer
    /// </summary>
    /// <param name="node">Current node</param>
    /// <param name="i">Target index</param>
    /// <param name="l">Lower bound of indices in the subtree</param>
    /// <param name="r">Upper bound of indices in the subtree</param>
    /// <returns>The value with the given index</returns>
    private TValue GetByIndexIter (AVLNode node, int i, int l, int r)
    {
      int
      l1 = (node.left == null ? l : l + node.left.subtreeQnt) - 1,
      r1 = (node.right == null ? r : r - node.right.subtreeQnt) + 1;
      if (i <= l1)
        return GetByIndexIter (node.left, i, l, l1);
      else if (i >= r1)
        return GetByIndexIter (node.right, i, r1, r);
      else
        return node.val;
    }

    /// <summary>
    /// Indexer
    /// </summary>
    /// <param name="i">Index</param>
    /// <returns>The value with the index i</returns>
    public TValue this[int i]
    {
      get
      {
        int l = 0, r = Count - 1;
        if (i < l || i > r)
          throw new IndexOutOfRangeException ("Errorous index in AVLBaseTree");

        return GetByIndexIter (_top, i, l, r);
      }
    }

    /// <summary>
    /// Procedure seeking for an element in the tree with the given value
    /// </summary>
    /// <param name="v">The value to be checked</param>
    /// <returns>true, if the value is in the tree; false, otherwise</returns>
    public bool Contains (TValue v)
    {
      AVLNode target = GetNode (v);
      return target != null;
    }

    /// <summary>
    /// Get a reference to the object by its value
    /// </summary>
    /// <param name="v">The value to be found</param>
    /// <param name="res">Reference to the found object: the object itself if found, default value otherwise</param>
    /// <returns>true if the object has been found, false otherwise</returns>
    public bool Find (TValue v, out TValue res)
    {
      AVLNode node = GetNode (v);
      if (node == null)
      {
        res = default (TValue);
        return false;
      }
      else
      {
        res = node.val;
        return true;
      }
    }

    #region My functions
    /// <summary>
    /// Take the minimal value in the tree
    /// </summary>
    /// <returns>The minmal value</returns>
    public TValue Min ()
    {
      if (_top == null)
        throw new InvalidOperationException ("The AVLBaseTree is empty!");

      AVLNode cur = _top;
      while (cur.left != null)
        cur = cur.left;

      return cur.val;
    }

    /// <summary>
    /// Take the maximal value in the tree
    /// </summary>
    /// <returns>The maximal value</returns>
    public TValue Max ()
    {
      if (_top == null)
        throw new InvalidOperationException ("The AVLBaseTree is empty!");

      AVLNode cur = _top;
      while (cur.right != null)
        cur = cur.right;

      return cur.val;
    }

    /// <summary>
    /// Take the minimal value in the tree and remove it
    /// </summary>
    /// <returns>The value</returns>
    public TValue Pop ()
    {
      if (_top == null)
        throw new InvalidOperationException ("The AVLBaseTree is empty!");
      TValue res = Min ();
      Remove (res);
      return res;
    }

    /// <summary>
    /// Take the maximal value in the tree and remove it
    /// </summary>
    /// <returns>The value</returns>
    public TValue Pop_Back ()
    {
      if (_top == null)
        throw new InvalidOperationException ("The AVLBaseTree is empty!");
      TValue res = Max ();
      Remove (res);
      return res;
    }

    /// <summary>
    /// Internal procedure for taking next/previous element after/before the given one (in linear or cyclic order)
    /// </summary>
    /// <param name="v">The given value</param>
    /// <param name="next">The next value</param>
    /// <param name="en">Enumerator that defines direction of the collection and its type</param>
    /// <returns></returns>
    private bool NextInternal (TValue v, out TValue next, AVLBaseEnumerator en)
    {
      if (en.IsValid)
      {
        if (comparer.Compare (v, en.Current) == 0)
        {
          if (!en.MoveNext ())
          {
            next = default (TValue);
            return false;
          }
        }
        next = en.Current;
        return true;
      }
      else
      {
        next = default (TValue);
        return false;
      }
    }

    /// <summary>
    /// Take the value following after the given one
    /// </summary>
    /// <param name="v">The given value</param>
    /// <param name="next">The next value</param>
    /// <returns>true, if the next value is taken successfully; 
    /// false, otherwise (the given value is maximal)</returns>
    public bool Next (TValue v, out TValue next)
    {
      return NextInternal (v, out next, GetEnumerator (v) as AVLBaseEnumerator);
    }

    /// <summary>
    /// Take the value cyclicly following after the given one
    /// </summary>
    /// <param name="v">The given value</param>
    /// <param name="next">The next value</param>
    /// <returns>true, if the next value is taken successfully; 
    /// false, otherwise (the given value is maximal)</returns>
    public bool CyclicNext (TValue v, out TValue next)
    {
      return NextInternal (v, out next, GetCyclicEnumerator (v) as AVLBaseEnumerator);
    }

    /// <summary>
    /// Take the value previous to the given one
    /// </summary>
    /// <param name="v">The given value</param>
    /// <param name="prev">The previous value</param>
    /// <returns>true, if the previous value is taken successfully; 
    /// false, otherwise (the given value is minimal)</returns>
    public bool Prev (TValue v, out TValue prev)
    {
      return NextInternal (v, out prev, GetReverseEnumerator (v) as AVLBaseEnumerator);
    }

    /// <summary>
    /// Take the value cyclicly previous to the given one
    /// </summary>
    /// <param name="v">The given value</param>
    /// <param name="prev">The previous value</param>
    /// <returns>true, if the previous value is taken successfully; false, otherwise</returns>
    public bool CyclicPrev (TValue v, out TValue prev)
    {
      return NextInternal (v, out prev, GetCyclicReverseEnumerator (v) as AVLBaseEnumerator);
    }
    #endregion

    #region Convertors
    /// <summary>
    /// Converts  the tree to a list of corresponding values according to the current tree structure
    /// </summary>
    /// <returns>The resultant list</returns>
    public List<TValue> ToList ()
    {
      List<TValue> l = new List<TValue> ();
      IEnumerator<TValue> en = GetEnumerator ();
      while (en.MoveNext ())
        l.Add (en.Current);
      en.Dispose();
      return l;
    }
    #endregion

    /// <summary>
    /// Clearing the tree
    /// </summary>
    public void Clear ()
    {
      _top = null;
    }

  }
}
