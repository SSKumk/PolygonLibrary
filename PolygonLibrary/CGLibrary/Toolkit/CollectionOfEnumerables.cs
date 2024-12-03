using System.Collections;

namespace CGLibrary.Toolkit
{
  /// <summary>
  /// A class that can logically concatenate a number of enumerables with the same type of elements 
  /// (in the order given by user) and traverse them in sequence. The resultant collection contains
  /// references to the original enumerables, therefore, if any changes are made with them,
  /// all enumerators over the entire collection should be considered as invalid
  /// </summary>
  /// <typeparam name="T">The type of the elements</typeparam>
  public class CollectionOfEnumerables<T> : IEnumerable<T>
    where T : IEquatable<T>
  {
    #region Internal storage
    /// <summary>
    /// The collection of enumerables to be concatenated
    /// </summary>
    private readonly List<IEnumerable<T>> _coll;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructor with variable number of enumerables to be added
    /// </summary>
    /// <param name="ens">The set of enumerables to be joined into collection</param>
    public CollectionOfEnumerables (params IEnumerable<T>[] ens) => 
      _coll = new List<IEnumerable<T>> (ens.Where(en => en != null && en.Any()));

    /// <summary>
    /// Constructor that takes a sequence of enumerables to be added
    /// </summary>
    /// <param name="ens">The set of enumerables to be joined into collection</param>
    public CollectionOfEnumerables (IEnumerable<IEnumerable<T>> ens) => 
      _coll = new List<IEnumerable<T>>(ens.Where(en => en != null && en.Any()));
    #endregion

    #region The class for an enumerator over a collection of enumerables
    public class EnumeratorOverCollection : IEnumerator<T>
    {
      /// <summary>
      /// The collection to which the enumerator is connected
      /// </summary>
      private readonly CollectionOfEnumerables<T> parent;

      /// <summary>
      /// Number of the enumerable in the collection that is currently traversed:
      ///   -1                  - before the first enumerable
      ///   0 .. parent.Count-1 - in some certain enumerable
      ///   >= parent.Count     - after the last enumerable
      /// </summary>
      private int curEnumerable;

      /// <summary>
      /// The current position of the enumerator in some enumerable from the collection
      /// </summary>
      private IEnumerator<T>? curPosition;

      /// <summary>
      /// Getting an enumerator set before beginning of the first enumerable in the collection
      /// </summary>
      /// <param name="parent">The collection of enumerables to which the enumerator to be connected</param>
      public EnumeratorOverCollection (CollectionOfEnumerables<T> parent)
      {
        this.parent = parent;
        curPosition = null;
        curEnumerable = -1;
      }

      /// <summary>
      /// Constructor setting the enumerator to the first occurence of the given value
      /// in the given collection. If there is no such a value in the entire collection, 
      /// then the enumerator is set to the beginning of the collection
      /// </summary>
      /// <param name="parent">The collection of enumerables to which the enumerator to be connected</param>
      /// <param name="val">The value to which the enumerator should be set</param>
      public EnumeratorOverCollection (CollectionOfEnumerables<T> parent, T val)
      {
        this.parent = parent;

        // The main search loop.
        // If the parent collection is empty, then it cannot contain the target value.
        // In this case, we try to start the main loop, but we shall not come into
        // and, therefore, just bypass to the final part, where the resultant enumerator 
        // is put "before" the collection

        // Initializing the search loop: the current enumerable is the first one, 
        // the current enumerator is put before its beginning
        curEnumerable = 0;
        while (curEnumerable < parent._coll.Count)
        {
          curPosition = parent._coll[curEnumerable].GetEnumerator ();
          while (curPosition.MoveNext ())
          {
            // If the desired value is found, stop the main loop and exit from the constructor
            if (curPosition.Current.Equals (val)) {
              return;
            }
          }
          curEnumerable++;
        }

        // If the current point is reached, no value has been found. 
        // Then set the enumerator before beginning of the first enumerable
        curPosition = null;
        curEnumerable = -1;
      }

      /// <summary>
      /// Getting property showing whether the iterator has a valid value
      /// </summary>
      private bool IsValid => curEnumerable != -1 && curEnumerable < parent._coll.Count;

      /// <summary>
      /// Getting property of the current value
      /// </summary>
      public T Current
      {
        get
        {
          if (!IsValid) {
            throw new InvalidOperationException ();
          } else {
            Debug.Assert(curPosition != null, nameof(curPosition) + " != null");
            return curPosition.Current;
          }
        }
      }

      /// <summary>
      /// Getting property of non-generic interface
      /// </summary>
      object IEnumerator.Current => Current;

      /// <summary>
      /// Dispose method (for the aim of compatibility)
      /// </summary>
      public void Dispose () { }

      /// <summary>
      /// Setting up the enumerator
      /// </summary>
      public void Reset ()
      {
        curPosition = null;
        curEnumerable = -1;
      }

      /// <summary>
      /// Step further
      /// </summary>
      /// <returns>true, if the iterator has been moved successfully; false, otherwise</returns>
      public bool MoveNext ()
      {
        // If the enumerator isn't initialized yet, try to initialize it
        if (curEnumerable == -1)
        {
          // If there is no enumerables in the collection, the MoveNext is impossible
          if (parent._coll.Count == 0) {
            return false;
          }

          // Otherwise, put the enumerator before beginning of the first enumerable
          curEnumerable++;
          curPosition = parent._coll[0].GetEnumerator ();
        }

        // Main loop over enumerables in the collection
        while (curEnumerable < parent._coll.Count)
        {
          Debug.Assert(curPosition != null, nameof(curPosition) + " != null");
          if (curPosition.MoveNext()) {
            return true;
          }

          curEnumerable++;
          if (curEnumerable < parent._coll.Count) {
            curPosition = parent._coll[curEnumerable].GetEnumerator();
          }
        }
        return false;
      }
    }
    #endregion
    
    #region Work with enumerators
    /// <summary>
    /// Returns an enumerator that is set before the beginning of the collection and iterates through it
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<T> GetEnumerator() => new EnumeratorOverCollection (this);

    /// <summary>
    /// Returns an enumerator that is set to the first occurence of the given element in the collection.
    /// If there is no such an element, then the enumerator is put before the beginning of the collection
    /// </summary>
    /// <param name="val">The element, to which the enumerator should be put</param>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<T> GetEnumerator (T val) => new EnumeratorOverCollection (this, val);

    /// <summary>
    /// Getting a non-generic enumerator
    /// </summary>
    /// <returns>A non-generic enumerator set to the beginning of the collection</returns>
    IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
    #endregion

    #region Methods
    /// <summary>
    /// Adding an enumerable to the end of the current collection
    /// All enumerators over the collection should be regarded as invalid
    /// </summary>
    /// <param name="en">The enumerable that should be added</param>
    public void Add (IEnumerable<T> en) => _coll.Add (en);

    /// <summary>
    /// Converting the collection to a continuous list
    /// </summary>
    /// <returns>The list of all elements in the collection</returns>
    private List<T> ToList()
    {
      List<T> res = new List<T>();
      EnumeratorOverCollection it = new EnumeratorOverCollection(this);
      while (it.MoveNext()) {
        res.Add(it.Current);
      }

      return res;
    }

    /// <summary>
    /// Converting the collection to an array
    /// </summary>
    /// <returns>The array of all elements in the collection</returns>
    public T[] ToArray() => ToList().ToArray();
    #endregion
  }
}
