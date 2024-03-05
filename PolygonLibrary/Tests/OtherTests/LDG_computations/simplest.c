// Name of the problem
ProblemName = "Simplest-Cube3D-problem";

// Path to the folder whereto the result should be written
path = "./";


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

Type of the set:

    1 - List of the vertices:

    2 - Cube:

*/

// 1 - case example
// PTypeSet = 1;
// PQnt = 3;
// PVert = {{0.0, 0.0, 0.0},{1.0, 0.0, 0.0}, {0.0, 1.0, 0.0}, {0.0, 0.0, 1.0}};

// 2 - case example
PTypeSet = 2;
PCube = 1.1;


/* =================================================================================
  Block of data defining the constraint for the second player's control

Type of the set:

    1 - List of the vertices:

    2 - Cube:

*/

// 1 - case example
// QTypeSet = 1;
// QQnt = 3;
// QVert = {{0.0, 0.0, 0.0},{1.0, 0.0, 0.0}, {0.0, 1.0, 0.0}, {0.0, 0.0, 1.0}};

// 2 - case example
QTypeSet = 2;
QCube = 1;

/* =================================================================================
  Block of data defining the Terminal set

  Type of the set:

    1 - List of the vertices:

    2 - Cube:

*/

// 1 - case example
// MTypeSet = 1;
// MQnt = 3;
// MVert = {{0.0, 0.0, 0.0},{1.0, 0.0, 0.0}, {0.0, 1.0, 0.0}, {0.0, 0.0, 1.0}};

// 2 - case example
MTypeSet = 2;
MCube = 1;

