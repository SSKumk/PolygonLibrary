reset
set xrange [-2:2]
set yrange [-2:2]
set style fill solid 0.3
set term gif animate delay 50
set output "Br.gif"
do for [i1=0:1] {
do for [i2=0:3] {
do for [i3=0:9] {
set multiplot
plot sprintf("W1_%d%d%d.dat", i1, i2, i3) with filledcurves fc rgbcolor "#FFFF0000", \
sprintf("W2_%d%d%d.dat", i1, i2, i3) with filledcurves fc rgbcolor "#FF00FF00", \
sprintf("Br_%d%d%d.dat", i1, i2, i3) with filledcurves fc rgbcolor "#FF0000FF"
unset multiplot
}}}
unset output


#fc rgbcolor "#FFFF0000" "#AARRGGBB" "#AARRGGBB" represents an RGB color with an alpha channel (transparency) value in the high bits. An alpha value of 0 represents a fully opaque color; i.e., "#00RRGGBB" is the same as "#RRGGBB". An alpha value of 255 (FF) represents full transparency. 

