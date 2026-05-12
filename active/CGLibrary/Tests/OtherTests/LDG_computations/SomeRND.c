// Name of the problem
ProblemName = "SomeRND";

// ==================================================

// Block of data defining the dynamics of the game
// Dimension of the phase vector
n = 5;

// The main matrix
A = {
    {3, -2, 4, 1, 0},
    {-1, 5, -3, 2, -4},
    {0, -5, 2, -1, 3},
    {4, 1, -2, 5, -3},
    {-3, 0, 1, -4, 2}
};

// Dimension of the useful control
pDim = 3;

// The useful control matrix
B = {{2, 3, 2},
{3, 2, 3},
{2, 3, 3},
{3, 2, 3},
{2, 3, 2}};

// Dimension of the disturbance
qDim = 6;

// The disturbance matrix
C = {
    {1, -4, 3, 0, 2, -1},
    {-5, 2, 4, -3, 0, 5},
    {3, 0, -2, 5, -4, 1},
    {2, -1, 5, -5, 3, -3},
    {0, 4, -3, 1, -2, 2}
};

// The initial instant
t0 = 0;

// The final instant
T = 3;

// The time step
dt = 0.1;

// The dimension of projected space
d = 2;

// The indices to project onto
projJ = {0, 1};

// ==================================================
PSetType = "RectParallel";
PRectPLeft = {-1,-1,-1};
PRectPRight = {1,1,1};

// ==================================================
QSetType = "Sphere";
QTheta = 4;
QPhi = 6;
QCenter = {0,0,0,0,0,0};
QRadius = 0.9;

// ==================================================
GoalType = "Itself";
MType = "TerminalSet";

MSetType = "Sphere";
MTheta = 4;
MPhi = 7;
MCenter = {0,0};
MRadius = 1;

// ==================================================
