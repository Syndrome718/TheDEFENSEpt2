using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    public static event Action<EnemyData> OnEnemyReachedEnd;
    public static event Action<Enemy> OnEnemyDestroyed;

    [SerializeField] private Path currentPath;

    private Vector3 _targetPosition;
    private int _currentWaypoint;
    private float _lives;

    [SerializeField] private Transform healthBar;
    private Vector3 _healthBarOriginalScale;

    private void Awake()
    {
        currentPath = GameObject.Find("Path1").GetComponent<Path>();
        _healthBarOriginalScale = healthBar.localScale;
    }

    private void OnEnable()
    {
        _currentWaypoint = 0;
        _targetPosition = currentPath.GetPosition(_currentWaypoint);
        _lives = data.lives;
        UpdateHealthBar();
    }

    void Update()
    {
        // move towards target position
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, data.speed * Time.deltaTime);

        // when target reached, set new target position
        float relativeDistance = (transform.position - _targetPosition).magnitude;
        if (relativeDistance < 0.1f)
        {
            if (_currentWaypoint < currentPath.Waypoints.Length - 1)
            {
                _currentWaypoint++;
                _targetPosition = currentPath.GetPosition(_currentWaypoint);
            }
            else
            {
                OnEnemyReachedEnd?.Invoke(data);
                gameObject.SetActive(false);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        _lives -= damage;
        _lives = Math.Max(_lives, 0);
        UpdateHealthBar();

        if (_lives <= 0)
        {
            OnEnemyDestroyed?.Invoke(this);
            gameObject.SetActive(false);
        }
    }

    private void UpdateHealthBar()
    {
        float healthPercent = _lives / data.lives;
        Vector3 scale = _healthBarOriginalScale;
        scale.x = _healthBarOriginalScale.x * healthPercent;
        healthBar.localScale = scale;
    }
}
