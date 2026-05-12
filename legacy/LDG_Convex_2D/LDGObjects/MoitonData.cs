using PolygonLibrary.Basics;

using ParamReaderLibrary;

namespace LDGObjects
{
	/// <summary>
	/// Class keeping data on the motion simulation problem
	/// </summary>
	public class MotionData
	{
		#region Motion data
		/// <summary>
		/// Reference to the corresponding object with game data
		/// </summary>
		public GameData gd { get; protected set; }

		/// <summary>
		/// Folder with data files of the problem
		/// </summary>
		public string problemFolder { get; protected set; }

		/// <summary>
		/// Name of the problem to which the motion is computed
		/// </summary>
		public string ProblemName { get; protected set; }

		/// <summary>
		/// Name of the motion
		/// </summary>
		public string MotionName { get; protected set; }

		/// <summary>
		/// Initial time instant 
		/// </summary>
		public double ts { get; protected set; }

		/// <summary>
		/// Initial point type:
		///		 0 - original coordinates(its dimension is equal to the dimension
		///				 of the phase vector given in the basic data of the example)
		///		 1 - equivalent(its dimension is equal to 2)
		/// </summary>
		public int initPointType { get; protected set; }

		/// <summary>
		/// The initial vector in the original coordinates
		/// </summary>
		public Point zs { get; protected set; }

		/// <summary>
		/// The initial vector in the equivalent coordinates
		/// </summary>
		public Point2D xs { get; protected set; }

		// =================================================================================
		// The first player control data

		/// <summary>
		/// The first player control type:
		///  0 - extremal shift control on the basis of computed system of bridges
		///  1 - switching line control (if the control is one dimensional or the constraints
		/// 		 are of box type)
		///  2 - constant
		///  3 - random within the vectogram multiplied by a coefficient
		/// </summary>
		public int pApplContrType { get; protected set; }

		/// <summary>
		/// Constant value in the case of constant constrol
		/// Dimension of the vector equals dimension of the first player control
		/// </summary>
		public Vector pConst { get; protected set; }

		/// <summary>
		/// Multiplier for the constraints in the case of random control
		/// </summary>
		public double pMultiplier { get; protected set; }

		/// <summary>
		/// List of switching surfaces for the first player control components
		/// (if the first player control is generated on the basis of switching surfaces)
		/// </summary>
		public List<SwitchingSurface> pSurfaces { get; protected set; }

		// =================================================================================
		// The second player control data

		/// <summary>
		/// The second player control type:
		///  0 - extremal shift control on the basis of computed system of bridges
		///  1 - switching line control (if the control is one dimensional or the constraints
		/// 		 are of box type)
		///  2 - constant
		///  3 - random within the vectogram multiplied by a coefficient
		/// </summary>
		public int qApplContrType { get; protected set; }

		/// <summary>
		/// Constant value in the case of constant constrol
		/// Dimension of the vector equals dimension of the second player control
		/// </summary>
		public Vector qConst { get; protected set; }

		/// <summary>
		/// Multiplier for the constraints in the case of random control
		/// </summary>
		public double qMultiplier { get; protected set; }

		/// <summary>
		/// List of switching surfaces for the second player control components
		/// (if the second player control is generated on the basis of switching surfaces)
		/// </summary>
		public List<SwitchingSurface> qSurfaces { get; protected set; }

		/// <summary>
		/// The random generated (in the case when at least one control is random)
		/// </summary>
		public Random rnd { get; protected set; }

		// =================================================================================

		/// <summary>
		/// List of stable bridges (if one or both players generate control on the basis
		/// of extremal shift procedure)
		/// </summary>
		public List<StableBridge2D> bridges { get; protected set; }

		/// <summary>
		/// Class keeping data about one trajectory point: 
		///  - time instant of the point;
		///  - the point itself in the equivalent space and, possibly, in the original space;
		///  - the applied controls of the players
		/// </summary>
		public struct TrajectoryPoint
		{
			/// <summary>
			/// Time instant of the point
			/// </summary>
			public double t;

			/// <summary>
			/// Point of the trajectory in the original space; null, if not computed
			/// </summary>
			public Point origPoint;

			/// <summary>
			/// Point of the trajectory in the equivalent space
			/// </summary>
			public Point2D eqPoint;

			/// <summary>
			/// First player control applied in this point (for the next time period in the direct time)
			/// </summary>
			public Point uControl;

			/// <summary>
			/// Second player control applied in this point (for the next time period in the direct time)
			/// </summary>
			public Point vControl;
		}

		/// <summary>
		/// Computed trajectories stored as pairs 
		/// (time instant, (original spape point, equivalent space point))
		/// </summary>
		public List<TrajectoryPoint> traj { get; protected set; }
		#endregion

		/// <summary>
		/// The constructor of the object that reads both game data and motion data
		/// </summary>
		/// <param name="gameDataFolder">Path to the folder where the game data file is located</param>
		/// <param name="gameDataFile">File name with the data of the game, for which the motion to be computed</param>
		/// <param name="motionDataFile">File name with the data on the motion to be computed</param>
		public MotionData(string gameDataFolder, string gameDataFile, string motionDataFile)
		{
			if (gameDataFolder[gameDataFolder.Length - 1] != '\\')
				gameDataFolder += '\\';

			string fullGDFile = gameDataFolder + gameDataFile;
			gd = new GameData(fullGDFile, ComputationType.Motions);

			problemFolder = gameDataFolder + gd.path;
			string motionFullDataFile = problemFolder + motionDataFile;
			ParamReader pr = new ParamReader(motionFullDataFile);

			ProblemName = pr.ReadString("ProblemName");
			if (ProblemName != gd.ProblemName)
				throw new ArgumentException("The problem names in the game and motion files differ");

			MotionName = pr.ReadString("MoitonName");

			ts = pr.ReadDouble("ts");

			int initPointType = pr.ReadInt("initPointType");
			double[] tempArr;

			// Read the initial point in the original space 
			// and compute its projection to the eqiovalent space
			if (initPointType == 0)
			{
				tempArr = pr.Read1DArray<double>("xs", gd.n);
				zs = new Point(tempArr);
				xs = (Point2D)(gd.cauchyMatrix[ts] * zs);
			}
			// Read the initial point in the equivalent space; the point in the original is not defined
			else
			{
				tempArr = pr.Read1DArray<double>("xs", 2);
				zs = null;
				xs = new Point2D(tempArr[0], tempArr[1]);
			}

			bridges = null;

			// Reading data defining the first player control
			pApplContrType = pr.ReadInt("pApplContrType");
			pSurfaces = null;
			switch (pApplContrType)
			{
				// Extremal shift control - read bridges
				case 0:
					ReadBridges();
					break;

				// Switching lines control - check the data and read surfaces
				case 1:
					if (gd.p != 1 && gd.pConstrType != 1)
						throw new ArgumentException(
							"The first player control is generated on the basis of switching surfaces, " +
							"but the control is multidimensional and has non-box constraints");
					ReadSurfaces(1);
					break;

				// Constant control
				case 2:
					pConst = new Vector(pr.Read1DArray<double>("pConst", gd.p));					
					break;

				// Random control
				case 3:
					pMultiplier = pr.ReadDouble("pMultiplier");
					break;
			}

			// Reading data defining the second player control
			qApplContrType = pr.ReadInt("qApplContrType");
			qSurfaces = null;
			switch (qApplContrType)
			{
				// Extremal shift control - read bridges (if read already, nothing happens)
				case 0:
					ReadBridges();
					break;

				// Switching lines control - check the data and read surfaces
				case 1:
					if (gd.q != 1 && gd.qConstrType != 1)
						throw new ArgumentException(
							"The second player control is generated on the basis of switching surfaces, " +
							"but the control is multidimensional and has non-box constraints");
					ReadSurfaces(1);
					break;

				// Constant control
				case 2:
					qConst = new Vector(pr.Read1DArray<double>("qConst", gd.q));
					break;

				// Random control
				case 3:
					qMultiplier = pr.ReadDouble("qMultiplier");
					break;
			}

			// If necessary, read the random seed
			if (pApplContrType == 3 || qApplContrType == 3)
			{
				int seed = pr.ReadInt("randomSeed");
				if (seed == -1)
					rnd = new Random();
				else
					rnd = new Random(seed);
			}
			else
				rnd = null;

			traj = new List<TrajectoryPoint>();
		}

		#region Writing data methods
		/// <summary>
		/// Writing the computed data into the given file in the following format:
		///   ProblemName = "...";
		///   ShortProblemName = "...";
		/// 
		///   hasOriginal = true/false;
		///   [origDim = ...;]
		/// 
		///   pointQnt = ...;
		/// 
		///   // ----------------------------------------------------
		///   t = ...;
		///   eqPoint = { ... , ... };
		///   [origPoint = { ... };]
		///   uControl = { ... };
		///   vControl = { ... };
		///   
		///   // ----------------------------------------------------
		///   ...
		/// </summary>
		/// <param name="sw">The file whereto the data should be written</param>
		public void WriteToFile(StreamWriter sw)
		{
			if (traj.Count == 0)
				throw new Exception("No trajectory has been computed");

			bool hasOriginal = initPointType == 0;

			sw.WriteLine("ProblemName = \"" + gd.ProblemName + "\";");
			sw.WriteLine("ShortProblemName = \"" + gd.ShortProblemName + "\";");
			sw.WriteLine();
			sw.WriteLine("hasOriginal = " + hasOriginal + ";");
			if (hasOriginal)
				sw.WriteLine("origDim = " + gd.n + ";");
			sw.WriteLine();
			sw.WriteLine("pointQnt = " + traj.Count + ";");

			for (int instInd = 0; instInd < traj.Count; instInd++)
			{
				TrajectoryPoint tp = traj[instInd];

				sw.WriteLine();
				sw.WriteLine("// ----------------------------------------------------");
				sw.WriteLine("t = " + tp.t + ";");
				sw.WriteLine("eqPoint = {" + tp.eqPoint.x + ", " + tp.eqPoint.y + "};");
				if (hasOriginal)
				{
					sw.Write("origPoint = {");
					for (int j = 0; j < gd.n; j++)
					{
						sw.Write(tp.origPoint[j]);
						if (j < gd.n - 1)
							sw.Write(", ");
						else
							sw.WriteLine("};");
					}
				}
				sw.Write("uControl = {");
				for (int j = 0; j < gd.p; j++)
				{
					sw.Write(tp.uControl[j]);
					if (j < gd.p - 1)
						sw.Write(", ");
					else
						sw.WriteLine("};");
				}
				sw.Write("vControl = {");
				for (int j = 0; j < gd.q; j++)
				{
					sw.Write(tp.vControl[j]);
					if (j < gd.q - 1)
						sw.Write(", ");
					else
						sw.WriteLine("};");
				}
			}
		}

		/// <summary>
		/// Writing the computed data into the file with the given name
		/// </summary>
		/// <param name="fileName">Name of the file whereto the data should be written</param>
		public void WriteToFile(string fileName)
		{
			StreamWriter sw = new StreamWriter(fileName);
			WriteToFile(sw);
			sw.Close();
		}
		#endregion

		#region Computation and auxiliary methods
		/// <summary>
		/// Auxiliary data: the instant when the shifting vector <see cref="vectExtr"/> is computed
		/// </summary>
		protected double tExtr;

		/// <summary>
		/// Auxiliary data: the shifting vector; stored for the situation when both controls
		/// are generated by the extremal shift procedure
		/// </summary>
		protected Vector2D vectExtr = null;

		/// <summary>
		/// Auxiliary method for generating control on the basis of extremal shift procedure
		/// </summary>
		/// <param name="t">The current time instant</param>
		/// <param name="x">The current point in the equivalent space</param>
		/// <param name="dynamMatr">The matrix of the corresponding player control in the equivalent game dynamics</param>
		/// <param name="polyhedron">The collection of point defining the polyhedron of the player control constraint</param>
		/// <returns>The vector of extremal control</returns>
		protected Vector ExtremalShiftControl(
			double t, Point2D x, Matrix dynamMatr, IEnumerable<Point> polyhedron) {
			throw new NotImplementedException();
/*
 TODO: Finalize the implementation
			int smallestInd, shiftInd;

			// Computing the extremal shift vector, if necessary
			if (vectExtr == null || Tools.NE(tExtr, t))
			{
				// Indices of time section from of the bridges at the given time instant
				int[] secInds = bridges.Select(br => br.GetSectionIndex(t)).ToArray();

				// If there is no non-empty sections, then put the shift vector to zero
				if (secInds[^1] == -1)
					vectExtr = Vector2D.Zero;

				// There are bridges with non-empty sections at the current instant.
				else
				{
					// Search for the smallest bridge having non-empty section at the current instant
					smallestInd = secInds.BinarySearchByPredicate(elem => elem != -1);

					// Search the index of the first bridge containing the point
					shiftInd = secInds.BinarySearchByPredicate(
						brInd => bridges[brInd][secInds[brInd]].section.Contains(x),
						smallestInd, secInds.Length - 1);

					// If the point is inside the smallest bridge, the shifting vector is set to zero
					if (shiftInd == smallestInd)
						vectExtr = Vector2D.Zero;

					// The point is not in the indifference zone. 
					// Find the largest section such that the point is outside it
					else
					{
						// If the point is outside the largest bridge, choose it for shifting
						if (shiftInd == -1)
							shiftInd = secInds.Length - 1;
						// Otherwise shift to the bridge preceding the found one
						else
							shiftInd--;

						// Search the closest point in the found section
					}
				}
			}
*/
		}

		/// <summary>
		/// Generating control of the given player at the given position 
		/// according the chosen method
		/// </summary>
		/// <param name="plNum">The number of the player, 1 or 2</param>
		/// <param name="t">The current time instant</param>
		/// <param name="x">The current equivalent state</param>
		/// <returns>The vector of the generated control</returns>
		protected Vector GenerateControl(int plNum, double t, Point2D x)
		{
			switch (plNum == 1 ? pApplContrType : qApplContrType)
			{
				// Extremal shift control
				case 0:
					if (plNum == 1)
						return ExtremalShiftControl(t, x, gd.cauchyMatrix[t] * gd.B, gd.pVertices);
					else
						return ExtremalShiftControl(t, x, gd.cauchyMatrix[t] * gd.C, gd.qVertices);

				// Switching line control
				case 1:
					throw new NotImplementedException("The switching line control is not implemented yet");
					// break;

				// Constant control
				case 2:
					if (plNum == 1)
						return pConst;
					else
						return qConst;

				// Random control
				case 3:
					throw new NotImplementedException("The random control is not implemented yet");
					// break;
				
				default:
					throw new ArgumentException("Wrong control type in the procedure for generating control");
			}
		}

		public void ComputeTrajectory()
		{
		}
		#endregion

		#region Auxiliary methods for reading resultant data - bridges, surfaces
		/// <summary>
		/// Auxiliary method for reading bridges
		/// </summary>
		protected void ReadBridges()
		{
			// If some bridges are read already, do nothing
			if (bridges != null) return;

			bridges = new List<StableBridge2D>();

			List<string> brFiles = Directory.EnumerateFiles(problemFolder,
				"*." + GameData.Extensions[ComputationType.StableBridge]).ToList();
			if (brFiles.Count == 0)
				throw new ArgumentException("No bridge files have been found");
			foreach (string brFile in brFiles)
				bridges.Add(new StableBridge2D(brFile));

			foreach (StableBridge2D br in bridges)
				if (br.ProblemName != ProblemName)
					throw new ArgumentException("The stable bride read from the file '" +
						StableBridge2D.GenerateFileName(br.c) + "' has problem name '" + br.ProblemName +
						"' that differs from the main problem name '" + ProblemName + "'");

			bridges.Sort();
		}

		/// <summary>
		/// Auxiliary method for reading switching surfaces of a certain player
		/// </summary>
		/// <param name="plNum">Number of the player (1 or 2), for which the surfaces to be read</param>
		protected void ReadSurfaces(int plNum)
		{
			List<SwitchingSurface> surfs;
			int compQnt;
			if (plNum == 1)
			{
				// If the first player surfaces are read already, do nothing
				if (pSurfaces != null) return;
				surfs = pSurfaces = new List<SwitchingSurface>();
				compQnt = gd.p;
			}
			else
			{
				// If the second player surfaces are read already, do nothing
				if (qSurfaces != null) return;
				surfs = qSurfaces = new List<SwitchingSurface>();
				compQnt = gd.q;
			}

			for (int i = 0; i < compQnt; i++)
				surfs.Add(new SwitchingSurface(SwitchingSurface.GenerateFileName(plNum, i)));
		}
		#endregion
	}
}
