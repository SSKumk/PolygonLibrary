// Name of the problem
ProblemName = "Simplest problem";

// Path to the folder whereto the result should be written
path = "./Ex00-new";


/* =================================================================================
  Block of data defining the dynamics of the game
*/

// Dimension of the phase vector
n = 3;

// The main matrix
A = {{0.0, 0.0, 0.0}, {0.0, 0.0, 0.0}, {0.0, 0.0, 0.0}};

// Dimension of the useful control
p = 3;

// The useful control matrix
B = {{1.0, 0.0, 0.0}, {0.0, 1.0, 0.0}, {0.0, 0.0, 1.0}};

// Dimension of the disturbance
q = 3;

// The disturbance matrix
C = {{1.0, 0.0, 0.0}, {0.0, 1.0, 0.0}, {0.0, 0.0, 1.0}};

// The initial instant
t0 = 0.0;

// The final instant
T = 1.0;

// The time step
dt = 0.1;

// The dimension of projected space
d = 3;

// The indices to project onto
projJ = {0, 1, 2};

/* =================================================================================
  Block of data defining the constraint for the first player's control

  All angles in radians
  Type of the set:

    1 - List of the vertices:

    2 - Rectangle-parallel:

    4 - Circle:

*/


// 0 - case example
PTypeSet = 1;
// PQnt = 3;
// PVert = {{1.0, 0.0, 0.0}, {0.0, 1.0, 0.0}, {0.0, 0.0, 1.0}};

// 1 - case example
PTypeSet = 2;
PRectParallelLeft = {0, 0, 0};
PRectParallelRight = {1, 1, 1};


// 2 - case example -- Sphere
// PTypeSet = ;



/* =================================================================================
  Block of data defining the constraint for the second player's control

Block of data defining the constraint for the first player's control

  All angles in radians
  Type of the set:

    1 - List of the vertices:

    2 - Rectangle-parallel:

    4 - Circle:

*/


// 0 - case example
QTypeSet = 1;
// PQnt = 3;
// PVert = {{1.0, 0.0, 0.0}, {0.0, 1.0, 0.0}, {0.0, 0.0, 1.0}};

// 1 - case example
QTypeSet = 2;
QRectParallelLeft = {0, 0, 0};
QRectParallelRight = {1, 1, 1};


// 2 - case example -- Sphere
// PTypeSet = ;

/* =================================================================================
  Block of data defining the Terminal set

  All angles in radians
  Type of the set:
    1 - List of the vertices: PQnt PVert --> number_of_points and their_coordinates
          PQnt = 4;
          PVert = { { 0.0, 0.0 }, { 1.0, 0.0 }, { 1.0, 1.0 }, { 0.0, 1.0 } };

    2 - Rectangle-parallel: x1 y1 x2 y2 -> opposite vertices
          PRectParallel = {x1, y1, x2, y2};

    3 - Rectangle-turned: x1 y1 x2 y2 angle -> opposite vertices and angle between Ox, Oy and sides of the rect.
          PRect = {x1, y1, x2, y2};
          PAngle = alpha;

    4 - Circle: x y R n angle -> abscissa ordinate radius number_of_vertices turn_angle
          PCenter = {x, y};
          PRadius = R;
          PQntVert = n;
          PAngle = alpha;

    5 - Ellipse: x y a b n angle angle_aux -> abscissa ordinate one_semiaxis another number_of_vertices turn_angle another_turn_angle
          PCenter = {x, y};
          PSemiaxes = {a, b};
          PQntVert = n;
          PAngle = alpha;
          PAngleAux = beta;
*/
//The projection coordinates
projI = 0; // <--- TODO
projJ = 1;

// 0 - case example
MTypeSet = 1;
MQnt = 4;
MVert = {{1.0, 1.0}, {-1.0, 1.0}, {-1.0, -1.0}, {1.0, -1.0}};

// 1 - case example
// MTypeSet = 2;
// MRectParallel = {0.0, 0.0, 1.0, 1.0};

// 2 - case example
// MTypeSet = 3;
// MRect = {0.0, 0.0, 1.0, 1.0};
// MAngle = 3.0;

// 3 - case example
// MTypeSet = 4;
// MCenter = {0.0, 0.0};
// MRadius = 1.0;
// MQntVert = 50;
// MAngle = 0.0;

// 4 - case example   x y a b n phi a0
// MTypeSet = 5;
// MCenter = {0.0, 0.0};
// MSemiaxes = {1.0, 0.5};
// MQntVert = 50;
// MAngle = 1.0;
// MAngleAux = 2.0;
