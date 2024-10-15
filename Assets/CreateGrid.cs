using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class CreateGrid : MonoBehaviour
{
    public GameObject gridPrefab; // ������ ��� ����� ����� (��������, ���)
    public int gridWidth = 10; // ������ �����
    public int gridHeight = 10; // ������ �����
    public float cellSize = 1f; // ������ ������ ������
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
                // ������ ������� ��� �������� ����
                Vector3 position = new Vector3(x * cellSize, 0f, z * cellSize);
                // �������� ���� ����� � �������� �������
                Instantiate(gridPrefab, position, Quaternion.identity, transform);

            }
        }
    }
}