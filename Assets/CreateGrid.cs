using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class CreateGrid : MonoBehaviour
{
    public GameObject gridPrefab; // Префаб для узлов сетки (например, куб)
    public int gridWidth = 10; // Ширина сетки
    public int gridHeight = 10; // Высота сетки
    public float cellSize = 1f; // Размер каждой ячейки
    public bool Do = false;
    void Start()
    {
    }

    private void Update()
    {
        if (Do)
        {
            GridCreate();
            Do = false;
        }
    }
    void GridCreate()
    {
        for (int x =(int)transform.position.x; x < (int)transform.position.x+gridWidth; x++)
        {
            for (int z = (int)transform.position.z; z < (int)transform.position.z + gridHeight; z++)
            {
                // Расчет позиции для текущего узла
                Vector3 position = new Vector3(x * cellSize, 0f, z * cellSize);
                // Создание узла сетки в заданной позиции
                Instantiate(gridPrefab, position, Quaternion.identity, transform);

            }
        }
    }
}