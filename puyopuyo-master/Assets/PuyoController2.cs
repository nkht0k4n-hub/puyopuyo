using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuyoController2 : MonoBehaviour
{
    // Start is called before the first frame update
    public float fallTimer = 0f;
    float fallInterval = 0.2f;

    public const int WIDTH = 6;
    public const int HEIGHT = 12;

    public int gridX;
    public int gridY;

    public PuyoColor color;

    public bool used = false;
    public bool deleteBase = true;


    public enum PuyoColor
    {
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
        gridX = WIDTH / 2;      // 6 / 2 = 3（ほぼ中央）
        gridY = HEIGHT - 1;     // 一番上の段
        transform.position = GridToWorld(gridX, gridY);
        //enable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!used)
        {

            //transform.position += Vector3.down * Time.deltaTime;
            //float h = Input.GetAxisRaw("Horizontal");
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (canMove(gridX - 1, gridY))
                {
                    //transform.position += GridToWorld(-1,0);
                    gridX--;
                    if (!canMove(gridX, gridY - 1))
                        FixToField();
                }
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (canMove(gridX + 1, gridY))
                {
                    //transform.position += GridToWorld(1,0);
                    gridX++;
                    if (!canMove(gridX, gridY - 1))
                        FixToField();
                }

            }
        }
        fallTimer += Time.deltaTime;
        if (fallTimer > fallInterval)
        {
            fallTimer = 0;
            fallOneCell();
        }

        transform.position = GridToWorld(gridX, gridY);

        //transform.position += Vector3.right * h * speed * Time.deltaTime;
    }

    void fallOneCell()
    {
        if (canMove(gridX, gridY - 1))
        {
            //transform.position += GridToWorld(0,-1);
            gridY--;
            if (!canMove(gridX, gridY - 1))
                FixToField();
        }
    }

    Vector3 GridToWorld(int x, int y)
    {
        return new Vector3(x, y, 0);
    }

    bool canMove(int nextX, int nextY)
    {
        if ((nextX < 0) || (nextX >= WIDTH))
            return false;
        if (nextY < 0)
            return false;
        if (GameManager.Instance.field[nextX, nextY] != 0)
            return false;
        return true;
    }

    void FixToField()
    {
        Debug.Log(GameManager.Instance);
        switch (color)
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
        if (deleteBase)
        {
            var connected = GameManager.Instance.JudgeConnect(gridX, gridY);
            if (connected.Count >= 4)
            {
                GameManager.Instance.DeletePuyo(connected);
            }
        }
        if (!used)
        {
            GameManager.Instance.SpawnNextPuyo();
            GameManager.Instance.SpawnNextPuyo2();
            used = true;
        }//3700+400+800+1600=6500
    }

    void SetRandomColor()
    {
        color = (PuyoColor)Random.Range(0, 4);
    }

    void ApplyColor()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        switch (color)
        {
            case PuyoColor.Red:
                sr.color = Color.red;
                break;
            case PuyoColor.Blue:
                sr.color = Color.blue;
                break;
            case PuyoColor.Green:
                sr.color = Color.green;
                break;
            case PuyoColor.Yellow:
                sr.color = Color.yellow;
                break;
        }
    }
}

