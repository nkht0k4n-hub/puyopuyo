using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class end_to_title : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(osu);
    }

    void osu()
    {
        SceneManager.LoadScene("title");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
