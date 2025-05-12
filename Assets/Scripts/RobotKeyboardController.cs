using UnityEngine;

/// <summary>
/// Управляет двухколесным роботом с помощью клавиатуры.
/// Использует компоненты RobotWheelController для левого и правого колес.
/// </summary>
public class RobotKeyboardController : MonoBehaviour
{
    [Header("Ссылки на колеса")]
    [Tooltip("Компонент управления левым колесом.")]
    public RobotWheelController leftWheelController;

    [Tooltip("Компонент управления правым колесом.")]
    public RobotWheelController rightWheelController;

    [Header("Параметры управления")]
    [Tooltip("Максимальная скорость движения вперед/назад (в процентах от maxTargetRPM колес).")]
    [Range(0f, 100f)]
    public float maxMoveSpeedPercentage = 80f;

    [Tooltip("Скорость поворота (в процентах от maxTargetRPM колес). Влияет на разницу скоростей колес при повороте.")]
    [Range(0f, 100f)]
    public float turnSpeedPercentage = 50f;

    [Header("Клавиши управления")]
    [Tooltip("Клавиша для сброса энкодеров колес.")]
    public KeyCode resetEncodersKey = KeyCode.R;

    // Приватные переменные для хранения текущих целевых скоростей
    private float currentLeftSpeedPercent = 0f;
    private float currentRightSpeedPercent = 0f;

    void Update()
    {
        // Проверяем, назначены ли контроллеры колес
        if (leftWheelController == null || rightWheelController == null)
        {
            Debug.LogError("Контроллеры колес (leftWheelController или rightWheelController) не назначены в инспекторе!");
            enabled = false; // Отключаем компонент, чтобы избежать ошибок в Update
            return;
        }

        // 1. Получаем ввод с осей "Vertical" и "Horizontal"
        // Обычно настроены на W/S/Up/Down и A/D/Left/Right соответственно
        // Значения находятся в диапазоне от -1 до 1
        float verticalInput = Input.GetAxis("Vertical");     // W/Up = 1, S/Down = -1
        float horizontalInput = Input.GetAxis("Horizontal"); // D/Right = 1, A/Left = -1

        // 2. Рассчитываем целевые скорости для каждого колеса (логика "танкового" управления)

        // Базовая скорость от движения вперед/назад
        float moveSpeed = verticalInput * maxMoveSpeedPercentage;

        // Модификатор скорости от поворота
        float turnSpeed = horizontalInput * turnSpeedPercentage;

        // Левое колесо: Движение вперед + Поворот влево (уменьшение скорости) / Поворот вправо (увеличение скорости)
        // Правое колесо: Движение вперед + Поворот влево (увеличение скорости) / Поворот вправо (уменьшение скорости)
        // Инвертируем поворот для одного из колес, чтобы получить эффект поворота
        currentLeftSpeedPercent = moveSpeed - turnSpeed;
        currentRightSpeedPercent = moveSpeed + turnSpeed;

        // 3. Ограничиваем итоговые скорости максимальным значением
        // Это предотвращает превышение 100% (или maxMoveSpeedPercentage), если одновременно нажаты W и A/D
        // Используем общий максимум для сохранения пропорций поворота, если возможно
        float maxMagnitude = Mathf.Max(Mathf.Abs(currentLeftSpeedPercent), Mathf.Abs(currentRightSpeedPercent));
        if (maxMagnitude > maxMoveSpeedPercentage) // Если хотя бы одно колесо превышает лимит
        {
            float scale = maxMoveSpeedPercentage / maxMagnitude;
            currentLeftSpeedPercent *= scale;
            currentRightSpeedPercent *= scale;
        }

        // Можно также жестко ограничить каждое колесо отдельно, если предыдущая логика не подходит:
        // currentLeftSpeedPercent = Mathf.Clamp(currentLeftSpeedPercent, -maxMoveSpeedPercentage, maxMoveSpeedPercentage);
        // currentRightSpeedPercent = Mathf.Clamp(currentRightSpeedPercent, -maxMoveSpeedPercentage, maxMoveSpeedPercentage);


        // 4. Отправляем команды на контроллеры колес
        leftWheelController.SetSpeedPercentage(currentLeftSpeedPercent);
        rightWheelController.SetSpeedPercentage(currentRightSpeedPercent);

        // 5. Проверяем нажатие клавиши сброса энкодеров
        if (Input.GetKeyDown(resetEncodersKey))
        {
            leftWheelController.ResetEncoder();
            rightWheelController.ResetEncoder();
            Debug.Log("Encoders reset by keyboard command.");
        }
    }

    // Можно добавить визуализацию в редакторе для отладки
    void OnDrawGizmosSelected()
    {
        // Показываем текущие целевые скорости (если приложение запущено)
        if (Application.isPlaying)
        {
#if UNITY_EDITOR
            string debugText = $"Target Speed L: {currentLeftSpeedPercent:F1}%\nTarget Speed R: {currentRightSpeedPercent:F1}%";
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, debugText); // Показываем текст над объектом
#endif
        }
    }
}