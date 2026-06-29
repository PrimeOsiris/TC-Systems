using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private Enum states;
    private Enum events;

    private Enum currentState;

    class Outcomes
    {
        public Dictionary<Enum, Enum> outcomeList; //Key is an event, the value the outcome state
        public Outcomes() { outcomeList = new Dictionary<Enum, Enum>(); }
    }

    Dictionary<Enum, Outcomes> transitions; //Key is the starting state, value is Dictionary of events linked to outcomes states

    public StateMachine(Enum states, Enum events, Enum intialState)
    {
        this.states = states;
        this.events = events;
        this.currentState = intialState;
        Debug.Log("State Machine created with " + Enum.GetValues(states.GetType()).Length + " states, " + Enum.GetValues(events.GetType()).Length + " events and begins in the " + currentState + "state.");
        transitions = new Dictionary<Enum, Outcomes>();
    }

    public void addEvent(Enum inState, Enum outState, Enum triggerEvent)
    {
        //Checks if there is a transition data point for that state and creates one if there isn't
        if (!transitions.ContainsKey(inState))
        {
            transitions.Add(inState, new Outcomes());
        }
        Outcomes outcomes = transitions[inState];

        //checks if there is a outcome stored for this event, and creates one if there isn't
        if (!outcomes.outcomeList.ContainsKey(triggerEvent))
        {
            outcomes.outcomeList.Add(triggerEvent, outState);
        }
        else
        {
            Debug.Log("Warning: replacing existing outcome state" + outcomes.outcomeList[triggerEvent] + " with " + outState + " for event " + triggerEvent);
            outcomes.outcomeList[triggerEvent] = outState;
        }
    }

    public void handleEvent(Enum newEvent)
    {
        //In currentState and a triggerEvent was triggered
        if (transitions.ContainsKey(currentState))
        {
            Outcomes outcomes = transitions[currentState];
            if (outcomes.outcomeList.ContainsKey(newEvent))
            {
                currentState = outcomes.outcomeList[newEvent];
                Debug.Log("Now in new state; " + currentState);
            }
            else { Debug.Log("Error 28.08.1055: Event " + newEvent + "does not have an effect on the current state: " + currentState); }
        }
        else { Debug.Log("Error 28.08.1053: No events exist for state: " + currentState); }
    }

    public Enum getState()
    {
        return currentState;
    }
}
