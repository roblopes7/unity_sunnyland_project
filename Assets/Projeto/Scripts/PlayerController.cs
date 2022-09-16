using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
public class PlayerController : MonoBehaviour
{
    private Animator playerAnimator;
    private Rigidbody2D playerRigidBody;
    private SpriteRenderer srPlayer;
    private bool playerInvencivel;

    public GameObject playerDie;

    public Transform groundCheck;
    public bool isGround = false;

    public float speed;

    public float touchRun = 0.0f;

    public bool facingRight = true;

    public int vidas = 3;
    public Color hitColor;
    public Color noHitColor;

    public bool jump = false;
    public int numberJump = 0;
    public int maxJumps = 2;
    public float jumpForce;

    private ControllerGame _controllerGame;

    public AudioSource fxGame;
    public AudioClip fxPulo;

    public ParticleSystem _poeira;

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerRigidBody = GetComponent<Rigidbody2D>();

        srPlayer = GetComponent<SpriteRenderer>();

        _controllerGame = FindObjectOfType (typeof(ControllerGame)) as ControllerGame;
    }


    // Update is called once per frame
    void Update()
    {
        isGround = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        playerAnimator.SetBool("isGrounded", isGround);

        touchRun = Input.GetAxisRaw("Horizontal");
        if(Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
    }

    void MovePlayer(float movimentoH)
    {
        playerRigidBody.velocity = new Vector2(movimentoH * speed, playerRigidBody.velocity.y);

        if(movimentoH < 0 && facingRight || (movimentoH > 0 && !facingRight))
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer(touchRun);
        SetaMovimentos();

        if(jump)
        {
            JumpPlayer();
        }
    }

    void Flip()
    {
        CriarPoeira();
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = new Vector3(theScale.x, transform.localScale.y, transform.localScale.z);
        //transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

    }

    void SetaMovimentos()
    {
        playerAnimator.SetBool("Walk", playerRigidBody.velocity.x != 0);
        playerAnimator.SetBool("Jump", !isGround);
    }

    void JumpPlayer()
    {
        if(isGround)
        {
            numberJump = 0;
            CriarPoeira();
        }

        if(isGround || numberJump < maxJumps)
        {
            playerRigidBody.AddForce(new Vector2(0f, jumpForce));
            isGround = false;
            numberJump++;

            //Som pulo
            fxGame.PlayOneShot(fxPulo);
            CriarPoeira();
        }
        jump = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Coletaveis":
                _controllerGame.Pontuacao(1);
                Destroy(collision.gameObject);
                break;
            case "Inimigo":

                //instanciar animação de explosão
                GameObject tempExplosao = Instantiate(_controllerGame.hitPrefab, transform.position, transform.localRotation);
                Destroy(tempExplosao, 0.5f);

                //jogado pára cima ao pular no inimigo
                Rigidbody2D rb = GetComponentInParent<Rigidbody2D>();
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(new Vector2(0, 900));

                //Som da explosão
                _controllerGame.fxGame.PlayOneShot(_controllerGame.fxExplosao);
                    

                Destroy(collision.gameObject);
                break;
            case "Damage":
                Hurt();
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch(collision.gameObject.tag)
        {
            case "Inimigo":
                Hurt();
                break;
            case "Plataforma":
                this.transform.parent = collision.transform;
                break;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Plataforma":
                this.transform.parent = null;
                break;
        }

    }

    void Hurt()
    {
        if(!playerInvencivel)
        {
            playerInvencivel = true;
            vidas--;
            StartCoroutine(Dano());
            _controllerGame.BarraVida(vidas);

            if(vidas < 1)
            {
                GameObject pDieTemp = Instantiate(playerDie, transform.position, Quaternion.identity);
                Rigidbody2D rbDie = pDieTemp.GetComponent<Rigidbody2D>();
                rbDie.AddForce(new Vector2(150f, 500f));

                _controllerGame.fxGame.PlayOneShot(_controllerGame.fxDie);
                Invoke("CarregaOJogo", 4f);
                gameObject.SetActive(false);
            }
        }
    }

    void CarregaOJogo()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator Dano()
    {
        srPlayer.color = noHitColor;
        yield return new WaitForSeconds(0.1f);
        for(float i = 0; i < 1; i += 0.1f)
        {
            srPlayer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            srPlayer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
        srPlayer.color = Color.white;
        playerInvencivel = false;
    }

    void CriarPoeira()
    {
        _poeira.Play();
    }
}
