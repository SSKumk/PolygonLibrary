using System;
using System.Collections.Generic;

namespace AVLUtils
{
  /// <summary>
  /// A class of non-cyclic AVL tree, which allows to change comparer during life of the object.
  /// Consistency of the data structure should be provided by the user !
  /// There is a method which checks the consistency with the current comparer
  /// </summary>
  /// <typeparam name="TValue">The type of the elements of the tree</typeparam>
  public class AVLTreeUnsafe<TValue> : AVLTree<TValue>, IUnsafeContainer<TValue>
  {
    #region Unsafe trees methods
    /// <summary>
    /// Checks consistency of the current structure of the tree
    /// </summary>
    /// <returns>true, if the order</returns>
    public bool CheckConsistency ()
    {
      if (Count <= 1) {
        return true;
      }

      IEnumerator<TValue> en = GetEnumerator ();
      TValue prev = en.Current;
      en.MoveNext ();
      do
      {
        if (comparer.Compare(prev, en.Current) > 0)
        {
          en.Dispose();
          return false;
        }

        prev = en.Current;
      } while (en.MoveNext ());

      en.Dispose();
      return true;
    }

    /// <summary>
    /// Rebuild the tree according to the current comparer
    /// </summary>
    public void Rebuild ()
    {
      IEnumerable<TValue> l = ToList ();
      Clear ();
      AddRange (l);
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
      comparer = newComp;
#if DEBUG
      if (!CheckConsistency ()) {
        throw new ArgumentException ();
      }
#else
      if (checkAfter && !CheckConsistency ()) {
        throw new ArgumentException ();
      }
#endif
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Default constructor with default order in the tree
    /// </summary>
    public AVLTreeUnsafe () { }

    /// <summary>
    /// Constructor that sets given order in the tree
    /// </summary>
    /// <param name="newComp">The comparer that defines the order in the tree</param>
    public AVLTreeUnsafe (IComparer<TValue> newComp) : base (newComp) { }
    #endregion 
  }
}
