reset
#set xrange [-15:15]
#set yrange [-3.5:3.5]
set size square
set term gif animate delay 50
set output "Br.gif"
do for [i1=0:1] {
  do for [i2=0:3] {
    do for [i3=0:9] {
      set multiplot
      plot \
        sprintf("W1_%d%d%d.dat", i1, i2, i3) with filledcurves fc 'green' fs transparent solid 0.25 lc 'green' title 'W1', \
        sprintf("W2_%d%d%d.dat", i1, i2, i3) with filledcurves fc 'blue'  fs transparent solid 0.25 lc 'blue' title 'W2', \
        sprintf("Br_%d%d%d.dat", i1, i2, i3) with filledcurves fc 'red'   fs transparent solid 0.25 lc 'red' title 'Main'
      unset multiplot
    }
  }
}
unset output

