using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager Instance;

    public const int WIDTH = 6;
    public const int HEIGHT = 12;

    public int[,] field = new int[WIDTH, HEIGHT];

    public GameObject puyoPrefab;

    public TextMeshProUGUI scoreText;

    public int score = 0;
    public int chain_add = 1;

    public int puyoCount = 0;

    public bool firstTouch=true;

    public int startX= WIDTH / 2;
    public int startY= HEIGHT - 1;

    public float fallTimer = 0f;
    float fallInterval = 0.2f;

    public int fallPuyoCount = 0;
    public int spaceCount=0;
    public int fixedCount = 0;

    //bool puyo2IsUnder = true;

    PuyoController puyo1;
    PuyoController puyo2;

    List<(int X, int Y)>[] deleteList;
    List<(int X, int Y)>[] nowDeleteList;
    int dLIdx = 0;
    int nDLIdx = 0;

    void Start()
    {
        deleteList= new List<(int X, int Y)>[1000];
        nowDeleteList = new List<(int X, int Y)>[1000];

        Debug.Log("ゲーム開始");
        updateScoreText();
        puyo1 = SpawnNextPuyo();
        puyo1.underPuyo = false;
        puyo2 = SpawnNextPuyo2();
        puyo2.underPuyo = true;
        //firstTouch = true;
        //if (firstTouch) Debug.Log("Start:firstTouch is true");
    }

    // Update is called once per frame
    void Update()
    {   
        /*
        if (firstTouch) {
            puyo1 = findPuyo(startX, startY);
            puyo2 = findPuyo(startX, startY-1);
            Debug.Log("firstTouch");
            firstTouch = false;//両方のぷよがfixされたら新しくぷよを生成してtrueにする.
        }*/

        if (puyo1 == null) Debug.Log("puyo1 is null");
        else Debug.Log("puyo1 is availabel");
        if (puyo2 == null) Debug.Log("puyo2 is null");
        else Debug.Log("puyo2 is availabel");
        if (puyo1!=null)MoveLeftRight(puyo1,puyo2);
        if(puyo2!=null)MoveLeftRight(puyo2,puyo1);

        fallTimer += Time.deltaTime;
        if (fallTimer > fallInterval)
        {
            fallTimer = 0;
            if(puyo1!=null)puyo1.fallOneCell();
            if(puyo2!=null)puyo2.fallOneCell();
        }
        if (Input.GetKeyDown(KeyCode.Space)&&puyo2.enabled==true)
        {
            spaceCount++;
            lotate(puyo2,spaceCount%4);
        }


        if (true)
        {
            if (puyo2 != null)//puyo2を消した後作る前にアクセスさせないため
            {
                if (!puyo2.canMove(puyo2.gridX, puyo2.gridY - 1)&&puyo2.used==false)
                {
                    puyo2.FixToField();
                    fixedCount++;
                    Debug.Log(fixedCount);
                    Debug.Log("puyo2 is fixed");
                    spaceCount = 0;
                    puyo2.Delete();
                    puyo2.used = true;
                    if (puyo2 != null) puyo2.transform.position = GridToWorld(puyo2.gridX, puyo2.gridY);

                }
            }
            if (puyo1 != null)
            {
                if (!puyo1.canMove(puyo1.gridX, puyo1.gridY - 1)&&puyo1.used==false)
                {
                    puyo1.FixToField();
                    fixedCount++;
                    Debug.Log(fixedCount);
                    Debug.Log("puyo1 is fixed");
                    //puyo1.used = true;
                    puyo1.Delete();
                    puyo1.used = true;
                    //
                    //

                    if (puyo1 != null) puyo1.transform.position = GridToWorld(puyo1.gridX, puyo1.gridY);
                    if (puyo2 != null) puyo2.transform.position = GridToWorld(puyo2.gridX, puyo2.gridY);


                }
            }
            if (fixedCount == 2) {

                NowDeleteListPuyo();
                nDLIdx = 0;
                puyo1 = SpawnNextPuyo();
                puyo2 = SpawnNextPuyo2();
                fixedCount = 0;
                //Debug.Log(fixedCount);
            }
        }
 


        if (fallPuyoCount == 0) {
            DeleteListPuyo();
            dLIdx = 0;
        }

        if (puyo1!=null)puyo1.transform.position = GridToWorld(puyo1.gridX, puyo1.gridY);
        if(puyo2!=null)puyo2.transform.position = GridToWorld(puyo2.gridX, puyo2.gridY);

    }

    public void lotate(PuyoController puyo,int step) {
        if (step == 0)//下
        {
            if (puyo.canMove(puyo.gridX - 1, puyo.gridY - 1))
            {
                puyo.gridX--;
                puyo.gridY--;
                //puyo2IsUnder = true;
                puyo.transform.position= GridToWorld(puyo.gridX, puyo.gridY);
            }
        }
        else if (step == 1)//左
        {
            if (puyo.canMove(puyo.gridX - 1, puyo.gridY+1))
            {
                puyo.gridX--;
                puyo.gridY++;
                puyo.transform.position = GridToWorld(puyo.gridX, puyo.gridY);
            }
        }
        else if (step == 2)//上
        {
            if (puyo.canMove(puyo.gridX+1, puyo.gridY+1))
            {
                puyo.gridX++;
                puyo.gridY++;
                //puyo2IsUnder = false;
                puyo.transform.position = GridToWorld(puyo.gridX, puyo.gridY);
            }
        }
        else if (step == 3)//右
        {
            if (puyo.canMove(puyo.gridX+1, puyo.gridY-1))
            {
                puyo.gridX++;
                puyo.gridY--;
                puyo.transform.position = GridToWorld(puyo.gridX, puyo.gridY);
            }
        }
    
    }
    
    public Vector3 GridToWorld(int x, int y) {
        return new Vector3(x,y,0);
    }

    public void AddDeleteList(List<(int X, int Y)> positions) {
        List<(int X, int Y)> tmpList = new List<(int X, int Y)>();
        foreach (var pos in positions) {
            int cur_x = pos.X;
            int cur_y = pos.Y;
            tmpList.Add((X: cur_x, Y: cur_y));
        }
        int tmpCnt = tmpList.Count;
        bool hasALRD=false;
        for (int i = 0; i < dLIdx; i++) {
            int dLCnt = deleteList[i].Count;
            if (tmpCnt > dLCnt)
            {
                List<(int X, int Y)> exceptList = tmpList.Except(deleteList[i]).ToList();
                if (exceptList.Count != 0)
                {
                    if (exceptList.Count != tmpList.Count)
                    {
                        deleteList[i] = tmpList;
                        return;
                    }
                }
            }
            else if (tmpCnt < dLCnt)
            {
                List<(int X, int Y)> exceptList = deleteList[i].Except(tmpList).ToList();
                if (exceptList.Count != 0) hasALRD = true;
            }
            else if (tmpCnt == dLCnt)
            {
                List<(int X, int Y)> exceptList = deleteList[i].Except(tmpList).ToList();
                if (exceptList.Count == 0) hasALRD = true;
            }
            
        }
        if (!hasALRD) {
            deleteList[dLIdx] = tmpList;
            dLIdx++;
        }
    }

    public void NowAddDeleteList(List<(int X, int Y)> positions)
    {
        List<(int X, int Y)> tmpList = new List<(int X, int Y)>();
        foreach (var pos in positions)
        {
            int cur_x = pos.X;
            int cur_y = pos.Y;
            tmpList.Add((X: cur_x, Y: cur_y));
        }
        int tmpCnt = tmpList.Count;
        bool hasALRD = false;
        for (int i = 0; i < nDLIdx; i++)
        {
            int dLCnt = nowDeleteList[i].Count;
            if (tmpCnt > dLCnt)
            {
                List<(int X, int Y)> exceptList = tmpList.Except(nowDeleteList[i]).ToList();
                if (exceptList.Count != 0)
                {
                    if (exceptList.Count != tmpList.Count)
                    {
                        nowDeleteList[i] = tmpList;
                        return;
                    }
                }
            }
            else if (tmpCnt < dLCnt)
            {
                List<(int X, int Y)> exceptList = nowDeleteList[i].Except(tmpList).ToList();
                if (exceptList.Count != 0) hasALRD = true;
            }
            else if (tmpCnt == dLCnt)
            {
                List<(int X, int Y)> exceptList = nowDeleteList[i].Except(tmpList).ToList();
                if (exceptList.Count == 0) hasALRD = true;
            }

        }
        if (!hasALRD)
        {
            nowDeleteList[nDLIdx] = tmpList;
            nDLIdx++;
        }
    }

    void MoveLeftRight(PuyoController puyo,PuyoController otherPuyo) {
        int x = puyo.gridX;
        int y = puyo.gridY;

        int Ox = otherPuyo.gridX;
        int Oy = otherPuyo.gridY;

        if (!puyo.used)
        {

            //transform.position += Vector3.down * Time.deltaTime;
            //float h = Input.GetAxisRaw("Horizontal");
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (puyo.canMove(x - 1, y)&&!((x-1==Ox)&&(y==Oy)&&!puyo.canMove(x-2,y)))
                {
                    //transform.position += GridToWorld(-1,0);
                    puyo.gridX=puyo.gridX-1;
                    /*
                    if (!canMove(gridX, gridY - 1))
                        FixToField();
                    */
                }
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (puyo.canMove(x + 1, y) && !((x + 1 == Ox) && (y == Oy) && !puyo.canMove(x+2,y)))
                {
                    //transform.position += GridToWorld(1,0);
                    puyo.gridX=puyo.gridX+1;
                    /*
                    if (!canMove(gridX, gridY - 1))
                        FixToField();
                    */
                }

            }
        }

    }

    void Awake() {
        Instance = this;
    }

    public PuyoController SpawnNextPuyo() {
        GameObject puyo = Instantiate(puyoPrefab);

        PuyoController pc = puyo.GetComponent<PuyoController>();

        pc.gridX = WIDTH / 2;
        pc.gridY = HEIGHT - 1;
        pc.enabled = true;
        return pc;
    }//2000+400+800+1200+1600

    public PuyoController SpawnNextPuyo2()
    {
        GameObject puyo = Instantiate(puyoPrefab);

        PuyoController pc = puyo.GetComponent<PuyoController>();

        pc.gridX = WIDTH / 2;
        pc.gridY = HEIGHT - 2;
        pc.enabled = true;
        return pc;
    }

    public int countPuyo() {
        puyoCount++;
        return puyoCount;
    }

    public List<(int X,int Y)> JudgeConnect(int x,int y) {
        List<(int X, int Y)> list = new List<(int X, int Y)>();
        List<(int X, int Y)> ans = new List<(int X, int Y)>();
        list.Add((X: x, Y: y));
        int[,] visited = new int[WIDTH, HEIGHT];
        int[,] looked = new int[WIDTH, HEIGHT];
        List<int> dx = new List<int>{ 1, -1, 0, 0 };//右,左,上,下
        List<int> dy = new List<int>{ 0, 0, 1, -1 };
        int connect_num = 1;
        int color_num = field[x,y];
        visited[x,y] = 1;
        looked[x,y] = 1;
        while (list.Count != 0) {
            int cur_x = list[0].X;
            int cur_y = list[0].Y;
            list.RemoveAt(0);
            visited[cur_x,cur_y] = 1;
            ans.Add((X: cur_x, Y: cur_y));
            for (int i = 0; i < 4; i++) {
                int candidate_x = cur_x + dx[i];
                int candidate_y = cur_y + dy[i];
                if (candidate_x >= 0 && candidate_x < WIDTH && candidate_y >= 0 && candidate_y < HEIGHT&&visited[candidate_x,candidate_y] == 0)
                {
                    if (field[candidate_x, candidate_y] == color_num) {
                        if (looked[candidate_x,candidate_y] == 0)
                        {
                            connect_num++;
                            looked[candidate_x,candidate_y] = 1;
                        }
                        list.Add((candidate_x, candidate_y));
                    }
                }
            }

        }
        if (connect_num >= 4)
        {
            return ans;
        }
        else
            return new List<(int X,int Y)> ();
    }

    public void DeleteListPuyo() {
        for (int i = 0; i < dLIdx; i++) {
            DeletePuyo(deleteList[i]);
        }
    }

    public void NowDeleteListPuyo()
    {
        for (int i = 0; i < nDLIdx; i++)
        {
            DeletePuyo(nowDeleteList[i]);
        }
    }

    public void DeletePuyo(List<(int X, int Y)> positions) {
        int deletedCount = positions.Count;
        //PuyoController pc = puyo.GetComponent<PuyoController>();
        AddCount(deletedCount, positions[0].X, positions[0].Y);
        foreach (var pos in positions) {
            int x = pos.X;
            int y = pos.Y;
            PuyoController puyo = findPuyo(x,y);
            if (puyo != null)
            {
                Destroy(puyo.gameObject);
                field[x, y] = 0;
            }

        }
        MakeEnable(positions);
    }



    public void MakeEnable(List<(int X, int Y)> positions) {//それぞれがdeleteしてもよい.但し,一つ上に同じ色がある時や,同時に落ちるときはその限りでない.
        //同時に落ちるときはどのように判定するか.
        //最初の高さがそろっていなくても,着地の高さで同時に消える可能性がある.

        //updateは同時ではないのでそれぞれ消すのをリストなどに入れて,gameManagerで消す.
        //gameManagerのupadateではリストの更新のタイミングと消すタイミングがかち合う可能性がある.
        //usedがenabledになった瞬間に変数を増やし,それがfixしリストを追加した時に変数を1減らす.それが0の時だけリストを確認する.
        int[,] willFall = new int[WIDTH, HEIGHT];

        foreach (var pos in positions)
        {
            int x = pos.X;
            int y = pos.Y;
            for (int i = 1; i < HEIGHT - y; i++)
            {
                PuyoController puyo = findPuyo(x, y + i);
                if (puyo != null)
                {
                    if(y>willFall[x,y+i])
                        willFall[x, y + i] = y;
                }

            }


        }

        foreach (var pos in positions)
        {
            int x = pos.X;
            int y = pos.Y;
            for(int i = 1; i < HEIGHT-y; i++)
            {
                PuyoController puyo = findPuyo(x, y+i);
                if (puyo != null)
                {
                    /*
                    if ((y + i + 1 < HEIGHT )&&(field[x, y + i] == field[x, y + i + 1])) {
                        puyo.deleteBase = false;
                        Debug.Log("make nonDeletable1");
                    }
                    */
                    /*
                    if ((x + 1 < WIDTH) && (willFall[x , y+i] != 0 )&& (willFall[x, y + i] == willFall[x + 1, y + i])&&(field[x,y+i]==field[x+1,y+i])) {
                        puyo.deleteBase = false;
                        Debug.Log("make nonDeletable2");
                    }*/
                    /*
                    for (var cnt = 0; cnt < 10; cnt++)
                    {
                        yield return null;
                    }
                    */
                    if (puyo.enabled == false)
                    {
                        if (!positions.Contains((x, y + i)))
                        {
                            fallPuyoCount++;//消えるやつも数えている.
                            Debug.Log(fallPuyoCount);
                        }
                    
                    }
                    puyo.enabled = true;
                    field[x, y + i] = 0;
                }

            }
            

        }
    }
    public bool JudgeUnderNew(int x,int y) {
        PuyoController puyo = findPuyo(x, y);
        if (puyo.used) return false;

        for (int i = 1; i < HEIGHT - y; i++)
        {
            puyo = findPuyo(x, y + i);
            if (puyo != null)
            {
                return false;
            }

        }
        return true;

    }

    public void AddCount(int deletedCount,int x,int y) {
        PuyoController puyo = findPuyo(x, y);
        if (puyo != null) {
            if (puyo.used)
            {
                chain_add++;
            }
            else {
                chain_add = 1;
            }
        }

        score += deletedCount * 100*chain_add;
        updateScoreText();
    }

    public PuyoController findPuyo(int x, int y) { 
        PuyoController[] puyos= FindObjectsOfType<PuyoController>();
        foreach (var puyo in puyos) {
            if (puyo.gridX == x && puyo.gridY == y) {
                return puyo;
            }
        }
        return null;
    }

    public bool hasUpper(int x,int y)
    {
        if ((y + 1 < HEIGHT) && findPuyo(x, y + 1))
        {
            return true;
        }
        else
            return false;

    }

    public void updateScoreText() {
        scoreText.text = "Score:" + score;
    }
}
