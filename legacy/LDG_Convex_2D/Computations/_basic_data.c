// Name of the problem; to be written in all resultant files
// for checking their consistency
string ProblemName;

// Short name of the problem; to be used in SharpEye
string ShortProblemName;

// Path to the folder whereto the result should be written
string path;


/* =================================================================================
  Block of data defining the dynamics of the game
*/
  
// Dimension of the phase vector
int n;

// The main matrix
double A[n][n];

// Dimension of the useful control
int p;

// The useful control matrix
double B[n][p];

// Dimension of the disturbance
int q;

// The disturbance matrix
double C[n][q];

// The initial instant
double t0;

// The final instant
double T;

// The time step
double dt;

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
int pConstrType;

// Number of points, which convex hull defins the constraint for the control of the first player,
// or number of vertices in the case of circle or elliplic constraints
int pVqnt;

// Collection of points, which convex hull defins the constraint for the control of the first player
double pVertices[pVqnt][2];

// Array of per coordinate boundaries for the case of a box constraint for the first player's control
double pBox[p][2];

// Data for circle and elliptic constraint: coordinates of the center
double px0; 
double py0;

// Data for circle constraints: radius of the circle
double pR;

// Data for elliptic constraints: semiaxes of the ellipse and angle of turn of the semiaxis a
double pa; 
double pb; 
double pPhi;

// Data for circle and elliptic constraints: angle of turn of the initial vertex
double pAlpha0;

// Flag showing whether to write vectograms of the first player;
// The output file name is standard "pVectograms.bridge" with ContentType = "First player's vectorgram"
bool pWrite;

/* =================================================================================
  Block of data defining the constraint for the second player's control

  Type of the second player control constraint:
    0 - a convex hull of a collection of points;
        now only for the case q = 2 
        (because the convex hull can be constructed in 2D only)
        is defined by an integer variable qVqnt
        and a qVqnt x 2 double array qVertices
    1 - box constraint; is defined by q x 2 array qBox
    2 - circle; only for q = 2; 
        defined by double variables qx0, qy0, qR, qVqnt, qAlpha0
    3 - ellipse; only for q = 2
        defined by double variables qx0, qy0, qa, qb, qPhi, qVqnt, qAlpha0
    
*/  
int qConstrType;

// Number of points, which convex hull defins the constraint for the control of the second player,
// or number of vertices in the case of circle or elliplic constraints
int qVqnt;

// Collection of points, which convex hull defins the constraint for the control of the second player
double qVertices[qVqnt][2];

// Array of per coordinate boundaries for the case of a box constraint for the second player's control
double qBox[q][2];

// Data for circle and elliptic constraint: coordinates of the center
double qx0; 
double qy0;

// Data for circle constraints: radius of the circle
double qR;

// Data for elliptic constraints: semiaxes of the ellipse and angle of turn of the semiaxis a
double qa; 
double qb; 
double qPhi;

// Data for circle and elliptic constraints: angle of turn of the initial vertex
double qAlpha0;

// Flag showing whether to write vectograms of the second player;
// The output file name is standard "pVectograms.bridge" with ContentType = "Second player's vectorgram"
bool qWrite;


/* =================================================================================
  Block of data defining the payoff function
  
  Type of the payoff:
    0 - distance to a given point in some given subspace;  
        this payoff is described by integer variables payI, payJ defining the indices 
        of the subspace coordinates and double variables payX, payY defining the coordinates
        of the ponit
    1 - Minkowski functional of a convex set in some given subspace         
        this payoff is described by integer variables payI, payJ defining the indices 
        of the subspace coordinates, an integer variable payVqnt - number of points, which convex hull 
        defines the set, and payVqnt x 2 double array payVertices - these points
    2 - distance to a convex set in some given subspace         
        this payoff is described by integer variables payI, payJ defining the indices 
        of the subspace coordinates, an integer variable payVqnt - number of points, which convex hull 
        defines the set, and payVqnt x 2 double array payVertices - these points
*/
int payoffType;

// Indices of the two coordinates defining the payoff function (counted from zero)
int payI;
int payJ;

// Data for the case of distance to a point - coordinates of the point
double payX;
double payY;

/* 
  Approximation quality of a payoff level set:
    - for the case of Minkowski functional or distance to a set: number of points in the set definition
    - for the case of distance to a point, number of vertices in the circle approximation
*/
int payVqnt;

// Definition of the set for the distance to a set variant or Minkowski functional case
double payVertices[payVqnt][2];


/* =================================================================================
  Block of data defining the grid of payoff values, for which the bridges are constructed
  
  Type of the grid:
    0 - uniform grid; defined by double variables cMin, cMax and an integer variable cQnt - 
        total number of points in the grid
    1 - some given grid of values defined by an integer variable cQnt - number of points in the grid -
        and one-dimensipnal array cValues, which contains the desired values
*/
int cGridType;

// Number of points in the grid
int cQnt;

// The interval of values for the uniform grid
double cMin;
double cMax;

// The array of values for an arbitrary grid
double cValues[cQnt];




        