using System;
using System.Collections.Generic;
using UnityEngine;

public class LSystem : MonoBehaviour
{
    public int n;
    public string axiom;
    
    [Serializable]
    public class Production
    {
        public string lhs;
        public string rhs;
    }

    public List<Production> productions;
    public GameObject branchTemplate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string tree = generate(axiom, n);
        Debug.Log(tree);
        interpret(tree);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public string replace(string token)
    {
        foreach (Production prod in productions)
        {
            if (prod.lhs == token)
            {
                return prod.rhs;
            }
        }
        return token;
    }

    public string generate(string current, int depth)
    {
        if (depth == 0)
        {
            return current;
        }
        else
        {
            string result = "";
            foreach (string token in current.Split(" "))
            {
                if (result != "") { result += " "; }
                result += generate(replace(token), depth - 1);
            }
            return result;
        }
    }

    public void interpret(string tree)
    {
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;
        Stack <(Vector3, Quaternion)> stack = new Stack<(Vector3, Quaternion)>();

        foreach (string token in tree.Split(" "))
        {
            if (token == "f")
            {
                GameObject g = Instantiate(branchTemplate, position, rotation);
                g.transform.parent = transform;
                position += g.transform.up;
            }
            if (token == "l")
            {
                rotation *= Quaternion.AngleAxis(30f, Vector3.forward);
            }
            if (token == "r")
            {
                rotation *= Quaternion.AngleAxis(-30f, Vector3.forward);
            }
            if (token == "[")
            {
                stack.Push((position, rotation));
            }
            if (token == "]")
            {
                (position, rotation) = stack.Pop();
            }
        }
    }
}
