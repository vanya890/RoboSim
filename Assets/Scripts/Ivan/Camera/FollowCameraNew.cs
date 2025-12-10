using UnityEngine;

/// <summary>
/// Камера, которая следует за указанным объектом с сохранением начального смещения
/// </summary>
public class FollowCameraNew : MonoBehaviour
{
    [Header("Настройки слежения")]
    [Tooltip("Объект, за которым следует камера")]
    [SerializeField] private Transform target;

    [Tooltip("Следить за вращением цели")]
    [SerializeField] private bool followRotation = false;

    [Tooltip("Направлять камеру на цель при вращении")]
    [SerializeField] private bool lookAtTarget = false;

    [Tooltip("Скорость следования камеры")]
    [Range(0.1f, 10f)]
    [SerializeField] private float followSpeed = 5f;

    [Tooltip("Скорость вращения камеры")]
    [Range(0.1f, 10f)]
    [SerializeField] private float rotationSpeed = 5f;

    [Tooltip("Плавность следования (0 - мгновенно, 1 - максимально плавно)")]
    [Range(0f, 1f)]
    [SerializeField] private float smoothness = 0.5f;
    
    [Tooltip("Полностью зафиксировать камеру на объекте (без плавности и задержек)")]
    [SerializeField] private bool rigidAttachment = false;

    [Header("Ограничения")]
    [Tooltip("Минимальная высота камеры над землей")]
    [SerializeField] private float minHeight = 1f;

    [Tooltip("Максимальное расстояние от цели")]
    [SerializeField] private float maxDistance = 50f;

    // Начальное смещение камеры относительно цели
    private Vector3 initialOffset;
    private Quaternion initialRotation;

    private void Start()
    {
        InitializeCamera();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Вычисляем целевую позицию
        Vector3 targetPosition = CalculateTargetPosition();

        // Применяем ограничения
        targetPosition = ApplyPositionConstraints(targetPosition);

        // Плавно перемещаем камеру
        MoveCamera(targetPosition);

        // Вычисляем и применяем целевое вращение
        Quaternion targetRotation = CalculateTargetRotation();
        RotateCamera(targetRotation);
    }

    /// <summary>
    /// Инициализация камеры при запуске
    /// </summary>
    private void InitializeCamera()
    {
        // Если цель не задана, пытаемся найти объект с тегом "Player"
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        // Запоминаем начальное смещение и поворот
        if (target != null)
        {
            initialOffset = transform.position - target.position;
            initialRotation = transform.rotation;
        }
        else
        {
            Debug.LogWarning("Цель для камеры не задана и не найдена объект с тегом 'Player'");
        }
    }

    /// <summary>
    /// Вычисление целевой позиции камеры
    /// </summary>
    /// <returns>Целевая позиция камеры</returns>
    private Vector3 CalculateTargetPosition()
    {
        if (followRotation)
        {
            // Если следуем за вращением, поворачиваем смещение вместе с целью
            // Это заставит камеру двигаться по окружности вокруг цели
            return target.position + target.rotation * initialOffset;
        }
        else
        {
            // Если не следуем за вращением, используем простое смещение
            return target.position + initialOffset;
        }
    }

    /// <summary>
    /// Применение ограничений к позиции камеры
    /// </summary>
    /// <param name="position">Исходная позиция</param>
    /// <returns>Позиция с примененными ограничениями</returns>
    private Vector3 ApplyPositionConstraints(Vector3 position)
    {
        // Применяем ограничение по высоте
        Vector3 constrainedPosition = position;
        constrainedPosition.y = Mathf.Max(position.y, minHeight);

        // Проверяем расстояние до цели
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > maxDistance)
        {
            // Если камера слишком далеко, возвращаем её в зону видимости
            Vector3 direction = (target.position - transform.position).normalized;
            constrainedPosition = target.position - direction * maxDistance;
        }

        return constrainedPosition;
    }

    /// <summary>
    /// Плавное перемещение камеры к целевой позиции
    /// </summary>
    /// <param name="targetPosition">Целевая позиция</param>
    private void MoveCamera(Vector3 targetPosition)
    {
        if (rigidAttachment)
        {
            // Полная фиксация - мгновенное перемещение без интерполяции
            transform.position = targetPosition;
        }
        else if (smoothness > 0f)
        {
            // Плавное перемещение
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
        else
        {
            // Перемещение без плавности
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Вычисление целевого вращения камеры
    /// </summary>
    /// <returns>Целевое вращение камеры</returns>
    private Quaternion CalculateTargetRotation()
    {
        if (followRotation)
        {
            if (lookAtTarget)
            {
                // Если следуем за вращением и нужно смотреть на цель
                Vector3 directionToTarget = target.position - transform.position;
                return Quaternion.LookRotation(directionToTarget, Vector3.up);
            }
            else
            {
                // Следуем за вращением цели, сохраняя начальную ориентацию относительно цели
                return target.rotation * initialRotation;
            }
        }
        else
        {
            // Используем начальный поворот
            return initialRotation;
        }
    }

    /// <summary>
    /// Плавное вращение камеры к целевому вращению
    /// </summary>
    /// <param name="targetRotation">Целевое вращение</param>
    private void RotateCamera(Quaternion targetRotation)
    {
        if (rigidAttachment)
        {
            // Полная фиксация - мгновенное вращение без интерполяции
            transform.rotation = targetRotation;
        }
        else if (smoothness > 0f)
        {
            // Плавное вращение
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            // Вращение без плавности
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    #region Публичные методы для управления камерой

    /// <summary>
    /// Установить новую цель для слежения
    /// </summary>
    /// <param name="newTarget">Новая цель</param>
    public void SetTarget(Transform newTarget)
    {
        if (newTarget != null)
        {
            target = newTarget;
            // Пересчитываем смещение для новой цели
            initialOffset = transform.position - target.position;
        }
    }

    /// <summary>
    /// Изменить начальное смещение камеры
    /// </summary>
    /// <param name="newOffset">Новое смещение</param>
    public void SetOffset(Vector3 newOffset)
    {
        initialOffset = newOffset;
    }

    /// <summary>
    /// Изменить скорость следования
    /// </summary>
    /// <param name="newSpeed">Новая скорость</param>
    public void SetFollowSpeed(float newSpeed)
    {
        followSpeed = Mathf.Max(0.1f, newSpeed);
    }

    /// <summary>
    /// Включить или выключить отслеживание вращения цели
    /// </summary>
    /// <param name="enabled">Следовать за вращением цели</param>
    public void SetFollowRotation(bool enabled)
    {
        followRotation = enabled;
    }

    /// <summary>
    /// Включить или выключить наведение камеры на цель
    /// </summary>
    /// <param name="enabled">Направлять камеру на цель</param>
    public void SetLookAtTarget(bool enabled)
    {
        lookAtTarget = enabled;
    }
    
    /// <summary>
    /// Включить или выключить режим полной фиксации камеры
    /// </summary>
    /// <param name="enabled">Зафиксировать камеру на объекте</param>
    public void SetRigidAttachment(bool enabled)
    {
        rigidAttachment = enabled;
    }

    /// <summary>
    /// Получить текущую цель слежения
    /// </summary>
    /// <returns>Текущая цель</returns>
    public Transform GetTarget()
    {
        return target;
    }

    /// <summary>
    /// Получить текущее смещение
    /// </summary>
    /// <returns>Текущее смещение</returns>
    public Vector3 GetOffset()
    {
        return initialOffset;
    }

    /// <summary>
    /// Получить текущее состояние следования за вращением
    /// </summary>
    /// <returns>Следует ли камера за вращением цели</returns>
    public bool GetFollowRotation()
    {
        return followRotation;
    }

    /// <summary>
    /// Получить текущее состояние наведения на цель
    /// </summary>
    /// <returns>Направлена ли камера на цель</returns>
    public bool GetLookAtTarget()
    {
        return lookAtTarget;
    }
    
    /// <summary>
    /// Получить текущее состояние полной фиксации
    /// </summary>
    /// <returns>Зафиксирована ли камера на объекте</returns>
    public bool GetRigidAttachment()
    {
        return rigidAttachment;
    }

    #endregion
}
