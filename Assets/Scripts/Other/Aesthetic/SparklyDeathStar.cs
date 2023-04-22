using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using BeriUtils.Core;
public class SparklyDeathStar : MonoBehaviour
{    
    public float localGroundYCoordinate; // <-- where in the world their current "ground floor" is 
    public int maxGroundBounces = 5;
    public float randRotation;
    [Range(0.1f,50.0f)] public float moveSpeed = 10.0f;
    public int _groundBounces = 0;
    float _gravity = 9.81f;
    float _backgroundLightKill = 0;

    Vector3 _internalPosition = new Vector3();    
    Vector3 _internalVelocity = new Vector3();
    Color RandomColor = new Color();
    [SerializeField][Range(0f,5.0f)] float _gravityScale = 2.0f;
    [SerializeField] GameObject lightObject;


    void Awake()
    {
        int whichColor = Random.Range(0,7);

        float randCol = Random.Range(0f,1f);
        float randX = Random.Range(-2f,2f);
        float randY = Random.Range(2f,3f);
        float randSize = Random.Range(0.5f,1.5f);
        
        randRotation = -randX * 3;

        switch(whichColor)
        {
            case 0 :
                RandomColor = new Color(1,randCol,0,1);
                break;
            case 1 :
                RandomColor = new Color(0,1,randCol,1);
                break;
            case 2 :
                RandomColor = new Color(randCol,0,1,1);
                break;
            case 3 :
                RandomColor = new Color(randCol,1,0,1);
                break;
            case 4 :
                RandomColor = new Color(0,randCol,1,1);
                break;
            case 5 :
                RandomColor = new Color(1,0,randCol,1);
                break;
            case 6 :
                RandomColor = new Color(1,0,randCol,1);
                break;
            case 7 :
                RandomColor = new Color(randCol,0,1,1);
                break;
        }

        GetComponent<Light2D>().color = RandomColor;
        GetComponent<SpriteRenderer>().color = RandomColor;
        LaunchStar(new Vector2(randX,randY));

        transform.localScale *= randSize;
    }

    void FixedUpdate()
    {
        if (CentralManager.GetStateManager())
        {
            if (CentralManager.CurrentContext == CentralManager.Context.OVERWORLD)
                Destroy(gameObject);
        }
        GetState();
        UpdateVelocity();
        CheckCollisions();
        SetState();
    }
    void GetState()
    {
        transform.Rotate(0,0,randRotation,Space.Self);
        _backgroundLightKill += Time.deltaTime;
        if (_backgroundLightKill >= 5 * Time.deltaTime)
            Destroy(lightObject);
        _internalPosition = transform.position;
    }
    void SetState()
    {
        _internalPosition += _internalVelocity;
        transform.position = _internalPosition;
        GetComponent<SpriteRenderer>().color = RandomColor;
    }
    void CheckCollisions() // <-- "collisions" in this case is just "are we above our ground coordinate?"
    {
        if ((transform.position.y + _internalVelocity.y) < localGroundYCoordinate)
        {
            
            _internalVelocity.y = -_internalVelocity.y * 0.5f;
            _internalVelocity.x *= 0.5f;
            _groundBounces++;
            RandomColor.a = 1 - (0.2f * _groundBounces);
            if (_groundBounces >= maxGroundBounces)
                Destroy(gameObject);
        }
    }
    void UpdateVelocity()
    {
        _internalVelocity.x = BeriMath.Accelerate(_internalVelocity.x,0,2.0f,Time.fixedDeltaTime);
        _internalVelocity.y = BeriMath.Accelerate(_internalVelocity.y,-5.0f,_gravity*_gravityScale,Time.fixedDeltaTime);
    }
    public void LaunchStar(Vector2 inputVelocity)
    {
        _internalVelocity = inputVelocity/(10.0f);
    }
}
