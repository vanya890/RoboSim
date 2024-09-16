using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    //�������� �������� � �������� ����� ������ ����� ������ ������� ������, ��� ����� ���� ��� ����� ��������, �� ��� Public
    public float speed; // �������� ��������
    public float rotationSpeed; // �������� ��������


    float moveHorizontal;
    float moveVertical; //��� ��� ���������� �������� �� ������� ������(��� ��������) �� -1 �� 1.

    private Rigidbody rb; //������� ���������� rb. ���� ��� ������, �� ������� ����� � �� ���������, ������� ����� ��� �� ������.

    void Start() //����������� ��� ��������� ������� �� �����. � ����� ������ ����������� �����, �� ����� ��� ���� �� �����.
    {
        rb = GetComponent<Rigidbody>(); //��������� ��� �� �������� ��������� � ������. � ������ �� ����� ����� � �� ������ � ������ ������ GetComponent<Rigidbody>(), �� ����� ������ ��� ��������� ������.
    }
    void Update() //������ ������� ����������� ������ ����.
    {
        moveHorizontal = Input.GetAxis("Horizontal"); // A/D ��� ������� �����/������
        moveVertical = Input.GetAxis("Vertical"); // W/S ��� ������� �����/���
        
        // ��������� ������ �������� ������.
        // transform.forward - ��� ������, ����������� ����������� "������" ������.
        // moveVertical * speed - ��� �������� �������� ������/����� � ������ ������� �������.
        // Time.deltaTime - ��� �����, ��������� � ���������� �����. 
        // ��������� �� Time.deltaTime ������ �������� ������� � ����������� �� ������� ������.
        // ���� ������, ������ �� ������� ��� �������, �� ����� ������� ��� �� ������� FPS ��� ����� ����� ��������� � ������ ���������. �� � � �������� ��� �������� ����� ��������
        Vector3 movement = transform.forward * moveVertical * speed * Time.deltaTime;

        //���������� ������ � ������ ������. 
        rb.AddForce(movement * speed, ForceMode.Force);

        float rotation = moveHorizontal * rotationSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, rotation, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}
