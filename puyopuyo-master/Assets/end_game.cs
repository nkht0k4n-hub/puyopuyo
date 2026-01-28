using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class end_game : MonoBehaviour
{
    // Start is called before the first frame update
    //public string score;

    //public TextMeshProUGUI scoreText;


    void Start()
    {

        TMP_Text text = GetComponent<TMP_Text>();
        text.text = GameData.score.ToString();
        //text.text = "1111";

    }

    // Update is called once per frame
    void Update()
    {
    }
}
