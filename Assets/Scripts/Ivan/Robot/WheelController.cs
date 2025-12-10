using UnityEngine;

/// <summary>
/// Контроллер для управления отдельным колесом робота с использованием WheelCollider
/// </summary>
[RequireComponent(typeof(WheelCollider))]
public class WheelController : MonoBehaviour
{
    [Header("Настройки колеса")]
    [SerializeField] private float wheelRadius = 0.1f;
    [SerializeField] private float motorTorque = 100f;
    [SerializeField] private float brakeTorque = 100f;
    [SerializeField] private bool isSteeringWheel = false;
    [SerializeField] private float maxSteerAngle = 30f;

    [Header("Визуальное представление колеса")]
    [SerializeField] private Transform wheelVisual;

    [Header("Корректировка вращения")]
    [SerializeField] private float yRotationOffset = 0f;
    [Tooltip("Дополнительный поворот по оси Y для асимметричных моделей колес (180 для левых или правых колес)")]
    [SerializeField] private float wheelModelRotationOffset = 0f;

    private WheelCollider wheelCollider;
    private float currentSpeed = 0f;
    private float currentSteerAngle = 0f;

    private void Awake()
    {
        wheelCollider = GetComponent<WheelCollider>();

        // Если визуальное представление не задано, пытаемся найти дочерний объект
        if (wheelVisual == null)
        {
            Transform[] children = GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child != transform)
                {
                    wheelVisual = child;
                    break;
                }
            }
        }
    }

    private void Update()
    {
        // Обновляем визуальное представление колеса
        UpdateWheelVisual();
    }

    /// <summary>
    /// Обновление визуального представления колеса
    /// </summary>
    private void UpdateWheelVisual()
    {
        if (wheelVisual == null) return;

        // Получаем данные из WheelCollider
        wheelCollider.GetWorldPose(out Vector3 position, out Quaternion rotation);
        // Применяем позицию к визуальному представлению
        wheelVisual.position = new Vector3(wheelVisual.position.x, position.y, wheelVisual.position.z);

        // Используем кватернионы вместо углов Эйлера для избежания gimbal lock
        // Создаем поворот вокруг оси Y с учетом корректировки
        Quaternion yRotation = Quaternion.AngleAxis(yRotationOffset, Vector3.up);
        // Добавляем дополнительный поворот для асимметричных моделей колес
        Quaternion modelRotation = Quaternion.AngleAxis(wheelModelRotationOffset, Vector3.up);
        // Применяем повороты, сохраняя оригинальную ориентацию колеса
        wheelVisual.rotation = rotation * yRotation * modelRotation;
    }

    /// <summary>
    /// Установить скорость вращения колеса
    /// </summary>
    /// <param name="speed">Скорость (-1 до 1)</param>
    public void SetSpeed(float speed)
    {
        // Ограничиваем значение
        speed = Mathf.Clamp(speed, -1f, 1f);
        currentSpeed = speed;

        // Применяем крутящий момент к WheelCollider
        wheelCollider.motorTorque = speed * motorTorque;
    }

    /// <summary>
    /// Установить угол поворота колеса (для рулевых колес)
    /// </summary>
    /// <param name="angle">Угол поворота (-1 до 1, где 1 - максимальный угол вправо)</param>
    public void SetSteerAngle(float angle)
    {
        if (!isSteeringWheel)
            return;

        // Ограничиваем значение
        angle = Mathf.Clamp(angle, -1f, 1f);
        currentSteerAngle = angle * maxSteerAngle;

        // Устанавливаем угол поворота для WheelCollider
        wheelCollider.steerAngle = currentSteerAngle;
    }

    /// <summary>
    /// Применить тормоз
    /// </summary>
    /// <param name="brakeForce">Сила торможения (0 до 1)</param>
    public void Brake(float brakeForce)
    {
        brakeForce = Mathf.Clamp01(brakeForce);

        // Применяем тормозной момент к WheelCollider
        wheelCollider.brakeTorque = brakeForce * brakeTorque;
    }

    /// <summary>
    /// Получить текущую скорость колеса
    /// </summary>
    /// <returns>Текущая скорость (-1 до 1)</returns>
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    /// <summary>
    /// Проверить, является ли колесо рулевым
    /// </summary>
    /// <returns>True, если колесо рулевое</returns>
    public bool IsSteeringWheel()
    {
        return isSteeringWheel;
    }

    /// <summary>
    /// Получить текущий угол поворота
    /// </summary>
    /// <returns>Текущий угол в градусах</returns>
    public float GetCurrentSteerAngle()
    {
        return currentSteerAngle;
    }

    /// <summary>
    /// Получить радиус колеса
    /// </summary>
    /// <returns>Радиус колеса</returns>
    public float GetWheelRadius()
    {
        return wheelRadius;
    }

    /// <summary>
    /// Получить RPM колеса из WheelCollider
    /// </summary>
    /// <returns>RPM колеса</returns>
    public float GetRPM()
    {
        return wheelCollider.rpm;
    }
}
