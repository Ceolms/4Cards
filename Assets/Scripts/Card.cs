using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public static int speed = 8;
    public enum Color {Spades ,Club ,Heart ,Diamond }
    public enum Owner {Deck ,Discard ,Player ,Player2 }
    public enum Position {Deck,Discard,Player_Slot1,Player_Slot2,Player_Slot3,Player_Slot4, Player_Slot5, Player_Slot6, Player2_Slot1,Player2_Slot2,Player2_Slot3,Player2_Slot4, Player2_Slot5,Player2_Slot6, PlayerChoice,Player2Choice} // to set position on the board
    private GameObject particleObject;
    private Outline outline;
    private Vector3 poPosition;
    public string value; // A , 2 ,6 , K...

    public Color color;
    public Owner owner;
    public Position position;

    public bool isVisible = false; // to show only top card of deck / discard
	public bool isHidden = true; // hidden card face
    public bool isMoving; 
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
        outline.enabled = false;
        Material mat = GetComponent<Renderer>().material;
        Material matNew = Instantiate(mat);
        GetComponent<Renderer>().material = matNew;

        foreach (Transform tr in this.transform)  { if (tr.tag == "ParticlesObject"){particleObject = tr.gameObject;} }
        poPosition = particleObject.transform.localPosition;
        SetParticles(false);
    }

    // Update is called once per frame
    void Update()
    {
		if(isVisible)
		{
            meshRenderer.enabled = true;
            boxCollider.enabled = true;
            if (isHidden) this.transform.eulerAngles = new Vector3(0,0,0);
			else this.transform.eulerAngles = new Vector3(0,180,0);
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
               // Debug.Log("Card Move completed");
            }
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
        yield return new WaitForSeconds(2.5f);
        isHidden = true;
    }

    public void SetParticles(bool b)
    {
        outline.enabled = b;
        if (b)
        {   /*
            particleObject.transform.localPosition = poPosition;
            ParticleSystem ps = particleObject.GetComponent<ParticleSystem>();
            ps.Simulate(4f);
            ps.Play();*/
        }
        else
        {
            
            ParticleSystem ps = particleObject.GetComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particleObject.transform.localPosition = new Vector3(10000,10000,10000);   
            
        }
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
                GameManager.Instance.cardsJ1[0] = this;
                owner = Owner.Player;
                isMoving = true;
                break;
            case Position.Player_Slot2:
                destination = GameObject.Find("PlayerHand_Slot2").transform;
                GameManager.Instance.cardsJ1[1] = this;
                owner = Owner.Player;
                isMoving = true;
                break;
            case Position.Player_Slot3:
                destination = GameObject.Find("PlayerHand_Slot3").transform;
                GameManager.Instance.cardsJ1[2] = this;
                owner = Owner.Player;
                isMoving = true;
                break;
            case Position.Player_Slot4:
                destination = GameObject.Find("PlayerHand_Slot4").transform;
                GameManager.Instance.cardsJ1[3] = this;
                owner = Owner.Player;
                isMoving = true;
                break;
            case Position.Player_Slot5:
                destination = GameObject.Find("PlayerHand_Slot5").transform;
                GameManager.Instance.cardsJ1[4] = this;
                owner = Owner.Player;
                isMoving = true;
                break;
            case Position.Player_Slot6:
                destination = GameObject.Find("PlayerHand_Slot6").transform;
                GameManager.Instance.cardsJ1[5] = this;
                owner = Owner.Player;
                isMoving = true;
                break;
            case Position.Player2_Slot1:
                destination = GameObject.Find("Player2Hand_Slot1").transform;
                GameManager.Instance.cardsJ2[0] = this;
                owner = Owner.Player2;
                isMoving = true;
                break;
            case Position.Player2_Slot2:
                destination = GameObject.Find("Player2Hand_Slot2").transform;
                GameManager.Instance.cardsJ2[1] = this;
                owner = Owner.Player2;
                isMoving = true;
                break;
            case Position.Player2_Slot3:
                destination = GameObject.Find("Player2Hand_Slot3").transform;
                GameManager.Instance.cardsJ2[2] = this;
                owner = Owner.Player2;
                isMoving = true;
                break;
            case Position.Player2_Slot4:
                destination = GameObject.Find("Player2Hand_Slot4").transform;
                GameManager.Instance.cardsJ2[3] = this;
                owner = Owner.Player2;
                isMoving = true;
                break;
            case Position.Player2_Slot5:
                destination = GameObject.Find("Player2Hand_Slot5").transform;
                GameManager.Instance.cardsJ2[4] = this;
                owner = Owner.Player2;
                isMoving = true;
                break;
            case Position.Player2_Slot6:
                destination = GameObject.Find("Player2Hand_Slot6").transform;
                GameManager.Instance.cardsJ2[5] = this;
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

