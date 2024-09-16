using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    //Скорость движения и поворота можно задать прямо внутри объекта робота, там будут поля для ввода значений, тк они Public
    public float speed; // Скорость движения
    public float rotationSpeed; // Скорость поворота


    float moveHorizontal;
    float moveVertical; //Эти две переменных отвечают за нажание клавиш(или геймпада) от -1 до 1.

    private Rigidbody rb; //Создали переменную rb. Пока она пустая, но добавим скоро в неё компонент, который висит уже на роботе.

    void Start() //Запускается при появлении объекта на сцене. В нашем случае запускается сразу, тк робот уже есть на сцене.
    {
        rb = GetComponent<Rigidbody>(); //Собтвенно тут мы добавили компонент с робота. В теории мы могли этого и не делать и всегда писать GetComponent<Rigidbody>(), но тогда просто код получится больше.
    }
    void Update() //Данная функция обновляется каждый кадр.
    {
        moveHorizontal = Input.GetAxis("Horizontal"); // A/D или стрелки влево/вправо
        moveVertical = Input.GetAxis("Vertical"); // W/S или стрелки вверх/вни
        
        // Вычисляем вектор движения робота.
        // transform.forward - это вектор, указывающий направление "вперед" робота.
        // moveVertical * speed - это скорость движения вперед/назад с учётом нажатой клавиши.
        // Time.deltaTime - это время, прошедшее с последнего кадра. 
        // Умножение на Time.deltaTime делает движение плавным и независимым от частоты кадров.
        // Если хотите, можете по приколу его удалить, но тогда увидите что от разного FPS ваш робот будет двигаться с разной скоростью. Ну и в принципе эта скорость будет огромной
        Vector3 movement = transform.forward * moveVertical * speed * Time.deltaTime;

        //Перемещаем робота с учётом физики. 
        rb.AddForce(movement * speed, ForceMode.Force);

        float rotation = moveHorizontal * rotationSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, rotation, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}
