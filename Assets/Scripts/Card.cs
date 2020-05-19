
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public static int speed = 7;
    public static float timeToHide = 1.8f;
    private static bool isPlayingHideSound;
    private static float frontZaxis = -0.2f;
    private static float backZaxis = 0.010f;
    [SerializeField]
    private float targetZ;
    private float stackIndex;

    public enum Color { Spades, Club, Heart, Diamond }
    public enum Owner { Deck = 3, Discard = 4, Player1 = 1, Player2 = 2 }
    public enum Position { Deck, Discard, Player1_Slot1, Player1_Slot2, Player1_Slot3, Player1_Slot4, Player1_Slot5, Player1_Slot6, Player2_Slot1, Player2_Slot2, Player2_Slot3, Player2_Slot4, Player2_Slot5, Player2_Slot6, Player1Choice, Player2Choice } // to set position on the board
    private Outline outline;
    public string value; // A , 2 ,6 , K...

    public Color color;
    public Owner owner;
    public Position position;

    [SerializeField]
    private bool isRendered = true; // to show only top card of deck / discard
    public bool isHidden = true; // hidden card face
    public bool isMoving { get; private set; }
    private bool isShaking;
    private bool shakeLeft;
    private Transform destination;
    private BoxCollider boxCollider;

    private GameObject frontFace;
    private GameObject backFace;

    public Card(string value, Color c, Owner o)
    {
        this.value = value;
        this.color = c;
        this.owner = Owner.Deck;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.boxCollider = this.GetComponent<BoxCollider>();
        this.outline = this.GetComponent<Outline>();
        outline.StartOutline();
        frontFace = transform.Find("Front").gameObject;
        backFace = transform.Find("Back").gameObject;

        if (GameManager.Instance.GetSelectedSprite() == null) Debug.LogError("Error selected sprite is null");
        else backFace.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.GetSelectedSprite();
    }

    // Update is called once per frame
    void Update()
    {

        if (!isShaking && this.transform.eulerAngles.z != 0) // Fix card rotation while not shaking
        {
            this.transform.rotation = new Quaternion(0, this.transform.rotation.y, 0, this.transform.rotation.w);
        }

        CheckPosition();

        if (isMoving)
        {
            if (Vector2.Distance(transform.position, destination.position) > 0.000001f)
            {
                float step = speed * Time.deltaTime; // calculate distance to move
                transform.position = Vector2.MoveTowards(transform.position, destination.position, step);
            }
            else
            {
                isMoving = false;
            }
        }

        if (isShaking)
        {
            float speed = 45f;
            float maxRotation = 10f;
            float rotY = this.transform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, 0f, maxRotation * Mathf.Sin(Time.time * speed));
        }

        if (GameManager.Instance.debugMode)
        {
            isHidden = false;
        }
    }

    public void SetHidden(bool h)
    {
        if (h)
        {
            StartCoroutine(Hide());
        }
        else
        {
            isHidden = false;
            if (!(GameManager.Instance.state is EndRound) && !(GameManager.Instance.state is NewRound))
            {
                SoundPlayer.Instance.Play("cardReturnVisible");
            }
        }
    }
    public IEnumerator Hide()
    {
        yield return new WaitForSeconds(timeToHide);
        if (this.position != Position.Discard) isHidden = true;
        if (!(GameManager.Instance.state is EndRound) && !(GameManager.Instance.state is NewRound) && !isPlayingHideSound)
        {
            isPlayingHideSound = true;
            SoundPlayer.Instance.Play("cardReturnHidden");
            yield return new WaitForSeconds(4);
            isPlayingHideSound = false;
        }
    }

    public void SetParticles(bool b)
    {
        outline.SetActive(b);
    }


    // visible position = -0,02
    // hidden positon 0,011        
    //                0,012...
    private void CheckPosition()
    {
        if (this.position != Position.Deck && this.position != Position.Discard)
        {
            isRendered = true;
        }
        else if ((Deck.Instance.stack.Count > 0 && Deck.Instance.stack[0] == this) || (Discard.Instance.stack.Count > 0 && Discard.Instance.stack[0] == this))
        {
            isRendered = true;
        }
        else
        {
            isRendered = false;
        }

        if (isRendered) // if the card is not hidden behind another card
        {
            boxCollider.enabled = true;
            this.transform.position = new Vector3(transform.position.x, transform.position.y, frontZaxis);

            float rotY = this.transform.eulerAngles.y;

            if (isHidden) // hidden face
            {
                if (this.transform.rotation.y != 0) this.transform.Rotate(0, 8, 0, Space.Self);
                if (-5 < rotY && rotY < 5) this.transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                if (this.transform.rotation.y != 180) this.transform.Rotate(0, 8, 0, Space.Self);
                if (170 < rotY && rotY < 190) this.transform.eulerAngles = new Vector3(0, 180, 0);
            }
            targetZ = -0.02f;
            stackIndex = 0;
        }
        else
        {
            if (this.position == Position.Deck) stackIndex = (float)Deck.Instance.stack.IndexOf(this);
            else stackIndex = (float)Discard.Instance.stack.IndexOf(this);
            targetZ = backZaxis + (stackIndex/1000f);

            this.transform.position = new Vector3(transform.position.x, transform.position.y, targetZ);
            boxCollider.enabled = false;
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
        if (!value.Equals("K") && !value.Equals("J") && !value.Equals("Q") && !value.Equals("A"))
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
        if (this.position == Card.Position.Player1_Slot1 || this.position == Card.Position.Player1_Slot2 || this.position == Card.Position.Player1_Slot3
            || this.position == Card.Position.Player1_Slot4 || this.position == Card.Position.Player1_Slot5 || this.position == Card.Position.Player1_Slot6)
        {
            GameManager.Instance.cardsJ1.Remove(this);
        }
        else if (this.position == Card.Position.Player2_Slot1 || this.position == Card.Position.Player2_Slot2 || this.position == Card.Position.Player2_Slot3
           || this.position == Card.Position.Player2_Slot4 || this.position == Card.Position.Player2_Slot5 || this.position == Card.Position.Player2_Slot6)
        {
            GameManager.Instance.cardsJ2.Remove(this);
        }
        this.position = p;
        if (p != Card.Position.Deck)
        {
            SoundPlayer.Instance.PlayRandomCard();
        }
        switch (p)
        {
            case Position.Deck:
                destination = GameObject.Find("DeckPosition").transform;
                owner = Owner.Deck;
                isMoving = true;
                SetHidden(true);
                Deck.Instance.stack.Insert(0, this);
                break;
            case Position.Discard:
                SetHidden(false);
                destination = GameObject.Find("DiscardPosition").transform;
                owner = Owner.Discard;
                isMoving = true;
                // this.SetFront(true);
                //    if (Discard.Instance.stack.Count > 0) Discard.Instance.stack[0].GetComponent<Card>().SetFront(false);
                Discard.Instance.stack.Insert(0, this);
                break;
            case Position.Player1_Slot1:
                destination = GameObject.Find("PlayerHand_Slot1").transform;
                GameManager.Instance.cardsJ1.Add(this);
                owner = Owner.Player1;
                isMoving = true;
                //  this.SetFront(true);
                break;
            case Position.Player1_Slot2:
                destination = GameObject.Find("PlayerHand_Slot2").transform;
                GameManager.Instance.cardsJ1.Add(this);
                owner = Owner.Player1;
                isMoving = true;
                //this.SetFront(true);
                break;
            case Position.Player1_Slot3:
                destination = GameObject.Find("PlayerHand_Slot3").transform;
                GameManager.Instance.cardsJ1.Add(this);
                owner = Owner.Player1;
                isMoving = true;
                //  this.SetFront(true);
                break;
            case Position.Player1_Slot4:
                destination = GameObject.Find("PlayerHand_Slot4").transform;
                GameManager.Instance.cardsJ1.Add(this);
                owner = Owner.Player1;
                isMoving = true;
                //  this.SetFront(true);
                break;
            case Position.Player1_Slot5:
                destination = GameObject.Find("PlayerHand_Slot5").transform;
                GameManager.Instance.cardsJ1.Add(this);
                owner = Owner.Player1;
                isMoving = true;
                //  this.SetFront(true);
                break;
            case Position.Player1_Slot6:
                destination = GameObject.Find("PlayerHand_Slot6").transform;
                GameManager.Instance.cardsJ1.Add(this);
                owner = Owner.Player1;
                isMoving = true;
                //    this.SetFront(true);
                break;
            case Position.Player2_Slot1:
                destination = GameObject.Find("Player2Hand_Slot1").transform;
                GameManager.Instance.cardsJ2.Add(this);
                owner = Owner.Player2;
                isMoving = true;
                //   this.SetFront(true);
                break;
            case Position.Player2_Slot2:
                destination = GameObject.Find("Player2Hand_Slot2").transform;
                GameManager.Instance.cardsJ2.Add(this);
                owner = Owner.Player2;
                isMoving = true;
                //  this.SetFront(true);
                break;
            case Position.Player2_Slot3:
                destination = GameObject.Find("Player2Hand_Slot3").transform;
                GameManager.Instance.cardsJ2.Add(this);
                owner = Owner.Player2;
                isMoving = true;
                //     this.SetFront(true);
                break;
            case Position.Player2_Slot4:
                destination = GameObject.Find("Player2Hand_Slot4").transform;
                GameManager.Instance.cardsJ2.Add(this);
                owner = Owner.Player2;
                isMoving = true;
                //        this.SetFront(true);
                break;
            case Position.Player2_Slot5:
                destination = GameObject.Find("Player2Hand_Slot5").transform;
                GameManager.Instance.cardsJ2.Add(this);
                owner = Owner.Player2;
                isMoving = true;
                //        this.SetFront(true);
                break;
            case Position.Player2_Slot6:
                destination = GameObject.Find("Player2Hand_Slot6").transform;
                GameManager.Instance.cardsJ2.Add(this);
                owner = Owner.Player2;
                isMoving = true;
                //          this.SetFront(true);
                break;
            case Position.Player1Choice:
                destination = GameObject.Find("PlayerChoicePosition").transform;
                owner = Owner.Player1;
                isMoving = true;
                //     this.SetFront(true);
                break;
            case Position.Player2Choice:
                destination = GameObject.Find("Player2ChoicePosition").transform;
                owner = Owner.Player2;
                isMoving = true;
                //      this.SetFront(true);
                break;
        }
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, destination.position.z);
    }
}

