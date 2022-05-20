using UnityEngine;
using System.Collections;


[RequireComponent(typeof(GhostMovement))]
public class Ghost : MonoBehaviour
{
    public GhostMovement movement { get; private set; }
    public GhostHome home { get; private set; }
    public GhostScatter scatter { get; private set; }
    public GhostChase chase { get; private set; }
    public GhostFrightened frightened { get; private set; }
    public GhostBehavior initialBehavior;
    public Transform target;
    public int points = 200;
    public string chosenAction = "";
    public int counter = 0;
    private ArrayList moveArr = new ArrayList();

    public GameObject pacman;

    private void Awake()
    {
        movement = GetComponent<GhostMovement>();
        home = GetComponent<GhostHome>();
        scatter = GetComponent<GhostScatter>();
        chase = GetComponent<GhostChase>();
        frightened = GetComponent<GhostFrightened>();
    }

    private void Start()
    {
        ResetState();   
    }

    public void GhostMovement(){
        //movement.SetDirection(Vector2.up);
        /*
        chosenAction = (string)movement.globalMinimax(0, 0, false, true, pacman.transform.position)[1];
        moveArr.Add(chosenAction);
        Debug.Log("Movement element GHOST: " + chosenAction);
        Debug.Log("Pacman Pos: "+ pacman.transform.position);

        if (moveArr[0].Equals("s"))
        {
            movement.SetDirection(Vector2.down);

        }
        else if (moveArr[0].Equals("n"))
        {
            movement.SetDirection(Vector2.up);
        }
        else if (moveArr[0].Equals("e"))
        {
            movement.SetDirection(Vector2.right);
        }
        else if (moveArr[0].Equals("w"))
        {
            movement.SetDirection(Vector2.left);
        }
        moveArr = new ArrayList();
        */
                
    }

    public void ResetState()
    {
        gameObject.SetActive(true);
        movement.ResetState();

        frightened.Disable();
        chase.Disable();
        scatter.Enable();

        if (home != initialBehavior) {
            home.Disable();
        }

        if (initialBehavior != null) {
            initialBehavior.Enable();
        }
    }

    public void SetPosition(Vector3 position)
    {
        // Keep the z-position the same since it determines draw depth
        position.z = transform.position.z;
        transform.position = position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            if (frightened.enabled) {
                FindObjectOfType<GameManager>().GhostEaten(this);
            } else {
                FindObjectOfType<GameManager>().PacmanEaten();
            }
        }
    }

}
