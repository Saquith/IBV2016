using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plexi
{
    public class FilterObjects : Processor
    {
        public override Matrix Process(Matrix sourceMatrix)
        {
            var returnMatrix = sourceMatrix;

            for (int imageY = 0; imageY < sourceMatrix.Y; imageY++)
            {
                for (int imageX = 0; imageX < sourceMatrix.X; imageX++)
                {
                    // when we encounter a non-black pixel we start a new boundary trace
                    if (returnMatrix[imageX, imageY].R != 0)
                    {
                        var startPoint = new Point(imageX, imageY);


                        // DEZE GAAT FOUT BIJ IETS GROTERE OBJECTEN
                        var objectPixels = ObjectPixels(startPoint, returnMatrix);

                        //DEZE KAN VOLGENS MIJ 10X EFFICIENTER
                        var boundary = BoundaryTrace(objectPixels, returnMatrix);

                        
                        var chordList = new List<Chord>();
                        var longestChord = new Chord();

                        // Bereken een chord door alle paren van boundary pixels te vergelijken.
                        foreach (var pixel1 in boundary)
                        {
                            foreach (var pixel2 in boundary)
                            {
                                if (pixel1 == pixel2){ break; }
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
                                    orientation = (int)(Math.Atan((double)dy/ (double)dx)*180/Math.PI);
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
                        var tolerance = Math.Max(10, (90 - 3*chordList.Count));

                        foreach (var chord in chordList)
                        {
                            //Chord zoeken die haaks op de longest chord staat, tolerantie is ingebouwd omdat zeker bij kleine objecten een rechte hoek lastig te vinden is.
                            if (Math.Abs(Math.Abs(chord.Orientation-longestChord.Orientation) - 90) < tolerance)
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
                        var color = returnMatrix[imageX, imageY];

                        //invullen met zwart als het object niet rechthoekig genoeg is
                        if (rectangularity < 0.9)
                        {
                            Labeling.FloodFill(new Point(imageX,imageY), color, Color.Black, returnMatrix);
                        }

                    }
                }
            }
            return returnMatrix;
        }

        private List<Point> BoundaryTrace(List<Point> objectPixels, Matrix matrix)
        {
            var boundaryPixels = new List<Point>();
            foreach (var pixel in objectPixels)
            {
                if (IsBorderpixel(pixel, matrix))
                {
                    boundaryPixels.Add(pixel);
                }
            }
            return boundaryPixels;
        }

        private List<Point> _ObjectPixels(Point point, Matrix matrix)
        {
            var visitedPixels = new List<Point>();
            Stack<Point> pixels = new Stack<Point>();
            var greyValue = matrix[point.X, point.Y].R;

            pixels.Push(point);
            while (pixels.Count != 0)
            {
                Point temp = pixels.Pop();
                int y1 = temp.Y;
                while (y1 >= 0 && matrix[temp.X, y1].R == greyValue)
                {
                    y1--;
                }
                y1++;
                bool spanLeft = false;
                bool spanRight = false;
                while (y1 < matrix.Y && matrix[temp.X, y1].R == greyValue)
                {
                    //matrix[temp.X, y1] = replacementColor);
                    var visitedPoint = new Point(temp.X, y1);
                    if (visitedPixels.IndexOf(visitedPoint) < 0)
                    {
                        visitedPixels.Add(visitedPoint);
                    }
                    if (!spanLeft && temp.X > 0 && matrix[temp.X - 1, y1].R == greyValue)
                    {
                        pixels.Push(new Point(temp.X - 1, y1));
                        spanLeft = true;
                    }
                    else if (spanLeft && temp.X - 1 == 0 && matrix[temp.X - 1, y1].R != greyValue)
                    {
                        spanLeft = false;
                    }
                    if (!spanRight && temp.X < matrix.X - 1 && matrix[temp.X + 1, y1].R == greyValue)
                    {
                        pixels.Push(new Point(temp.X + 1, y1));
                        spanRight = true;
                    }
                    else if (spanRight && temp.X < matrix.X - 1 && matrix[temp.X + 1, y1].R != greyValue)
                    {
                        spanRight = false;
                    }
                    y1++;
                }

            }
            return visitedPixels;
        }

        private List<Point> ObjectPixels(Point point, Matrix matrix)
        {
            var queue = new List<Point>();
            var visited = new List<Point>();
            var color = matrix[point.X, point.Y];
            //add starting point
            queue.Add(point);

            while (queue.Count > 0)
            {
                var n = queue.First();
                var left = n;
                var right = n;

                queue.RemoveAt(0);

                // keep going left untill we reach the border
                while (left.X > 0 && matrix[left.X - 1, left.Y] == color)
                {
                    left.X--;
                }
                // keep going right untill we reach the border
                while (right.X < matrix.X - 1 && matrix[right.X + 1, right.Y] == color)
                {
                    right.X++;
                }
                // walk along the line from left to right, checking neighbours up and down and adding 1 tot the total area for each pixel.
                for (int i = left.X; i <= right.X; i++)
                {
                    var visitedPoint = new Point(i, n.Y);
                    if (visited.IndexOf(visitedPoint) < 0)
                    {
                        visited.Add(visitedPoint);
                    }

                    if ((n.Y - 1) >= 0 && matrix[i, (n.Y - 1)] == color && visited.IndexOf(new Point(i, (n.Y - 1))) < 0)
                    {
                        queue.Add(new Point(i, (n.Y - 1))); // add upstairs neighbour to the queue
                    }
                    if ((n.Y + 1) < matrix.Y && matrix[i, (n.Y + 1)] == color && visited.IndexOf(new Point(i, (n.Y + 1))) < 0)
                    {
                        queue.Add(new Point(i, (n.Y + 1))); // add downstairs neighbour to the queue
                    }
                }
            }

            return visited;
        }

        private bool IsBorderpixel(Point point, Matrix returnMatrix)
        {
            for (int y = point.Y - 1; y < point.Y+1; y++)
            {
                for (int x = point.X - 1; x < point.X + 1; x++)
                {
                    if (x < 0 || y < 0 || x >= returnMatrix.X || y >= returnMatrix.Y || returnMatrix[x,y].R == 0)
                    {
                        return true;
                    }
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
