reset
set term pngcairo size 1600,1200
do for [i=0:19] {
  set output sprintf("./PNGs/Br_%03d.png", i)
  plot \
    sprintf("W1_%03d.dat", i) with filledcurves fc 'green' fs transparent solid 0.25 lc 'green' title 'W1', \
    sprintf("W2_%03d.dat", i) with filledcurves fc 'blue'  fs transparent solid 0.25 lc 'blue' title 'W2', \
    sprintf("Br_%03d.dat", i) with filledcurves fc 'red'   fs transparent solid 0.25 lc 'red' title 'Main'
  unset output

  set output sprintf("./PNGs/W1-W2_%03d.png", i)
  plot \
    sprintf("W1_%03d.dat", i) with filledcurves fc 'green' fs transparent solid 0.25 lc 'green' title 'W1', \
    sprintf("W2_%03d.dat", i) with filledcurves fc 'blue'  fs transparent solid 0.25 lc 'blue' title 'W2'
  unset output

}

