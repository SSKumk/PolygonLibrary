using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Class whose exemplar takes a data from a file and disassembles it to objects on demand
  /// </summary>
  public class ParamReader {

    /// <summary>
    /// Enumerable type of the current state of the engine
    /// </summary>
    private enum State {

      /// <summary>
      /// Reading a token
      /// </summary>
      ReadingToken

     ,

      /// <summary>
      /// Waiting for terminal colon
      /// </summary>
      ReadingTerminator

     ,

      /// <summary>
      /// State when the reader is inside a string
      /// </summary>
      ReadingString

     ,

      /// <summary>
      /// State when the token is read
      /// </summary>
      TokenRead

    }

    /// <summary>
    /// The content of the given file
    /// </summary>
    private readonly string data;

    /// <summary>
    /// Current position in the string
    /// </summary>
    private int ind = 0;

    /// <summary>
    /// Current state of the reader
    /// </summary>
    private State state;

    /// <summary>
    /// The only constructor connecting the object to the given file
    /// </summary>
    /// <param name="fileName">The name of the file to be read</param>
    public ParamReader(string fileName) {
      StreamReader sr = new StreamReader(fileName);
      data = sr.ReadToEnd();
      sr.Close();
    }

#region Reading methods
    /// <summary>
    /// Read the next object and treat it as a double.
    /// </summary>
    /// <param name="name">Name of the object.</param>
    /// <returns>The read double value.</returns>
    public T ReadNumber<T>(string name) where T : IParsable<T> {
      ReadNameAndPassEquivalence(name);
      state = State.ReadingToken;
      ReadToken(name, ";", out string readData);

      if (!T.TryParse(readData, CultureInfo.InvariantCulture, out T? res)) {
        throw new Exception
          ("Cannot convert data '" + readData + "' read during parsing object '" + name + "' into " + typeof(T).FullName + "!");
      }

      return res;
    }

    /// <summary>
    /// Read the next object and treat it as a string
    /// </summary>
    /// <param name="name">Name of the object</param>
    /// <returns>The read string value</returns>
    public string ReadString(string name) {
      ReadNameAndPassEquivalence(name);

      state = State.ReadingString;
      string readData = ReadStringToken(name);
      ReadTerminator(name, ";");

      return readData;
    }

    /// <summary>
    /// Method for reading one-dimensional array.
    /// </summary>
    /// <typeparam name="T">The type of array elements must be IParsable: bool, int, double, string, etc.</typeparam>
    /// <param name="name">The name of the object.</param>
    /// <param name="elemQnt">The number of elements in the array.</param>
    /// <returns>The read array of appropriate type and size.</returns>
    public T[] Read1DArray<T>(string name, int elemQnt) where T : IParsable<T> {
      // Reading and checking the name
      ReadNameAndPassEquivalence(name);

      IEnumerable<T> objs = ReadArrayRow<T>(name, elemQnt);
      IEnumerator<T> en   = objs.GetEnumerator();
      T[]            res  = new T[elemQnt];
      for (int i = 0; i < elemQnt; i++) {
        en.MoveNext();
        res[i] = en.Current;
      }
      en.Dispose();

      state = State.ReadingTerminator;
      ReadTerminator(name, ";");

      return res;
    }

    /// <summary>
    /// Reads the vector from file.
    /// </summary>
    /// <param name="name">The name of the vector.</param>
    /// <returns>The read vector.</returns>
    public Vector ReadVector(string name) => new(ReadList<TNum>(name).ToArray());

    /// <summary>
    /// Method for reading a list.
    /// </summary>
    /// <typeparam name="T">The type of array elements must be IParsable: bool, int, double, string, etc.</typeparam>
    /// <param name="name">The name of the object</param>
    /// <returns>The read list of appropriate type and arbitrary size.</returns>
    public List<T> ReadList<T>(string name) where T : IParsable<T> {
      // Reading and checking the name
      ReadNameAndPassEquivalence(name);

      List<T> objs = ReadArrayRow<T>(name).ToList();

      state = State.ReadingTerminator;
      ReadTerminator(name, ";");

      return objs;
    }


    /// <summary>
    /// Method for reading two-dimensional array.
    /// </summary>
    /// <typeparam name="T">The type of array elements must be IParsable: bool, int, double, string, etc.</typeparam>
    /// <param name="name">The name of the object</param>
    /// <returns>The read array of appropriate type and size</returns>
    public List<List<T>> Read2DJaggedArray<T>(string name) where T : IParsable<T> {
      // Reading and checking the name
      ReadNameAndPassEquivalence(name);

      // Passing the initial "{"
      state = State.ReadingTerminator;
      ReadTerminator(name, "{");

      List<List<T>> res = new List<List<T>>();

      char readTerm;
      bool flagContinue = true;
      while (flagContinue) {
        // Reading the coming row of the array
        List<T> objs = ReadArrayRow<T>(name).ToList();
        res.Add(objs);

        // Passing the following symbol: comma or closing '}'
        state    = State.ReadingTerminator;
        readTerm = ReadTerminator(name, ",}");
        if (readTerm == '}') {
          flagContinue = false;
        }
      }

      // Passing the final colon
      state = State.ReadingTerminator;
      ReadTerminator(name, ";");

      return res;
    }

    /// <summary>
    /// Reads the collection of vectors.
    /// </summary>
    /// <param name="name">The name of the collection of vectors.</param>
    /// <returns>The collection of vectors.</returns>
    public List<Vector> ReadVectors(string name)
      => Read2DJaggedArray<TNum>(name).Select(l => new Vector(l.ToArray())).ToList();

    /// <summary>
    /// Method for reading two-dimensional array.
    /// </summary>
    /// <typeparam name="T">The type of array elements must be IParsable: bool, int, double, string, etc.</typeparam>
    /// <param name="name">The name of the object.</param>
    /// <param name="rows">Number of rows in the array.</param>
    /// <param name="cols">Number of columns in the array.</param>
    /// <returns>The read array of appropriate type and size.</returns>
    public T[,] Read2DArray<T>(string name, int rows, int cols) where T : IParsable<T> {
      // Reading and checking the name
      ReadNameAndPassEquivalence(name);

      // Passing the initial '{'
      state = State.ReadingTerminator;
      ReadTerminator(name, "{");

      T[,] res = new T[rows, cols];

      for (int j = 0; j < rows; j++) {
        // Reading the coming row of the array
        IEnumerable<T> objs = ReadArrayRow<T>(name, cols);
        IEnumerator<T> en   = objs.GetEnumerator();
        for (int i = 0; i < cols; i++) {
          en.MoveNext();
          res[j, i] = en.Current;
        }
        en.Dispose();

        // Passing the following symbol: comma or closing '}'
        state = State.ReadingTerminator;
        if (j == rows - 1) {
          ReadTerminator(name, "}");
        } else {
          ReadTerminator(name, ",");
        }
      }

      // Passing the final colon
      state = State.ReadingTerminator;
      ReadTerminator(name, ";");

      return res;
    }
#endregion

#region Internal methods
    /// <summary>
    /// Passing one row comment
    /// </summary>
    private void PassOneRowComment() {
      if (data[ind] == '/' && data[ind + 1] == '/') {
        ind += 2;
        while (data[ind] != '\n')
          ind++;
        ind++;
      } else
        throw new Exception("There is no one row comment!");
    }

    /// <summary>
    /// Passing multirow comment
    /// </summary>
    private void PassMultiRowComment() {
      if (data[ind] == '/' && data[ind + 1] == '*') {
        ind += 2;
        while (data[ind] != '*' || data[ind + 1] != '/')
          ind++;
        ind += 2;
      } else
        throw new Exception("There is no multirow comment!");
    }

    /// <summary>
    /// Method that reads the name of the next object and passes the sign =.
    /// Finally, the read name is compared with the given name
    /// </summary>
    /// <param name="ObjectName">The planned name</param>
    private void ReadNameAndPassEquivalence(string ObjectName) {
      state = State.ReadingToken;
      ReadToken(ObjectName, "=", out string readName);

      if (!ObjectName.Equals(readName))
        throw new Exception("The read name '" + readName + "' doesn't coincide with the given name '" + ObjectName + "'");
    }

    /// <summary>
    /// Method that reads the termination colon
    /// </summary>
    /// <param name="name">The planned object name (for diagnostic messages)</param>
    /// <param name="term">The expected termination symbols</param>
    /// <returns>The actually read terminator.</returns>
    private char ReadTerminator(string name, string term) {
      if (state != State.ReadingTerminator)
        throw new Exception("Internal: ReadTerminator() method is called in a wrong state of the reader");

      char readTerm     = '\0';
      bool flagContinue = true;
      while (flagContinue) {
        if (ind >= data.Length) {
          throw new IndexOutOfRangeException
            ("ReadTerminator: During reading the string, the automaton has reached the end of the string!");
        }
        switch (data[ind]) {
          case ' ':
          case '\t':
          case '\n':
          case '\r':
            ind++;

            break;

          case '/':
            switch (data[ind + 1]) {
              case '/':
                PassOneRowComment();

                break;
              case '*':
                PassMultiRowComment();

                break;
              default:
                throw new Exception("Erroneous symbol '" + data[ind] + "' during reading the terminator of the object " + name);
            }

            break;

          default:
            if (term.Contains(data[ind])) {
              readTerm = data[ind];
              ind++;
              state        = State.TokenRead;
              flagContinue = false;
            } else
              throw new Exception("Erroneous symbol '" + data[ind] + "' during reading the terminator of the object " + name);

            break;
        }
      }

      return readTerm;
    }

    /// <summary>
    /// Method that reads a token and the given terminator after it
    /// </summary>
    /// <param name="name">The planned object name (for diagnostic messages)</param>
    /// <param name="term">The expected termination symbol</param>
    /// <param name="readData">The data has been read up to the terminator.</param>
    private char ReadToken(string name, string term, out string readData) {
      if (state != State.ReadingToken)
        throw new Exception("Internal: ReadToken() method is called in a wrong state of the reader");

      readData = "";
      bool flagContinue = true;
      char readTerm     = '\0';

      while (flagContinue) {
        if (ind >= data.Length) {
          throw new IndexOutOfRangeException
            ("ReadToken: During reading the string, the automaton has reached the end of the string!");
        }
        switch (state) {
          case State.ReadingToken:
            switch (data[ind]) {
              case ' ':
              case '\t':
              case '\n':
              case '\r':
                ind++;
                if (readData != "") {
                  state        = State.ReadingTerminator;
                  flagContinue = false;
                }

                break;

              case '/':
                switch (data[ind + 1]) {
                  case '/': {
                    PassOneRowComment();
                    if (readData != "") {
                      state        = State.ReadingTerminator;
                      flagContinue = false;
                    }

                    break;
                  }
                  case '*': {
                    PassMultiRowComment();
                    if (readData != "") {
                      state        = State.ReadingTerminator;
                      flagContinue = false;
                    }

                    break;
                  }
                  default:
                    readData += data[ind];
                    ind++;

                    break;
                }

                break;
              default:
                if (term.Contains(data[ind])) {
                  readTerm = data[ind];
                  ind++;
                  state        = State.TokenRead;
                  flagContinue = false;
                } else {
                  readData += data[ind];
                  ind++;
                }

                break;
            }

            break;
        }
      }

      if (state == State.ReadingTerminator) {
        readTerm = ReadTerminator(name, term);
      }

      return readTerm;
    }

    /// <summary>
    /// Method that reads a string token according rules od string definition
    /// </summary>
    /// <param name="name">The name of the object</param>
    /// <returns>The read string</returns>
    private string ReadStringToken(string name) {
      if (state != State.ReadingString)
        throw new Exception("Internal: ReadStringToken() method is called in a wrong state of the reader");

      bool   flagContinue = true;
      string readData     = "";

      state = State.ReadingTerminator;
      ReadTerminator(name, "\"");

      while (flagContinue) {
        switch (data[ind]) {
          case '\"':
            ind++;
            state        = State.ReadingTerminator;
            flagContinue = false;

            break;

          case '\\':
            readData += data[ind + 1] switch
                          {
                            '\\' => '\\'
                          , '"'  => '"'
                          , 'n'  => '\n'
                          , 'r'  => '\r'
                          , 't'  => '\t'
                          , _ => throw new Exception
                                   ("Erroneous escape symbol '\\" + data[ind + 1] + "' when reading string object '" + name + "'")
                          };
            ind += 2;

            break;

          default:
            readData += data[ind];
            ind++;

            break;
        }
      }

      return readData;
    }

    // /// <summary>
    // /// Method that reads a collection of objects given in '{...}' and separated by comma
    // /// </summary>
    // /// <typeparam name="T">The type of array elements must be IParsable: int, double, string, etc.</typeparam>
    // /// <param name="name">The name of the object</param>
    // /// <param name="elemQnt">The number of elements in the array</param>
    // /// <returns>Lazy enumerable of the read objects</returns>
    // private IEnumerable<T> ReadArrayRow<T>(string name, int elemQnt) where T : IParsable<T> {
    //   Type elemType = typeof(T);
    //
    //   // Passing up to the opening "{"
    //   state = State.ReadingTerminator;
    //   ReadTerminator(name, "{");
    //
    //   // Reading and parsing elements
    //   string readData;
    //   if (elemType == typeof(String)) {
    //     for (int i = 0; i < elemQnt; i++) {
    //       state    = State.ReadingString;
    //       readData = ReadStringToken(name);
    //
    //       ReadTerminator(name, i == elemQnt - 1 ? '}' : ',');
    //
    //       yield return (T)(object)readData;
    //     }
    //   } else {
    //     for (int i = 0; i < elemQnt; i++) {
    //       state    = State.ReadingToken;
    //       readData = ReadToken(name, i == elemQnt - 1 ? '}' : ',');
    //
    //       if (!T.TryParse(readData, CultureInfo.InvariantCulture, out T? result)) {
    //         throw new Exception
    //           (
    //            "Cannot convert data '" + readData + "' read during parsing object '" + name + "' into " + elemType.FullName + "!"
    //           );
    //       }
    //
    //       yield return result;
    //     }
    //   }
    // }

    /// <summary>
    /// Method that reads a collection of objects given in '{...}' and separated by comma
    /// </summary>
    /// <typeparam name="T">The type of array elements must be IParsable: int, double, string, etc.</typeparam>
    /// <param name="name">The name of the object</param>
    /// <param name="elemQnt"></param>
    /// <returns>Lazy enumerable of the read objects</returns>
    private IEnumerable<T> ReadArrayRow<T>(string name, int? elemQnt = null) where T : IParsable<T> {
      Type elemType = typeof(T);

      // Passing up to the opening "{"
      state = State.ReadingTerminator;
      ReadTerminator(name, "{");

      // Reading and parsing elements
      string readData;
      bool   flagContinue = true;
      char   readTerm;
      if (elemType == typeof(string)) {
        int i = 0;
        while (flagContinue) {
          state    = State.ReadingString;
          readData = ReadStringToken(name);
          readTerm = ReadTerminator(name, ",}");
          if (readTerm == '}') {
            if (elemQnt is not null) {
              if (i != elemQnt - 1) {
                throw new IndexOutOfRangeException
                  ("ReadArrayRow: Tokens in the string before '}' must be " + elemQnt + ", but found only " + i + "!");
              }
            }
            flagContinue = false;
          }
          i++;

          yield return (T)(object)readData;
        }
      } else {
        int i = 0;
        while (flagContinue) {
          state    = State.ReadingToken;
          readTerm = ReadToken(name, ",}", out readData);
          if (readTerm == '}') {
            if (elemQnt is not null) {
              if (i != elemQnt - 1) {
                throw new IndexOutOfRangeException
                  ("ReadArrayRow: Tokens in the string before '}' must be " + elemQnt + ", but found only " + i + "!");
              }
            }
            flagContinue = false;
          }
          if (!T.TryParse(readData, CultureInfo.InvariantCulture, out T? result)) {
            throw new Exception
              (
               "ReadArrayRow: Cannot convert data '" + readData + "' read during parsing object '" + name + "' into " +
               typeof(T).FullName + "!"
              );
          }
          i++;

          yield return result;
        }
      }
    }
#endregion

  }

}
