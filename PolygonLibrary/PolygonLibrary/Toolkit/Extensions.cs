﻿namespace CGLibrary;

/// <summary>
/// Extensions to standard ListOfT container
/// </summary>
public static class ListExtensions {

  /// <summary>
  /// Method realizing binary search in a list by a predicate.
  /// </summary>
  /// <typeparam name="T">The type elements in the list</typeparam>
  /// <param name="list">The list object</param>
  /// <param name="pred">The predicate distinguishing elements</param>
  /// <returns>
  /// The method searches for the index of the first element that gives true for the predicate.
  /// If there are no such elements, the result is -1.
  /// </returns>
  /// <remarks>
  /// It is assumed that some initial part of the list contains elements that do not
  /// obey the predicate and the complementary tail part of the list consists of
  /// elements that provide true to the predicate.
  /// </remarks>
  public static int BinarySearchByPredicate<T>(this List<T> list, Predicate<T> pred) {
    if (list == null || list.Count == 0) {
      return -1;
    }

    return BinarySearchByPredicate(list, pred, 0, list.Count - 1);
  }

  /// <summary>
  /// Method realizing binary search in a part of a list by a predicate.
  /// </summary>
  /// <typeparam name="T">The type elements in the list</typeparam>
  /// <param name="list">The list object</param>
  /// <param name="pred">The predicate distinguishing elements</param>
  /// <param name="lower">The lower index of the part where to search</param>
  /// <param name="upper">The upper index of the part where to search</param>
  /// <returns>
  /// The method searches for the index of the first element that gives true for the predicate.
  /// If there are no such elements, the result is -1.
  /// </returns>
  /// <remarks>
  /// It is assumed that some head of the shown part of the list contains elements that
  /// do not obey the predicate and the complementary tail of the shown part of the list
  /// consists of elements that provide true to the predicate.
  ///
  /// If <see cref="lower"/> is less than 0, it is set to 0.
  /// If <see cref="upper"/> is greater or equal to the size of the list, it is set to size - 1.
  /// If <see cref="lower"/> is greater than <see cref="upper"/>, the method returns -1.
  /// </remarks>
  public static int BinarySearchByPredicate<T>(this List<T> list, Predicate<T> pred, int lower, int upper) {
    if (list == null) {
      return -1;
    }

    if (lower < 0) {
      lower = 0;
    }

    if (upper >= list.Count) {
      upper = list.Count - 1;
    }

    if (upper < lower || !pred(list[upper])) {
      return -1;
    }

    if (pred(list[lower])) {
      return lower;
    }

    while (lower + 1 < upper) {
      int mid = (lower + upper) / 2;
      if (pred(list[mid])) {
        upper = mid;
      }
      else {
        lower = mid;
      }
    }

    return upper;
  }

  /// <summary>
  /// Auxiliary method that normalizes index - reduces it to the range [0,size)
  /// by modulo size.
  /// </summary>
  /// <typeparam name="T">Type of the elements of the list</typeparam>
  /// <param name="list">The list object</param>
  /// <param name="index">Non-normalized index - any integer</param>
  /// <returns>The normalized index from the range [0,size)</returns>
  public static int NormalizeIndex<T>(this List<T> list, int index) {
    if (index < 0) {
      return index % list.Count + list.Count;
    }
    else {
      return index % list.Count;
    }
  }

  /// <summary>
  /// Method realizing cyclic binary search in a part of a list by a predicate.
  /// </summary>
  /// <typeparam name="T">The type elements in the list</typeparam>
  /// <param name="list">The list object</param>
  /// <param name="pred">The predicate distinguishing elements</param>
  /// <param name="lower">The lower index of the part where to search</param>
  /// <param name="upper">The upper index of the part where to search</param>
  /// <returns>
  /// The method searches for the index of the first element that gives true for the predicate.
  /// The index is from range [0,size). If there are no such elements, the result is -1.
  /// </returns>
  /// <remarks>
  /// It is assumed that some head of the shown part of the list contains elements that
  /// do not obey the predicate and the complementary tail of the shown part of the list
  /// consists of elements that provide true to the predicate.
  ///
  /// The indices <see cref="lower"/> and <see cref="upper"/> are treated cyclically and,
  /// therefore, can be less than 0 and greater or equal to the size of the list.
  /// If <see cref="lower"/> is greater than <see cref="upper"/>, the method searches
  /// in the final part of the list, which starts from <see cref="upper"/>, and cyclically
  /// passes to the initial part of the list, which finishes at <see cref="lower"/>.
  /// </remarks>
  public static int BinaryCyclicSearchByPredicate<T>(this List<T> list, Predicate<T> pred, int lower, int upper) {
    if (list == null) {
      return -1;
    }

    if (!pred(list.GetAtCyclic(upper))) {
      return -1;
    }

    if (pred(list.GetAtCyclic(lower))) {
      return lower;
    }

    lower = list.NormalizeIndex(lower);
    upper = list.NormalizeIndex(upper);
    if (upper < lower) {
      upper += list.Count;
    }

    while (lower + 1 < upper) {
      int mid = (lower + upper) / 2;
      if (pred(list.GetAtCyclic(mid))) {
        upper = mid;
      }
      else {
        lower = mid;
      }
    }

    return list.NormalizeIndex(upper);
  }

  /// <summary>
  /// Get an element from the list whereas the index can be any integer
  /// reduced to the interval [0, size) by modulo size
  /// </summary>
  /// <typeparam name="T">The type of elements in the list</typeparam>
  /// <param name="list">The list object</param>
  /// <param name="i">The index</param>
  /// <exception cref="ArgumentNullException"></exception>
  /// <returns>The necessary element</returns>
  public static T GetAtCyclic<T>(this List<T> list, int i) {
    ArgumentNullException.ThrowIfNull(list, "List.GetAtCyclic: a null list");

    if (list.Count == 0) {
      throw new ArgumentException("List.GetAtCyclic: an empty list");
    }

    return list[list.NormalizeIndex(i)];
  }

  /// <summary>
  /// Shuffle the array using the Durstenfeld's algorithm
  /// </summary>
  /// <typeparam name="T">Array element type</typeparam>
  /// <param name="array">Array to shuffle</param>
  /// <param name="rnd">A random generator. If is not passed, some internal generator will be used</param>
  public static void Shuffle<T>(this List<T> array, RandomLC? rnd = null) {
    int      n   = array.Count;
    RandomLC random = rnd ?? new RandomLC();
    for (int i = 0; i < n; i++) {
      int r = random.NextInt(0, i);
      (array[r], array[i]) = (array[i], array[r]);
    }
  }

  /// <summary>
  /// Finds all subsets of a given length for an array.
  /// </summary>
  /// <typeparam name="T">The type of the array elements.</typeparam>
  /// <param name="arr">The input array.</param>
  /// <param name="subsetLength">The length of the subsets.</param>
  /// <returns>A list of subsets of the specified length.</returns>
  public static List<List<T>> Subsets<T>(this IReadOnlyList<T> arr, int subsetLength) {
    List<List<T>> subsets = new List<List<T>>();

    void FindSubset(List<T> currentSubset, int currentIndex) {
      if (currentSubset.Count == subsetLength) {
        subsets.Add(new List<T>(currentSubset));

        return;
      }

      if (currentIndex == arr.Count) {
        return;
      }

      FindSubset(new List<T>(currentSubset) { arr[currentIndex] }, currentIndex + 1);
      FindSubset(new List<T>(currentSubset), currentIndex + 1);
    }

    FindSubset(new List<T>(), 0);

    return subsets;
  }

  /// <summary>
  /// Generates all subsets of a given list.
  /// </summary>
  /// <typeparam name="T">The type of elements in the list.</typeparam>
  /// <param name="arr">The input list.</param>
  /// <returns>A list of all subsets of the input list.</returns>
  public static List<List<T>> AllSubsets<T>(this IReadOnlyList<T> arr) {
    List<List<T>> subsets      = new List<List<T>>();
    int           totalSubsets = 1 << arr.Count;

    for (int i = 0; i < totalSubsets; i++) {
      List<T> subset = new List<T>();

      for (int j = 0; j < arr.Count; j++) {
        if ((i & (1 << j)) != 0) {
          subset.Add(arr[j]);
        }
      }

      subsets.Add(subset);
    }

    return subsets;
  }

}

/// <summary>
/// Extensions to standard array
/// </summary>
public static class ArrayExtensions {

  /// <summary>
  /// Method realizing binary search in an array by a predicate.
  /// </summary>
  /// <typeparam name="T">The type elements in the array</typeparam>
  /// <param name="array">The array object</param>
  /// <param name="pred">The predicate distinguishing elements</param>
  /// <returns>
  /// The method searches for the index of the first element that gives true for the predicate.
  /// If there are no such elements, the result is -1.
  /// </returns>
  /// <remarks>
  /// It is assumed that some initial part of the array contains elements that do not
  /// obey the predicate and the complementary tail part of the array consists of
  /// elements that provide true to the predicate.
  /// </remarks>
  public static int BinarySearchByPredicate<T>(this T[] array, Predicate<T> pred) {
    if (array == null || array.Rank > 1 || array.Length == 0) {
      return -1;
    }

    return BinarySearchByPredicate(array, pred, 0, array.Length - 1);
  }

  /// <summary>
  /// Method realizing binary search in a part of an array by a predicate.
  /// </summary>
  /// <typeparam name="T">The type elements in the array</typeparam>
  /// <param name="array">The array object</param>
  /// <param name="pred">The predicate distinguishing elements</param>
  /// <param name="lower">The lower index of the part where to search</param>
  /// <param name="upper">The upper index of the part where to search</param>
  /// <returns>
  /// The method searches for the index of the first element that gives true for the predicate.
  /// If there are no such elements, the result is -1.
  /// </returns>
  /// <remarks>
  /// It is assumed that some head of the shown part of the array contains elements that
  /// do not obey the predicate and the complementary tail of the shown part of the array
  /// consists of elements that provide true to the predicate.
  ///
  /// If <see cref="lower"/> is less than 0, it is set to 0.
  /// If <see cref="upper"/> is greater or equal to the size of the array, it is set to size - 1.
  /// If <see cref="lower"/> is greater than <see cref="upper"/>, the method returns -1.
  /// </remarks>
  public static int BinarySearchByPredicate<T>(this T[] array, Predicate<T> pred, int lower, int upper) {
    if (array == null || array.Rank > 1) {
      return -1;
    }

    if (lower < 0) {
      lower = 0;
    }

    if (upper >= array.Length) {
      upper = array.Length - 1;
    }

    if (upper < lower || !pred(array[upper])) {
      return -1;
    }

    if (pred(array[lower])) {
      return lower;
    }

    while (lower + 1 < upper) {
      int mid = (lower + upper) / 2;
      if (pred(array[mid])) {
        upper = mid;
      }
      else {
        lower = mid;
      }
    }

    return upper;
  }

  /// <summary>
  /// Auxiliary method that normalizes index - reduces it to the range [0,size)
  /// by modulo size.
  /// </summary>
  /// <typeparam name="T">Type of the elements of the array</typeparam>
  /// <param name="array">The array object</param>
  /// <param name="index">Non-normalized index - any integer</param>
  /// <returns>The normalized index from the range [0,size)</returns>
  private static int NormalizeIndex<T>(this IReadOnlyCollection<T> array, int index) {
    if (index < 0) {
      return index % array.Count + array.Count;
    }
    else {
      return index % array.Count;
    }
  }

  /// <summary>
  /// Method realizing cyclic binary search in a part of a array by a predicate.
  /// </summary>
  /// <typeparam name="T">The type elements in the array</typeparam>
  /// <param name="array">The array object</param>
  /// <param name="pred">The predicate distinguishing elements</param>
  /// <param name="lower">The lower index of the part where to search</param>
  /// <param name="upper">The upper index of the part where to search</param>
  /// <returns>
  /// The method searches for the index of the first element that gives true for the predicate.
  /// The index is from range [0,size). If there are no such elements, the result is -1.
  /// </returns>
  /// <remarks>
  /// It is assumed that some head of the shown part of the array contains elements that
  /// do not obey the predicate and the complementary tail of the shown part of the array
  /// consists of elements that provide true to the predicate.
  ///
  /// The indices <see cref="lower"/> and <see cref="upper"/> are treated cyclically and,
  /// therefore, can be less than 0 and greater or equal to the size of the list.
  /// If <see cref="lower"/> is greater than <see cref="upper"/>, the method searches
  /// in the final part of the array, which starts from <see cref="upper"/>, and cyclically
  /// passes to the initial part of the array, which finishes at <see cref="lower"/>.
  /// </remarks>
  public static int BinaryCyclicSearchByPredicate<T>(this T[] array, Predicate<T> pred, int lower, int upper) {
    if (array == null) {
      return -1;
    }

    if (!pred(array.GetAtCyclic(upper))) {
      return -1;
    }

    if (pred(array.GetAtCyclic(lower))) {
      return lower;
    }

    lower = array.NormalizeIndex(lower);
    upper = array.NormalizeIndex(upper);
    if (upper < lower) {
      upper += array.Length;
    }

    while (lower + 1 < upper) {
      int mid = (lower + upper) / 2;
      if (pred(array.GetAtCyclic(mid))) {
        upper = mid;
      }
      else {
        lower = mid;
      }
    }

    return array.NormalizeIndex(upper);
  }

  /// <summary>
  /// Get an element from the array whereas the index can be any integer
  /// reduced to the interval [0, size) by modulo size
  /// </summary>
  /// <typeparam name="T">The type of elements in the array</typeparam>
  /// <param name="array">The array object</param>
  /// <param name="i">The index</param>
  /// <returns>The necessary element</returns>
  private static T GetAtCyclic<T>(this IReadOnlyList<T> array, int i) {
    ArgumentNullException.ThrowIfNull(array, "Array.GetAtCyclic: null array");

    if (array.Count == 0) {
      throw new ArgumentException("Array.GetAtCyclic: empty array");
    }

    return array[array.NormalizeIndex(i)];
  }

}

/// <summary>
/// Extensions to standard LinkedList container
/// </summary>
public static class LinkedListExtensions {

  /// <summary>
  /// Cyclic shift to left by one position
  /// </summary>
  /// <example>2 4 1 5 --> 4 1 5 2</example>
  /// <param name="lst">The linked list to be cycled</param>
  /// <typeparam name="T">The type of elements in the linked list</typeparam>
  public static void CyclicShift<T>(this LinkedList<T> lst) {
    ArgumentNullException.ThrowIfNull(lst, $"{lst} is null");

    T temp = lst.First();
    lst.RemoveFirst();
    lst.AddLast(temp);
  }

}

public static class EnumerableExtensions {

  public static SortedSet<T> ToSortedSet<T>(this IEnumerable<T> source) => new(source);

}
