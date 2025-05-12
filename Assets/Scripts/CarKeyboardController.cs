using UnityEngine;

/// <summary>
/// Управляет четырехколесным роботом/машиной с помощью клавиатуры.
/// Передние колеса отвечают за поворот, задние - за движение (RWD).
/// Использует компоненты WheelCollider.
/// </summary>
public class CarKeyboardController : MonoBehaviour
{
    [Header("Ссылки на коллайдеры колес")]
    [Tooltip("Коллайдер левого переднего колеса (для поворота).")]
    public WheelCollider frontLeftWheelCollider;
    [Tooltip("Коллайдер правого переднего колеса (для поворота).")]
    public WheelCollider frontRightWheelCollider;
    [Tooltip("Коллайдер левого заднего колеса (для движения).")]
    public WheelCollider rearLeftWheelCollider;
    [Tooltip("Коллайдер правого заднего колеса (для движения).")]
    public WheelCollider rearRightWheelCollider;

    [Header("Ссылки на визуальные модели колес (Опционально)")]
    [Tooltip("Transform визуальной модели левого переднего колеса.")]
    public Transform frontLeftWheelTransform;
    [Tooltip("Transform визуальной модели правого переднего колеса.")]
    public Transform frontRightWheelTransform;
    [Tooltip("Transform визуальной модели левого заднего колеса.")]
    public Transform rearLeftWheelTransform;
    [Tooltip("Transform визуальной модели правого заднего колеса.")]
    public Transform rearRightWheelTransform;

    [Header("Параметры управления")]
    [Tooltip("Максимальный крутящий момент двигателя (сила движения).")]
    public float maxMotorTorque = 1500f;
    [Tooltip("Максимальный угол поворота передних колес (в градусах).")]
    public float maxSteeringAngle = 35f;
    [Tooltip("Сила торможения.")]
    public float maxBrakeTorque = 5000f;

    [Header("Клавиши управления")]
    [Tooltip("Клавиша для торможения.")]
    public KeyCode brakeKey = KeyCode.Space;

    // Приватные переменные
    private float currentMotorTorque = 0f;
    private float currentSteeringAngle = 0f;
    private float currentBrakeTorque = 0f;

    void FixedUpdate() // Используем FixedUpdate для работы с физикой
    {
        // Проверяем, назначены ли основные коллайдеры колес
        if (frontLeftWheelCollider == null || frontRightWheelCollider == null || rearLeftWheelCollider == null || rearRightWheelCollider == null)
        {
            Debug.LogError("Не все коллайдеры колес (WheelColliders) назначены в инспекторе!");
            enabled = false; // Отключаем компонент
            return;
        }

        // 1. Получаем ввод с осей "Vertical" и "Horizontal"
        float verticalInput = Input.GetAxis("Vertical");     // W/Up = 1, S/Down = -1
        float horizontalInput = Input.GetAxis("Horizontal"); // D/Right = 1, A/Left = -1

        // 2. Рассчитываем и применяем поворот
        currentSteeringAngle = maxSteeringAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteeringAngle;
        frontRightWheelCollider.steerAngle = currentSteeringAngle;

        // 3. Рассчитываем и применяем крутящий момент (движение)
        // Применяем к задним колесам (RWD - Rear Wheel Drive)
        // Если нужен полный привод (AWD), примените и к передним
        currentMotorTorque = maxMotorTorque * verticalInput;
        rearLeftWheelCollider.motorTorque = currentMotorTorque;
        rearRightWheelCollider.motorTorque = currentMotorTorque;
        // Для AWD раскомментируйте:
        // frontLeftWheelCollider.motorTorque = currentMotorTorque;
        // frontRightWheelCollider.motorTorque = currentMotorTorque;

        // 4. Рассчитываем и применяем торможение
        if (Input.GetKey(brakeKey))
        {
            currentBrakeTorque = maxBrakeTorque;
        }
        else
        {
            // Если не тормозим активно, можно убрать остаточное торможение
            // или оставить небольшое значение для имитации трения
            currentBrakeTorque = 0f;
        }
        // Применяем торможение ко всем колесам
        frontLeftWheelCollider.brakeTorque = currentBrakeTorque;
        frontRightWheelCollider.brakeTorque = currentBrakeTorque;
        rearLeftWheelCollider.brakeTorque = currentBrakeTorque;
        rearRightWheelCollider.brakeTorque = currentBrakeTorque;


        // 5. Обновляем визуальное положение и вращение колес (если ссылки заданы)
        UpdateWheelVisuals(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateWheelVisuals(frontRightWheelCollider, frontRightWheelTransform);
        UpdateWheelVisuals(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateWheelVisuals(rearRightWheelCollider, rearRightWheelTransform);
    }

    /// <summary>
    /// Обновляет позицию и вращение визуальной модели колеса
    /// в соответствии с состоянием WheelCollider.
    /// </summary>
    /// <param name="wheelCollider">Коллайдер колеса.</param>
    /// <param name="wheelTransform">Transform визуальной модели колеса.</param>
    void UpdateWheelVisuals(WheelCollider wheelCollider, Transform wheelTransform)
    {
        if (wheelTransform == null) return; // Пропускаем, если Transform не назначен

        Vector3 position;
        Quaternion rotation;
        // Получаем текущее мировое положение и вращение коллайдера
        wheelCollider.GetWorldPose(out position, out rotation);

        // Применяем их к визуальной модели
        wheelTransform.position = position;
        wheelTransform.rotation = rotation;
    }

    // Можно добавить визуализацию в редакторе для отладки
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

#if UNITY_EDITOR
        string debugText = $"Motor Torque: {currentMotorTorque:F0}\nSteer Angle: {currentSteeringAngle:F1}°\nBrake Torque: {currentBrakeTorque:F0}";
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1.0f, debugText);
#endif
    }
}