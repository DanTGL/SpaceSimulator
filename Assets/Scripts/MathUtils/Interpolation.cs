using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpolation {

    class InterpolationValue<T, T2> {
        public T value;

        public T2 pos;
    }

    public static Func<float, float> createInterpolant(List<float> xs, List<float> ys) {
        int i, length = xs.Count;

        // Deal with length issues
        if (length != ys.Count) { throw new Exception("Need an equal count of xs and ys."); }
        if (length == 0) return (x) => { return 0; };
        if (length == 1) {
            // Impl: Precomputing the result prevents problems if ys is mutated later and allows garbage collection of ys
		    // Impl: Unary plus properly converts values to numbers
            float result = ys[0];
            return (x) => {
                return x;
            };
        }

        // Rearrange xs and ys so that xs is sorted
        List<int> indexes = new List<int>();
        for (i = 0; i < length; i++) {
            indexes.Add(i);
        }

        indexes.Sort((a, b) => (xs[a].CompareTo(xs[b])));
        
        List<float> oldXs = new List<float>(xs);
        List<float> oldYs = new List<float>(ys);
        // Impl: Creating new arrays also prevents problems if the input arrays are mutated later
        xs.Clear();
        ys.Clear();

        // Impl: Unary plus properly converts values to numbers
        for (i = 0; i < length; i++) {
            xs.Add(oldXs[indexes[i]]);
            ys.Add(oldYs[indexes[i]]);
        }

        // Get consecutive differences and slopes
        List<float> dys = new List<float>();
        List<float> dxs = new List<float>();
        List<float> ms = new List<float>();

        for (i = 0; i < length - 1; i++) {
            float dx = xs[i + 1] - xs[i], dy = ys[i + 1] - ys[i];
            dxs.Add(dx);
            dys.Add(dy);
            ms.Add(dy / dx);
        }

        // Get degree-1 coefficients
        List<float> c1s = new List<float> {ms[0]};
        for (i = 0; i < dxs.Count - 1; i++) {
            float m = ms[i], mNext = ms[i + 1];
            if (m * mNext <= 0) {
                c1s.Add(0);
            } else {
                float dx_ = dxs[i], dxNext = dxs[i + 1], common = dx_ + dxNext;
                c1s.Add(3 * common / ((common + dxNext) / m + (common + dx_) / mNext));
            }
        }

        c1s.Add(ms[ms.Count - 1]);

        // Get degree-2 and degree-3 coefficients
	    List<float> c2s = new List<float>();
        List<float> c3s = new List<float>();

	    for (i = 0; i < c1s.Count - 1; i++) {
		    float c1 = c1s[i], m_ = ms[i], invDx = 1/dxs[i], common_ = c1 + c1s[i + 1] - m_ - m_;
		    c2s.Add((m_ - c1 - common_)*invDx);
            c3s.Add(common_*invDx*invDx);
	    }
        
        // return interpolant function
        return (x) => {
            // The rightmost point in the dataset should give an exact result
            i = xs.Count - 1;
            if (x == xs[i]) return ys[i];

            // Search for the interval x is in, returning the corresponding y if x is one of the original xs
            float low = 0, high = c3s.Count - 1;
            int mid;
            while (low <= high) {
                mid = (int) Math.Floor(0.5 * (low + high));
                float xHere = xs[mid];
                if (xHere < x) low = mid + 1;
                else if (xHere > x) high = mid - 1;
                else return ys[mid];
            }

            i = (int) Math.Max(0, high);

            // Interpolate
            float diff = x - xs[i], diffSq = diff * diff;
            return ys[i] + c1s[i] * diff + c2s[i] * diffSq + c3s[i] * diff * diffSq;
        };
    }
}
