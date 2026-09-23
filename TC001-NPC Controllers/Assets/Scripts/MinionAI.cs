using UnityEngine;

public class MinionAI : MonoBehaviour
{
    public enum States{PassiveState, AttackState, DeadState}
    public enum Events{FoundTarget, LostTarget, Died}

    //Public Variables that differ between minions
    public bool isRanged, isTimed, isAttackLimited, isActive;
    public int AttackLimit;
    public float LifeSpan;
    public float AttackDelay;

    //Private Variables that are changed/used by the script without user/editor input
    private StateMachine stateMachine;
    float timer;
    float delay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stateMachine = new StateMachine(new States(), new Events(), States.PassiveState);

        //Events that handle targeting
        stateMachine.addEvent(States.PassiveState, States.AttackState, Events.FoundTarget);
        stateMachine.addEvent(States.AttackState, States.PassiveState, Events.LostTarget);

        //Events that handle death
        stateMachine.addEvent(States.PassiveState, States.DeadState, Events.Died);
        stateMachine.addEvent(States.AttackState, States.DeadState, Events.Died);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Current state is: " + stateMachine.getState());

        switch (stateMachine.getState())
        {   
            //Passive State
            case States.PassiveState:
                if (isTimed == true)
                {
                    timer += Time.deltaTime;
                    if (timer >= LifeSpan){ Died(); }
                }
                else
                {
                    
                }
                break;
            
            //Attack State
            case States.AttackState:
                if (isTimed == true)
                {
                    timer += Time.deltaTime;
                    if (timer >= LifeSpan){ Died(); }
                }
                else
                {
                    
                }
                break;
            
            //Death State
            case States.DeadState:
                
                break;
        }
    }

    public void FoundTarget() { stateMachine.handleEvent(Events.FoundTarget);   }
    public void LostTarget() { stateMachine.handleEvent(Events.LostTarget);   }
    public void Died() { stateMachine.handleEvent(Events.Died);   }
}
