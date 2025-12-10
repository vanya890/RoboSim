using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Сенсор камеры, захватывающий изображение и публикующий его в RenderTexture
/// </summary>
public class CameraSensor : SensorBase
{
    [Header("Настройки камеры")]
    [Tooltip("Камера для захвата изображения")]
    [SerializeField] private Camera targetCamera = null;

    [Tooltip("Ширина изображения в пикселях")]
    [SerializeField] private int imageWidth = 640;

    [Tooltip("Высота изображения в пикселях")]
    [SerializeField] private int imageHeight = 480;

    [Header("Настройки захвата")]
    [Tooltip("Частота кадров захвата (FPS)")]
    [SerializeField] private int captureFPS = 30;

    [Header("Настройки рендеринга")]
    [Tooltip("Формат текстуры рендера")]
    [SerializeField] private RenderTextureFormat renderTextureFormat = RenderTextureFormat.Default;

    [Tooltip("Глубина буфера")]
    [SerializeField] private int depthBuffer = 24;

    [Tooltip("Создавать mip-карты")]
    [SerializeField] private bool useMipMaps = false;

    [Tooltip("Включить антиалиасинг")]
    [SerializeField] private int antiAliasing = 0;

    [Header("Отладка")]
    [Tooltip("Копия текстуры рендера для отладки")]
    public RenderTexture debugRenderTexture;

    // Компоненты
    private RenderTexture renderTexture;

    // Последний захваченный кадр
    private Texture2D lastFrame;
    private byte[] lastFrameBytes;

    // Флаг, указывающий, был ли изменен кадр
    private bool frameChanged = false;

    // Таймер для контроля FPS
    private float frameTimer = 0f;
    private float frameInterval = 0f;

    // Переопределяем метод Update для работы с FPS вместо updateInterval
    private new void Update()
    {
        if (!isRunning)
            return;

        // Проверяем, прошло ли достаточно времени для обновления на основе FPS
        frameTimer += Time.deltaTime;
        if (frameTimer >= frameInterval)
        {
            frameTimer = 0f;

            // Получаем данные от сенсора
            object data = ReadSensorData();

            // Уведомляем подписчиков через базовый класс
            TriggerDataUpdate(data);
        }
    }

    protected override void OnStart()
    {
        // Устанавливаем имя по умолчанию, если оно не было установлено
        if (string.IsNullOrEmpty(sensorName))
        {
            sensorName = "CameraSensor";
        }

        // Вычисляем интервал между кадрами на основе FPS
        frameInterval = 1f / captureFPS;

        InitializeCamera();
    }

    protected override void OnStop()
    {
        CleanupResources();
    }

    /// <summary>
    /// Инициализация камеры и текстуры рендера
    /// </summary>
    private void InitializeCamera()
    {
        // Проверяем, указана ли камера
        if (targetCamera == null)
        {
            // Если камера не указана, ищем компонент на том же объекте
            targetCamera = GetComponent<Camera>();

            // Если и там нет, выводим предупреждение
            if (targetCamera == null)
            {
                Debug.LogWarning("Камера не указана и не найдена на объекте. Сенсор не будет работать.");
                return;
            }
        }

        // Создаем текстуру рендера
        renderTexture = new RenderTexture(imageWidth, imageHeight, depthBuffer, renderTextureFormat);
        renderTexture.useMipMap = useMipMaps;
        renderTexture.antiAliasing = antiAliasing;
        renderTexture.Create();

        // Сохраняем оригинальную текстуру рендера камеры
        RenderTexture originalTargetTexture = targetCamera.targetTexture;

        // Назначаем нашу текстуру рендера камере
        targetCamera.targetTexture = renderTexture;

        // Создаем текстуру для чтения данных
        lastFrame = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);
    }

    /// <summary>
    /// Очистка ресурсов
    /// </summary>
    private void CleanupResources()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
            renderTexture = null;
        }

        if (targetCamera != null && targetCamera.targetTexture == renderTexture)
        {
            targetCamera.targetTexture = null;
        }

        if (lastFrame != null)
        {
            Destroy(lastFrame);
            lastFrame = null;
        }
    }

    /// <summary>
    /// Читает данные с сенсора камеры
    /// </summary>
    /// <returns>Данные сенсора</returns>
    protected override object ReadSensorData()
    {
        if (targetCamera == null || renderTexture == null)
        {
            return new CameraSensorData
            {
                width = imageWidth,
                height = imageHeight,
                imageData = null,
                hasData = false
            };
        }

        // Устанавливаем активную текстуру рендера
        RenderTexture.active = renderTexture;

        // Читаем пиксели
        lastFrame.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
        lastFrame.Apply();

        // Возвращаем активную текстуру рендера
        RenderTexture.active = null;

        // Создаем копию для отладки, если нужно
        if (debugRenderTexture != null)
        {
            Graphics.Blit(renderTexture, debugRenderTexture);
        }

        // Преобразуем в байты (PNG)
        lastFrameBytes = lastFrame.EncodeToPNG();
        frameChanged = true;

        // Возвращаем данные
        return new CameraSensorData
        {
            width = imageWidth,
            height = imageHeight,
            imageData = lastFrameBytes,
            hasData = true
        };
    }

    /// <summary>
    /// Получить последнее захваченное изображение
    /// </summary>
    /// <returns>Текстура2D с последним кадром</returns>
    public Texture2D GetLastFrame()
    {
        return lastFrame;
    }

    /// <summary>
    /// Получить последнее захваченное изображение в виде байтов
    /// </summary>
    /// <returns>Массив байтов изображения</returns>
    public byte[] GetLastFrameBytes()
    {
        return lastFrameBytes;
    }

    /// <summary>
    /// Проверить, был ли изменен кадр с последнего чтения
    /// </summary>
    /// <returns>True, если кадр был изменен</returns>
    public bool HasFrameChanged()
    {
        bool changed = frameChanged;
        frameChanged = false; // Сбрасываем флаг после чтения
        return changed;
    }

    /// <summary>
    /// Установить разрешение изображения
    /// </summary>
    /// <param name="width">Ширина</param>
    /// <param name="height">Высота</param>
    public void SetResolution(int width, int height)
    {
        if (width > 0 && height > 0)
        {
            imageWidth = width;
            imageHeight = height;

            // Пересоздаем ресурсы с новым разрешением
            CleanupResources();
            InitializeCamera();
        }
    }

    /// <summary>
    /// Получить текущее разрешение
    /// </summary>
    /// <returns>Ширина и высота изображения</returns>
    public Vector2Int GetResolution()
    {
        return new Vector2Int(imageWidth, imageHeight);
    }

    /// <summary>
    /// Установить целевую камеру
    /// </summary>
    /// <param name="camera">Новая целевая камера</param>
    public void SetTargetCamera(Camera camera)
    {
        if (camera != targetCamera)
        {
            // Очищаем ресурсы для старой камеры
            CleanupResources();

            // Устанавливаем новую камеру
            targetCamera = camera;

            // Инициализируем ресурсы для новой камеры
            InitializeCamera();
        }
    }

    /// <summary>
    /// Получить целевую камеру
    /// </summary>
    /// <returns>Текущая целевая камера</returns>
    public Camera GetTargetCamera()
    {
        return targetCamera;
    }

    /// <summary>
    /// Установить частоту кадров захвата
    /// </summary>
    /// <param name="fps">Новая частота кадров</param>
    public void SetCaptureFPS(int fps)
    {
        if (fps > 0)
        {
            captureFPS = fps;
            frameInterval = 1f / captureFPS;
        }
    }

    /// <summary>
    /// Получить частоту кадров захвата
    /// </summary>
    /// <returns>Текущая частота кадров</returns>
    public int GetCaptureFPS()
    {
        return captureFPS;
    }

    /// <summary>
    /// Получить текстуру рендера
    /// </summary>
    /// <returns>Текущая текстура рендера</returns>
    public RenderTexture GetRenderTexture()
    {
        return renderTexture;
    }

    /// <summary>
    /// Структура для хранения данных сенсора камеры
    /// </summary>
    public struct CameraSensorData : ISensorData
    {
        public int width;
        public int height;
        public byte[] imageData;
        public bool hasData;

        /// <summary>
        /// Получить словарь с данными сенсора в формате ключ-значение
        /// </summary>
        /// <returns>Словарь с данными</returns>
        public Dictionary<string, object> GetData()
        {
            var data = new Dictionary<string, object>();
            data["width"] = width;
            data["height"] = height;
            data["hasData"] = hasData;

            if (hasData && imageData != null)
            {
                data["imageSize"] = imageData.Length;
            }

            return data;
        }
    }
}
