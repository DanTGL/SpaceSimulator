using System;
using UnityEngine;
using System.Collections.Generic;

public class DragCube : DragShape {

    public float[] coefficients;

    // Start is called before the first frame update
    void Start() {
        //coefficients = new float[Enum.GetValues(typeof(Side)).Length];
        Func<float, float> f = Interpolation.createInterpolant(new List<float>() {0, 1, 2, 3, 4, 5, 6, 7}, new List<float>() {-5, -3, -1, 1, 3, 5, 7, 9});

        var message = "";
        for (float x = 1; x <= 10; x += 0.5f) {
            float xSquared = f(x);
            message += x + " squared is about " + xSquared + "\n";
        }

        Debug.Log(message);
    }
}
