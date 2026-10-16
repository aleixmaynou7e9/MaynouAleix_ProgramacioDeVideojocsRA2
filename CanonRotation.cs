using UnityEngine;
using UnityEngine.InputSystem;

public class CanonRotation : MonoBehaviour , InputSystem_Actions.ICanonActions
{
    private Vector2 mousePosition;
    public Vector3 _maxRotation;
    public Vector3 _minRotation;
    private float _offset = -51.6f;
    public GameObject ShootPoint;
    public GameObject Bullet;
    public float ProjectileSpeed = 0;
    public float MaxSpeed;
    public float MinSpeed;
    public GameObject PotencyBar;
    private float _initialScaleX;
    private Vector2 _distanceBetweenMouseAndPlayer;
    private bool isRaising = false;
    private InputSystem_Actions inputActions;
    [SerializeField] private float _multiplier = 10f;    
    private void Awake()
    {
        _initialScaleX = PotencyBar.transform.localScale.x;
        inputActions = new InputSystem_Actions();
        inputActions.Canon.SetCallbacks(this);
    }

	public void OnEnable()
	{
		inputActions.Enable();
	}

	public void OnDisable()
	{
		inputActions.Disable();
	}

	void Update()
    {
        Vector2 mousePos = mousePosition; //obtenir el valor del click del cursor (Fer amb new input system)
        _distanceBetweenMouseAndPlayer = mousePos - (Vector2) transform.position; //obtenir el vector distància entre el canó i el cursor
        var ang = (Mathf.Atan2(_distanceBetweenMouseAndPlayer.y, _distanceBetweenMouseAndPlayer.x) * 180f / Mathf.PI + _offset);
        if (ang < -66)
        {
            ang = -66;
        }
        else if (ang > 40) 
        {
            ang = 40;
        }
        transform.rotation = Quaternion.Euler(0,0,ang); 

        if (isRaising)
        {
            ProjectileSpeed = Time.deltaTime * _multiplier + ProjectileSpeed; //acotar entre dos valors (mirar variables)
            CalculateBarScale();
        }
        
        CalculateBarScale();

    }
    public void CalculateBarScale()
    {
        PotencyBar.transform.localScale = new Vector3(Mathf.Lerp(0, _initialScaleX, ProjectileSpeed / MaxSpeed),
            transform.localScale.y,
            transform.localScale.z);

        if (!isRaising) 
        {
			_initialScaleX = PotencyBar.transform.localScale.x;
		}
    }
    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.performed) 
        {
			mousePosition =Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
        }
    }

    public void OnShoot(InputAction.CallbackContext context) 
    {
		if (context.started)
		{
			isRaising = true;
		}
		if (context.canceled)
		{
			var projectile = Instantiate(Bullet, transform.position, Quaternion.identity); //canviar la posició on s'instancia
			projectile.GetComponent<Rigidbody2D>().linearVelocity = ProjectileSpeed * _distanceBetweenMouseAndPlayer;
			ProjectileSpeed = 0f;
			isRaising = false;
		}
	}
}
