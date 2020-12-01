using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomMath {

    public static float CalculateCharacteristicLength(float volume, float surfaceArea) {
        return volume / surfaceArea;
    }

    public static float CalculateReynoldsNumber(float fluidDensity, float flowSpeed, float characteristicLength, float dViscosity) {
        return fluidDensity * flowSpeed * characteristicLength / dViscosity;
    }

    public static float CalculateBejanNumber(float pressureDrop, float characteristicLength, float dViscosity, float kViscosity) {
        return pressureDrop * Mathf.Pow(characteristicLength, 2) / (dViscosity * kViscosity);
    }

    public static float CalculateDragCoefficient(float wetArea, float frontArea, float bejanNumber, float reynoldsNumber) {
        return 2 * (wetArea / frontArea) * (bejanNumber / Mathf.Pow(reynoldsNumber, 2));
    }

    public static Vector3 CalculateDragForce(float fluidDensity, Vector3 relativeSpeed, Vector3 crossSectionalArea, Vector3 dragCoefficient) {
        float dragProperties = Vector3.Scale(dragCoefficient, Vector3.Scale(relativeSpeed, crossSectionalArea)).magnitude;
        return fluidDensity * relativeSpeed * dragProperties;
    }

}
