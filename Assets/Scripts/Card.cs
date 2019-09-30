
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public static int speed = 8;
    public static float timeToHide = 1.8f; 
    public enum Color {Spades ,Club ,Heart ,Diamond }
    public enum Owner {Deck ,Discard ,Player ,Player2 }
    public enum Position {Deck,Discard,Player_Slot1,Player_Slot2,Player_Slot3,Player_Slot4, Player_Slot5, Player_Slot6, Player2_Slot1,Player2_Slot2,Player2_Slot3,Player2_Slot4, Player2_Slot5,Player2_Slot6, PlayerChoice,Player2Choice} // to set position on the board
    private Outline outline;
    public string value; // A , 2 ,6 , K...

    public Color color;
    public Owner owner;
    public Position position;

    public bool isVisible = false; // to show only top card of deck / discard
	public bool isHidden = true; // hidden card face
    public bool isMoving; 
    private bool isShaking;
    private bool shakeLeft;
    private Transform destination;
	private MeshRenderer meshRenderer;
	private BoxCollider boxCollider;

    public Card(string value,Color c,Owner o)
    {
        this.value = value;
        this.color = c;
        this.owner = Owner.Deck;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.meshRenderer = this.GetComponent<MeshRenderer>();
        this.boxCollider = this.GetComponent<BoxCollider>();
        this.outline = this.GetComponent<Outline>();
        Material mat = GetComponent<Renderer>().material;
        Material matNew = Instantiate(mat);
        GetComponent<Renderer>().material = matNew;
        outline.StartOutline();
    }

    // Update is called once per frame
    void Update()
    {
		if(isVisible)
		{
            meshRenderer.enabled = true;
            boxCollider.enabled = true;

            float rotY = this.transform.eulerAngles.y;
            if (isHidden &&  (-5 < rotY && rotY < 5)) this.transform.eulerAngles = new Vector3(0, 0, 0);
            else if (!isHidden &&  (170 < rotY && rotY < 190)) this.transform.eulerAngles = new Vector3(0, 180, 0);

            if (isHidden && this.transform.rotation.y != 0) this.transform.Rotate(0, 8, 0, Space.Self);
            else if (!isHidden && this.transform.rotation.y != 180) this.transform.Rotate(0, 8, 0, Space.Self);
        }
		else
		{
            meshRenderer.enabled = false;
            boxCollider.enabled = false;
		}	
        if(isMoving)
        {
            if (Vector3.Distance(transform.position, destination.position) > 0.001f)
            {
                float step = speed * Time.deltaTime; // calculate distance to move
                transform.position = Vector3.MoveTowards(transform.position, destination.position, step);
            }
            else
            {
                Deck.Instance.UpdatePosition();
                Discard.Instance.UpdatePosition();
                isMoving = false;
                transform.position = new Vector3(destination.position.x, destination.position.y, transform.position.z);
               // Debug.Log("Card Move completed");
            }
        }
        if(isShaking)
        {
             float speed = 45f;
             float maxRotation = 10f;
             float rotY = this.transform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f,0f,maxRotation * Mathf.Sin(Time.time * speed));
        }
    }
    
    public void SetHidden(bool h)
    {
        
        if(h)
        {
            StartCoroutine(Hide());
        }
        else
        {
            isHidden = false;
        }
        //TODO animation rotation
    }
    public IEnumerator Hide()
    {
        yield return new WaitForSeconds(timeToHide);
        isHidden = true;
    }

    public void SetParticles(bool b)
    {
        outline.SetActive(b);
    }

    public void SetFront(bool b)
    {
        if(b)
        {
             this.transform.position = new Vector3(transform.position.x, transform.position.y, -0.5f);
        }
        else
        {
            this.transform.position = new Vector3(transform.position.x, transform.position.y, -0.4f);
        }
    }

    public void Shake()
    {
        isShaking = true;
        StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        isShaking = false;
        float eulerY = this.transform.eulerAngles.y;
        this.transform.eulerAngles.Set(0, eulerY, 0);
    }

    public int ValueToInt()
    {
        if(!value.Equals("K") && !value.Equals("J") && !value.Equals("Q") && !value.Equals("A"))
        {
            return int.Parse(value);
        }
        else
        {
            if (value.Equals("K") && (color == Card.Color.Diamond || color == Card.Color.Heart)) return 0;
            else if (value.Equals("K")) return 20;
            else if (value.Equals("Q")) return 10;
            else if (value.Equals("J")) return 10;
            else return 1; // "A"
            // the extra points from the position is not really important
        }
    }

    public override string ToString()
    {
        return "Card: " + value + "," + color;
    }
    public void MoveTo(Position p)
    {
       // Debug.Log("Card(" + value+ "," + color + ") MoveTo " + p);
        this.position = p;
        switch(p)
        {
            case Position.Deck:
                destination = GameObject.Find("DeckPosition").transform;
                owner = Owner.Deck;
                isMoving = true;
                Deck.Instance.stack.Insert(0, this.gameObject);               
                break;
            case Position.Discard:
                SetHidden(false);
                destination = GameObject.Find("DiscardPosition").transform;
                owner = Owner.Discard;
                isMoving = true;
                Discard.Instance.stack.Insert(0, this.gameObject);      
                break;
            case Position.Player_Slot1:
                destination = GameObject.Find("PlayerHand_Slot1").transform;
                GameManager.Instance.cardsJ1.Add(this);
                owner = Owner.Player;
                isMoving = true;
                break;
            case Position.Player_Slot2:
                destination = GameObject.Find("PlayerHand_Slot2").transform;
                GameManager.Instance.cardsJ1.Add(this);
                owner = Owner.Player;
                isMoving = true;
                break;
            case Position.Player_Slot3:
                destination = GameObject.Find("PlayerHand_Slot3").transform;
                GameManager.Instance.cardsJ1.Add(this);
                owner = Owner.Player;
                isMoving = true;
                break;
            case Position.Player_Slot4:
                destination = GameObject.Find("PlayerHand_Slot4").transform;
                GameManager.Instance.cardsJ1.Add(this);
                owner = Owner.Player;
                isMoving = true;
                break;
            case Position.Player_Slot5:
                destination = GameObject.Find("PlayerHand_Slot5").transform;
                GameManager.Instance.cardsJ1.Add(this);
                owner = Owner.Player;
                isMoving = true;
                break;
            case Position.Player_Slot6:
                destination = GameObject.Find("PlayerHand_Slot6").transform;
                GameManager.Instance.cardsJ1.Add(this);
                owner = Owner.Player;
                isMoving = true;
                break;
            case Position.Player2_Slot1:
                destination = GameObject.Find("Player2Hand_Slot1").transform;
                GameManager.Instance.cardsJ2.Add(this);
                owner = Owner.Player2;
                isMoving = true;
                break;
            case Position.Player2_Slot2:
                destination = GameObject.Find("Player2Hand_Slot2").transform;
                GameManager.Instance.cardsJ2.Add(this);
                owner = Owner.Player2;
                isMoving = true;
                break;
            case Position.Player2_Slot3:
                destination = GameObject.Find("Player2Hand_Slot3").transform;
                GameManager.Instance.cardsJ2.Add(this);
                owner = Owner.Player2;
                isMoving = true;
                break;
            case Position.Player2_Slot4:
                destination = GameObject.Find("Player2Hand_Slot4").transform;
                GameManager.Instance.cardsJ2.Add(this);
                owner = Owner.Player2;
                isMoving = true;
                break;
            case Position.Player2_Slot5:
                destination = GameObject.Find("Player2Hand_Slot5").transform;
                GameManager.Instance.cardsJ2.Add(this);
                owner = Owner.Player2;
                isMoving = true;
                break;
            case Position.Player2_Slot6:
                destination = GameObject.Find("Player2Hand_Slot6").transform;
                GameManager.Instance.cardsJ2.Add(this);
                owner = Owner.Player2;
                isMoving = true;
                break;
            case Position.PlayerChoice:
                destination = GameObject.Find("PlayerChoicePosition").transform;
                owner = Owner.Player;
                isMoving = true;
                break;
            case Position.Player2Choice:
                destination = GameObject.Find("Player2ChoicePosition").transform;
                owner = Owner.Player2;
                isMoving = true;
                break;
        }     
    }
}

