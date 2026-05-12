set size 1, 1
set mxtics 5
set mytics 5

set grid xtics ytics mxtics mytics
set style fill transparent solid 0.75

set linetype 1 lc rgb "dark-violet" lw 2 pt 7
set linetype 2 lc rgb "red" lw 2 pt 1
set linetype 3 lc rgb "green" lw 2 pt 1
 


plot [0:20] [-10:10] \
  'Test8M1.txt' every :::0::0 with filledcurves closed linetype 1  title "M1", \
  'Test8M1.txt' every :::1::1 with filledcurves lc rgb "#ffffff"  title "M1", \
  'Test8M1.txt' every :::1::1 with l lw 2 lc rgb "dark-violet"  title "M1", \
  'Test8M2.txt' every :::0::0 with filledcurves closed linetype 2 title "M2", \
  'Test8ResAND.txt' every :::0::0 with filledcurves closed linetype 3 title "AND", \
  'Test8ResOR.txt' every :::0::0 with filledcurves closed linetype 3 title "OR", \
  'Test8ResOR.txt' every :::1::1 with filledcurves lc rgb "#ffffff" title "OR" , \
  'Test8ResOR.txt' every :::1::1 with l lw 2 lc rgb "green" title "OR", \
  'Test8ResM1SUBM2.txt' every :::0::0 with filledcurves closed linetype 3 title "M1SUBM2", \
  'Test8ResM2SUBM1.txt' every :::0::0 with filledcurves closed linetype 3 title "M2SUBM1", \
  'Test8ResM2SUBM1.txt' every :::1::1 with filledcurves closed linetype 3 title "M2SUBM1", \
  'Test8ResSIMSUB.txt' every :::0::0 with filledcurves closed linetype 3 title "SIMSUB" , \
  'Test8ResSIMSUB.txt' every :::1::1 with filledcurves lc rgb "#ffffff" title "SIMSUB" , \
  'Test8ResSIMSUB.txt' every :::1::1 with l lw 2 lc rgb "green" title "SIMSUB" ,\
  'Test8ResSIMSUB.txt' every :::2::2 with filledcurves lc rgb "#ffffff" title "SIMSUB" , \
  'Test8ResSIMSUB.txt' every :::2::2 with l lw 2 lc rgb "green" title "SIMSUB" 
  

pause -1

