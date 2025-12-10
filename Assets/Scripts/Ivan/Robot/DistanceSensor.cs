using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Датчик расстояния, использующий Raycast для определения расстояния до объектов.
/// Наследуется от SensorBase и отправляет измеренное расстояние в систему робота.
/// </summary>
public class DistanceSensor : SensorBase
{
    [Header("Настройки датчика расстояния")]
    [Tooltip("Максимальная дистанция обнаружения объектов")]
    [SerializeField] private float maxDistance = 10f;

    [Tooltip("Слои, на которые реагирует сенсор")]
    [SerializeField] private LayerMask detectionLayers = -1; // Все слои по умолчанию

    [Header("Настройки отображения луча")]
    [Tooltip("Показывать отладочный луч в редакторе")]
    [SerializeField] private bool showDebugRay = true;

    [Tooltip("Показывать луч в игровом режиме")]
    [SerializeField] private bool showGameRay = false;

    [Tooltip("Цвет отладочного луча")]
    [SerializeField] private Color debugRayColor = Color.red;
    
    [Tooltip("Цвет луча в игровом режиме")]
    [SerializeField] private Color gameRayColor = Color.yellow;
    
    [Tooltip("Размер точки столкновения")]
    [SerializeField] private float hitPointSize = 0.1f;
    
    [Header("Настройки определения цвета")]
    [Tooltip("Включить определение цвета в точке столкновения")]
    [SerializeField] private bool detectColor = false;
    
    [Tooltip("Размер текстуры для считывания цвета")]
    [SerializeField] private int textureSize = 1;

    // Последнее измеренное расстояние
    private float lastDistance = 0f;
    
    // Компоненты для отображения луча в игровом режиме
    private LineRenderer lineRenderer;
    private GameObject hitPointIndicator;
    
    // Информация о последнем столкновении для отладки
    private bool hasHit = false;
    private Vector3 hitPoint = Vector3.zero;
    
    // Информация о цвете в точке столкновения
    private Color detectedColor = Color.white;

    protected override void OnStart()
    {
        // Устанавливаем имя по умолчанию, если оно не было установлено
        if (string.IsNullOrEmpty(sensorName))
        {
            sensorName = "DistanceSensor";
        }
        
        // Инициализация компонентов для отображения в игровом режиме
        InitializeGameRay();
    }
    
    /// <summary>
    /// Инициализация компонентов для отображения луча в игровом режиме
    /// </summary>
    private void InitializeGameRay()
    {
        // Создаем LineRenderer для отображения луча
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = gameRayColor;
        lineRenderer.endColor = gameRayColor;
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = showGameRay;
        
        // Создаем индикатор точки столкновения
        hitPointIndicator = new GameObject("HitPointIndicator");
        hitPointIndicator.transform.SetParent(transform);
        
        // Добавляем компонент для отображения сферы в точке столкновения
        var sphereRenderer = hitPointIndicator.AddComponent<SpriteRenderer>();
        sphereRenderer.sprite = Resources.Load<Sprite>("Circle"); // Попробуйте загрузить спрайт круга
        
        // Если спрайт не найден, создаем простой индикатор
        if (sphereRenderer.sprite == null)
        {
            // Создаем простой индикатор через LineRenderer
            var indicatorRenderer = hitPointIndicator.AddComponent<LineRenderer>();
            indicatorRenderer.material = new Material(Shader.Find("Sprites/Default"));
            indicatorRenderer.startColor = Color.red;
            indicatorRenderer.endColor = Color.red;
            indicatorRenderer.startWidth = hitPointSize;
            indicatorRenderer.endWidth = hitPointSize;
            indicatorRenderer.loop = true;
            indicatorRenderer.positionCount = 20;
            
            // Создаем круг
            for (int i = 0; i < 20; i++)
            {
                float angle = i * Mathf.PI * 2 / 20;
                indicatorRenderer.SetPosition(i, new Vector3(Mathf.Cos(angle) * hitPointSize, Mathf.Sin(angle) * hitPointSize, 0));
            }
        }
        
        hitPointIndicator.SetActive(false);
    }

    /// <summary>
    /// Читает данные с сенсора расстояния
    /// </summary>
    /// <returns>Измеренное расстояние до объекта</returns>
    protected override object ReadSensorData()
    {
        // Выпускаем луч вперед от трансформа сенсора
        RaycastHit hit;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(transform.position, direction, out hit, maxDistance, detectionLayers))
        {
            // Если попали в объект, сохраняем расстояние до него
            lastDistance = hit.distance;

            // Сохраняем информацию о столкновении
            hasHit = true;
            hitPoint = hit.point;

            // Рисуем отладочный луч до точки столкновения
            if (showDebugRay)
            {
                Debug.DrawLine(transform.position, hit.point, debugRayColor);
            }
            
            // Определяем цвет в точке столкновения, если включена эта функция
            if (detectColor)
            {
                detectedColor = GetColorAtHitPoint(hit);
            }
            
            // Обновляем луч в игровом режиме
            UpdateGameRay(hit.point, true);
        }
        else
        {
            // Если не попали ни в один объект, сохраняем максимальное расстояние
            lastDistance = maxDistance;

            // Рисуем отладочный луч на максимальную дистанцию
            if (showDebugRay)
            {
                Debug.DrawLine(transform.position, transform.position + direction * maxDistance, debugRayColor);
            }
            
            // Обновляем луч в игровом режиме
            UpdateGameRay(transform.position + direction * maxDistance, false);
            
            // Сбрасываем информацию о столкновении
            hasHit = false;
        }

        // Возвращаем структуру с информацией о расстоянии и цвете
        var sensorData = new SensorData
        {
            distance = lastDistance,
            color = detectColor ? detectedColor : Color.white,
            hasDetectedColor = detectColor && hasHit
        };
        
        return sensorData;
    }

    /// <summary>
    /// Получить последнее измеренное расстояние
    /// </summary>
    /// <returns>Расстояние до объекта или максимальная дистанция</returns>
    public float GetLastDistance()
    {
        return lastDistance;
    }
    
    /// <summary>
    /// Получить последний определенный цвет
    /// </summary>
    /// <returns>Цвет в точке столкновения или белый по умолчанию</returns>
    public Color GetDetectedColor()
    {
        return detectedColor;
    }
    
    /// <summary>
    /// Включить или выключить определение цвета
    /// </summary>
    /// <param name="enable">Включить определение цвета</param>
    public void SetColorDetection(bool enable)
    {
        detectColor = enable;
    }
    
    /// <summary>
    /// Проверить, включено ли определение цвета
    /// </summary>
    /// <returns>Включено ли определение цвета</returns>
    public bool IsColorDetectionEnabled()
    {
        return detectColor;
    }

    /// <summary>
    /// Получить максимальную дистанцию обнаружения
    /// </summary>
    /// <returns>Максимальная дистанция</returns>
    public float GetMaxDistance()
    {
        return maxDistance;
    }

    /// <summary>
    /// Установить максимальную дистанцию обнаружения
    /// </summary>
    /// <param name="distance">Новая максимальная дистанция</param>
    public void SetMaxDistance(float distance)
    {
        maxDistance = Mathf.Max(0.1f, distance);
    }

    /// <summary>
    /// Получить слои, на которые реагирует сенсор
    /// </summary>
    /// <returns>Маска слоев</returns>
    public LayerMask GetDetectionLayers()
    {
        return detectionLayers;
    }

    /// <summary>
    /// Установить слои, на которые реагирует сенсор
    /// </summary>
    /// <param name="layers">Новая маска слоев</param>
    public void SetDetectionLayers(LayerMask layers)
    {
        detectionLayers = layers;
    }

    /// <summary>
    /// Обновляет отображение луча в игровом режиме
    /// </summary>
    /// <param name="endPoint">Конечная точка луча</param>
    /// <param name="isHit">Было ли столкновение</param>
    private void UpdateGameRay(Vector3 endPoint, bool isHit)
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = showGameRay;
            if (showGameRay)
            {
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, endPoint);
                
                // Обновляем индикатор точки столкновения
                if (hitPointIndicator != null)
                {
                    hitPointIndicator.SetActive(isHit && showGameRay);
                    if (isHit)
                    {
                        hitPointIndicator.transform.position = endPoint;
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Включает или выключает отображение луча в игровом режиме
    /// </summary>
    /// <param name="show">Показывать ли луч</param>
    public void SetGameRayVisibility(bool show)
    {
        showGameRay = show;
        if (lineRenderer != null)
        {
            lineRenderer.enabled = show;
        }
        if (hitPointIndicator != null)
        {
            hitPointIndicator.SetActive(false);
        }
    }
    
    /// <summary>
    /// Получает текущее состояние видимости луча в игровом режиме
    /// </summary>
    /// <returns>Видим ли луч</returns>
    public bool GetGameRayVisibility()
    {
        return showGameRay;
    }
    
    /// <summary>
    /// Устанавливает цвет луча в игровом режиме
    /// </summary>
    /// <param name="color">Новый цвет</param>
    public void SetGameRayColor(Color color)
    {
        gameRayColor = color;
        if (lineRenderer != null)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }
    }
    
    /// <summary>
    /// Отрисовка вспомогательных элементов в редакторе
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Рисуем сферу в редакторе для визуализации максимальной дистанции
        Gizmos.color = new Color(debugRayColor.r, debugRayColor.g, debugRayColor.b, 0.2f);
        Gizmos.DrawSphere(transform.position, maxDistance);

        // Рисуем направление луча
        Gizmos.color = debugRayColor;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * maxDistance);
        
        // Рисуем сферу в точке столкновения, если она есть
        if (hasHit)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(hitPoint, hitPointSize);
        }
    }
    
    /// <summary>
    /// Получает цвет в точке столкновения
    /// </summary>
    /// <param name="hit">Информация о столкновении</param>
    /// <returns>Цвет в точке столкновения</returns>
    private Color GetColorAtHitPoint(RaycastHit hit)
    {
        Renderer renderer = hit.collider.GetComponent<Renderer>();
        if (renderer == null)
        {
            return Color.white;
        }
        
        Material material = renderer.material;
        if (material == null)
        {
            return Color.white;
        }
        
        Texture2D texture = material.mainTexture as Texture2D;
        if (texture == null)
        {
            // Если текстуры нет, возвращаем цвет материала
            return material.color;
        }
        
        // Получаем UV координаты точки столкновения
        Vector2 pixelUV = hit.textureCoord;
        
        // Преобразуем UV в пиксельные координаты
        int x = Mathf.FloorToInt(pixelUV.x * texture.width);
        int y = Mathf.FloorToInt(pixelUV.y * texture.height);
        
        // Проверяем, что координаты в пределах текстуры
        if (x < 0 || x >= texture.width || y < 0 || y >= texture.height)
        {
            return material.color;
        }
        
        try
        {
            // Создаем временную текстуру для чтения пикселя
            Texture2D tempTexture = new Texture2D(textureSize, textureSize);
            
            // Копируем область текстуры
            int startX = Mathf.Max(0, x - textureSize / 2);
            int startY = Mathf.Max(0, y - textureSize / 2);
            
            // Получаем пиксели из исходной текстуры
            Color[] pixels = texture.GetPixels(startX, startY, textureSize, textureSize);
            
            // Устанавливаем пиксели во временную текстуру
            tempTexture.SetPixels(pixels);
            tempTexture.Apply();
            
            // Получаем средний цвет
            Color averageColor = Color.white;
            foreach (Color pixel in pixels)
            {
                averageColor += pixel;
            }
            averageColor /= pixels.Length;
            
            // Освобождаем ресурсы
            Object.Destroy(tempTexture);
            
            return averageColor;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Не удалось получить цвет из текстуры: {e.Message}");
            return material.color;
        }
    }
    
    /// <summary>
    /// Структура для хранения данных сенсора
    /// </summary>
    public struct SensorData : ISensorData
    {
        public float distance;
        public Color color;
        public bool hasDetectedColor;
        
        /// <summary>
        /// Получить словарь с данными сенсора в формате ключ-значение
        /// </summary>
        /// <returns>Словарь с данными</returns>
        public Dictionary<string, object> GetData()
        {
            var data = new Dictionary<string, object>();
            data["distance"] = distance;
            data["hasDetectedColor"] = hasDetectedColor;
            
            if (hasDetectedColor)
            {
                data["color_r"] = color.r;
                data["color_g"] = color.g;
                data["color_b"] = color.b;
                data["color_a"] = color.a;
            }
            
            return data;
        }
    }
}
