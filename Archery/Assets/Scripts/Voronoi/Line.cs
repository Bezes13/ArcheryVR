using UnityEngine;

namespace Voronoi
{
    /// <summary>
    /// Represents a Line between two Vector3, has a method to calculate if two lines are intersecting for
    /// the calculation of the delaunay algorithm.
    /// </summary>
    public class Line
    {
        public Vector3 a;
        public Vector3 b;

        public Line(Vector3 a, Vector3 b)
        {
            this.a = a;
            this.b = b;
        }
    
        public static bool IsIntersecting(Line line1, Line line2)
        {
            var direction1 = line1.b - line1.a;
            var direction2 = line2.b - line2.a;
            var normalizedDirection1 = Vector3.Normalize(direction1);
            var normalizedDirection2 = Vector3.Normalize(direction2);

            var dotProduct = Vector3.Dot(normalizedDirection1, normalizedDirection2);
            var difference = line1.a - line2.a;
            var rho = Vector3.Dot(difference, normalizedDirection1 - dotProduct * normalizedDirection2) / (dotProduct * dotProduct - 1f);
            var position1 = line1.a + rho * normalizedDirection1;
            var tau = Vector3.Dot(difference, dotProduct * normalizedDirection1 - normalizedDirection2) / (dotProduct * dotProduct - 1f);
            var position2 = line2.a + tau * normalizedDirection2;
            var positionsWithinThreshold = Vector3.SqrMagnitude(position1 - position2) < Delaunay.Threshold;

            rho /= direction1.magnitude;
            tau /= direction2.magnitude;
            var parametersInRange = rho is >= 0f and <= 1f && tau is >= 0f and <= 1f;

            return positionsWithinThreshold && parametersInRange;
        }
    }
}