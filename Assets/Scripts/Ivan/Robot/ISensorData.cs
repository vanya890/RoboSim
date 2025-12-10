using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Универсальный интерфейс для данных сенсоров
/// </summary>
public interface ISensorData
{
    /// <summary>
    /// Получить словарь с данными сенсора в формате ключ-значение
    /// </summary>
    /// <returns>Словарь с данными</returns>
    Dictionary<string, object> GetData();
}
