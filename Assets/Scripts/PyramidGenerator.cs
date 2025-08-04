using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyramidGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    [Range(6.0f, 100.0f)]
    private float v3Frac = 15;
    [SerializeField]
    private int amountSteps;

    [Header("Vectors")]
    [SerializeField]
    private Vector3 v1;
    [SerializeField]
    private Vector3 v2;
    [SerializeField]
    private Vector3 v3;

    [Header("Controls")]
    [SerializeField]
    private KeyCode keyRegenPyramid = KeyCode.R;

    private void PrintPyramidData() {
        if (amountSteps <= 0 || v1 == Vector3.zero || v2 == Vector3.zero || v3 == Vector3.zero)
            return;
        
        double totalPerimeter = 0;
        double totalArea = 0;
        double totalVolume = 0;
        
        var stepHeight = v3.magnitude;
        var baseLength = v1.magnitude;
        var baseWidth  = v2.magnitude;

        for (var i = 0; i < amountSteps; i++) {
            var stepScale = 1f - ((float)i / amountSteps);
            
            double width = baseWidth * stepScale;
            double length = baseLength * stepScale;
            double height = stepHeight;
            Debug.Log($"[Escalon {i}] Dimensiones: W; {(float)width} L; {(float)length} H; {(float)height}");
            
            var perimeter = 4 * length + 4 * width + 4 * height;
            Debug.Log($"[Escalon {i}] Perimetro: {perimeter}");
            
            double amountToSubtract = 0;
            if (i + 1 < amountSteps) {
                var nextScale = 1f - ((float)(i + 1) / amountSteps);
                var nextSize = v1.magnitude * nextScale;
                amountToSubtract = nextSize * nextSize;
            }
            
            // Las dos caras de ancho + las dos caras de largo + la cara de arriba - la cara de abajo del escalon de arriba.
            var area =  2 * (width * height) + 2 * (length * height) + (width * length) - amountToSubtract;
            if (i == 0)
            { // Si es el primer escalon le sumamos la cara de abajo.
                area += width * length;
            }
            Debug.Log($"[Escalon {i}] Area: {area}");

            var volume = length * width * height;
            Debug.Log($"[Escalon {i}] Volumen: {volume}");

            totalPerimeter += perimeter;
            totalArea += area;
            totalVolume += volume;
            Debug.Log("------------------------------------------------------------------------");
        }
        
        Debug.Log($"Perimetro total de la piramide: {totalPerimeter}");
        Debug.Log($"Area total de la piramide: {totalArea}");
        Debug.Log($"Volumen total de la piramide: {totalVolume}");
    }
    
    private void DrawPyramidGizmos() {
        if (amountSteps <= 0 || v1 == Vector3.zero || v2 == Vector3.zero || v3 == Vector3.zero)
            return;

        var verticalDir = v3.normalized; // direccion vertical
        var stepHeight = v3.magnitude;
        
        var baseCenter = transform.position + (v1 + v2) * 0.5f; //centro de la base
        var baseLength = v1.magnitude;
        var baseWidth  = v2.magnitude;

        for (var i = 0; i < amountSteps; i++) {
            var stepScale = 1f - ((float)i / amountSteps); // escalado
            
            var currentLength = baseLength * stepScale;
            var currentWidth = baseWidth * stepScale;

            var halfV1 = v1.normalized * (currentLength * 0.5f); // mitades para calcular las esquinas
            var halfV2 = v2.normalized * (currentWidth * 0.5f);
            
            var stepOrigin = baseCenter + verticalDir * (stepHeight * i); // centro actual

            // piso
            var b1 = stepOrigin - halfV1 - halfV2;
            var b2 = stepOrigin + halfV1 - halfV2;
            var b3 = stepOrigin + halfV1 + halfV2;
            var b4 = stepOrigin - halfV1 + halfV2;

            // techo
            var t1 = b1 + verticalDir * stepHeight;
            var t2 = b2 + verticalDir * stepHeight;
            var t3 = b3 + verticalDir * stepHeight;
            var t4 = b4 + verticalDir * stepHeight;

            // piso
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(b1, b2);
            Gizmos.DrawLine(b2, b3);
            Gizmos.DrawLine(b3, b4);
            Gizmos.DrawLine(b4, b1);
            
            // techo
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(t1, t2);
            Gizmos.DrawLine(t2, t3);
            Gizmos.DrawLine(t3, t4);
            Gizmos.DrawLine(t4, t1);

            // lados
            Gizmos.color = Color.white;
            Gizmos.DrawLine(b1, t1);
            Gizmos.DrawLine(b2, t2);
            Gizmos.DrawLine(b3, t3);
            Gizmos.DrawLine(b4, t4);
        }
    }

    private void GenerateInitialVectors() {
        var randomDir = Random.onUnitSphere;
        var randomLength = Random.Range(3f, 10f);

        var safeUp = Mathf.Abs(Vector3.Dot(v1.normalized, Vector3.up)) > 0.99f ? Vector3.forward : Vector3.up;
        
        v1 = randomDir * randomLength;
        v2 = Vector3.Cross(v1, safeUp).normalized * v1.magnitude;
        v3 = Vector3.Cross(v1, v2).normalized * (v1.magnitude / v3Frac);


        amountSteps = Mathf.RoundToInt(v1.magnitude / (2 * v3.magnitude));
        
        PrintPyramidData();
    }

    private void Awake() {
        GenerateInitialVectors();
    }

    private void Update() {
        if (Input.GetKeyDown(keyRegenPyramid)) {
            GenerateInitialVectors();
        }
    }

    private void OnDrawGizmos() {
        var startPoint = transform.position;
        
        DrawPyramidGizmos();

        if (!(v1.magnitude > 0) || !(v2.magnitude > 0) || !(v3.magnitude > 0)) return;
        // v1
        Gizmos.color = Color.red;
        Gizmos.DrawRay(startPoint, v1);

        // v2
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(startPoint, v2);

        // v3
        Gizmos.color = Color.green;
        Gizmos.DrawRay(startPoint, v3);
    }
}
