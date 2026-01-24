using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuyoController : MonoBehaviour
{
    // Start is called before the first frame update
    public float UsedfallTimer = 0f;
    float fallInterval = 0.2f;

    public const int WIDTH = 6;
    public const int HEIGHT = 12;

    public int gridX;
    public int gridY;

    public PuyoColor color;

   public bool used = false;
   public bool deleteBase=true;

    public bool underPuyo = false;

    public Sprite red;
    public Sprite blue;
    public Sprite green;
    public Sprite yellow;


    public enum PuyoColor { 
        Red,
        Blue,
        Green,
        Yellow
    }


    //bool enable;

    void Start()
    {
        SetRandomColor();
        ApplyColor(); 
        /*
        gridX = WIDTH / 2;      
        gridY = HEIGHT - 1;
        */
        //transform.position = GameManager.Instance.GridToWorld(gridX, gridY);
        //enable = true;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(gridY-1>=0 && canMove(gridX,gridY - 1)==false)
        {
            
            if (GameManager.Instance.JudgeUnderNew(gridX, gridY))
            {
                underPuyo = true;
            }
            FixToField();
        }*/
        if (used)
        {
            UsedfallTimer += Time.deltaTime;
            if (UsedfallTimer > fallInterval)
            {
                //GameManager.Instance.fallTimer = 0;
                fallOneCell();
                if (!canMove(gridX, gridY - 1))
                {
                    FixToField();
                    MakeDeleteList();
                    if (deleteBase) Debug.Log("possible to delete");
                    if (!deleteBase) Debug.Log("not DeleteBase");
                    GameManager.Instance.fallPuyoCount--;
                    Debug.Log("used delete after");
                }
            }
            transform.position = GameManager.Instance.GridToWorld(gridX, gridY);
        }   
        
   }

    public void fallOneCell()
    {
        if (canMove(gridX, gridY - 1))
        {
            //transform.position += GridToWorld(0,-1);
            gridY--;
            /*
            if (!canMove(gridX, gridY - 1)
            {
                //underPuyo = true;
                if (GameManager.Instance.JudgeUnderNew(gridX, gridY))
                {
                    underPuyo = true;
                }
                FixToField();
            }
            /*
        }
    }
    /*
    public Vector3 GridToWorld(int x, int y) {
        return new Vector3(x,y,0);
    }*/
        }
    }


    public bool canMove(int nextX, int nextY) {
        if ((nextX < 0) || (nextX >= WIDTH))
            return false;
        if (nextY < 0)
            return false;
        if (GameManager.Instance.field[nextX, nextY] != 0)
            return false;
        /*
        PuyoController puyo = GameManager.Instance.findPuyo(nextX, nextY);
        if (puyo != null)
            return false;
        */
        return true;
    }

    public void FixToField()
    {
        Debug.Log(GameManager.Instance);
        switch(color)
        {
            case PuyoColor.Red:
                GameManager.Instance.field[gridX, gridY] = 1;
                break;

            case PuyoColor.Blue:
                GameManager.Instance.field[gridX, gridY] = 2;
                break;

            case PuyoColor.Green:
                GameManager.Instance.field[gridX, gridY] = 3;
                break;

            case PuyoColor.Yellow:
                GameManager.Instance.field[gridX, gridY] = 4;
                break;
        }
        //GameManager.Instance.field[gridX, gridY] = 1;
        enabled = false;

        //Delete();

        //Invoke("Delete", 1);
        /*
        if (!underPuyo)
        {
            Spawn();
        }*/

    }

    public void Delete()
    {
            var connected = GameManager.Instance.JudgeConnect(gridX, gridY);
            if (connected.Count >= 4)
            {
                GameManager.Instance.NowAddDeleteList(connected);
            }

    }

    public void MakeDeleteList() {
        var connected = GameManager.Instance.JudgeConnect(gridX, gridY);
        if (connected.Count >= 4)
        {
            GameManager.Instance.AddDeleteList(connected);
        }
    }

    public void Spawn() {
        if (!used)
        {
            if (!GameManager.Instance.hasUpper(gridX, gridY))
            {
                GameManager.Instance.SpawnNextPuyo();
                GameManager.Instance.SpawnNextPuyo2();
            }
            used = true;

        }//3700+400+800+1600=6500
    }

    void SetRandomColor() {
        color = (PuyoColor)Random.Range(0,4);
    }

    void ApplyColor() {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        switch (color)
        {
            case PuyoColor.Red:
                //sr.color = Color.red;
                sr.sprite = red;
                break;
            case PuyoColor.Blue:
                sr.sprite = blue;
                break;
            case PuyoColor.Green:
                sr.sprite = green;
                break;
            case PuyoColor.Yellow:
                sr.sprite = yellow;
                break;
        }
    }
}

