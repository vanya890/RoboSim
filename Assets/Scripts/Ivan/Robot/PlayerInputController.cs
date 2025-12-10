using UnityEngine;

/// <summary>
/// Простой контроллер для управления роботом с помощью ввода игрока
/// </summary>
public class PlayerInputController : MonoBehaviour
{
    [Header("Настройки управления")]
    [SerializeField] private RobotController robotController;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float rotationSpeed = 1f;

    private void Update()
    {
        if (robotController == null)
            return;

        // Получаем ввод от игрока
        float verticalInput = Input.GetAxis("Vertical") * moveSpeed;
        float horizontalInput = Input.GetAxis("Horizontal") * rotationSpeed;

        // Создаем вектор движения
        Vector2 movementInput = new Vector2(horizontalInput, verticalInput);

        // Отправляем команду движения роботу
        robotController.Move(movementInput);
    }
}
