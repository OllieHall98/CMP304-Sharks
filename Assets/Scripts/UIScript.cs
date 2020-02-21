using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIScript : MonoBehaviour
{
    public static UIScript UI;

    [SerializeField] Text stateText;

    // Start is called before the first frame update
    void Start()
    {
        UI = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setStateText(string state)
    {
        stateText.text = "State: " + state;
    }
}
