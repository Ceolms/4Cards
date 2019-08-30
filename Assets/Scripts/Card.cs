﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public static int speed = 8;
    public enum Color {Spades ,Club ,Heart ,Diamond }
    public enum Owner {Deck ,Discard ,Player ,Player2 }
    public enum Position {Deck,Discard,Player_Slot1,Player_Slot2,Player_Slot3,Player_Slot4, Player_Slot5, Player_Slot6, Player2_Slot1,Player2_Slot2,Player2_Slot3,Player2_Slot4, Player2_Slot5,Player2_Slot6, PlayerChoice,Player2Choice} // to set position on the board
    private GameObject particleObject;
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
        else
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount > 0 && Input.touchCount < 2)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                        CheckTouch(ray);
                    }
                }
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    CheckTouch(ray);
                }
            }
        }
    }


    private void CheckTouch(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject cardHit = hit.collider.gameObject;
            Card c = cardHit.GetComponent<Card>();
            if (c == this)
            {
                if(this.gameObject.GetComponent<DoubleClick>() == null)
                {
                    DoubleClick dc = this.gameObject.AddComponent<DoubleClick>();
                    dc.card = this;
                    dc.CheckDoubleClick();
                }    
                else
                {
                    DoubleClick dc = this.gameObject.GetComponent<DoubleClick>();
                    dc.count += 1;
                }
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
        
        if (b)
        {
            particleObject.transform.localPosition = poPosition;
            ParticleSystem ps = particleObject.GetComponent<ParticleSystem>();
            ps.Simulate(4f);
            ps.Play();
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
           // isVisible = true;
        }
        else
        {
            this.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
           // isVisible = false;
        }
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
/*
  if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            //Debug.Log("MobileApplication");
            if (Input.touchCount > 0 && Input.touchCount < 2)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    CheckTouch(ray);
                }
            }
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            //Debug.Log("WindowsApplication");
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                CheckTouch(ray);
            }
        }
 */
