using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platercontroller : MonoBehaviour
{
    // Start is called before the first frame update
    private CharacterController controller;
    private Vector3 direction;
    public float forwardSpeed;
    public float maxSpeed;

    private int desiredLane = 1;
    public float laneDistance = 4;

    public float jumpForce;
    public float Gravity = -20;

    public Animator animator;      //애니메이션
    private bool isSliding = false;
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playermanager.isGameStarted)    
            return;
        //스피드 증가
        if(forwardSpeed < maxSpeed)
            forwardSpeed += 0.1f * Time.deltaTime;






        animator.SetBool("isGameStarted",true);   //애니메이션

        direction.z = forwardSpeed;
        direction.y += Gravity * Time.deltaTime;
       // animator.SetBool("isGrounded", isGrounded);   //애니메이션
        if (controller.isGrounded) 
        {
           
            if (SwipeManager.swipeUp)
            {
                Jump();
            }
              
        }

        if (SwipeManager.swipeDown &&!isSliding)
        {
            StartCoroutine(Slide());
        }
           




        if (SwipeManager.swipeRight)
        {
            desiredLane++;
            if (desiredLane == 3)
                desiredLane = 2; 
        }

        if (SwipeManager.swipeLeft)
        {
            desiredLane--;
            if (desiredLane == -1)
                desiredLane = 0;
        }

        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;

        if(desiredLane == 0)
        {
            targetPosition += Vector3.left * laneDistance;
        }else if(desiredLane == 2)
        {
            targetPosition += Vector3.right * laneDistance;
        }
        //transform.position = Vector3.Lerp(transform.position, targetPosition, 70 * Time.deltaTime);
        if (transform.position == targetPosition)
            return;
        Vector3 diff = targetPosition - transform.position;
        Vector3 moveDir = diff.normalized * 25 * Time.deltaTime;
        if(moveDir.sqrMagnitude < diff.sqrMagnitude)
            controller.Move(moveDir);
        else
            controller.Move(diff);

    }

    private void FixedUpdate()
    {
        if (!playermanager.isGameStarted)
            return;
        controller.Move(direction * Time.fixedDeltaTime);
    }

    private void Jump()
    {
        direction.y = jumpForce;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Obsticle")
        {
            playermanager.gameOver = true;
            FindObjectOfType<AudioManager>().PlaySound("GameOver");
        }
    }

    private IEnumerator Slide()    //슬라이드
    {
        isSliding = true;
        animator.SetBool("isSliding", true);
        controller.center = new Vector3(0, -0.5f, 0);
        controller.height = 1;

        yield return new WaitForSeconds(1.3f);

        controller.center = new Vector3(0, 0, 0);
        controller.height = 2;
        animator.SetBool("isSliding", false);
        isSliding = false;
    }

}
