reset
set xrange [-2:2]
set yrange [-2:2]
set term gif animate delay 50
set output "Br.gif"
do for [i=1:20] {
set multiplot
plot sprintf("Br_%d.dat", i) with lines lc 1 title sprintf("Br %d", i)
plot sprintf("W1_%d.dat", i) with lines lc 2 title sprintf("W1 %d", i)
plot sprintf("W2_%d.dat", i) with lines lc 3 title sprintf("W2 %d", i)
unset multiplot
}
unset output