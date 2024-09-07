using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;


public class CharecterController2D : MonoBehaviour
{
    [SerializeField] public bool playertype;   
    [SerializeField] public bool grounded;
    [SerializeField] public string surface;
    public List<Collider2D> currentTriggers = new List<Collider2D>();
    [SerializeField] Transform PushCheck;
    [SerializeField] public List<DynamicBox> dynamicBoxes = new List<DynamicBox>();
    // Start is called before the first frame update
    public int heath = 12;
    public int invincibilityTimer;
    [SerializeField] public float SpawnX;
    [SerializeField] public float SpawnY;
    Rigidbody2D m_rigidbody2D;
    [SerializeField] public float runSpeed = 40f;
    [SerializeField] public float jumpForce;
    private float horizontalMove = 0f;
    [SerializeField] public float windAmount;
    private Vector3 velocity = Vector3.zero;
    [Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;
    [Range(0, 2f)][SerializeField] private float m_IceMovementSmoothing = .3f;
    public bool OnHead;
    [SerializeField] float amountPush;
    [SerializeField] public LayerMask boxLayer;
    public bool m_FacingRight;
    public bool frozen;
    public float FrozenX;
    public float FrozenY;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] private float gradient;
    void OnTriggerEnter2D(Collider2D col)
    {

        // Add the GameObject collided with to the list.
        currentTriggers.Add(col);
        
    }

    void OnTriggerExit2D(Collider2D col)
    {

        // Remove the GameObject collided with from the list.
        currentTriggers.Remove(col);
        
    }
    void Awake()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckDeath();
        CheckImputs();
    }
    private void CheckImputs()
    {
        
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        Vector3 targetVelocity = new Vector2(horizontalMove * 10f + windAmount, m_rigidbody2D.velocity.y);
        if (horizontalMove > 0 && !m_FacingRight)

            {
            // ... flip the player.
            Flip();
        }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (horizontalMove < 0 && m_FacingRight)
        {
            // ... flip the player.
            Flip();
        }
        if (Input.GetButton("Jump") && grounded)
        {
            
            m_rigidbody2D.velocity = new Vector2(m_rigidbody2D.velocity.x, jumpForce);
            
        }
        if (!frozen)
        {
            if (surface == "Ground")
            {
                m_rigidbody2D.velocity = Vector3.SmoothDamp(m_rigidbody2D.velocity, targetVelocity, ref velocity, m_MovementSmoothing);
            }
            else if (surface == "Ice")
            {
                m_rigidbody2D.velocity = Vector3.SmoothDamp(m_rigidbody2D.velocity, targetVelocity, ref velocity, m_IceMovementSmoothing);
            }
        }
        if (playertype)
        {
            if (Input.GetButton("Push"))
            {
                Collider2D[] boxpush = Physics2D.OverlapCircleAll(PushCheck.position, 0.01f, boxLayer);
                for (int i = 0; i < boxpush.Length; i++)
                {
                    if (boxpush[i].gameObject != gameObject)
                    {
                        if (boxpush[i].gameObject.tag == "Invincible")
                        {
                            Vector2 push = new Vector2(amountPush * transform.localScale.x * -1, 0f);
                            boxpush[i].gameObject.GetComponent<Rigidbody2D>().AddForce(push);
                        }
                        else
                        {
                            boxpush[i].gameObject.SetActive(false);
                        }

                    }

                }
            }

        }
        else
        {
            if (Input.GetButtonDown("Freeze"))
            {
                if (!frozen)
                {
                    FrozenX = transform.position.x;
                    FrozenY = transform.position.y;
                }
                frozen = !frozen;

            }
        }
        if (frozen)
        {
            transform.position = new Vector2(FrozenX, FrozenY);
            m_rigidbody2D.gravityScale = 0f;
            m_rigidbody2D.velocity = Vector2.zero;
        }
        else
        {
            m_rigidbody2D.gravityScale = 3f;
        }
        
    }
    private void CheckDeath()
    {
        if (invincibilityTimer > 20)
        {
            gradient = 1f - (40f - invincibilityTimer) / 40f;
        }
        else if (invincibilityTimer > 0)
        {
            gradient = 0.5f + (20f - invincibilityTimer) / 40f;
        }
        if (invincibilityTimer > 0)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, gradient);
        }
        
        if (invincibilityTimer == 0)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        }    
        foreach (Collider2D gObject in currentTriggers)
        {
            
                if (gObject.gameObject.layer == 9)
                {
                    DeathReset();

                }
                else if (gObject.gameObject.layer == 8)
                {


                    Damage();
                }
            
            

        }
        invincibilityTimer--;
    }
    private void DeathReset()
    {
        heath = 12;
        invincibilityTimer = 40;
        frozen = false;
        ResetPlayers();
        
        ResetWorld();

    }
    private void ResetPlayers()
    {
        transform.position = new Vector2(SpawnX, SpawnY);
        m_rigidbody2D.velocity = Vector2.zero;
        
    }
    private void ResetWorld()
    {
        foreach (DynamicBox box in dynamicBoxes)
        {
            Debug.Log(box);
            box.gameobject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            box.gameobject.SetActive(true);
            box.gameobject.transform.position = new Vector2(box.Xposition, box.Yposition);

        }
    }
    private void Damage()
    {
        if (invincibilityTimer <= 0)
        {
            heath--;
        }
        if (heath == 0)
        {
            DeathReset();
        }
    }
    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        
        m_FacingRight = !m_FacingRight;
        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

}
