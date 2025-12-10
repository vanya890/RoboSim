using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Главный контроллер робота, управляющий всеми компонентами
/// </summary>
public class RobotController : MonoBehaviour
{
    [Header("Колеса робота")]
    [SerializeField] private WheelController[] wheels;

    [Header("Сенсоры робота")]
    [SerializeField] private SensorBase[] sensors;

    [Header("Управление")]
    [SerializeField] private bool enableKeyboardControl = false;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private KeyCode toggleControlKey = KeyCode.Tab;
    [SerializeField] private KeyCode logSensorDataKey = KeyCode.S;

    // Словарь для хранения данных от сенсоров
    private Dictionary<string, object> sensorData = new Dictionary<string, object>();

    // События для уведомления подписчиков об изменении данных сенсоров
    public delegate void SensorDataUpdateHandler(string sensorName, object data);
    public event SensorDataUpdateHandler OnSensorDataUpdate;

    // Событие для уведомления об изменении режима управления
    public delegate void ControlModeChangeHandler(bool isEnabled);
    public event ControlModeChangeHandler OnControlModeChange;

    private void Awake()
    {
        // Регистрируем все сенсоры
        if (sensors != null)
        {
            foreach (var sensor in sensors)
            {
                if (sensor != null)
                {
                    // Подписываемся на события обновления данных сенсора
                    sensor.OnDataUpdate += HandleSensorDataUpdate;
                }
            }
        }
    }

    private void Start()
    {
        // Запускаем все сенсоры
        if (sensors != null)
        {
            foreach (var sensor in sensors)
            {
                if (sensor != null)
                {
                    sensor.StartSensor();
                }
            }
        }
    }

    private void Update()
    {
        // Проверяем нажатие клавиши переключения режима управления
        if (Input.GetKeyDown(toggleControlKey))
        {
            enableKeyboardControl = !enableKeyboardControl;
            Debug.Log($"Ручное управление {(enableKeyboardControl ? "включено" : "выключено")}");
            OnControlModeChange?.Invoke(enableKeyboardControl);
        }
        
        // Проверяем нажатие клавиши для вывода данных сенсоров
        if (Input.GetKeyDown(logSensorDataKey))
        {
            LogAllSensorData();
        }

        // Обрабатываем ввод с клавиатуры, если ручное управление включено
        if (enableKeyboardControl)
        {
            HandleKeyboardInput();
        }
    }

    /// <summary>
    /// Обработка ввода с клавиатуры
    /// </summary>
    private void HandleKeyboardInput()
    {
        // Получаем ввод с клавиатуры
        float forwardInput = Input.GetAxis("Vertical");
        float rotationInput = Input.GetAxis("Horizontal");

        // Применяем множители скорости
        Vector2 movementInput = new Vector2(rotationInput * rotationSpeed, forwardInput * moveSpeed);

        // Двигаем робота
        Move(movementInput);
    }

    private void OnDestroy()
    {
        // Отписываемся от событий при уничтожении объекта
        if (sensors != null)
        {
            foreach (var sensor in sensors)
            {
                if (sensor != null)
                {
                    sensor.OnDataUpdate -= HandleSensorDataUpdate;
                    sensor.StopSensor();
                }
            }
        }
    }

    /// <summary>
    /// Обработчик обновления данных сенсора
    /// </summary>
    /// <param name="sensorName">Имя сенсора</param>
    /// <param name="data">Новые данные</param>
    private void HandleSensorDataUpdate(string sensorName, object data)
    {
        // Сохраняем данные
        sensorData[sensorName] = data;

        // Уведомляем подписчиков
        OnSensorDataUpdate?.Invoke(sensorName, data);
    }

    /// <summary>
    /// Получить данные от сенсора
    /// </summary>
    /// <typeparam name="T">Тип данных</typeparam>
    /// <param name="sensorName">Имя сенсора</param>
    /// <returns>Данные сенсора или значение по умолчанию</returns>
    public T GetSensorData<T>(string sensorName)
    {
        if (sensorData.TryGetValue(sensorName, out object data) && data is T)
        {
            return (T)data;
        }
        return default(T);
    }

    /// <summary>
    /// Движение робота
    /// </summary>
    /// <param name="movementInput">Вектор движения (X - поворот, Y - вперед/назад)</param>
    public void Move(Vector2 movementInput)
    {
        if (wheels == null || wheels.Length == 0)
            return;

        // Простая реализация движения для всех колес
        float forwardSpeed = movementInput.y;
        float rotationInput = movementInput.x;

        foreach (var wheel in wheels)
        {
            if (wheel != null)
            {
                // Применяем скорость к колесу
                wheel.SetSpeed(forwardSpeed);

                // Управление поворотом рулевых колес
                // Проверяем, является ли колесо рулевым
                if (wheel.IsSteeringWheel())
                {
                    // Для рулевых колес применяем поворот
                    wheel.SetSteerAngle(rotationInput);
                }

                // Для движения используем дифференциал между левыми и правыми колесами
                if (wheel.transform.localPosition.x > 0) // Правое колесо
                {
                    wheel.SetSpeed(forwardSpeed + rotationInput * 0.5f); // Уменьшаем влияние поворота на скорость
                }
                else // Левое колесо
                {
                    wheel.SetSpeed(forwardSpeed - rotationInput * 0.5f); // Уменьшаем влияние поворота на скорость
                }
            }
        }
    }

    /// <summary>
    /// Включить или выключить ручное управление
    /// </summary>
    /// <param name="enable">Включить управление</param>
    public void SetKeyboardControl(bool enable)
    {
        enableKeyboardControl = enable;
        OnControlModeChange?.Invoke(enableKeyboardControl);
    }

    /// <summary>
    /// Получить текущий режим управления
    /// </summary>
    /// <returns>True, если ручное управление включено</returns>
    public bool IsKeyboardControlEnabled()
    {
        return enableKeyboardControl;
    }
    
    /// <summary>
    /// Вывести в консоль данные всех сенсоров
    /// </summary>
    public void LogAllSensorData()
    {
        Debug.Log("=== ДАННЫЕ ВСЕХ СЕНСОРОВ ===");
        
        foreach (var pair in sensorData)
        {
            string sensorName = pair.Key;
            object data = pair.Value;
            
            // Формируем универсальный вывод данных
            string dataString = GetSensorDataString(data);
            Debug.Log($"[Сенсор {sensorName}] {dataString}");
        }
        
        Debug.Log("=== КОНЕЦ ДАННЫХ СЕНСОРОВ ===");
    }
    
    /// <summary>
    /// Получить строковое представление данных сенсора
    /// </summary>
    /// <param name="data">Данные сенсора</param>
    /// <returns>Строковое представление данных</returns>
    private string GetSensorDataString(object data)
    {
        // Если данные реализуют интерфейс ISensorData, используем универсальный подход
        if (data is ISensorData sensorData)
        {
            var dataDict = sensorData.GetData();
            var result = new System.Text.StringBuilder();
            
            foreach (var kvp in dataDict)
            {
                if (result.Length > 0)
                    result.Append(", ");
                    
                result.Append($"{kvp.Key}: {kvp.Value}");
            }
            
            return result.ToString();
        }
        // Для простых типов данных выводим их напрямую
        else if (data is float || data is int || data is bool || data is string)
        {
            return data.ToString();
        }
        // Для других типов данных выводим их строковое представление
        else
        {
            return $"Тип: {data.GetType().Name}, Значение: {data}";
        }
    }
}
