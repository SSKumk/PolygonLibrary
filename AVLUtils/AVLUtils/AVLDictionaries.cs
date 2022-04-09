using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AVLUtils
{
  /// <summary>
  /// Base class for all dictionary classes of the library
  /// </summary>
  /// <typeparam name="TKey">Type of the key of the dictionary</typeparam>
  /// <typeparam name="TValue">Type of data to be stored in the dictionary</typeparam>
  /// <typeparam name="TTree">The tree type on which the container is based</typeparam>
  public class AVLBaseDictionary<TKey, TValue, TTree> :
    IDictionary<TKey, TValue>, IMultiEnumerable<KeyValuePair<TKey, TValue>>
    where TValue : new ()
    where TTree : AVLTree<KeyValuePair<TKey, TValue>>
  {
    /// <summary>
    /// The internal container
    /// </summary>
    protected TTree _tree;

    /// <summary>
    /// Default constructor
    /// </summary>
    public AVLBaseDictionary()
    {
      _tree = null;
    }

    /// <summary>
    /// Take a dictionary element by integer index
    /// </summary>
    /// <param name="i">The index</param>
    /// <returns>The key-value pair at the given index</returns>
    public KeyValuePair<TKey, TValue> GetAt (int i)
    {
      return _tree[i];
    }

    #region Comparer for the tree that takes into account keys only
    /// <summary>
    /// Getting the key comparer
    /// </summary>
    public IComparer<TKey> comparer => (_tree.comparer as MyComparer).keyComp;

    /// <summary>
    /// Comparer for pairs that compares keys only
    /// </summary>
    protected class MyComparer : IComparer<KeyValuePair<TKey, TValue>>
    {
      /// <summary>
      /// The reference to the key comparer
      /// </summary>
      public IComparer<TKey> keyComp { get; protected set; }

      /// <summary>
      /// Default constructor that takes the default comparer of keys
      /// </summary>
      public MyComparer () { keyComp = Comparer<TKey>.Default; }

      /// <summary>
      /// Constructor for the pair comparer on the basis of key comparer
      /// </summary>
      /// <param name="newComp">The basic key comparer</param>
      public MyComparer (IComparer<TKey> newComp) { keyComp = newComp; }

      /// <summary>
      /// Compares two pairs and returns a value indicating whether one is less than, equal to, or greater than the other.
      /// </summary>
      /// <param name="p1">The first pair</param>
      /// <param name="p2">The second pair</param>
      /// <returns>-1, if the first key is less than the second key; 0, if keys are equal; +1, otherwise</returns>
      public int Compare (KeyValuePair<TKey, TValue> p1, KeyValuePair<TKey, TValue> p2)
      {
        return keyComp.Compare (p1.Key, p2.Key);
      }
    }
    #endregion

    #region IMultiEnumerable<T> and related methods
    /// <summary>
    /// Returns an enumerator that directly iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator () { return _tree.GetEnumerator (); }

    /// <summary>
    /// Returns an untyped enumerator that directly iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator () { return GetEnumerator (); }

    /// <summary>
    /// Returns an enumerator that iterates through the collection put at the given value or after it 
    /// (if there is no such a value in the collection)
    /// </summary>
    /// <param name="key">The key the enumerator to be put on</param>
    /// <returns>An enumerator that can be used to iterate through the collection</returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator (TKey key)
    {
      return _tree.GetEnumerator (new KeyValuePair<TKey, TValue> (key, default (TValue)));
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection put at the given value or after it 
    /// (if there is no such a value in the collection)
    /// </summary>
    /// <param name="pair">The pair the enumerator to be put on</param>
    /// <returns>An enumerator that can be used to iterate through the collection</returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator (KeyValuePair<TKey, TValue> pair)
    {
      return _tree.GetEnumerator (pair);
    }

    /// <summary>
    /// Returns an enumerator that reversely iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetReverseEnumerator () { return _tree.GetReverseEnumerator (); }

    /// <summary>
    /// Returns an enumerator that reversely iterates through the collection put at the given value or before it 
    /// (if there is no such a value in the collection)
    /// </summary>
    /// <param name="key">The key the enumerator to be put on</param>
    /// <returns>An enumerator that can be used to iterate through the collection</returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetReverseEnumerator (TKey key)
    {
      return _tree.GetEnumerator (new KeyValuePair<TKey, TValue> (key, default (TValue)));
    }

    /// <summary>
    /// Returns an enumerator that iterates reversely through the collection put at the given value or before it 
    /// (if there is no such a value in the collection)
    /// </summary>
    /// <param name="pair">The pair the enumerator to be put on</param>
    /// <returns>An enumerator that can be used to iterate through the collection</returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetReverseEnumerator (KeyValuePair<TKey, TValue> pair)
    {
      return _tree.GetReverseEnumerator (pair);
    }

    /// <summary>
    /// Returns an enumerator that directly iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetCyclicEnumerator () { return _tree.GetCyclicEnumerator (); }

    /// <summary>
    /// Returns a cyclic enumerator that iterates through the collection put at the given value or after it 
    /// (if there is no such a value in the collection)
    /// </summary>
    /// <param name="key">The key the enumerator to be put on</param>
    /// <returns>An enumerator that can be used to iterate through the collection</returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetCyclicEnumerator (TKey key)
    {
      return _tree.GetCyclicEnumerator (new KeyValuePair<TKey, TValue> (key, default (TValue)));
    }

    /// <summary>
    /// Returns a cyclic enumerator that iterates through the collection put at the given value or after it 
    /// (if there is no such a value in the collection)
    /// </summary>
    /// <param name="pair">The pair the enumerator to be put on</param>
    /// <returns>An enumerator that can be used to iterate through the collection</returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetCyclicEnumerator (KeyValuePair<TKey, TValue> pair)
    {
      return _tree.GetCyclicEnumerator (pair);
    }

    /// <summary>
    /// Returns a cyclic enumerator that reversely iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetCyclicReverseEnumerator ()
    {
      return _tree.GetCyclicReverseEnumerator ();
    }

    /// <summary>
    /// Returns a cyclic enumerator that reversely iterates through the collection put at the given value or before it 
    /// (if there is no such a value in the collection)
    /// </summary>
    /// <param name="key">The key the enumerator to be put on</param>
    /// <returns>An enumerator that can be used to iterate through the collection</returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetCyclicReverseEnumerator (TKey key)
    {
      return _tree.GetCyclicReverseEnumerator (new KeyValuePair<TKey, TValue> (key, default (TValue)));
    }

    /// <summary>
    /// Returns a cyclic enumerator that reversely iterates through the collection put at the given value or before it 
    /// (if there is no such a value in the collection)
    /// </summary>
    /// <param name="pair">The pair the enumerator to be put on</param>
    /// <returns>An enumerator that can be used to iterate through the collection</returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetCyclicReverseEnumerator (KeyValuePair<TKey, TValue> pair)
    {
      return _tree.GetCyclicReverseEnumerator (pair);
    }
    #endregion

    #region IDictionary<T> methods
    /// <summary>
    /// Gets or sets the element with the specified key.
    /// </summary>
    /// <param name="key">The key of the element to get or set.</param>
    /// <returns>The element with the specified key.</returns>
    public TValue this[TKey key]
    {
      get
      {
        if (key == null)
          throw new ArgumentNullException (nameof(key));

        KeyValuePair<TKey, TValue> res;
        if (!_tree.Find (new KeyValuePair<TKey, TValue> (key, default (TValue)), out res))
          throw new KeyNotFoundException ();
        return res.Value;
      }
      set
      {
        if (key == null)
          throw new ArgumentNullException (nameof(key));

        KeyValuePair<TKey, TValue> temp = new KeyValuePair<TKey, TValue> (key, value);
        if (_tree.Contains (temp))
          _tree.Remove (temp);
        _tree.Add (temp);
      }
    }

    /// <summary>
    /// Adds an element with the provided key and value to the dictionary.
    /// If there is an element with the given key in the dictionary, nothing changes
    /// </summary>
    /// <param name="key">The object to use as the key of the element to add.</param>
    /// <param name="value">The object to use as the value of the element to add.</param>
    public void Add (TKey key, TValue value)
    {
      if (key == null)
        throw new ArgumentNullException (nameof(key));

      KeyValuePair<TKey, TValue> temp = new KeyValuePair<TKey, TValue> (key, value);
      if (_tree.Contains (temp))
        return;
      _tree.Add (temp);
    }

    /// <summary>
    /// Determines whether the dictionary contains an element with the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the dictionary </param>
    /// <returns>true if the dictionary contains an element with the key; false otherwise. </returns>
    public bool ContainsKey (TKey key)
    {
      if (key == null)
        throw new ArgumentNullException (nameof(key));

      KeyValuePair<TKey, TValue> temp = new KeyValuePair<TKey, TValue> (key, default (TValue));
      return _tree.Contains (temp);
    }

    /// <summary>
    /// Remove the element with the given key from the dictionary
    /// </summary>
    /// <param name="key">The given key</param>
    /// <returns>true, if there was such an element and it has been removed successfully; false, otherwise</returns>
    public bool Remove (TKey key)
    {
      KeyValuePair<TKey, TValue> temp = new KeyValuePair<TKey, TValue> (key, default (TValue));
      return _tree.Remove (temp);
    }

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key whose value to get.</param>
    /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
    /// <returns>true if the dictionary contains an element with the specified key; false, otherwise</returns>
    public bool TryGetValue (TKey key, out TValue value)
    {
      KeyValuePair<TKey, TValue> temp = new KeyValuePair<TKey, TValue> (key, default (TValue)), res;

      // Check whether there is an element in the dictionary with the given key
      if (_tree.Find (temp, out res))
      {
        // If yes, then return true and the obtained value
        value = res.Value;
        return true;
      }
      else
      {
        // If no, return false and the default value
        value = default (TValue);
        return false;
      }
    }

    #endregion

    #region ICollection<T> methods
    /// <summary>
    /// Number of elements in the tree
    /// </summary>
    public int Count => _tree.Count;

    /// <summary>
    /// Read-only property (permanently false)
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Adds a pair key-value to the dictionary. If there is an element with the given key, nothing changes
    /// </summary>
    /// <param name="pair">The pair to be added</param>
    public void Add (KeyValuePair<TKey, TValue> pair) { _tree.Add (pair); }

    /// <summary>
    /// Removes all items from the dictionary
    /// </summary>
    public void Clear () { _tree.Clear (); }

    /// <summary>
    /// Determines whether the dictionary contains a specific pair key-value
    /// </summary>
    /// <param name="pair">Pair to be checked</param>
    /// <returns>true if there is such a pair, false otherwise</returns>
    public bool Contains (KeyValuePair<TKey, TValue> pair)
    {
      KeyValuePair<TKey, TValue> temp;

      // Check whether there is an element in the dictionary with the given key
      if (!_tree.Find (pair, out temp))
        // If no, do nothing
        return false;

      // If there is such a key, check whether the value is just the given one
      return Comparer<TValue>.Default.Compare (pair.Value, temp.Value) == 0;
    }

    /// <summary>
    /// Copies pairs key-value from the dictionary to an array, starting at a particular index
    /// </summary>
    /// <param name="array">The one-dimensional array that is the destination of pairs copied from the dictionary. 
    /// The array must have zero-based indexing</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins</param>
    public void CopyTo (KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      if (array == null)
        throw new ArgumentNullException (nameof(array));
      if (arrayIndex < 0)
        throw new ArgumentOutOfRangeException (nameof(array));
      if (array.Length - arrayIndex < Count)
        throw new ArgumentException ("array too small");

      IEnumerator<KeyValuePair<TKey, TValue>> en = _tree.GetEnumerator ();
      for (int i = 0; i < Count; i++)
      {
        en.MoveNext ();
        array[arrayIndex + i] = en.Current;
      }

      en.Dispose();
    }

    /// <summary>
    /// Removes the specific pair key-value from the dictionary
    /// </summary>
    /// <param name="pair">The pair to be removed</param>
    /// <returns>true if the pair has been removed, false otherwise</returns>
    public bool Remove (KeyValuePair<TKey, TValue> pair)
    {
      KeyValuePair<TKey, TValue> temp;

      // Check whether there is an element in the dictionary with the given key
      if (!_tree.Find (pair, out temp))
        // If no, do nothing
        return false;

      // If there is such a key, check whether the value is just the given one
      if (Comparer<TValue>.Default.Compare (pair.Value, temp.Value) != 0)
        // It the values do not coincide, do nothing
        return false;

      return _tree.Remove (pair);
    }

    #endregion

    #region My functions
    /// <summary>
    /// Gets a value indicating whether this tree is empty.
    /// </summary>
    /// <value><c>true</c> if empty; otherwise, <c>false</c>.</value>
    public bool IsEmpty => Count == 0;

    /// <summary>
    /// Take the minimal pair key-value in the dictionary
    /// </summary>
    /// <returns>The minmal value</returns>
    public KeyValuePair<TKey, TValue> Min () { return _tree.Min (); }

    /// <summary>
    /// Take the minimal key in the dictionary
    /// </summary>
    /// <returns>The minmal value</returns>
    public TKey MinKey () { return _tree.Min ().Key; }

    /// <summary>
    /// Take the value with the minimal key in the dictionary
    /// </summary>
    /// <returns>The minmal value</returns>
    public TValue MinValue () { return _tree.Min ().Value; }

    /// <summary>
    /// Take the maximal pair key-value in the dictionary
    /// </summary>
    /// <returns>The minmal value</returns>
    public KeyValuePair<TKey, TValue> Max () { return _tree.Max (); }

    /// <summary>
    /// Take the maximal key in the dictionary
    /// </summary>
    /// <returns>The minmal value</returns>
    public TKey MaxKey () { return _tree.Max ().Key; }

    /// <summary>
    /// Take the value with the maximal key in the dictionary
    /// </summary>
    /// <returns>The minmal value</returns>
    public TValue MaxValue () { return _tree.Max ().Value; }

    /// <summary>
    /// Remove and return the minimal pair key-value in the dictionary
    /// </summary>
    /// <returns>The minmal value</returns>
    public KeyValuePair<TKey, TValue> Pop () { return _tree.Pop (); }

    /// <summary>
    /// Remove and return the minimal key in the dictionary
    /// </summary>
    /// <returns>The minmal value</returns>
    public TKey PopKey () { return _tree.Pop ().Key; }

    /// <summary>
    /// Remove and return the value with the minimal key in the dictionary
    /// </summary>
    /// <returns>The minmal value</returns>
    public TValue PopValue () { return _tree.Pop ().Value; }

    /// <summary>
    /// Remove and return the maximal pair key-value in the dictionary
    /// </summary>
    /// <returns>The minmal value</returns>
    public KeyValuePair<TKey, TValue> Pop_Back () { return _tree.Pop_Back (); }

    /// <summary>
    /// Remove and return the maximal key in the dictionary
    /// </summary>
    /// <returns>The minmal value</returns>
    public TKey PopKey_Back () { return _tree.Pop_Back ().Key; }

    /// <summary>
    /// Remove and return the value with the maximal key in the dictionary
    /// </summary>
    /// <returns>The minmal value</returns>
    public TValue PopValue_Back () { return _tree.Pop_Back ().Value; }

    /// <summary>
    /// Take the pair key-value following after the given pair (only the key is taken into account)
    /// </summary>
    /// <param name="pair">The given pair</param>
    /// <param name="nextPair">The next pair</param>
    /// <returns>true, if the next value is taken successfully; 
    /// false, otherwise (the given value is maximal)</returns>
    public bool Next (KeyValuePair<TKey, TValue> pair, out KeyValuePair<TKey, TValue> nextPair)
    {
      return _tree.Next (pair, out nextPair);
    }

    /// <summary>
    /// Take the pair key-value following after the given pair in cyclic order (only the key is taken into account)
    /// </summary>
    /// <param name="pair">The given pair</param>
    /// <param name="nextPair">The next pair</param>
    /// <returns>true, if the next value is taken successfully; false, otherwise</returns>
    public bool CyclicNext (KeyValuePair<TKey, TValue> pair, out KeyValuePair<TKey, TValue> nextPair)
    {
      return _tree.CyclicNext (pair, out nextPair);
    }

    /// <summary>
    /// Take the pair key-value following after the pair with given key 
    /// </summary>
    /// <param name="key">The given key</param>
    /// <param name="nextPair">The next pair</param>
    /// <returns>true, if the next value is taken successfully; 
    /// false, otherwise (the given value is maximal)</returns>
    public bool Next (TKey key, out KeyValuePair<TKey, TValue> nextPair)
    {
      return Next (new KeyValuePair<TKey, TValue> (key, default (TValue)), out nextPair);
    }

    /// <summary>
    /// Take the pair key-value following after the pair with given key in the cyclic order
    /// </summary>
    /// <param name="key">The given key</param>
    /// <param name="nextPair">The next pair</param>
    /// <returns>true, if the next value is taken successfully; false, otherwise</returns>
    public bool CyclicNext (TKey key, out KeyValuePair<TKey, TValue> nextPair)
    {
      return CyclicNext (new KeyValuePair<TKey, TValue> (key, default (TValue)), out nextPair);
    }

    /// <summary>
    /// Take the pair key-value preceeding the given pair (only the key is taken into account)
    /// </summary>
    /// <param name="pair">The given pair</param>
    /// <param name="nextPair">The previous pair</param>
    /// <returns>true, if the previuos value is taken successfully; 
    /// false, otherwise (the given value is minimal)</returns>
    public bool Prev (KeyValuePair<TKey, TValue> pair, out KeyValuePair<TKey, TValue> nextPair)
    {
      return _tree.Prev (pair, out nextPair);
    }

    /// <summary>
    /// Take the pair key-value preceeding the given pair in the cyclic order (only the key is taken into account)
    /// </summary>
    /// <param name="pair">The given pair</param>
    /// <param name="nextPair">The previous pair</param>
    /// <returns>true, if the previuos value is taken successfully; false, otherwise</returns>
    public bool CyclicPrev (KeyValuePair<TKey, TValue> pair, out KeyValuePair<TKey, TValue> nextPair)
    {
      return _tree.CyclicPrev (pair, out nextPair);
    }

    /// <summary>
    /// Take the pair key-value preceeding the pair with given key 
    /// </summary>
    /// <param name="key">The given key</param>
    /// <param name="nextPair">The previous pair</param>
    /// <returns>true, if the previous value is taken successfully; 
    /// false, otherwise (the given value is minimal)</returns>
    public bool Prev (TKey key, out KeyValuePair<TKey, TValue> nextPair)
    {
      return Prev (new KeyValuePair<TKey, TValue> (key, default (TValue)), out nextPair);
    }

    /// <summary>
    /// Take the pair key-value preceeding the pair with given key in the cyclic order
    /// </summary>
    /// <param name="key">The given key</param>
    /// <param name="nextPair">The previous pair</param>
    /// <returns>true, if the previous value is taken successfully; false, otherwise</returns>
    public bool CyclicPrev (TKey key, out KeyValuePair<TKey, TValue> nextPair)
    {
      return CyclicPrev (new KeyValuePair<TKey, TValue> (key, default (TValue)), out nextPair);
    }
    #endregion

    #region Keys and Values properties
    /// <summary>
    /// Gets an ICollection object containing the keys of the dictionary.
    /// The order of keys is the same as in the dictionary
    /// </summary>
    public ICollection<TKey> Keys => new KeyCollection (this);

    /// <summary>
    /// Gets an ICollection object containing the values of the dictionary.
    /// The order of values is the same as in the dictionary
    /// </summary>
    public ICollection<TValue> Values => new ValueCollection (this);

    #endregion

    #region Auxiliary enumerator class for the keys and values collection classes
    /// <summary>
    /// Class of the enumerator of a key collection
    /// </summary>
    internal abstract class BasicKeyValueEnumerator<T> : IEnumerator<T>
    {
      /// <summary>
      /// Internal reference to a parent dictionary enumerator
      /// </summary>
      protected IEnumerator<KeyValuePair<TKey, TValue>> _dictEnum;

      /// <summary>
      /// Constructor of an enumerator that connects it to the parent dictionary
      /// </summary>
      /// <param name="en">Reference to the enumerator to be used</param>
      internal BasicKeyValueEnumerator (IEnumerator<KeyValuePair<TKey, TValue>> en) { _dictEnum = en; }

      /// <summary>
      /// Disposition of the enumertor
      /// </summary>
      public void Dispose () { _dictEnum.Dispose (); }

      /// <summary>
      /// Moving the enumerator
      /// </summary>
      /// <returns>true if the enumerator was moved, false otherwise</returns>
      public bool MoveNext () { return _dictEnum.MoveNext (); }

      /// <summary>
      /// Getting the current key
      /// </summary>
      abstract public T Current { get; }

      /// <summary>
      /// Getting the current key
      /// </summary>
      object IEnumerator.Current => Current;

      /// <summary>
      /// Resetting the enumerator
      /// </summary>
      void IEnumerator.Reset () { _dictEnum.Reset (); }
    }
    #endregion

    #region Keys collection
    /// <summary>
    /// Class of immutable collection of keys of a dictionary. The order of keys is the same as in the dictionary elements
    /// </summary>
    public sealed class KeyCollection : ICollection<TKey>, IMultiEnumerable<TKey>
    {
      /// <summary>
      /// Class of key enumerator
      /// </summary>
      internal class KeyEnumerator : BasicKeyValueEnumerator<TKey>
      {
        /// <summary>
        /// COnstructor that takes an enumerator in the parent dictionary
        /// </summary>
        /// <param name="en">An enumerator in the parent dictionary</param>
        public KeyEnumerator (IEnumerator<KeyValuePair<TKey, TValue>> en) : base (en) { }

        /// <summary>
        /// Getting key of the current pair in the dictionary
        /// </summary>
        override public TKey Current => _dictEnum.Current.Key;
      }

      /// <summary>
      /// Internal reference to the parent dictionary
      /// </summary>
      private AVLBaseDictionary<TKey, TValue, TTree> _dict;

      /// <summary>
      /// Constructor that creates the key collection and connects it to the given dictionary
      /// </summary>
      /// <param name="newDict">The dictionary to which the created collection should be connected</param>
      public KeyCollection (AVLBaseDictionary<TKey, TValue, TTree> newDict)
      {
        if (newDict == null)
          throw new ArgumentNullException (nameof(newDict));
        this._dict = newDict;
      }

      #region IMultiEnumerable<T> and related methods
      /// <summary>
      /// Getting enumerator for key collection
      /// </summary>
      /// <returns>An enumerator to access the collection</returns>
      public IEnumerator<TKey> GetEnumerator () { return new KeyEnumerator (_dict.GetEnumerator ()); }

      /// <summary>
      /// Getting enumerator for key collection
      /// </summary>
      /// <returns>An enumerator to access the collection</returns>
      IEnumerator IEnumerable.GetEnumerator () { return GetEnumerator (); }

      /// <summary>
      /// Getting enumerator for key collection set to the given value (or after it) 
      /// </summary>
      /// <param name="key">The key to which or after which the enumerator should be put</param>
      /// <returns>An enumerator to access the collection</returns>
      public IEnumerator<TKey> GetEnumerator (TKey key) { return new KeyEnumerator (_dict.GetEnumerator (key)); }

      /// <summary>
      /// Getting enumerator for key collection
      /// </summary>
      /// <returns>An enumerator to access the collection</returns>
      public IEnumerator<TKey> GetCyclicEnumerator () { return new KeyEnumerator (_dict.GetCyclicEnumerator ()); }

      /// <summary>
      /// Getting enumerator for key collection set to the given value (or after it) regarding it as a cyclic container
      /// </summary>
      /// <param name="key">The key to which or after which the enumerator should be put</param>
      /// <returns>An enumerator to access the collection</returns>
      public IEnumerator<TKey> GetCyclicEnumerator (TKey key) { return new KeyEnumerator (_dict.GetCyclicEnumerator (key)); }

      /// <summary>
      /// Getting reverse enumerator for key collection
      /// </summary>
      /// <returns>An enumerator to access the collection</returns>
      public IEnumerator<TKey> GetReverseEnumerator () { return new KeyEnumerator (_dict.GetReverseEnumerator ()); }

      /// <summary>
      /// Getting reverse enumerator for key collection set to the given value (or before it)
      /// </summary>
      /// <param name="key">The key to which or after which the enumerator should be put</param>
      /// <returns>An enumerator to access the collection</returns>
      public IEnumerator<TKey> GetReverseEnumerator (TKey key) { return new KeyEnumerator (_dict.GetReverseEnumerator (key)); }

      /// <summary>
      /// Getting reverse enumerator for key collection
      /// </summary>
      /// <returns>An enumerator to access the collection</returns>
      public IEnumerator<TKey> GetCyclicReverseEnumerator () { return new KeyEnumerator (_dict.GetCyclicReverseEnumerator ()); }

      /// <summary>
      /// Getting reverse enumerator for key collection set to the given value (or before it) regarding it as a cyclic container
      /// </summary>
      /// <param name="key">The key to which or after which the enumerator should be put</param>
      /// <returns>An enumerator to access the collection</returns>
      public IEnumerator<TKey> GetCyclicReverseEnumerator (TKey key)
      {
        return new KeyEnumerator (_dict.GetCyclicReverseEnumerator (key));
      }
      #endregion

      #region ICollection<T> methods
      /// <summary>
      /// Copies the elements of the collection to an array, starting at a particular index
      /// </summary>
      /// <param name="array">The one-dimensional array that is the destination of the elements copied from collection. The array must have zero-based indexing</param>
      /// <param name="index">The zero-based index in array at which copying begins</param>            
      public void CopyTo (TKey[] array, int index)
      {
        if (array == null)
          throw new ArgumentNullException (nameof(array));
        if (array.Rank != 1)
          throw new ArgumentException ("array should be one-dimensional");
        if (index < 0)
          throw new ArgumentOutOfRangeException (nameof(index));
        if (array.Length - index < Count)
          throw new ArgumentException ("array too small");

        for (int i = 0; i < Count; i++)
        {
          KeyValuePair<TKey, TValue> p = _dict.GetAt (i);
          array[index + i] = p.Key;
        }

      }

      /// <summary>
      /// Getting number of keys in the collection
      /// </summary>
      public int Count => _dict.Count;

      /// <summary>
      /// Getting read-only flag (constantly true)
      /// </summary>
      bool ICollection<TKey>.IsReadOnly => true;

      /// <summary>
      /// A stub for Add method
      /// </summary>
      /// <param name="item">The item to be added</param>
      void ICollection<TKey>.Add (TKey item) { throw new NotSupportedException ("KeyCollection"); }

      /// <summary>
      /// A stub for Clear method
      /// </summary>
      void ICollection<TKey>.Clear () { throw new NotSupportedException ("KeyCollection"); }

      /// <summary>
      /// Checking presence of a key in the collection
      /// </summary>
      /// <param name="item">The to be checked</param>
      /// <returns>true if the key is present, false otherwise</returns>
      bool ICollection<TKey>.Contains (TKey item)
      {
        return _dict.ContainsKey (item);
      }

      /// <summary>
      /// A stub for Remove method
      /// </summary>
      /// <param name="item">The item to be removed</param>
      /// <returns>Actually, no return value</returns>
      bool ICollection<TKey>.Remove (TKey item)
      {
        throw new NotSupportedException ("KeyCollection");
      }
      #endregion
    }
    #endregion

    #region Values collection
    /// <summary>
    /// Class of immutable collection of values of a dictionary. The order of values is the same as in the dictionary elements
    /// </summary>
    public sealed class ValueCollection : ICollection<TValue>, IMultiEnumerable<TValue>
    {
      /// <summary>
      /// Class of value enumerator
      /// </summary>
      internal class ValueEnumerator : BasicKeyValueEnumerator<TValue>
      {
        /// <summary>
        /// Constructor that takes an enumerator in the parent dictionary
        /// </summary>
        /// <param name="en">An enumerator in the parent dictionary</param>
        public ValueEnumerator (IEnumerator<KeyValuePair<TKey, TValue>> en) : base (en) { }

        /// <summary>
        /// Getting value of the current pair in the dictionary
        /// </summary>
        override public TValue Current => _dictEnum.Current.Value;
      }

      /// <summary>
      /// Internal reference to the parent dictionary
      /// </summary>
      private AVLBaseDictionary<TKey, TValue, TTree> _dict;

      /// <summary>
      /// Constructor that creates the value collection and connects it to the given dictionary
      /// </summary>
      /// <param name="newDict">The dictionary to which the created collection should be connected</param>
      public ValueCollection (AVLBaseDictionary<TKey, TValue, TTree> newDict)
      {
        if (newDict == null)
          throw new ArgumentNullException (nameof(newDict));
        this._dict = newDict;
      }

      #region IMultiEnumerable<T> and related methods
      /// <summary>
      /// Getting enumerator for value collection
      /// </summary>
      /// <returns>An enumerator to access the collection</returns>
      public IEnumerator<TValue> GetEnumerator () { return new ValueEnumerator (_dict.GetEnumerator ()); }

      /// <summary>
      /// Getting enumerator for value collection
      /// </summary>
      /// <returns>An enumerator to access the collection</returns>
      IEnumerator IEnumerable.GetEnumerator () { return GetEnumerator (); }

      /// <summary>
      /// Getting enumerator for value collection set to the given value (or after it).
      /// Not realized because the collection is unordered
      /// </summary>
      /// <param name="val">The val to which or after which the enumerator should be put</param>
      /// <returns>An enumerator to access the collection</returns>
      public IEnumerator<TValue> GetEnumerator (TValue val)
      {
        throw new NotSupportedException ("ValueCollection");
      }

      /// <summary>
      /// Getting enumerator for value collection
      /// </summary>
      /// <returns>An enumerator to access the collection</returns>
      public IEnumerator<TValue> GetCyclicEnumerator () { return new ValueEnumerator (_dict.GetCyclicEnumerator ()); }

      /// <summary>
      /// Getting enumerator for value collection set to the given value (or after it) regarding it as a cyclic container
      /// Not realized because the collection is unordered
      /// </summary>
      /// <param name="val">The val to which or after which the enumerator should be put</param>
      /// <returns>An enumerator to access the collection</returns>
      public IEnumerator<TValue> GetCyclicEnumerator (TValue val)
      {
        throw new NotSupportedException ("ValueCollection");
      }

      /// <summary>
      /// Getting reverse enumerator for value collection
      /// </summary>
      /// <returns>An enumerator to access the collection</returns>
      public IEnumerator<TValue> GetReverseEnumerator () { return new ValueEnumerator (_dict.GetReverseEnumerator ()); }

      /// <summary>
      /// Getting reverse enumerator for value collection set to the given value (or before it)
      /// Not realized because the collection is unordered
      /// </summary>
      /// <param name="val">The val to which or after which the enumerator should be put</param>
      /// <returns>An enumerator to access the collection</returns>
      public IEnumerator<TValue> GetReverseEnumerator (TValue val)
      {
        throw new NotSupportedException ("ValueCollection");
      }

      /// <summary>
      /// Getting reverse enumerator for value collection
      /// </summary>
      /// <returns>An enumerator to access the collection</returns>
      public IEnumerator<TValue> GetCyclicReverseEnumerator () { return new ValueEnumerator (_dict.GetCyclicReverseEnumerator ()); }

      /// <summary>
      /// Getting reverse enumerator for value collection set to the given value (or before it) regarding it as a cyclic container
      /// Not realized because the collection is unordered
      /// </summary>
      /// <param name="val">The val to which or after which the enumerator should be put</param>
      /// <returns>An enumerator to access the collection</returns>
      public IEnumerator<TValue> GetCyclicReverseEnumerator (TValue val)
      {
        throw new NotSupportedException ("ValueCollection");
      }
      #endregion

      #region ICollection<T> methods
      /// <summary>
      /// Copies the elements of the collection to an array, starting at a particular index
      /// </summary>
      /// <param name="array">The one-dimensional array that is the destination of the elements copied from collection. The array must have zero-based indexing</param>
      /// <param name="index">The zero-based index in array at which copying begins</param>            
      public void CopyTo (TValue[] array, int index)
      {
        if (array == null)
          throw new ArgumentNullException (nameof(array));
        if (array.Rank != 1)
          throw new ArgumentException ("array should be one-dimensional");
        if (index < 0)
          throw new ArgumentOutOfRangeException (nameof(index));
        if (array.Length - index < Count)
          throw new ArgumentException ("array too small");

        for (int i = 0; i < Count; i++)
        {
          KeyValuePair<TKey, TValue> p = _dict.GetAt (i);
          array[index + i] = p.Value;
        }

      }

      /// <summary>
      /// Getting number of values in the collection
      /// </summary>
      public int Count => _dict.Count;

      /// <summary>
      /// Getting read-only flag (constantly true)
      /// </summary>
      bool ICollection<TValue>.IsReadOnly => true;

      /// <summary>
      /// A stub for Add method
      /// </summary>
      /// <param name="item">The item to be added</param>
      void ICollection<TValue>.Add (TValue item) { throw new NotSupportedException ("ValueCollection"); }

      /// <summary>
      /// A stub for Clear method
      /// </summary>
      void ICollection<TValue>.Clear () { throw new NotSupportedException ("ValueCollection"); }

      /// <summary>
      /// Checking presence of a value in the collection
      /// </summary>
      /// <param name="item">The to be checked</param>
      /// <returns>true if the value is present, false otherwise</returns>
      bool ICollection<TValue>.Contains (TValue item)
      {
        return this.Contains (item);
      }

      /// <summary>
      /// A stub for Remove method
      /// </summary>
      /// <param name="item">The item to be removed</param>
      /// <returns>Actually, no return value</returns>
      bool ICollection<TValue>.Remove (TValue item)
      {
        throw new NotSupportedException ("ValueCollection");
      }
      #endregion
    }
    #endregion
  }

  /// <summary>
  /// Class of dictionary 
  /// </summary>
  /// <typeparam name="TKey">Type of keys</typeparam>
  /// <typeparam name="TValue">Type of values</typeparam>
  public class AVLDictionary<TKey, TValue> : AVLBaseDictionary<TKey, TValue, AVLTree<KeyValuePair<TKey, TValue>>>
    where TValue : new ()
  {
    /// <summary>
    /// Default constructor that use the default key order
    /// </summary>
    public AVLDictionary ()
    {
      _tree = new AVLTree<KeyValuePair<TKey, TValue>> (new MyComparer ());
    }

    /// <summary>
    /// Constructor that use the key order given by user
    /// </summary>
    public AVLDictionary (IComparer<TKey> comp)
    {
      _tree = new AVLTree<KeyValuePair<TKey, TValue>> (new MyComparer (comp));
    }
  }

  /// <summary>
  /// Class of dictionary that can change the comparer during work
  /// </summary>
  /// <typeparam name="TKey">Type of keys</typeparam>
  /// <typeparam name="TValue">Type of values</typeparam>
  public class AVLDictionaryUnsafe<TKey, TValue> :
    AVLBaseDictionary<TKey, TValue, AVLTreeUnsafe<KeyValuePair<TKey, TValue>>>, IUnsafeContainer<TKey>
    where TValue : new ()
  {
    /// <summary>
    /// Default constructor that use the default key order
    /// </summary>
    public AVLDictionaryUnsafe ()
    {
      _tree = new AVLTreeUnsafe<KeyValuePair<TKey, TValue>> (new MyComparer ());
    }

    /// <summary>
    /// Constructor that use the key order given by user
    /// </summary>
    public AVLDictionaryUnsafe (IComparer<TKey> comp)
    {
      _tree = new AVLTreeUnsafe<KeyValuePair<TKey, TValue>> (new MyComparer (comp));
    }

    #region Methods for work with comparer
    /// <summary>
    /// Checks cosistency of the current structure of the tree
    /// </summary>
    /// <returns>true, if the order</returns>
    public bool CheckConsistency () { return _tree.CheckConsistency (); }

    /// <summary>
    /// Rebuild the tree according to the current comparer
    /// </summary>
    public void Rebuild () { _tree.Rebuild (); }

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
    public void SetComparer (IComparer<TKey> newComp, bool checkAfter = false)
    {
      _tree.SetComparer (new MyComparer (newComp), checkAfter);
    }
    #endregion
  }
}
