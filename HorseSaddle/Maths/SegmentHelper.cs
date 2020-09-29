using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseSaddle.Maths
{
    public static class SegmentHelper
    {
        public static float CalculateSegmentIndex(float rotation, int segmentCount)
        {
            // Calculate how much of the half circumference a single segment takes.
            float segmentRatio = 2.0f / segmentCount;

            // Normalise the rotation so that it ranges from -1 to 1. Where -0.5 is pointing upwards and 0.5 is pointing downwards. Add in the rotation of the indicator as an offset.
            float normalisedRotation = MathHelper.WrapAngle(rotation) / MathHelper.Pi;

            // Calculate the negative/positive segment. If the segment count is odd, then add half the ratio to the normalised rotation.
            float segment = (normalisedRotation - ((segmentCount % 2 == 0) ? 0 : (segmentRatio / 2))) / segmentRatio;

            // If the segment is negative, then the final segment is the absolute value of it. Otherwise, the segment is the inverse so the count is subtracted from it.
            segment = segment < 0 ? Math.Abs(segment) : segmentCount - segment;

            // Technically if the angle is exactly pi or -pi, the segment will be the segment count, so ensure it can only go as high as the last element.
            return Math.Min(segmentCount - 1, segment);
        }

        public static float CalculateSegmentStartAngle(int segmentIndex, int segmentCount)
        {
            //// How many radians a single segment takes up.
            float segmentSize = MathHelper.TwoPi / segmentCount;


            return MathHelper.WrapAngle((segmentSize * segmentIndex) - (segmentCount % 2 != 0 ? segmentSize / 2 : 0));
        }
    }
}
