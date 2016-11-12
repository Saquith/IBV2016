using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plexi
{

    public class Labeling : Processor
    {
        public override Matrix Process(Matrix sourceMatrix)
        {
            var grayCounter = 50;
            var returnMatrix = sourceMatrix;

            for (int imageY = 0; imageY < sourceMatrix.Y; imageY++)
            {
                for (int imageX = 0; imageX < sourceMatrix.X; imageX++)
                {
                    // when we encounter a white pixel, we fill the object with a new grayscale value
                    if (returnMatrix[imageX,imageY] == Color.FromArgb(255, 255, 255, 255))
                    {
                        FloodFill(new Point(imageX,imageY), Color.FromArgb(255, 255, 255, 255), Color.FromArgb(grayCounter, grayCounter, grayCounter), returnMatrix);
                        if (grayCounter < 255)
                        {
                            grayCounter++; // increment the value to assign different labels
                        }
                    }
                }
            }
            return returnMatrix;
        }

        public static void FloodFill(Point point, Color color, Color resultColor, Matrix matrix)
        {
            var queue = new List<Point>();

            //add starting point
            queue.Add(point);

            while (queue.Count > 0)
            {
                var n = queue.First();
                var left = n;
                var right = n;
                queue.RemoveAt(0);

                // keep going left untill we reach the border
                while (left.X > 0 && matrix[left.X-1, left.Y] == color)
                {
                    left.X--;
                }
                // keep going right untill we reach the border
                while (right.X < matrix.X && matrix[right.X+1, right.Y] == color)
                {
                    right.X++;
                }
                // walk along the line from left to right, coloring pixels and checking neighbours up and down.
                for (int i = left.X; i <= right.X; i++)
                {
                    matrix[i, n.Y] = resultColor;
                    if ((n.Y - 1) >= 0 && matrix[i,(n.Y -1)] == color)
                    {
                        queue.Add(new Point(i, (n.Y - 1))); // add upstairs neighbour to the queue
                    }
                    if ((n.Y + 1) < matrix.Y && matrix[i, (n.Y + 1)] == color)
                    {
                        queue.Add(new Point(i, (n.Y + 1))); // add downstairs neighbour to the queue
                    }
                }
            }
        }
    }
}
