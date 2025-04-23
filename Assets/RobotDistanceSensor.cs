using UnityEngine;

/// <summary>
/// Многофункциональный сенсор робота: дальномер, датчик яркости, датчик цвета и детектор источника света.
/// </summary>
public class RobotDistanceSensor : MonoBehaviour
{
    [Header("Настройки сенсора")]
    [Tooltip("Максимальная дистанция обнаружения.")]
    public float maxDistance = 10f;

    [Tooltip("Частота обновления сенсора (в секундах). 0 - обновлять каждый кадр.")]
    public float updateFrequency = 0.1f;

    [Tooltip("Слой(и), которые сенсор должен обнаруживать.")]
    public LayerMask detectionLayerMask = ~0; // ~0 означает все слои по умолчанию

    [Header("Данные сенсора (Read-Only)")]
    [SerializeField]
    [Tooltip("Обнаруженное расстояние до объекта. Равно maxDistance, если ничего не обнаружено.")]
    private float detectedDistance = -1f; // Используем -1 для обозначения "еще не измерено" или "вне диапазона"

    [SerializeField]
    [Tooltip("Обнаруженный цвет поверхности.")]
    private Color detectedColor = Color.clear; // Используем прозрачный цвет как "ничего не обнаружено"

    [SerializeField]
    [Tooltip("Рассчитанная яркость обнаруженной поверхности (0-1).")]
    private float detectedBrightness = 0f;

    [SerializeField]
    [Tooltip("Обнаружен ли источник света напрямую?")]
    private bool isLightSourceDetected = false;

    [SerializeField]
    [Tooltip("Есть ли объект в пределах досягаемости сенсора?")]
    private bool objectDetected = false;


    // Публичные свойства для доступа к данным сенсора из других скриптов
    public float DetectedDistance => detectedDistance;
    public Color DetectedColor => detectedColor;
    public float DetectedBrightness => detectedBrightness;
    public bool IsLightSourceDetected => isLightSourceDetected;
    public bool IsObjectDetected => objectDetected;


    private float timeSinceLastUpdate = 0f;

    void Start()
    {
        // Инициализация начальных значений
        detectedDistance = maxDistance;
        detectedColor = Color.clear;
        detectedBrightness = 0f;
        isLightSourceDetected = false;
        objectDetected = false;

        // Первый замер при старте, если частота не нулевая
        if (updateFrequency > 0)
        {
            PerformSensorSweep();
        }
    }

    void Update()
    {
        // Обновление по таймеру или каждый кадр
        if (updateFrequency <= 0)
        {
            PerformSensorSweep();
        }
        else
        {
            timeSinceLastUpdate += Time.deltaTime;
            if (timeSinceLastUpdate >= updateFrequency)
            {
                PerformSensorSweep();
                timeSinceLastUpdate = 0f;
            }
        }
    }

    /// <summary>
    /// Выполняет основную логику сенсора: пускает луч и обрабатывает результат.
    /// </summary>
    void PerformSensorSweep()
    {
        // Пускаем луч из текущей позиции сенсора вперед
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        // Сбрасываем флаг обнаружения перед новым замером
        objectDetected = false;

        if (Physics.Raycast(ray, out hitInfo, maxDistance, detectionLayerMask))
        {
            // --- Объект обнаружен ---
            objectDetected = true;

            // 1. Дальномер
            detectedDistance = hitInfo.distance;

            // 2. Датчик цвета и яркости
            Renderer objectRenderer = hitInfo.collider.GetComponent<Renderer>();
            Color surfaceColor = Color.clear; // Цвет по умолчанию, если не удалось определить

            if (objectRenderer != null && objectRenderer.material != null)
            {
                Material material = objectRenderer.material;

                // Пытаемся получить цвет из текстуры
                if (material.mainTexture != null && hitInfo.collider is MeshCollider)
                {
                    Texture2D texture = material.mainTexture as Texture2D;
                    if (texture != null && texture.isReadable) // Текстура должна быть помечена как Read/Write Enabled в настройках импорта
                    {
                        Vector2 pixelUV = hitInfo.textureCoord;
                        surfaceColor = texture.GetPixelBilinear(pixelUV.x, pixelUV.y);
                    }
                    else
                    {
                        // Если текстура нечитаема или не Texture2D, используем основной цвет материала
                        surfaceColor = material.color;
                    }
                }
                else
                {
                    // Если нет текстуры, используем основной цвет материала
                    surfaceColor = material.color;
                }
            }
            else
            {
                // Если нет Renderer или материала, можно попробовать использовать цвет вершин,
                // но это сложнее. Пока оставим цвет по умолчанию (прозрачный).
                // Или можно установить какой-то стандартный цвет, например, серый.
                surfaceColor = Color.grey; // Пример
            }

            detectedColor = surfaceColor;

            // Рассчитываем яркость (Luminance)
            detectedBrightness = CalculateLuminance(surfaceColor);


            // 3. Датчик света
            // Проверяем, есть ли компонент Light на объекте, в который попал луч
            Light lightComponent = hitInfo.collider.GetComponent<Light>();
            isLightSourceDetected = (lightComponent != null && lightComponent.enabled);

        }
        else
        {
            // --- Объект не обнаружен в пределах maxDistance ---
            detectedDistance = maxDistance; // Можно использовать float.PositiveInfinity или maxDistance
            detectedColor = Color.clear;    // Нет цвета
            detectedBrightness = CalculateAmbientBrightness(); // Яркость равна окружающему освещению
            isLightSourceDetected = false;
            objectDetected = false;
        }
    }

    /// <summary>
    /// Рассчитывает воспринимаемую яркость (Luminance) цвета.
    /// </summary>
    /// <param name="color">Входной цвет.</param>
    /// <returns>Яркость в диапазоне [0, 1].</returns>
    float CalculateLuminance(Color color)
    {
        // Стандартная формула для расчета яркости (учитывает восприятие человеческим глазом)
        return 0.2126f * color.r + 0.7152f * color.g + 0.0722f * color.b;
    }

    /// <summary>
    /// Рассчитывает примерную яркость окружающего освещения.
    /// </summary>
    /// <returns>Яркость окружения в диапазоне [0, 1].</returns>
    float CalculateAmbientBrightness()
    {
        // Простой способ: использовать интенсивность окружающего света
        // Можно добавить и другие источники, например, основной направленный свет (RenderSettings.sun)
        return RenderSettings.ambientIntensity * CalculateLuminance(RenderSettings.ambientLight);
        // Более сложный вариант мог бы использовать Light Probes API для более точного локального освещения.
    }


    // --- Визуализация в редакторе ---
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 direction = transform.forward * detectedDistance;

        // Рисуем линию луча
        Gizmos.DrawRay(transform.position, direction);

        // Если что-то обнаружено, рисуем сферу в точке попадания
        if (objectDetected)
        {
            Gizmos.DrawWireSphere(transform.position + direction, 0.1f); // Маленькая сфера в точке контакта

            // Можно также отобразить обнаруженный цвет
#if UNITY_EDITOR
            UnityEditor.Handles.color = detectedColor;
            UnityEditor.Handles.Label(transform.position + direction + Vector3.up * 0.2f,
                $"Dist: {detectedDistance:F2}\nColor: {detectedColor}\nBright: {detectedBrightness:F2}\nLight: {isLightSourceDetected}");
#endif
        }
        else // Если ничего не обнаружено, рисуем до maxDistance
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawRay(transform.position, transform.forward * maxDistance);
        }
    }
}