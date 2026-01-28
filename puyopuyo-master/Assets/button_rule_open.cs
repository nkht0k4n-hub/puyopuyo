using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class button_rule_open : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(osu);
    }

    void osu()
    {
        SceneManager.LoadScene("rule");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
