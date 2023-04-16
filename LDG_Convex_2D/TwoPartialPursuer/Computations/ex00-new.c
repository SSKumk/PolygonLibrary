// Name of the problem
ProblemName = "Simple problem";

// Short name of the problem; to be used in SharpEye
ShortProblemName = "First test";

// Path to the folder whereto the result should be written
path = "./Ex00-new";


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
  Block of data defining the Terminal set

  All angles in radians
  Type of the set:
    0 - List of the vertexs: number of points and their coordinates
    1 - Rectangle-parallel: x1 y1 x2 y2 -> oposite vertices
    2 - Rectangle-turned: x1 y1 x2 y2 angle -> oposite vertices and angle between Ox, Oy and sides of the rect.
    3 - Circle: x y R n a0 -> abscissa ordinate radius number_of_verticses turn_angle
    4 - Ellipse: x y a b n phi a0 -> abscissa ordinate one_semiaxis another number_of_vertices turn_angle another_turn_angle  
*/
//The projection coordinates
projI = 0;
projJ = 1;

//Type of the set
typeSet = 4;


// 0 - case example
// MQnt = 4;
// MVert = { { 0.0, 0.0 }, { 1.0, 0.0 }, { 1.0, 1.0 }, { 0.0, 1.0 } };

// 1 - case example
// MRectParallel = {0.0, 0.0, 1.0, 1.0};

// 2 - case example
// MRect = {0.0, 0.0, 1.0, 1.0};
// MAngle = 3.0;

// 3 - case example
// MCenter = {0.0, 0.0};
// MRadius = 1.0;
// MQntVert = 50;
// MAngle = 1.0;

// 4 - case example   x y a b n phi a0
MCenter = {0.0, 0.0};
MSemiaxes = {1.0, 0.5};
MQntVert = 50;
MAngle = 1.0;
MAngleAux = 2.0; 




        
