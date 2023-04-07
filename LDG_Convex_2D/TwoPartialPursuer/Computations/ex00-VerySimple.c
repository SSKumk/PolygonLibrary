// Name of the problem
ProblemName = "Simple problem";

// Short name of the problem; to be used in SharpEye
ShortProblemName = "First test";

// Path to the folder whereto the result should be written
path = "./Ex00";


/* =================================================================================
  Block of data defining the dynamics of the game
*/
  
// Dimension of the phase vector
n = 2;

// The main matrix
A = { { 0.0, 0.0 }, { 0.0, 0.0 } };

// Dimension of the useful control
p = 1;

// The useful control matrix
B = { { 0.0 }, { 1.0 } };

// Dimension of the disturbance
q = 1;

// The disturbance matrix
C = { { 1.0 }, { 0.0 } };

// The initial instant
t0 = 0.0;

// The final instant
T = 1.0;

// The time step
dt = 0.05;

/* =================================================================================
  Block of data defining the constraint for the first player's control

  Type of the first player control constraint:
    0 - a convex hull of a collection of points;
        now only for the case p = 2 
        (because the convex hull can be constructed in 2D only)
        is defined by an integer variable pVqnt
        and a pVqnt x 2 double array pVertices
    1 - box constraint; is defined by p x 2 array pBox
    2 - circle; only for p = 2; 
        defined by double variables px0, py0, pR, pVqnt, pAlpha0
    3 - ellipse; only for p = 2
        defined by double variables px0, py0, pa, pb, pPhi, pVqnt, pAlpha0
    
*/  
pConstrType = 1;

pBox = { { -1.0, 1.0 } };

// Flag showing whether to write vectograms of the first player
pWrite = false;


/* =================================================================================
  Block of data defining the constraint for the second player's control

  Type of the second player control constraint:
    0 - a convex hull of a collection of points;
        now only for the case q = 2 
        (because the convex hull can be constructed in 2D only)
        is defined by an integer variable qVqnt
        and a qVqnt x 2 double array qVertices
    1 - box constraint; is defined by q x 2 array qBox


    Need Q, Q1, Q2
        where Q = Q1 \cup Q2 AND intQ1 \cap intQ2 = \empty
*/  
qConstrType = 1;

qBox = { { -1.0, 1.0 } }; 
qBox1 = { { -1.0, -0.5 } };
qBox2 = { { -0.5, 1.0 } };

// Flag showing whether to write vectograms of the second player
qWrite = false;


/* =================================================================================
  Block of data defining the payoff function
  
  Type of the payoff:
    1 - Minkowski functional of a convex set in some given subspace         
        this payoff is described by integer variables payI, payJ defining the indices 
        of the subspace coordinates, an integer variable payVqnt - number of points, which convex hull 
        defines the set, and payVqnt x 2 double array payVertices - these points
*/
payoffType = 1;

// Indices of the two coordinates defining the payoff function (counted from zero)
payI = 0;
payJ = 1;

// Data for the case of Minkowski functional or distance to a set: number of points and their coordinates
payVqnt = 4;
payVertices = { { 1.0, 1.0 }, { -1.0, 1.0 }, { -1.0, -1.0 }, { 1.0, -1.0 } };



/* =================================================================================
  Block of data defining the grid of payoff values, for which the bridges are constructed
  
  Type of the grid:
    0 - uniform grid; defined by double variables cMin, cMax and an integer variable cQnt - 
        total number of points in the grid
    1 - some given grid of values defined by an integer variable cQnt - number of points in the grid -
        and one-dimensipnal array cValues, which contains the desired values
*/
cGridType = 1;

// Number of points in the grid
cQnt = 1;

// The array of values for an arbitrary grid
cValues = { 1.0 };



        
