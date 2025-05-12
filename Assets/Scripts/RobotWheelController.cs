using UnityEngine;

/// <summary>
/// Управляет одним колесом робота с помощью WheelCollider.
/// Позволяет задавать скорость вращения и отслеживает пройденные обороты (энкодер).
/// </summary>
[RequireComponent(typeof(WheelCollider))] // Гарантирует наличие WheelCollider
public class RobotWheelController : MonoBehaviour
{
    [Header("Ссылки")]
    [Tooltip("Wheel Collider, которым управляет этот скрипт.")]
    [SerializeField] // Показываем в инспекторе, но не делаем публичным для изменения извне
    private WheelCollider wheelCollider;

    [Tooltip("Визуальное представление колеса (Transform меша). Необязательно, для синхронизации вращения.")]
    public Transform wheelMeshTransform;

    [Header("Параметры управления")]
    [Tooltip("Максимальная скорость вращения (оборотов в минуту), которую можно задать колесу.")]
    public float maxTargetRPM = 300f; // Оборотов в минуту (RPM)

    [Tooltip("Максимальный крутящий момент, который может приложить мотор колеса.")]
    public float maxMotorTorque = 100f;

    [Tooltip("Максимальный тормозной момент.")]
    public float maxBrakeTorque = 150f;

    [Tooltip("Коэффициент усиления для П-регулятора скорости. Определяет, насколько агрессивно колесо пытается достичь целевой скорости.")]
    public float speedControlGain = 5f; // Коэффициент P (Пропорциональный)

    [Header("Данные энкодера (Read-Only)")]
    [SerializeField]
    [Tooltip("Общее количество оборотов, пройденное колесом с момента последнего сброса.")]
    private double totalRevolutions = 0; // Используем double для большей точности при долгих симуляциях

    [SerializeField]
    [Tooltip("Текущая скорость вращения колеса (оборотов в минуту).")]
    private float currentRPM = 0;
    // --- ДОБАВЛЕНО ---
    [SerializeField] // Делаем видимым в инспекторе
    [Tooltip("Текущая ЦЕЛЕВАЯ скорость вращения колеса (оборотов в минуту), заданная контроллером.")]
    private float targetRPM = 0f; // Целевая скорость вращения
    // --- КОНЕЦ ДОБАВЛЕНИЯ ---
    private double cumulativeRotation = 0; // Внутренний счетчик для оборотов

    // Публичные свойства для доступа к данным из других скриптов
    public double TotalRevolutions => totalRevolutions;
    public float CurrentRPM => currentRPM;
    public float TargetRPM => targetRPM; // Полезно для отладки

    void Awake()
    {
        // Получаем компонент WheelCollider, если он не назначен вручную
        if (wheelCollider == null)
        {
            wheelCollider = GetComponent<WheelCollider>();
        }

        // Важно: Убедитесь, что настройки WheelCollider (масса, демпфирование, подвеска, трение)
        // подходят для вашего робота. Их можно настроить в инспекторе.
        // Пример настройки трения (если нужно):
        /*
        WheelFrictionCurve ff = wheelCollider.forwardFriction;
        ff.stiffness = 1.5f; // Пример значения
        wheelCollider.forwardFriction = ff;
        WheelFrictionCurve sf = wheelCollider.sidewaysFriction;
        sf.stiffness = 1.0f; // Пример значения
        wheelCollider.sidewaysFriction = sf;
        */
    }

    void Update()
    {
        // Обновляем визуальное представление колеса (если оно есть)
        // Делаем это в Update для плавной визуализации
        UpdateWheelVisuals();
    }

    void FixedUpdate()
    {
        // Физические расчеты и управление делаем в FixedUpdate

        // 1. Получаем текущую скорость от WheelCollider
        currentRPM = wheelCollider.rpm;

        // 2. Управляем скоростью колеса (применяем момент или тормоз)
        ControlSpeed();

        // 3. Обновляем данные энкодера (считаем обороты)
        UpdateEncoder();
    }

    /// <summary>
    /// Задает целевую скорость вращения колеса.
    /// </summary>
    /// <param name="percentage">Процент от максимальной скорости (от -100 до 100). Отрицательное значение - вращение назад.</param>
    /// <param name="direction">Направление вращения (1 - вперед, -1 - назад, 0 - остановка). Перемножается с percentage.</param>
    /// <param name="maxSpeedRPM">Максимальная скорость вращения (RPM), используемая для расчета процента.</param>
    public void SetSpeed(float percentage, int direction, float maxSpeedRPM)
    {
        // Устанавливаем новый максимум, если нужно
        this.maxTargetRPM = maxSpeedRPM;

        // Нормализуем направление
        direction = (int)Mathf.Sign(direction); // 1, -1 или 0

        // Ограничиваем процент
        percentage = Mathf.Clamp(percentage, 0f, 100f); // Процент всегда положительный

        // Рассчитываем целевую RPM с учетом направления
        targetRPM = (percentage / 100.0f) * this.maxTargetRPM * direction;
    }

    /// <summary>
    /// Альтернативный метод: Задает целевую скорость напрямую в процентах от текущего maxTargetRPM.
    /// </summary>
    /// <param name="percentage">Процент от maxTargetRPM (-100 до 100).</param>
    public void SetSpeedPercentage(float percentage)
    {
        percentage = Mathf.Clamp(percentage, -100f, 100f);
        targetRPM = (percentage / 100.0f) * maxTargetRPM;
    }


    /// <summary>
    /// Реализует П-регулятор для достижения целевой скорости вращения
    /// и применяет торможение при необходимости.
    /// </summary>
    private void ControlSpeed()
    {
        // Определяем, является ли целевая скорость нулевой (остановка)
        // Используем небольшой порог для точности с плавающей запятой
        bool isTargetStopping = Mathf.Abs(targetRPM) < 0.1f;

        if (isTargetStopping)
        {
            // --- Цель: Остановка ---
            wheelCollider.motorTorque = 0f; // Убираем любой моторный момент

            // Применяем тормоз, если колесо все еще вращается
            // Порог currentRPM можно настроить (например, 0.5 или 1.0)
            if (Mathf.Abs(currentRPM) > 0.5f)
            {
                // Применяем максимальный тормозной момент для быстрой остановки
                wheelCollider.brakeTorque = maxBrakeTorque;
            }
            else
            {
                // Колесо почти остановилось или уже стоит, отпускаем тормоз
                // Это важно, чтобы робот не был "заблокирован" тормозом при старте
                wheelCollider.brakeTorque = 0f;
            }
        }
        else
        {
            // --- Цель: Движение к targetRPM ---
            wheelCollider.brakeTorque = 0f; // Убеждаемся, что тормоз выключен

            // Рассчитываем ошибку между целевой и текущей скоростью
            float error = targetRPM - currentRPM;

            // Рассчитываем необходимый крутящий момент (пропорционально ошибке)
            float motorTorque = error * speedControlGain;

            // Ограничиваем максимальный крутящий момент
            motorTorque = Mathf.Clamp(motorTorque, -maxMotorTorque, maxMotorTorque);

            // Применяем моторный момент
            wheelCollider.motorTorque = motorTorque;
        }
    }

    /// <summary>
    /// Обновляет счетчик оборотов на основе текущей скорости вращения.
    /// </summary>
    private void UpdateEncoder()
    {
        // Рассчитываем изменение угла за время FixedUpdate
        // RPM (обороты/минуту) -> RPS (обороты/секунду) -> Обороты за Time.fixedDeltaTime
        double deltaRotation = (double)currentRPM * Time.fixedDeltaTime / 60.0;

        // Накапливаем общее количество оборотов
        cumulativeRotation += deltaRotation;
        totalRevolutions = cumulativeRotation; // Обновляем публичное значение
    }

    /// <summary>
    /// Обновляет позицию и вращение визуального меша колеса (если он назначен).
    /// </summary>
    private void UpdateWheelVisuals()
    {
        if (wheelMeshTransform != null)
        {
            Vector3 position;
            Quaternion rotation;
            // Получаем текущее положение и вращение коллайдера в мире
            wheelCollider.GetWorldPose(out position, out rotation);
            // Применяем их к визуальному мешу
            //wheelMeshTransform.position = position;
            wheelMeshTransform.rotation = rotation;
        }
    }

    /// <summary>
    /// Сбрасывает счетчик оборотов энкодера в ноль.
    /// </summary>
    public void ResetEncoder()
    {
        cumulativeRotation = 0;
        totalRevolutions = 0;
        Debug.Log($"Encoder reset for wheel: {gameObject.name}");
    }

    // --- Пример использования из другого скрипта (например, контроллера робота) ---
    /*
    public RobotWheelController leftWheel;
    public RobotWheelController rightWheel;
    public float robotSpeedPercentage = 50f; // 50% от макс скорости
    public float robotMaxRPM = 250f;

    void ControlRobot()
    {
        // Ехать вперед со скоростью 50%
        leftWheel.SetSpeed(robotSpeedPercentage, 1, robotMaxRPM);
        rightWheel.SetSpeed(robotSpeedPercentage, 1, robotMaxRPM);

        // Или используя SetSpeedPercentage (если maxTargetRPM уже настроен в инспекторе)
        // leftWheel.SetSpeedPercentage(robotSpeedPercentage);
        // rightWheel.SetSpeedPercentage(robotSpeedPercentage);

        // Поворот на месте (левое вперед, правое назад)
        // leftWheel.SetSpeedPercentage(30f);
        // rightWheel.SetSpeedPercentage(-30f);

        // Получить данные энкодера
        double leftRevs = leftWheel.TotalRevolutions;
        double rightRevs = rightWheel.TotalRevolutions;
        // Debug.Log($"Left Revs: {leftRevs}, Right Revs: {rightRevs}");

        // Сбросить энкодеры
        // if (Input.GetKeyDown(KeyCode.R))
        // {
        //     leftWheel.ResetEncoder();
        //     rightWheel.ResetEncoder();
        // }
    }
    */
}