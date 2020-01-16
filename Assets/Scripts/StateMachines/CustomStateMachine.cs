using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomStateMachine : StateMachineBehaviour
{
    public abstract void Execute(Card c);
    public abstract void ChangePhase();

    public abstract bool  CanDeleteCard();
}
