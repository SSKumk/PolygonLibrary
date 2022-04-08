using System;
using System.Drawing;
using System.Collections;
using Widgets;


namespace Robust
{
	public class DrawingFactory
	{
		public IDrawing BridgeDrawing(Bridge bridge)
		{
			PointF[][] draw = new PointF[bridge.nTime][];
			for (int i = 0; i < bridge.nTime; i++)
				draw[i] = PolyToPoints(bridge[i]);
			PolygonIndexDrawing drawing =  new PolygonIndexDrawing(draw);
			drawing.Name = "bridge";
			return drawing;
		}

		public IDrawing[] ControlDrawing(IControl control)
		{
			History history = control.History;
			ArrayList drawings = new ArrayList();
			ArrayList record;
			
			record = history.GetRecords("switchingsurface");
			if (record != null)
			{
				int count = ((SwitchLine)record[0]).nLines;
				for (int i = 0; i < count; i++)
				{
					PointF[][] draw = new PointF[record.Count][];
					for (int t = 0; t < draw.Length; t++)
					{
						SwitchLine line = (SwitchLine)record[t];
						draw[t] = new PointF[line.nPoints(i)];
						for (int j = 0; j < draw[t].Length; j++)
							draw[t][j] = ToPoint(line[i, j]);
					}
					PolylineIndexDrawing drawing = new PolylineIndexDrawing(draw);
					drawing.Name = "switchingsurface";
					drawing.penLine = new Pen(Color.Brown, 2);
					drawings.Add(drawing);
				}
			}

			record = history.GetRecords("bridge_aux");
			if (record != null)
			{
				PointF[][] draw = new PointF[record.Count][];
				for (int t = 0; t < record.Count; t++)
					draw[t] = PolyToPoints((Poly)record[t]);
				PolygonIndexDrawing drawing = new PolygonIndexDrawing(draw);
				drawing.Name = "bridge_aux";
				drawing.penLine = new Pen(Color.Blue, 2);
				drawings.Add(drawing);
			}

			record = history.GetRecords("bridges_aux");
			if (record != null)
			{
				int count = ((Poly[])record[0]).Length;
				for (int i = 0; i < count; i++)
				{
					PointF[][] draw = new PointF[record.Count][];
					for (int t = 0; t < record.Count; t++)
						draw[t] = PolyToPoints(((Poly[])record[t])[i]);
					PolygonIndexDrawing drawing = new PolygonIndexDrawing(draw);
					drawing.Name = "bridge_aux";
					drawing.penLine = new Pen(Color.Blue, 2);
					drawings.Add(drawing);
				}
			}

			record = history.GetRecords("bridge_main");
			if (record != null)
			{
				PointF[][] draw = new PointF[record.Count][];
				for (int t = 0; t < record.Count; t++)
					draw[t] = PolyToPoints((Poly)record[t]);
				PolygonIndexDrawing drawing = new PolygonIndexDrawing(draw);
				drawing.Name = "bridge_main";
				drawing.penLine = new Pen(Color.DarkBlue, 3);
				drawings.Add(drawing);
			}

			record = history.GetRecords("trajectory");
			if (record != null)
			{
				Point[] traj = (Point[])record.ToArray(typeof(Point));

				/*record = history.GetRecords("control_vectogram");
				if (record != null)
				{
					PointF[][] draw = new PointF[record.Count][];
					for (int t = 0; t < record.Count; t++)
					{
						draw[t] = new PointF[((Poly)record[t]).nPoints];
						for (int i = 0; i < draw[t].Length; i++)
							draw[t][i] = ToPoint(((Poly)record[t])[i] + traj[t]);
					}
					PolygonIndexDrawing drawing = new PolygonIndexDrawing(draw);
					drawing.Name = "control_vectogram";
					drawing.IncludeBounds = false;
					drawing.penLine = new Pen(Color.Green, 2);
					drawings.Add(drawing);
				}*/

				record = history.GetRecords("control_vector");
				if (record != null)
				{
					PointF[][] draw = new PointF[record.Count][];
					for (int t = 0; t < record.Count; t++)
					{
						draw[t] = new PointF[2];
						draw[t][0] = ToPoint(traj[t]);
						//draw[t][1] = ToPoint((Point)record[t] + traj[t]);
						Point p = (Point)record[t]; double l = Math.Sqrt(p.x*p.x+p.y*p.y);
						p.x /= l; p.y /= l; draw[t][1] = ToPoint(p + traj[t]);
					}
					PolylineIndexDrawing drawing = new PolylineIndexDrawing(draw);
					drawing.Name = "control_vector";
					drawing.IncludeBounds = false;
					drawing.penLine = new Pen(Color.Red, 2);
					drawing.penLine.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
					drawings.Add(drawing);
				}

				record = history.GetRecords("aux_vector");
				if (record != null)
				{
					PointF[][] draw = new PointF[record.Count][];
					for (int t = 0; t < record.Count; t++)
					{
						draw[t] = new PointF[2];
						draw[t][0] = ToPoint(traj[t]);
						draw[t][1] = ToPoint((Point)record[t] + traj[t]);
					}
					PolylineIndexDrawing drawing = new PolylineIndexDrawing(draw);
					drawing.Name = "aux_vector";
					drawing.IncludeBounds = false;
					drawing.penLine = new Pen(Color.DarkRed, 2);
					drawing.penLine.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
					drawings.Add(drawing);
				}

				PointF[] dtraj = new PointF[traj.Length];
				for (int t = 0; t < traj.Length; t++)
					dtraj[t] = ToPoint(traj[t]);
				PolylineDrawing drawing1 = new PolylineDrawing(dtraj);
				drawing1.Name = "trajectory";
				drawing1.penLine = new Pen(Color.Gray, 1);
				drawings.Add(drawing1);
				PointIndexDrawing drawing2 = new PointIndexDrawing(dtraj);
				drawing2.Name = "trajectory_point";
				drawings.Add(drawing2);
			}

			return (IDrawing[])drawings.ToArray(typeof(IDrawing));
		}

		public double[][] ControlRealizationRecord(IControl control)
		{
			ArrayList record = control.History.GetRecords("control");
			if (record == null) return null;
			return (double[][])(record.ToArray(typeof(double[])));
		}

		public float[] ControlRealization(IControl control, int controlindex)
		{
			ArrayList record = control.History.GetRecords("control");
			if (record == null) return null;
			return ToFloat(Reverse(control.History, "control")[controlindex]);
		}

		public float[] ControlTimeAxis(IControl control)
		{
			ArrayList record = control.History.GetRecords("time");
			if (record == null) return null;
			return ToFloat((double[])record.ToArray(typeof(double)));
		}

		public double[][] SimulatorTrajectoryRecord(ISimulator sim)
		{
			ArrayList record = sim.History.GetRecords("trajectory");
			if (record == null) return null;
			return (double[][])(record.ToArray(typeof(double[])));
		}
		
		public double[][] SimulatorTrajectory(ISimulator sim)
		{
			return Reverse(sim.History, "trajectory");
		}

		public double[][] SimulatorFirstPlayer(ISimulator sim)
		{
			return Reverse(sim.History, "firstplayer");
		}

		public double[][] SimulatorSecondPlayer(ISimulator sim)
		{
			return Reverse(sim.History, "secondplayer");
		}

		public double[] SimulatorTimeAxis(ISimulator sim)
		{
			ArrayList record = sim.History.GetRecords("time");
			if (record == null) return null;
			return (double[])record.ToArray(typeof(double));
		}

		private PointF ToPoint(Point p)
		{
			return new PointF((float)p.x, (float)p.y);
		}
		
		private PointF[] PolyToPoints(Poly poly)
		{
			PointF[] pts = new PointF[poly.nPoints];
			for (int i = 0; i < pts.Length; i++)
				pts[i] = ToPoint(poly[i]);
			return pts;
		}
		
		public float[] ToFloat(double[] array)
		{
			float[] farray = new float[array.Length];
			for (int i = 0; i < farray.Length; i++) farray[i] = (float)array[i];
			return farray;
		}

		public double[][] Reverse(History history, string token)
		{
			ArrayList record = history.GetRecords(token);
			if (record == null) return null;
			double[][] z = new double[((double[])record[0]).Length][];
			for (int i = 0; i < z.Length; i++)
			{
				z[i] = new double[record.Count];
				for (int t = 0; t < z[i].Length; t++)
					z[i][t] = ((double[])record[t])[i];
			}
			return z;
		}
	}
}
