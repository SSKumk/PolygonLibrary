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
A = { { 1.0, 0.0 }, { 0.0, 1.0 } };

// Dimension of the useful control
p = 2;

// The useful control matrix
B = { { 0.0, 1.0 }, { 0.0, 0.0 } };

// Dimension of the disturbance
q = 2;

// The disturbance matrix
C = { { 0.0, 1.0 }, { 1.0, 0.0 } };

// The initial instant
t0 = 0.0;

// The final instant
T = 1.0;

// The time step
dt = 0.05;

/* =================================================================================
  Block of data defining the constraint for the first player's control

  All angles in radians
  Type of the set: 
    0 - List of the vertexes: number of points and their coordinates
    1 - Rectangle-parallel: x1 y1 x2 y2 -> opposite vertices
    2 - Rectangle-turned: x1 y1 x2 y2 angle -> opposite vertices and angle between Ox, Oy and sides of the rect.
    3 - Circle: x y R n a0 -> abscissa ordinate radius number_of_vertices turn_angle
    4 - Ellipse: x y a b n phi a0 -> abscissa ordinate one_semiaxis another number_of_vertices turn_angle another_turn_angle  
*/

// 0 - case example
PTypeSet = 0;
PQnt = 4;
PVert = { { 0.0, 0.0 }, { 1.0, 0.0 }, { 1.0, 1.0 }, { 0.0, 1.0 } };

// 1 - case example
// PTypeSet = 1;
// PRectParallel = {0.0, 0.0, 1.0, 1.0};

// 2 - case example
// PTypeSet = 2;
// PRect = {0.0, 0.0, 1.0, 1.0};
// PAngle = 3.0;

// 3 - case example
// PTypeSet = 3;
// PCenter = {0.0, 0.0};
// PRadius = 1.0;
// PQntVert = 50;
// PAngle = 1.0;

// 4 - case example   x y a b n phi a0
// PTypeSet = 4;
// PCenter = {0.0, 0.0};
// PSemiaxes = {1.0, 1.5};
// PQntVert = 30;
// PAngle = 0.0;
// PAngleAux = 0.0; 



/* =================================================================================
  Block of data defining the constraint for the second player's control

All angles in radians
  Type of the set: 
    0 - List of the vertexes: number of points and their coordinates
    1 - Rectangle-parallel: x1 y1 x2 y2 -> opposite vertices
    2 - Rectangle-turned: x1 y1 x2 y2 angle -> opposite vertices and angle between Ox, Oy and sides of the rect.
    3 - Circle: x y R n a0 -> abscissa ordinate radius number_of_vertices turn_angle
    4 - Ellipse: x y a b n phi a0 -> abscissa ordinate one_semiaxis another number_of_vertices turn_angle another_turn_angle

*/  

// 0 - case example
QTypeSet = 0;
QQnt = 4;
QVert = { { 0.0, 0.0 }, { 1.0, 0.0 }, { 1.0, 1.0 }, { 0.0, 1.0 } };

// 1 - case example
// QTypeSet = 1;
// QRectParallel = {0.0, 0.0, 1.0, 1.0};

// 2 - case example
// QTypeSet = 2;
// QRect = {0.0, 0.0, 1.0, 1.0};
// QAngle = 3.0;

// 3 - case example
// QTypeSet = 3;
// QCenter = {0.0, 0.0};
// QRadius = 1.0;
// QQntVert = 10;
// QAngle = 0.0;

// 4 - case example   x y a b n phi a0
// QTypeSet = 4;
// QCenter = {0.0, 0.0};
// QSemiaxes = {1.0, 0.5};
// QQntVert = 50;
// QAngle = 1.0;
// QAngleAux = 2.0; 


/*
 * Q set partitioning
 *
 * 0 - List of the vertexes:  n array -> number_of_points and their coordinates: Q1 and Q2
 * 1 - k alternating partitioning: k array -> number_of_parts  [an array of vertex indices of size 2 x k]
 * 2 - rotating partitioning: ind step -> index_of_origin and step of rotating 
 */

// QTypePart = 0;
// Q1Qnt = 2;
// Q1Vert = { {a, b} , {c, d} };
// Q2Qnt = 2;
// Q2Vert = { {a, b} , {c, d} };

// QTypePart = 1;
// QK = 2;
// QPart = { {0, 3}, {5, 6} };

QTypePart = 2;
QOrigin = 0;
QStep = 1;

/* =================================================================================
  Block of data defining the Terminal set

  All angles in radians
  Type of the set:
    0 - List of the vertexes: number of points and their coordinates
    1 - Rectangle-parallel: x1 y1 x2 y2 -> opposite vertices
    2 - Rectangle-turned: x1 y1 x2 y2 angle -> opposite vertices and angle between Ox, Oy and sides of the rect.
    3 - Circle: x y R n a0 -> abscissa ordinate radius number_of_vertices turn_angle
    4 - Ellipse: x y a b n phi a0 -> abscissa ordinate one_semiaxis another number_of_vertices turn_angle another_turn_angle  
*/
//The projection coordinates
projI = 0;
projJ = 1;

// 0 - case example
// MTypeSet = 0;
// MQnt = 4;
// MVert = { { 0.0, 0.0 }, { 1.0, 0.0 }, { 1.0, 1.0 }, { 0.0, 1.0 } };

// 1 - case example
// MTypeSet = 1;
// MRectParallel = {0.0, 0.0, 1.0, 1.0};

// 2 - case example
// MTypeSet = 2;
// MRect = {0.0, 0.0, 1.0, 1.0};
// MAngle = 3.0;

// 3 - case example
// MTypeSet = 3;
// MCenter = {0.0, 0.0};
// MRadius = 1.0;
// MQntVert = 50;
// MAngle = 1.0;

// 4 - case example   x y a b n phi a0
MTypeSet = 4;
MCenter = {0.0, 0.0};
MSemiaxes = {1.0, 0.5};
MQntVert = 50;
MAngle = 1.0;
MAngleAux = 2.0; 




        
