﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public static int speed = 8;
    public enum Color {Spades ,Club ,Heart ,Diamond }
    public enum Owner {Deck ,Discard ,Player ,Player2 }
    public enum Position {Deck,Discard,Player_Slot1,Player_Slot2,Player_Slot3,Player_Slot4, Player_Slot5, Player_Slot6, Player2_Slot1,Player2_Slot2,Player2_Slot3,Player2_Slot4, Player2_Slot5,Player2_Slot6, PlayerChoice,Player2Choice} // to set position on the board
    public ParticleSystem ps;
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

        Material mat = GetComponent<Renderer>().material;
        Material matNew = Instantiate(mat);
        GetComponent<Renderer>().material = matNew;
        ps = this.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
        ps.Stop();
        var main = ps.main;
        main.prewarm = true;
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
        yield return new WaitForSeconds(1.5f);
        isHidden = true;
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
                destination = GameObject.Find("DiscardPosition").transform;
                owner = Owner.Discard;
                isMoving = true;
                Deck.Instance.stack.Insert(0, this.gameObject);
                SetHidden(false);
                break;
            case Position.Player_Slot1:
                destination = GameObject.Find("PlayerHand_Slot1").transform;
                owner = Owner.Player;
                isMoving = true;
                break;
            case Position.Player_Slot2:
                destination = GameObject.Find("PlayerHand_Slot2").transform;
                owner = Owner.Player;
                isMoving = true;
                break;
            case Position.Player_Slot3:
                destination = GameObject.Find("PlayerHand_Slot3").transform;
                owner = Owner.Player;
                isMoving = true;
                break;
            case Position.Player_Slot4:
                destination = GameObject.Find("PlayerHand_Slot4").transform;
                owner = Owner.Player;
                isMoving = true;
                break;
            case Position.Player_Slot5:
                destination = GameObject.Find("PlayerHand_Slot5").transform;
                owner = Owner.Player;
                isMoving = true;
                break;
            case Position.Player_Slot6:
                destination = GameObject.Find("PlayerHand_Slot6").transform;
                owner = Owner.Player;
                isMoving = true;
                break;
            case Position.Player2_Slot1:
                destination = GameObject.Find("Player2Hand_Slot1").transform;
                owner = Owner.Player2;
                isMoving = true;
                break;
            case Position.Player2_Slot2:
                destination = GameObject.Find("Player2Hand_Slot2").transform;
                owner = Owner.Player2;
                isMoving = true;
                break;
            case Position.Player2_Slot3:
                destination = GameObject.Find("Player2Hand_Slot3").transform;
                owner = Owner.Player2;
                isMoving = true;
                break;
            case Position.Player2_Slot4:
                destination = GameObject.Find("Player2Hand_Slot4").transform;
                owner = Owner.Player2;
                isMoving = true;
                break;
            case Position.Player2_Slot5:
                destination = GameObject.Find("Player2Hand_Slot5").transform;
                owner = Owner.Player2;
                isMoving = true;
                break;
            case Position.Player2_Slot6:
                destination = GameObject.Find("Player2Hand_Slot6").transform;
                owner = Owner.Player2;
                isMoving = true;
                break;
            case Position.PlayerChoice:
                destination = GameObject.Find("PlayerChoicePosition").transform;
                isMoving = true;
                break;
            case Position.Player2Choice:
                destination = GameObject.Find("Player2ChoicePosition").transform;
                isMoving = true;
                break;
        }
    }
}