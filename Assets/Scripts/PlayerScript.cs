using Mirror;
using UnityEngine;
using TMPro;
using System;

public class PlayerScript : NetworkBehaviour
{
    [SerializeField]
    private float maxHealth = 100f;
    [SyncVar(hook = nameof(OnHPCanged))]
    [SerializeField]
    private float curHealth;
    [SyncVar(hook = nameof(OnDeathCountCanged))]
    private int dethCount = 0;

    //Для первичной реализации проклятия(горения)
    private float curseDamage = 5;
    private float curseCount = 0;
    private float lastCurseDamageTime;

    [SerializeField]
    private GameObject fireball;

    [SerializeField]
    private GameObject firePoint;

    [SerializeField]
    private GameObject camHolder;

    Rigidbody rb;

    public TMP_Text playerHPText;
    public GameObject floatingInfo;
    
    private Material playerMaterialClone;

    [SyncVar(hook = nameof(OnColorChanged))]
    public Color playerColor = Color.white;

    bool grounded = true;
    private bool cursorLock = false;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float sensX;
    [SerializeField]
    private float sensY;

    float camHolderAngle;

    float jumpForce = 25;

    void OnColorChanged(Color _Old, Color _New)
    {
        playerMaterialClone = new Material(GetComponent<Renderer>().material);
        playerMaterialClone.color = _New;
        GetComponent<Renderer>().material = playerMaterialClone;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        curHealth = maxHealth;     
    }

    public override void OnStartLocalPlayer()
    {
        //Установка камеры
        Camera.main.transform.SetParent(camHolder.transform);
        Camera.main.transform.localPosition = new Vector3(0, 0, 0);

        floatingInfo.transform.localPosition = new Vector3(0, 1.5f, 0);
        floatingInfo.transform.localScale = new Vector3(-0.1f, 0.1f, 0.1f);

        string name = "Player" + UnityEngine.Random.Range(100, 999);
        Color color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        CmdSetupPlayer(color);
    }

    [Command]
    public void CmdSetupPlayer(Color _col)
    {
        playerColor = _col;
    }

        
    void Update()
    {
        playerHPText.text = ((int)curHealth).ToString();


        if (isServer && curseCount > 0)
        {
            if(Time.time > lastCurseDamageTime)
            {
                lastCurseDamageTime = Time.time + 1;
                curseCount--;
                curHealth -= curseDamage;
            }
        }

        if (!isLocalPlayer)
        {
            floatingInfo.transform.LookAt(Camera.main.transform);
            return;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(moveX, 0, moveZ).normalized * speed * Time.deltaTime;

        camHolderAngle += Input.GetAxisRaw("Mouse Y") * sensY;

        camHolderAngle = Mathf.Clamp(camHolderAngle, -90f, 90f);

        camHolder.transform.localEulerAngles = Vector3.left * camHolderAngle;

        transform.Rotate(0, Input.GetAxisRaw("Mouse X") * sensX, 0);
        transform.Translate(moveDir);
        

        if(Input.GetKey(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }

        //Пуск огненного шара
        if (Input.GetButtonDown("Fire1"))
        {
            CmdFire();
        }

        // Взаимодействие с предметами
        if (Input.GetButtonDown("Fire2"))
        {
                
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            } 
            else if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void SetGrounded(bool _grounded)
    {
        grounded = _grounded;
    }

    [Command]
    void CmdFire()
    {
        GameObject projectile = Instantiate(fireball, firePoint.transform.position, firePoint.transform.rotation);
        NetworkServer.Spawn(projectile);
    }


    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<FireBall>() != null)
        {
            lastCurseDamageTime = Time.time + 1;
            curseCount = 5;

            curHealth -= 20;    
        }
    }

    void OnHPCanged(float _Old, float _New)
    {
        if (curHealth < 0)
        {
            curHealth = 0;
        }

        if (curHealth == 0)
        {
            dethCount++;
        }
    }

    void OnDeathCountCanged(int _Old, int _New)
    {
        curHealth = maxHealth;
        curseCount = 0;

        this.transform.position = NetworkManager.startPositions[UnityEngine.Random.Range(0, NetworkManager.startPositions.Count)].position;    
    }
}

