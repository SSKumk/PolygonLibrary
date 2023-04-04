using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AVLUtils{
  public partial class AVLTree<TValue>{
    /// <summary>
    /// Class of the node of the tree
    /// </summary>
    internal class AVLNode{
      #region The properties of the class
      /// <summary>
      /// Getting property of the stored value
      /// </summary>
      public TValue val { get; }

      /// <summary>
      /// The property for the left child
      /// </summary>
      public AVLNode? left { get; set; }

      /// <summary>
      /// The property for the right child
      /// </summary>
      public AVLNode? right { get; set; }

      /// <summary>
      /// The property for the balance
      /// </summary>
      public int balance { get; set; }

      /// <summary>
      /// Number of data in the subtree of this node (including the data in the node)
      /// </summary>
      public int subtreeQnt { get; set; }
      #endregion

      /// <summary>
      /// Constructor for the node, which takes the new value only
      /// </summary>
      /// <param name="newVal">The value to be stored</param>
      public AVLNode(TValue newVal) {
        val = newVal;
        left = right = null;
        balance = 0;
        subtreeQnt = 1;
      }

      /// <summary>
      /// Calculating the weight of the subtree
      /// </summary>
      public void SetQnt() {
        subtreeQnt = 1;
        if (left != null) {
          subtreeQnt += left.subtreeQnt;
        }

        if (right != null) {
          subtreeQnt += right.subtreeQnt;
        }
      }
    }

    /// <summary>
    /// An auxiliary string for converting the tree to a string. 
    /// Converts a subtree from a given node
    /// </summary>
    /// <param name="curNode">The _top node of the subtree</param>
    /// <param name="res">The string accumulated up to the current instant</param>
    /// <param name="prefix">The prefix of the string</param>
    /// <param name="addPrefix">The additional prefix of the string</param>
    private void ToStringIter(AVLNode? curNode, ref string res, string prefix, string addPrefix) {
      if (curNode == null) {
        res += prefix + "+--X\n";
      } else {
        res += prefix + "+--" +
               curNode.val + " (" + curNode.balance + ";" + curNode.subtreeQnt + ")\n";
        string newPrefix = prefix + addPrefix;
        ToStringIter(curNode.left, ref res, newPrefix, "|  ");
        ToStringIter(curNode.right, ref res, newPrefix, "   ");
      }
    }

    /// <summary>
    /// Default tree representation - as a list of its elements
    /// </summary>
    /// <returns>The resulting string</returns>
    public override string ToString() {
      
      string res = "{";
      bool first = true;
      foreach (TValue val in this) {
        if (first) {
          first = false;
        } else {
          res += ",\n ";
        }

        Debug.Assert(val != null, nameof(val) + " != null");
        res += val.ToString();
      }

      res += "}";

      return res;
    }

    /// <summary>
    /// Converting the tree to a string for pretty output
    /// </summary>
    /// <param name="format">Format string: L - print the tree as a list (default), T - print as a tree </param>
    /// <returns>The resulting string</returns>
    public virtual string ToString(string format) {
      if (format == "L") {
        return ToString();
      }

      if (format != "T") {
        throw new ArgumentException("Bad format string!");
      }

      if (_top == null) {
        return "X\n";
      }  else {
        string res = "" + _top.val + " (" + _top.balance + ";" + _top.subtreeQnt + ")\n";
        ToStringIter(_top.left, ref res, "", "|  ");
        ToStringIter(_top.right, ref res, "", "   ");
        return res;
      }
    }

    /// <summary>
    /// Getting node containing the given value
    /// </summary>
    /// <param name="v">The value to be found</param>
    /// <returns>The node where the value is located, or null if there is no such a value in the tree</returns>
    private AVLNode? GetNode(TValue v) {
      AVLNode? curNode = _top;
      bool notFound = true;

      while (curNode != null && notFound) {
        int res = comparer.Compare(v, curNode.val);
        if (res < 0) {
          curNode = curNode.left;
        } else if (res > 0) {
          curNode = curNode.right;
        } else {
          notFound = false;
        }
      }

      return curNode;
    }

    /// <summary>
    /// Getting node parent to the given one
    /// </summary>
    /// <param name="n">The given node</param>
    /// <returns>The parent node</returns>
    private AVLNode? GetParentNode(AVLNode? n) {
      if (n == null) {
        return null;
      }

      AVLNode? parent = null, cur = _top;
      int res = comparer.Compare(n.val, cur!.val);
      while (cur != null && res != 0) {
        parent = cur;
        cur = res < 0 ? cur.left : cur.right;

        if (cur != null) {
          res = comparer.Compare(n.val, cur.val);
        }
      }

      if (cur == null) {
        return null;
      } else {
        return parent;
      }
    }

    /// <summary>
    /// Getting path to the node containing the given value (or null, if the value is absent in the tree)
    /// </summary>
    /// <param name="v">The given value</param>
    /// <returns>A stack containing the path, or null</returns>
    private Stack<AVLNode>? GetPath(TValue? v) {
      if (_top == null) {
        return null;
      }

      Stack<AVLNode> st = new Stack<AVLNode>();
      AVLNode? cur = _top;
      int res = comparer.Compare(v, cur.val);
      while (cur != null && res != 0) {
        st.Push(cur);
        cur = res < 0 ? cur.left : cur.right;

        if (cur != null) {
          res = comparer.Compare(v, cur.val);
        }
      }

      if (cur == null) {
        return null;
      } else {
        st.Push(cur);
        return st;
      }
    }
  }
}
