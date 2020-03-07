using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    public enum Mode { FSM, Fuzzy }
    public Mode mode;

    // Start is called before the first frame update
    void Start()
    {
        mode = Mode.FSM;
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void selectFSM()
    {
        mode = Mode.FSM;
        SceneManager.LoadScene(1);
    }

    public void selectFuzzy()
    {
        mode = Mode.Fuzzy;
        SceneManager.LoadScene(1);
    }
}
