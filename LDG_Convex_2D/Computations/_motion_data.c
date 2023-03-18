// Name of the problem; to check the consistency
string ProblemName;

// Name of the motion; also defines the file name with the computed data
string MotionName;

/* =================================================================================
  Block of data defining the initial point
*/
  
// Initial time instant 
// (will be shifted to the closest time instant from the computed time grid)
double ts;

/* Initial point type:
     0 - original coordinates (its dimension is equal to the dimension
         of the phase vector given in the basic data of the example)
     1 - equivalent (its dimension is equal to 2)
*/
int initPointType;

// Dimension of the original phase vector
extern int n;     

// The initial vector
double xs[...];

// =================================================================================
// The first player control data

/* The first player control type:
     0 - extremal shift control on the basis of computed system of bridges
     1 - switching line control (if the control is one dimensional or the constraints
         are of box type)
     2 - constant
     3 - random within the vectogram multiplied by a coefficient
*/
int pApplContrType;

// Constant value in the case of constant constrol
// Dimension of the vector equals dimension of the first player control
double pConst[...];

// Multiplier for the constraints in the case of random control
double pMultiplier;

// =================================================================================
// The second player control data

/* The second player control type:
     0 - extremal shift control on the basis of computed system of bridges
     1 - switching line control (if the control is one dimensional or the constraints
         are of box type)
     2 - constant
     3 - random within the vectogram multiplied by a coefficient
*/
int qApplContrType;

// Constant value in the case of constant constrol
// Dimension of the vector equals dimension of the second player control
double qConst[...];

// Multiplier for the constraints in the case of random control
double qMultiplier;

// =================================================================================

// Random seed (when at least onr control is random)
// Value -1 shows that the random generator should be initialized from the clock
int randomSeed;
