using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using PlayerIF;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    private Rigidbody2D rb2d;
    private BoxCollider2D boxCol2d;
    private List<char> stackMove = new List<char>();
    private PlayerItem playerItem;
    
    [SerializeField]
    private float speed, fJump;
    private int jumpTime, dodgeTime;
    private bool isGround, isWall;
    public Animator animator;
    public GameObject header;
    void Awake(){
        if (instance == null) instance = this;
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        boxCol2d = gameObject.GetComponent<BoxCollider2D>();
        jumpTime = 3;
    }
    void Start(){
        isWall = true;
        isWall = false;
        if (GameManagement.instance != null){
            playerItem = GameManagement.instance.currentItem;
        }
        ChangeHeader();
    }
    void FixedUpdate(){
        if (!GameController.instance.isGameOver) if (!isWall) Move();
        Gravity();
    }
    private void ChangeHeader(){
        header.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Imgs/TexturePlayer/"+playerItem.texture);
    }
    private void Gravity(){
        rb2d.velocity = new Vector2(rb2d.velocity.x,rb2d.velocity.y-9.8f*Time.deltaTime);
    }
    private void Move(){
        if (stackMove.Count() == 0) {
            // idle
            animator.SetTrigger("Idle");
        } else {
            animator.SetTrigger("Run");
            if (stackMove[stackMove.Count() - 1] == 'L'){ // Jump left
                rb2d.velocity = new Vector2( -speed, rb2d.velocity.y);
            } else
                if (stackMove[stackMove.Count() - 1] == 'R'){ // Jump Right
                    rb2d.velocity = new Vector2( speed, rb2d.velocity.y);
                }
        }        
    }
    public void PushMove(char move){
        stackMove.Add(move);
    }

    public void PopMove(char move){
        stackMove.Remove(move);
    }

    public void Jump(){
        if (jumpTime>0){
            jumpTime--;
            rb2d.velocity = new Vector2(rb2d.velocity.x, fJump);
        }
    }
    public void Dodge(){
        if (dodgeTime == 0) {
            animator.SetTrigger("Dodge");
            StartCoroutine(TurnOffCollision());
            StartCoroutine(DodgeCountDown());
        }
    }

    IEnumerator DodgeCountDown(){
        dodgeTime = 4;
        yield return new WaitForSeconds((float) dodgeTime);
        dodgeTime = 0;
    }
    IEnumerator TurnOffCollision(){
        Physics2D.IgnoreLayerCollision(6,7,true);
        yield return new WaitForSeconds(0.85f);
        Physics2D.IgnoreLayerCollision(6,7,false);
    }
    void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.tag == "ground") {
            jumpTime = 3;
        }
        if (other.gameObject.tag == "Wall"){
            isWall = true;
        }
    }
    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Wall"){
            isWall = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "VOB"){
            rb2d.freezeRotation = false;
            if (GameController.instance != null) GameController.instance.OnGameOver();
        }
    }
}
