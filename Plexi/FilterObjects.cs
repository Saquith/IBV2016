using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plexi {
	public class FilterObjects : Processor {
		private float _rectangleThreshold;
		private int _tiniestObjectSize;
		private int _tolerance;

		/// <param name="rectangleThreshold">Hoe rechthoekig moet het zijn? tussen 0 en 1 waarbij 1 een rechthoek is</param>
		/// <param name="tiniestObjectSize">tolerantie voor de hoek van de chords</param>
		/// <param name="tolerance">threshold voor extra filtering te kleine objecten</param>
		public FilterObjects(float rectangleThreshold = 0.80f, int tiniestObjectSize = 80, int tolerance = 1) {
			_rectangleThreshold = rectangleThreshold;
			_tiniestObjectSize = tiniestObjectSize;
			_tolerance = tolerance;
		}

        public override Matrix Process(Matrix sourceMatrix)
        {
            var intermediaryMatrix = new Matrix(sourceMatrix.X, sourceMatrix.Y);
            var returnMatrix = sourceMatrix;
            // clone source matrix
            for (int y = 0; y < sourceMatrix.Y - 1; y++)
            {
                for (int x = 0; x < sourceMatrix.X - 1; x++)
                {
                    intermediaryMatrix[x, y] = sourceMatrix[x, y];
                }
            }

            for (int imageY = 0; imageY < sourceMatrix.Y; imageY++)
            {
                for (int imageX = 0; imageX < sourceMatrix.X; imageX++)
                {
                    // non-black pixel start een nieuwe filterronde
                    if (intermediaryMatrix[imageX, imageY].R != 0)
                    {
                        var startPoint = new Point(imageX, imageY);
                        var boundary = BoundaryTrace(startPoint, returnMatrix);
                        var color = returnMatrix[imageX, imageY];
                        var objectPixels = ObjectPixels(startPoint, returnMatrix);

                        if (objectPixels.Count > _tiniestObjectSize)
                        {
                            var chordList = new List<Chord>();
                            var longestChord = new Chord();

                            // Bereken een chord door alle paren van boundary pixels te vergelijken.
                            foreach (var pixel1 in boundary)
                            {
                                foreach (var pixel2 in boundary)
                                {
                                    if (pixel1 == pixel2)
                                    {
                                        break;
                                    }
                                    var x1 = pixel1.X;
                                    var y1 = pixel1.Y;
                                    var x2 = pixel2.X;
                                    var y2 = pixel2.Y;
                                    var dx = x2 - x1;
                                    var dy = y2 - y1;

                                    //afronding naar boven zorgt voor een overestimation van de bounding box zodat we niet te klein uitkomen
                                    var length = Math.Ceiling(Math.Sqrt(Math.Pow(dx, 2) + Math.Pow((dy), 2)));
                                    var orientation = 0;
                                    if (dx == 0)
                                    {
                                        orientation = 90;
                                    }
                                    else
                                    {
                                        orientation = (int) (Math.Atan((double) dy/(double) dx)*180/Math.PI);
                                    }


                                    var chord = new Chord(new Point(x1, y1), new Point(x2, y2), length, orientation);

                                    chordList.Add(chord);

                                    if (chord.Length > longestChord.Length)
                                    {
                                        longestChord = chord;
                                    }
                                }
                            }

                            var perpChord = new Chord();
                            

                            foreach (var chord in chordList)
                            {
                                //Chord zoeken die haaks op de longest chord staat, tolerantie is ingebouwd omdat zeker bij kleine objecten een rechte hoek lastig te vinden is.
                                if (Math.Abs(Math.Abs(chord.Orientation - longestChord.Orientation) - 90) < _tolerance)
                                {
                                    if (chord.Length > perpChord.Length)
                                    {
                                        perpChord = chord;
                                    }
                                }
                            }

                            // Bereken boubingbox area, object area en deel deze om de Rectangularity te krijgen
                            var bbArea = perpChord.Length*longestChord.Length;
                            var objectArea = objectPixels.Count;

                            var rectangularity = objectArea/bbArea;
                            if (bbArea < 1)
                            {
                                rectangularity = 0;
                            }

                            //invullen met zwart als het object niet rechthoekig genoeg is
                            if (rectangularity < _rectangleThreshold)
                            {
                                Labeling.FloodFill(new Point(imageX, imageY), color, Color.Black, returnMatrix);
                            }

                            // kleurrand
                            //else { ColorBorder(boundary, returnMatrix);}
                        }
                        else
                        {
                            Labeling.FloodFill(new Point(imageX, imageY), color, Color.Black, returnMatrix);
                        }

                        Labeling.FloodFill(startPoint, color, Color.Black, intermediaryMatrix);
                    }
                }
            }
            return returnMatrix;
        }

        private List<Point> BoundaryTrace(Point startPoint, Matrix matrix)
        {
            var boundaryPixels = new List<Point> {startPoint};
            var color = matrix[startPoint.X, startPoint.Y].R;
            var stack = new Stack<Point>();
            stack.Push(startPoint);

            while (stack.Count > 0)
            {
                var n = stack.Pop();
                for (int y = n.Y - 1; y <= n.Y + 1; y++)
                {
                    for (int x = n.X - 1; x <= n.X + 1; x++)
                    {
                        var p = new Point(x, y);
                        if (IsBorderpixel(p, matrix) && x >= 0 && y >= 0 && x < matrix.X - 1 && y < matrix.Y - 1 &&
                            matrix[p.X, p.Y].R == color && boundaryPixels.IndexOf(p) < 0)
                        {
                            boundaryPixels.Add(p);
                            stack.Push(p);
                        }
                    }
                }
            }

            return boundaryPixels;
        }

        private List<Point> ObjectPixels(Point startPoint, Matrix matrix)
        {
            var objectPixels = new List<Point> {startPoint};
            var color = matrix[startPoint.X, startPoint.Y].R;
            var stack = new Stack<Point>();
            stack.Push(startPoint);

            while (stack.Count > 0)
            {
                var n = stack.Pop();
                for (int y = n.Y - 1; y <= n.Y + 1; y++)
                {
                    for (int x = n.X - 1; x <= n.X + 1; x++)
                    {
                        var p = new Point(x, y);
                        if (x >= 0 && y >= 0 && x < matrix.X && y < matrix.Y && matrix[p.X, p.Y].R == color &&
                            objectPixels.IndexOf(p) < 0)
                        {
                            objectPixels.Add(p);
                            stack.Push(p);
                        }
                    }
                }
            }

            return objectPixels;
        }

        private void ColorBorder(List<Point> points, Matrix matrix)
        {
            foreach (var point in points)
            {
                matrix[point.X, point.Y] = Color.Aqua;
            }
        }

        private bool IsBorderpixel(Point point, Matrix returnMatrix)
        {
            for (int y = point.Y - 1; y <= point.Y + 1; y++)
            {
                if (y < 0 || y >= returnMatrix.Y || returnMatrix[point.X, y].R == 0)
                {
                    return true;
                }
            }
            for (int x = point.X - 1; x <= point.X + 1; x++)
            {
                if (x < 0 || x >= returnMatrix.X || returnMatrix[x, point.Y].R == 0)
                {
                    return true;
                }
            }
            return false;
        }
    }

    class Chord
    {
        public Point StartPoint;
        public Point EndPoint;
        public double Length;
        public double Orientation;

        public Chord()
        {
            StartPoint = Point.Empty;
            EndPoint = Point.Empty;
            Length = 0;
            Orientation = 0;
        }

        public Chord(Point startPoint, Point endPoint, double length, double orientation)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            Length = length;
            Orientation = orientation;
        }
    }
}
