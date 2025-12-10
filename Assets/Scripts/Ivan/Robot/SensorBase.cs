using UnityEngine;
using System;

/// <summary>
/// Базовый класс для всех сенсоров робота
/// </summary>
public abstract class SensorBase : MonoBehaviour
{
    [Header("Настройки сенсора")]
    [SerializeField] protected string sensorName = "Sensor";
    [SerializeField] protected float updateInterval = 0.1f; // Интервал обновления в секундах

    protected float lastUpdateTime = 0f;
    protected bool isRunning = false;

    // Событие для уведомления об обновлении данных
    public event Action<string, object> OnDataUpdate;

    /// <summary>
    /// Запуск сенсора
    /// </summary>
    public virtual void StartSensor()
    {
        isRunning = true;
        lastUpdateTime = 0f;
        OnStart();
    }

    /// <summary>
    /// Остановка сенсора
    /// </summary>
    public virtual void StopSensor()
    {
        isRunning = false;
        OnStop();
    }

    /// <summary>
    /// Получить имя сенсора
    /// </summary>
    /// <returns>Имя сенсора</returns>
    public string GetSensorName()
    {
        return sensorName;
    }

    /// <summary>
    /// Установить имя сенсора
    /// </summary>
    /// <param name="name">Новое имя</param>
    public void SetSensorName(string name)
    {
        sensorName = name;
    }

    /// <summary>
    /// Получить интервал обновления
    /// </summary>
    /// <returns>Интервал обновления в секундах</returns>
    public float GetUpdateInterval()
    {
        return updateInterval;
    }

    /// <summary>
    /// Установить интервал обновления
    /// </summary>
    /// <param name="interval">Новый интервал в секундах</param>
    public void SetUpdateInterval(float interval)
    {
        updateInterval = Mathf.Max(0.01f, interval);
    }

    /// <summary>
    /// Проверить, запущен ли сенсор
    /// </summary>
    /// <returns>True, если сенсор запущен</returns>
    public bool IsRunning()
    {
        return isRunning;
    }

    private void Update()
    {
        if (!isRunning)
            return;

        // Проверяем, прошло ли достаточно времени для обновления
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            lastUpdateTime = Time.time;

            // Получаем данные от сенсора
            object data = ReadSensorData();

            // Уведомляем подписчиков
            OnDataUpdate?.Invoke(sensorName, data);
        }
    }

    /// <summary>
    /// Метод для чтения данных с сенсора (должен быть реализован в дочерних классах)
    /// </summary>
    /// <returns>Данные сенсора</returns>
    protected abstract object ReadSensorData();

    /// <summary>
    /// Метод, вызываемый при запуске сенсора (опционально для переопределения)
    /// </summary>
    protected virtual void OnStart() { }

    /// <summary>
    /// Метод, вызываемый при остановке сенсора (опционально для переопределения)
    /// </summary>
    protected virtual void OnStop() { }

    /// <summary>
    /// Защищенный метод для вызова события обновления данных
    /// </summary>
    /// <param name="data">Данные сенсора</param>
    protected void TriggerDataUpdate(object data)
    {
        OnDataUpdate?.Invoke(sensorName, data);
    }
}
