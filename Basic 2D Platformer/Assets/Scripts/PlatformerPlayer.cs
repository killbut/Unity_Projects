using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerPlayer : MonoBehaviour
{
    [SerializeField] private float _speed = 250.0f;
    [SerializeField] private float _jumpForce = 12.0f;
    private Rigidbody2D _body;
    private Animator _anim;
    private BoxCollider2D _box;
    
    // Start is called before the first frame update
    void Start()
    {
        _body = GetComponent<Rigidbody2D>(); // Нужно, чтобы к обьекту GameObject Был прикреплен этот второй комопнент
        _anim = GetComponent<Animator>();
        _box = GetComponent<BoxCollider2D>(); // Получаем этот компонент, чтобы использовать его как проверочную область для коллайдера персонажа.
    }

    // Update is called once per frame
    void Update()
    {
        float deltaX = Input.GetAxis("Horizontal") * _speed * Time.deltaTime;
        Vector2 movement = new Vector2(deltaX, _body.velocity.y); // Задаем только горизонтальное движение; Сохраняем задание вертикальное смещение
        _body.velocity = movement;
        Vector3 max = _box.bounds.max;
        Vector3 min = _box.bounds.min;
        Vector2 corner1 = new Vector2(max.x, min.y - 0.1f); 
        Vector2 corner2 = new Vector2(min.x, min.y - 0.2f);
        // Проверяем значение минимальной Y-координаты коллайдера
        Collider2D hit = Physics2D.OverlapArea(corner1, corner2);
        bool grounded = false;
        // Если под персонажем обнаружен коллайдер
        if (hit != null)
            grounded = true;
        _body.gravityScale = (grounded && Mathf.Approximately(deltaX, 0)) ? 0 : 1; // Остановка при нахождение на поверхности и в статичном состоянии
        if (grounded && Input.GetKeyDown(KeyCode.Space))
        {
            _body.AddForce(Vector2.up * _jumpForce,ForceMode2D.Impulse); // Сила добавляется только при нажатии клавиш пробела
        }

        MovingPlatform platform = null;
        if (hit != null)
        {
            platform = hit.GetComponent<MovingPlatform>(); // Проверяем, может ли двигаться платформа, находящаяся над персонажем
        }

        if (platform != null)
        {
            transform.parent = platform.transform; // Выполняем связывание с платформой 
        }
        else
        {
            transform.parent = null; // или очищаем переменую
        }
        _anim.SetFloat("speed",Mathf.Abs(deltaX)); // Скорость больше нуля даже при отрицательных значениях velocity
        
        Vector3 pScale=Vector3.one; // При нахождении вне движущейся платформы масштаб по умолчанию = 1
        if (platform != null)
        {
            pScale = platform.transform.localScale;
        }
        if (!Mathf.Approximately(deltaX ,0))
        {
            transform.localScale = new Vector3(Mathf.Sign(deltaX) / pScale.x, 1 / pScale.y, 1); // Замещаем существующее масштабирование новым кодом
        }
    }
}
