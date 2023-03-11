using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditorInternal;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        GroundcheckRadius = GroundCheck.GetComponent<CircleCollider2D>().radius;
        WallCheCkRadiusUp = WallCheckUp.GetComponent<CircleCollider2D>().radius;
        WallCheCkRadiusDown = WallCheckDown.GetComponent<CircleCollider2D>().radius;
        gravityDef = rb.gravityScale;
    }

    void Update()
    {
        Walk();
        Jump();
        Retlect();
        CheckingGround();
        CheckingWall();
        MoveOnWall();
        Walljump();
    }
    public Vector2 moveVector;
    public float speed = 3;
    void Walk()
    {
        moveVector.x = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveVector.x * speed, rb.velocity.y);
        anim.SetFloat("moveX", Mathf.Abs(moveVector.x));
    }

    public bool faceRight = true;
    void Retlect()
    {
        if ((moveVector.x > 0 && !faceRight) || (moveVector.x < 0 && faceRight))
        {
            transform.localScale *= new Vector2(-1, 1);
            faceRight = !faceRight;
        }
    }

    private bool jumpControl = true;
    public int jumpForce = 7;
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Physics2D.IgnoreLayerCollision(9, 10, true);
            Invoke("IgnoreLayerOff", 0.5f);
            jumpControl = false;
        }

        if (jumpControl)
        {
            if (Input.GetKeyDown(KeyCode.Space) && onGround)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(Vector2.up * jumpForce * 50);
            }

            if (rb.velocity.y == 0) { anim.SetBool("zeroVelocityY", true); }
            else { anim.SetBool("zeroVelocityY", false); }
        }
    }

    public bool onGround;
    public LayerMask Ground;
    public Transform GroundCheck;
    public float GroundcheckRadius;
    void CheckingGround()
    {
        onGround = Physics2D.OverlapCircle(GroundCheck.position, GroundcheckRadius, Ground);
        anim.SetBool("onGround", onGround);
    }

    public bool onWall;
    public LayerMask Wall;
    public Transform WallCheckUp;
    public Transform WallCheckDown;
    private float WallCheCkRadiusUp;
    private float WallCheCkRadiusDown;
    void CheckingWall()
    {
        onWall = (Physics2D.OverlapCircle(WallCheckUp.position, WallCheCkRadiusUp, Wall) && Physics2D.OverlapCircle(WallCheckDown.position, WallCheCkRadiusDown, Wall));
        anim.SetBool("onWall", onWall);
    }
    public float upDownSpeed = 2f;
    public float slideSpeed = 0;
    private float gravityDef;

    void MoveOnWall()
    {
        if (onWall && !onGround)
        {
            moveVector.y = Input.GetAxisRaw("Vertical");
            anim.SetFloat("UpDown", moveVector.y);
            anim.StopPlayback();
            anim.Play("UpDown");
            if (moveVector.y == 0)
            {
                rb.gravityScale = 0;
                rb.velocity = new Vector2(0, slideSpeed);
            }
            if (moveVector.y != 0) { rb.velocity = new Vector2(rb.velocity.x, moveVector.y * upDownSpeed); }
        }
        else if (!onGround && !onWall) { rb.gravityScale = gravityDef; }
    }

    private bool blockMoveX;
    public float jumpWallTime = 0.5f;
    private float timerJumpWall;
    public Vector2 jumpAngle = new Vector2(3.5f, 10);

    void Walljump()
    {
        if (onWall && !onGround && Input.GetKeyDown(KeyCode.Space))
        {
            transform.localScale *= new Vector2(-1, 1);
            faceRight = !faceRight;
            rb.velocity = new Vector2(transform.localScale.x * jumpAngle.x, jumpAngle.y);
        }
    }
}
