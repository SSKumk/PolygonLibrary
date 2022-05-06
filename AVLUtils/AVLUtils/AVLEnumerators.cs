using System;
using System.Collections;
using System.Collections.Generic;

namespace AVLUtils{
  public partial class AVLTree<TValue>{
    /// <summary>
    /// The class basic for all enumerators used in the library
    /// </summary>
    public abstract class AVLBaseEnumerator : IEnumerator<TValue>{
      /// <summary>
      /// States of the enumerators
      /// </summary>
      protected enum IteratorState{
        /// <summary>
        /// The enumerator is before the "beginning" of the collection (taking into account the direction of the enumerator);
        /// it is invalid here
        /// </summary>
        Before

        ,

        /// <summary>
        /// The enumerator is inside the collection and, therefore, valid
        /// </summary>
        Inside

        ,

        /// <summary>
        /// The enumerator is after the "end" of the collection (taking into account the direction of the enumerator);
        /// it is invalid here
        /// </summary>
        After
      }

      /// <summary>
      /// Current state of the enumerator
      /// </summary>
      protected IteratorState state;

      /// <summary>
      /// Getting property showing whether the iterator has a valid value
      /// </summary>
      public bool IsValid => state == IteratorState.Inside;

      /// <summary>
      /// Reference to the connected collection
      /// </summary>
      protected readonly AVLTree<TValue> tree;

      /// <summary>
      /// The current node of the iterator
      /// </summary>
      internal AVLNode curNode;

      /// <summary>
      /// Path to the current node (keeping all nodes where we turned left, that is, nodes where to we can return)
      /// </summary>
      internal readonly Stack<AVLNode> st;

      /// <summary>
      /// Default constructor for the enumerator (set to the "beginning")
      /// </summary>
      /// <param name="newTree">The tree to be served</param>
      protected AVLBaseEnumerator(AVLTree<TValue> newTree) {
        st = new Stack<AVLNode>();
        tree = newTree;
        state = IteratorState.Before;
      }

      /// <summary>
      /// Getting property of the current value
      /// </summary>
      public TValue Current {
        get {
          if (state != IteratorState.Inside) {
            throw new InvalidOperationException();
          } else {
            return curNode.val;
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
      public void Dispose() { }

      /// <summary>
      /// Setting up the enumerator
      /// </summary>
      public void Reset() {
        st.Clear();
        state = IteratorState.Before;
      }

      /// <summary>
      /// Step further
      /// </summary>
      /// <returns>true, if the iterator has been moved successfully; false, otherwise</returns>
      public virtual bool MoveNext() => MoveNextBase();

      /// <summary>
      /// Internal realization of step further
      /// </summary>
      /// <returns>true, if the iterator has been moved successfully; false, otherwise</returns>
      protected abstract bool MoveNextBase();

      /// <summary>
      /// Internal realization of cyclic step further (based on the regular step further procedure)
      /// </summary>
      /// <returns>true, if the iterator has been moved successfully; false, otherwise</returns>
      protected bool MoveNextCyclic() {
        if (tree._top == null || tree.Count == 0 || state == IteratorState.After) {
          state = IteratorState.After;
          return false;
        }

        MoveNextBase();
        if (state == IteratorState.After) {
          Reset();
          MoveNextBase();
        }

        return true;
      }
    }

    /// <summary>
    /// Class of a direct enumerator for an AVL tree
    /// </summary>
    public class AVLEnumerator : AVLBaseEnumerator{
      /// <summary>
      /// Default constructor for the enumerator (set to the beginning of the tree)
      /// </summary>
      /// <param name="newTree">The tree to be served</param>
      public AVLEnumerator(AVLTree<TValue> newTree) : base(newTree) { }

      /// <summary>
      /// Constructor for the enumerator setting it to the given value or to the next one 
      /// if the given value is absent in the tree
      /// </summary>
      /// <param name="newTree">The tree to be served</param>
      /// <param name="v">The value to be set to</param>
      public AVLEnumerator(AVLTree<TValue> newTree, TValue v)
        : base(newTree) {
        curNode = tree._top;
        if (curNode == null) {
          state = IteratorState.After;
        } else {
          int res = tree.comparer.Compare(v, curNode.val);
          while (curNode != null && res != 0) {
            if (res < 0) {
              st.Push(curNode);
              curNode = curNode.left;
            } else {
              curNode = curNode.right;
            }

            if (curNode != null) {
              res = tree.comparer.Compare(v, curNode.val);
            }
          }

          if (curNode == null) {
            if (st.Count == 0) {
              state = IteratorState.After;
            } else {
              state = IteratorState.Inside;
              curNode = st.Pop();
            }
          } else {
            state = IteratorState.Inside;
          }
        }
      }

      /// <summary>
      /// Subsidiary procedure goes from the current node to the left as far as possible 
      /// (keeping the track in the stack)
      /// </summary>
      private void GoFarLeft() {
        while (curNode.left != null) {
          st.Push(curNode);
          curNode = curNode.left;
        }
      }

      /// <summary>
      /// Internal realization for step further
      /// </summary>
      /// <returns>true, if the iterator has been moved successfully; false, otherwise</returns>
      protected override bool MoveNextBase() {
        switch (state) {
          case IteratorState.After:
            return false;

          case IteratorState.Before when tree._top == null:
            state = IteratorState.After;
            return false;

          case IteratorState.Before:
            st.Clear();
            curNode = tree._top;
            GoFarLeft();
            state = IteratorState.Inside;
            return true;

          default: {
            if (curNode.right != null) {
              curNode = curNode.right;
              GoFarLeft();
              return true;
            } else if (st.Count != 0) {
              curNode = st.Pop();
              return true;
            } else {
              state = IteratorState.After;
              return false;
            }
          }
        }
      }
    }

    /// <summary>
    /// Class of a reverse enumerator for an AVL tree
    /// </summary>
    public class AVLReverseEnumerator : AVLBaseEnumerator{
      /// <summary>
      /// Default constructor for the enumerator (set to the end of the tree)
      /// </summary>
      /// <param name="newTree">The tree to be served</param>
      public AVLReverseEnumerator(AVLTree<TValue> newTree) : base(newTree) { }

      /// <summary>
      /// Constructor for the enumerator setting it to the given value or to the previous one (in direct order)
      /// if the given value is absent in the tree
      /// </summary>
      /// <param name="newTree">The tree to be served</param>
      /// <param name="v">The value to be set to</param>
      public AVLReverseEnumerator(AVLTree<TValue> newTree, TValue v)
        : base(newTree) {
        curNode = tree._top;
        if (curNode == null) {
          state = IteratorState.After;
        } else {
          int res = tree.comparer.Compare(v, curNode.val);
          while (curNode != null && res != 0) {
            if (res > 0) {
              st.Push(curNode);
              curNode = curNode.right;
            } else {
              curNode = curNode.left;
            }

            if (curNode != null) {
              res = tree.comparer.Compare(v, curNode.val);
            }
          }

          if (curNode == null) {
            if (st.Count == 0) {
              state = IteratorState.After;
            } else {
              state = IteratorState.Inside;
              curNode = st.Pop();
            }
          } else {
            state = IteratorState.Inside;
          }
        }
      }

      /// <summary>
      /// Subsidiary procedure goes from the current node to the left as far as possible 
      /// (keeping the track in the stack)
      /// </summary>
      private void GoFarRight() {
        while (curNode.right != null) {
          st.Push(curNode);
          curNode = curNode.right;
        }
      }

      /// <summary>
      /// Internal realization for the step further (in reverse order)
      /// </summary>
      /// <returns>true, if the iterator has been moved successfully; false, otherwise</returns>
      protected override bool MoveNextBase() {
        switch (state) {
          case IteratorState.After:
            return false;

          case IteratorState.Before when tree._top == null:
            state = IteratorState.After;
            return false;

          case IteratorState.Before:
            st.Clear();
            curNode = tree._top;
            GoFarRight();
            state = IteratorState.Inside;
            return true;

          default: {
            if (curNode.left != null) {
              curNode = curNode.left;
              GoFarRight();
              return true;
            } else if (st.Count != 0) {
              curNode = st.Pop();
              return true;
            } else {
              state = IteratorState.After;
              return false;
            }
          }
        }
      }
    }

    /// <summary>
    /// Class of a cyclic direct enumerator for an AVL tree
    /// </summary>
    public class AVLCyclicEnumerator : AVLEnumerator{
      /// <summary>
      /// Default constructor for the enumerator (set to the beginning of the tree)
      /// </summary>
      /// <param name="newTree">The tree to be served</param>
      public AVLCyclicEnumerator(AVLTree<TValue> newTree) : base(newTree) { }

      /// <summary>
      /// Constructor for the enumerator setting it to the given value or to the next one 
      /// if the given value is absent in the tree
      /// </summary>
      /// <param name="newTree">The tree to be served</param>
      /// <param name="v">The value to be set to</param>
      public AVLCyclicEnumerator(AVLTree<TValue> newTree, TValue v)
        : base(newTree, v) {
        if (state == IteratorState.After) {
          Reset();
          MoveNext();
        }
      }

      /// <summary>
      /// Cyclic step further
      /// </summary>
      /// <returns>true, if the iterator has been moved successfully; false, otherwise</returns>
      public override bool MoveNext() => MoveNextCyclic();
    }

    /// <summary>
    /// Class of a cyclic reverse enumerator for an AVL tree
    /// </summary>
    public class AVLCyclicReverseEnumerator : AVLReverseEnumerator{
      /// <summary>
      /// Default constructor for the enumerator (set to the end of the tree)
      /// </summary>
      /// <param name="newTree">The tree to be served</param>
      public AVLCyclicReverseEnumerator(AVLTree<TValue> newTree) : base(newTree) { }

      /// <summary>
      /// Constructor for the enumerator setting it to the given value or to the previous one 
      /// if the given value is absent in the tree
      /// </summary>
      /// <param name="newTree">The tree to be served</param>
      /// <param name="v">The value to be set to</param>
      public AVLCyclicReverseEnumerator(AVLTree<TValue> newTree, TValue v)
        : base(newTree, v) {
        if (state == IteratorState.After) {
          Reset();
          MoveNext();
        }
      }

      /// <summary>
      /// Cyclic step further
      /// </summary>
      /// <returns>true, if the iterator has been moved successfully; false, otherwise</returns>
      public override bool MoveNext() => MoveNextCyclic();
    }
  }
}
