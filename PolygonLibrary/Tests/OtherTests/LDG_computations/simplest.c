// Name of the problem
ProblemName = "Simplest-Cube3D-problem";

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
dt = 0.2;

// The dimension of projected space
d = 3;

// The indices to project onto
projJ = {0, 1, 2};

/* =================================================================================
  Block of data defining the constraint for the first player's control

Type of the set:
    1 - List of the vertices:
      PTypeSet = 1;
      PQnt     = 3;
      PVert    = {{0.0, 0.0, 0.0},{1.0, 0.0, 0.0}, {0.0, 1.0, 0.0}, {0.0, 0.0, 1.0}};

    2 - Rectangle axis-parallel:
      PTypeSet    = 2;
      PRectPLeft  = {-0.5, -0.5, -0.5};
      PRectPRight = {0.5, 0.5, 0.5};

    3 - Sphere:
      PTheta  = 11;
      PPhi    = 7;
      PRadius = 1;

*/

PTypeSet = 2;
PRectPLeft = {-0.6, -0.6, -0.6};
PRectPRight = {0.6, 0.6, 0.6};


/* =================================================================================
  Block of data defining the constraint for the second player's control

Type of the set:

    1 - List of the vertices:
      QTypeSet = 1;
      QQnt     = 3;
      QVert    = {{0.0, 0.0, 0.0},{1.0, 0.0, 0.0}, {0.0, 1.0, 0.0}, {0.0, 0.0, 1.0}};

    2 - Rectangle axis-parallel:
      QTypeSet    = 2;
      QRectPLeft  = {-0.5, -0.5, -0.5};
      QRectPRight = {0.5, 0.5, 0.5};

    3 - Sphere:
      QTheta  = 11;
      QPhi    = 7;
      QRadius = 1;

*/

QTypeSet = 2;
QRectPLeft = {-0.5, -0.5, -0.5};
QRectPRight = {0.5, 0.5, 0.5};

/* =================================================================================
  Block of data defining the Terminal set

  Type of the set:

    1 - List of the vertices:
      MTypeSet = 1;
      MQnt     = 3;
      MVert    = {{0.0, 0.0, 0.0},{1.0, 0.0, 0.0}, {0.0, 1.0, 0.0}, {0.0, 0.0, 1.0}};

    2 - Rectangle axis-parallel:
      MTypeSet    = 2;
      MRectPLeft  = {-0.5, -0.5, -0.5};
      MRectPRight = {0.5, 0.5, 0.5};

    3 - Sphere:
      MTheta  = 11;
      MPhi    = 7;
      MRadius = 1;

*/


MTypeSet = 2;
MRectPLeft = {-0.5, -0.5, -0.5};
MRectPRight = {0.5, 0.5, 0.5};
