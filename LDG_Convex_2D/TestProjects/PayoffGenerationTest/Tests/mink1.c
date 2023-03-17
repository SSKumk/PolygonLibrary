// Path to the folder whereto the result should be written
path = ".";


/* =================================================================================
  Block of data defining the dynamics of the game
*/
  
// Dimension of the phase vector
n = 2;

// The main matrix
A = { { 0.0, 0.0 }, { 0.0, 0.0 } };

// Dimension of the useful control
p = 2;

// The useful control matrix
B = { { 1.0, 0.0 }, { 0.0, 1.0 } };

// Dimension of the disturbance
q = 2;

// The disturbance matrix
C = { { 1.0, 0.0 }, { 0.0, 1.0 } };

// The initial instant
t0 = 0.0;

// The final instant
T = 2.0;

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
pConstrType = 2;

// Number of points, which convex hull defins the constraint for the control of the first player,
// or number of vertices in the case of circle or elliplic constraints
pVqnt = 52;

// Data for circle and elliptic constraint: coordinates of the center
px0 = 0.0;
py0 = 0.0;

// Data for circle constraints: radius of the circle
pR = 1.0;

// Data for circle and elliptic constraints: angle of turn of the initial vertex
pAlpha0 = 0.0;


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
qConstrType = 2;

// Number of points, which convex hull defins the constraint for the control of the second player,
// or number of vertices in the case of circle or elliplic constraints
qVqnt = 52;

// Data for circle and elliptic constraint: coordinates of the center
qx0 = 0.0;
qy0 = 0.0;

// Data for circle constraints: radius of the circle
qR = 0.9;

// Data for circle and elliptic constraints: angle of turn of the initial vertex
qAlpha0 = 0.0;


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
payoffType = 1;

// Indices of the two coordinates defining the payoff function (counted from zero)
payI = 0;
payJ = 1;

// Data for the case of Minkowski functional or distance to a set: number of points and their coordinates
payVqnt = 4;
payVertices = { { 1, 1 }, { -1, 1 }, { -1, -1 }, { 1, -1} };



/* =================================================================================
  Block of data defining the grid of payoff values, for which the bridges are constructed
  
  Type of the grid:
    0 - uniform grid; defined by double variables cMin, cMax and an integer variable cQnt - 
        total number of points in the grid
    1 - some given grid of values defined by an integer variable cQnt - number of points in the grid -
        and one-dimensipnal array cValues, which contains the desired values
*/
cGridType = 0;

// Number of points in the grid
cQnt = 11;

// The interval of values for the uniform grid
cMin = 0.0;
cMax = 1.0;




        