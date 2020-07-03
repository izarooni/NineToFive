﻿using System;
using System.Drawing;

namespace NineToFive.Util {
    public class SlopeEquation {
        public float B { get; }
        public float Slope { get; }

        public SlopeEquation(Tuple<int, int> Point1, Tuple<int, int> Point2) {
            float Denominator = (Point1.Item1 - Point2.Item1);
            Slope = (Point1.Item2 - Point2.Item2) / (Denominator == 0 ? 1 : Denominator);
            B = -Point1.Item1 * Slope + Point1.Item2;
        }

        public int GetYLocation(int X) {
            return (int)(Slope * X + B);
        }
    }
}