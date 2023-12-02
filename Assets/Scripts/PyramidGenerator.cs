using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyramidGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    [Range(2.0f, 20.0f)]
    private float v3Frac = 3;
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

    private List<GameObject> boxes = new List<GameObject>();

    private Vector3 GetPointAtSpecificVectorLength(Vector3 startPoint, Vector3 endPoint, float length)
    {
        float currentLength = Vector3.Distance(startPoint, endPoint);

        if (currentLength <= 0)
            return startPoint;

        float scale = length / currentLength;
        Vector3 result;

        /* Scalar factor of a vector, this way you can get a position at a specified length while mantaining it's direction/angle.
            More info: https://en.wikipedia.org/wiki/Scalar_multiplication
        */
        result.x = startPoint.x + (endPoint.x - startPoint.x) * scale;
        result.y = startPoint.y + (endPoint.y - startPoint.y) * scale;
        result.z = startPoint.z + (endPoint.z - startPoint.z) * scale;

        return result;
    }

    private void PrintPyramidData()
    {
        if (boxes.Count < 1) return;

        double totalPerimeter = 0;
        double totalArea = 0;
        double totalVolume = 0;

        for(int i = 0; i < boxes.Count; i++)
        {
            GameObject box = boxes[i];
            double width = box.transform.localScale.y;
            double length = box.transform.localScale.z;
            double height = box.transform.localScale.x;

            double perimeter = 4 * length + 4 * width + 4 * height;
            Debug.Log(box.name + "[" + i + "] Perimetro: " + perimeter);

            double area = 2 * (length * width + length * height + width * height);
            Debug.Log(box.name + "[" + i + "] Area: " + area);

            double volume = length * width * height;
            Debug.Log(box.name + "[" + i + "] Volumen: " + volume);

            totalPerimeter += perimeter;
            totalArea += area;
            totalVolume += volume;
            Debug.Log("------------------------------------------------------------------------");
        }

        Debug.Log("Perimetro total de la piramide: " + totalPerimeter);
        Debug.Log("Area total de la piramide: " + totalArea);
        Debug.Log("Volumen total de la piramide: " + totalVolume);
    }

    private void GenerateBoxes()
    {
        if (boxes.Count > 0)
        {
            int amountBoxes = boxes.Count;
            for (int i = 0; i < amountBoxes; i++)
            {
                GameObject box = boxes[i];
                DestroyImmediate(box);
            }
            boxes.Clear();
        }

        Vector3 startPoint = transform.position;

        // La punta de arriba del lado contrario a v3
        Vector3 topFarCorner = (startPoint + v1 + v2 + v3);

        // El centro de abajo de la piramide
        Vector3 centroidVectorStart = GetPointAtSpecificVectorLength(v1, v2, (Vector3.Distance(v1, v2) * .5f));

        // El centro de arriba de el primer escalon de la piramide
        Vector3 centroidVectorEnd = GetPointAtSpecificVectorLength(v3, topFarCorner, (Vector3.Distance(v3, topFarCorner) * .5f));

        for (int i = 0; i < amountSteps; i++)
        {
            Vector3 bottomCenter = GetPointAtSpecificVectorLength(centroidVectorStart, centroidVectorEnd, (Vector3.Distance(centroidVectorStart, centroidVectorEnd) * i) + 0.5f);
            Vector3 topCenter = GetPointAtSpecificVectorLength(centroidVectorStart, centroidVectorEnd, (Vector3.Distance(centroidVectorStart, centroidVectorEnd) * (i + 1)) - 0.5f);

            float sideSize = v1.magnitude - (v3.magnitude * 2) * i;

            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.transform.position = (bottomCenter + topCenter) * 0.5f;
            box.transform.rotation = Quaternion.LookRotation(v1, v2);
            box.transform.localScale = new Vector3(v3.magnitude, sideSize, sideSize);

            box.transform.parent = transform;

            boxes.Add(box);
        }
    }

    private void GenerateInitialVectors()
    {
        Vector3 startPoint = transform.position;
        Vector3 randomDir = Random.onUnitSphere;
        float randomLength = Random.Range(3f, 10f);

        v1 = randomDir * randomLength;
        v2 = Vector3.Cross(v1, Vector3.up).normalized * Vector3.Distance(startPoint, v1); // Distancia euclidiana entre el punto de inicio y vector 1.
        v3 = Vector3.Cross(v1, v2).normalized * (v1.magnitude / (v3Frac * v1.magnitude));

        amountSteps = Mathf.RoundToInt(v1.magnitude / (2 * v3.magnitude));

        GenerateBoxes();
        PrintPyramidData();
    }

    private void Awake()
    {
        GenerateInitialVectors();
    }

    private void Update()
    {
        if (Input.GetKeyDown(keyRegenPyramid))
        {
            GenerateInitialVectors();
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 startPoint = transform.position;

        if (v1.magnitude > 0 && v2.magnitude > 0 && v3.magnitude > 0)
        {
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
}
