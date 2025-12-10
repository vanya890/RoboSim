using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Датчик столкновений, который обнаруживает физические контакты с другими объектами.
/// Наследуется от SensorBase и отправляет информацию о столкновениях в систему робота.
/// </summary>
public class CollisionSensor : SensorBase
{
    [Header("Настройки датчика столкновений")]
    [Tooltip("Обнаруживать только столкновения с определенными слоями")]
    [SerializeField] private LayerMask detectionLayers = -1; // Все слои по умолчанию

    [Tooltip("Минимальная сила столкновения для регистрации")]
    [SerializeField] private float minCollisionForce = 0.5f;

    [Tooltip("Показывать отладочную информацию при столкновениях")]
    [SerializeField] private bool showDebugInfo = true;
    
    [Tooltip("Конкретный коллайдер для отслеживания столкновений (если не указан, используется первый найденный)")]
    [SerializeField] private Collider targetCollider = null;

    // Информация о последнем столкновении
    private CollisionInfo lastCollision = new CollisionInfo();

    /// <summary>
    /// Структура для хранения информации о столкновении
    /// </summary>
    public struct CollisionInfo : ISensorData
    {
        public bool hasCollision;
        public GameObject collidedObject;
        public Vector3 contactPoint;
        public Vector3 contactNormal;
        public float collisionForce;
        
        /// <summary>
        /// Получить словарь с данными сенсора в формате ключ-значение
        /// </summary>
        /// <returns>Словарь с данными</returns>
        public Dictionary<string, object> GetData()
        {
            var data = new Dictionary<string, object>();
            data["hasCollision"] = hasCollision;
            
            if (hasCollision && collidedObject != null)
            {
                data["collidedObject"] = collidedObject.name;
                data["contactPoint_x"] = contactPoint.x;
                data["contactPoint_y"] = contactPoint.y;
                data["contactPoint_z"] = contactPoint.z;
                data["contactNormal_x"] = contactNormal.x;
                data["contactNormal_y"] = contactNormal.y;
                data["contactNormal_z"] = contactNormal.z;
                data["collisionForce"] = collisionForce;
            }
            
            return data;
        }
    }

    protected override void OnStart()
    {
        // Устанавливаем имя по умолчанию, если оно не было установлено
        if (string.IsNullOrEmpty(sensorName))
        {
            sensorName = "CollisionSensor";
        }

        // Сбрасываем информацию о столкновении
        lastCollision = new CollisionInfo();
    }

    protected override object ReadSensorData()
    {
        return lastCollision;
    }

    /// <summary>
    /// Обработка столкновения с другим объектом
    /// </summary>
    /// <param name="collision">Информация о столкновении</param>
    private void OnCollisionEnter(Collision collision)
    {
        // Проверяем, что столкновение произошло с указанным коллайдером (если он указан)
        if (targetCollider != null && collision.collider != targetCollider)
            return;
        // Проверяем, соответствует ли столкновение нужному слою
        if ((detectionLayers.value & (1 << collision.gameObject.layer)) == 0)
            return;

        // Вычисляем силу столкновения (упрощенная формула)
        float collisionForce = collision.relativeVelocity.magnitude;

        // Проверяем, достаточна ли сила столкновения для регистрации
        if (collisionForce < minCollisionForce)
            return;

        // Сохраняем информацию о столкновении
        lastCollision.hasCollision = true;
        lastCollision.collidedObject = collision.gameObject;
        lastCollision.contactPoint = collision.contacts[0].point;
        lastCollision.contactNormal = collision.contacts[0].normal;
        lastCollision.collisionForce = collisionForce;

        // Выводим отладочную информацию
        if (showDebugInfo)
        {
            Debug.Log($"Столкновение с {collision.gameObject.name} на расстоянии {collisionForce:F2}");
        }
    }

    /// <summary>
    /// Сброс информации о столкновении после прекращения контакта
    /// </summary>
    /// <param name="collision">Информация о столкновении</param>
    private void OnCollisionExit(Collision collision)
    {
        // Проверяем, что столкновение произошло с указанным коллайдером (если он указан)
        if (targetCollider != null && collision.collider != targetCollider)
            return;

        // Проверяем, соответствует ли столкновение нужному слою
        if ((detectionLayers.value & (1 << collision.gameObject.layer)) == 0)
            return;

        // Сбрасываем информацию о столкновении
        lastCollision = new CollisionInfo();
    }
    
    /// <summary>
    /// Обработка входа в триггер (для коллайдеров с флагом Is Trigger)
    /// </summary>
    /// <param name="other">Коллайдер другого объекта</param>
    private void OnTriggerEnter(Collider other)
    {   
        // Проверяем, соответствует ли объект нужному слою
        if ((detectionLayers.value & (1 << other.gameObject.layer)) == 0)
            return;
            
        // Для триггера мы не можем получить точную силу столкновения,
        // поэтому используем относительную скорость Rigidbody, если она есть
        float collisionForce = 1.0f; // Значение по умолчанию
        
        Rigidbody otherRigidbody = other.GetComponent<Rigidbody>();
        if (otherRigidbody != null)
        {
            collisionForce = otherRigidbody.velocity.magnitude;
        }
        
        // Проверяем, достаточна ли "сила" для регистрации
        if (collisionForce < minCollisionForce)
            return;
            
        // Сохраняем информацию о столкновении
        lastCollision.hasCollision = true;
        lastCollision.collidedObject = other.gameObject;
        lastCollision.contactPoint = other.ClosestPointOnBounds(transform.position);
        lastCollision.contactNormal = (transform.position - other.transform.position).normalized;
        lastCollision.collisionForce = collisionForce;
        
        // Выводим отладочную информацию
        if (showDebugInfo)
        {
            Debug.Log($"Триггер с {other.gameObject.name} с силой {collisionForce:F2}");
        }
    }
    
    /// <summary>
    /// Сброс информации при выходе из триггера
    /// </summary>
    /// <param name="other">Коллайдер другого объекта</param>
    private void OnTriggerExit(Collider other)
    {
        // Проверяем, что событие вызвано указанным коллайдером (если он указан)
        if (targetCollider != null && other != targetCollider)
            return;
            
        // Проверяем, соответствует ли объект нужному слою
        if ((detectionLayers.value & (1 << other.gameObject.layer)) == 0)
            return;
            
        // Сбрасываем информацию о столкновении
        lastCollision = new CollisionInfo();
    }

    /// <summary>
    /// Получить информацию о последнем столкновении
    /// </summary>
    /// <returns>Информация о столкновении</returns>
    public CollisionInfo GetLastCollision()
    {
        return lastCollision;
    }

    /// <summary>
    /// Получить минимальную силу столкновения для регистрации
    /// </summary>
    /// <returns>Минимальная сила</returns>
    public float GetMinCollisionForce()
    {
        return minCollisionForce;
    }

    /// <summary>
    /// Установить минимальную силу столкновения для регистрации
    /// </summary>
    /// <param name="force">Новая минимальная сила</param>
    public void SetMinCollisionForce(float force)
    {
        minCollisionForce = Mathf.Max(0f, force);
    }
    
    /// <summary>
    /// Получить целевой коллайдер
    /// </summary>
    /// <returns>Целевой коллайдер или null, если не установлен</returns>
    public Collider GetTargetCollider()
    {
        return targetCollider;
    }
    
    /// <summary>
    /// Установить целевой коллайдер
    /// </summary>
    /// <param name="collider">Новый целевой коллайдер</param>
    public void SetTargetCollider(Collider collider)
    {
        targetCollider = collider;
    }
}
