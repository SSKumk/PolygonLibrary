set size 1, 1
set mxtics 5
set mytics 5

set grid xtics ytics mxtics mytics
set style fill transparent solid 0.25 

set linetype 1 lc rgb "dark-violet" lw 2 pt 1
set linetype 2 lc rgb "red" lw 2 pt 1
set linetype 3 lc rgb "green" lw 2 pt 1



plot [-10:10] [-10:10] \
  'Test2M1.txt' every :::0::0 with filledcurves closed linetype 1 title "M1", \
  'Test2M2.txt' every :::0::0 with filledcurves closed linetype 2 title "M2", \
  'Test2ResAND.txt' every :::0::0 with filledcurves closed linetype 3 title "AND", \
  'Test2ResOR.txt' every :::0::0 with filledcurves closed linetype 3 title "OR", \
  'Test2ResM1SUBM2.txt' every :::0::0 with filledcurves closed linetype 3 title "M1SUBM2", \
  'Test2ResM2SUBM1.txt' every :::0::0 with filledcurves closed linetype 3 title "M2SUBM1", \
  'Test2ResSIMSUB.txt' every :::0::0 with filledcurves closed linetype 3  title "SIMSUB"


pause -1

