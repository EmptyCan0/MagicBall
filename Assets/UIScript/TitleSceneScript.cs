using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NCMB;
using UnityEngine.SceneManagement;
using System.Linq;


public class TitleSceneScript : MonoBehaviour
{
    public Text Name;
    public Text NameGene;
    public RectTransform TextPos;
    public RectTransform TextPos2;
    string BeforeRate, BeforeRate2;
    int BeforeRank, BeforeRank2;
    Text[] Texts = new Text[10];
    Text[] Rivals = new Text[10];
    public Text[] LogIns = new Text[10];
    public GameObject canvas;
    int i, j = 0;
    int indexcount;
    bool RateisSame = false;
    bool NameisSame = false;
    int IndexNum = 0;
    int MyRank = 0;

    bool isSecond, isSecond2 = false;



    void Start()
    {
        NCMBObject obj2 = new NCMBObject("PlayerRate");
        obj2.ObjectId = PlayerPrefs.GetString("PlayerID");
        print(PlayerPrefs.GetString("PlayerID"));
        obj2.FetchAsync((NCMBException e) => {
            if (e != null)
            {
                //エラー処理
                print("データ取得失敗");
            }
            else
            {
                //成功時の処
                var Rate = obj2["Rate"];
                print(obj2["Name"]);
                Name.text = PlayerPrefs.GetString("PlayerName") + ", レート: " + Rate;
            }
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void GotoOnline()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void GotoNotOnline()
    {
        SceneManager.LoadScene("GameSceneNotOnline");
    }

    public void GotoNotBattingPractice()
    {
        SceneManager.LoadScene("GameSceneAI");
    }


    public void PanleAppear(RectTransform rect)
    {
        rect.anchoredPosition = Vector2.zero;
    }

    public void PanelDelete(RectTransform rect)
    {
        rect.anchoredPosition = new Vector2(0, 1350);
    }

    public void Serch()
    {
        if (isSecond2)
        {
            return;
        }
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("PlayerRate");
        query.OrderByDescending("Rate");
        query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
        {
            if (e != null)
            {
                Debug.LogWarning("取得に失敗: " + e.ErrorMessage);
            }
            else
            {
                var rates = objList.Select(o => System.Convert.ToInt32(o["Rate"]));
                var names = objList.Select(o => System.Convert.ToString(o["Name"]));

                foreach (var item in rates)
                {
                    if (i > 9)
                    {
                        continue;
                    }
                    float Bias = 50f;
                    Text UI = Instantiate(NameGene, new Vector2(TextPos.anchoredPosition.x, TextPos.anchoredPosition.y - (Bias * i)), Quaternion.identity);
                    UI.text = item + "";
                    i++;
                    Texts[i - 1] = UI;
                    if (item == PlayerPrefs.GetInt("PlayerRate") && !RateisSame)
                    {
                        RateisSame = true;
                        IndexNum = i - 1;
                    }

                }
                for (int n = i; n < 10; n++)
                {
                    float Bias = 50f;
                    Text UI = Instantiate(NameGene, new Vector2(TextPos.anchoredPosition.x, TextPos.anchoredPosition.y - (Bias * n)), Quaternion.identity);
                    UI.text = "---";
                    Texts[n] = UI;
                    Texts[n].transform.SetParent(this.canvas.transform, false);
                }
                i = 0;
                bool EqualToPlayerRate = false;
                foreach (var item in names)
                {
                    if (i > 9)
                    {
                        continue;
                    }
                    // Debug.Log(": " + item);
                    string Rate = Texts[i].text;
                    if (Rate == BeforeRate) {
                        Texts[i].text = BeforeRank + "位:" + item + ":" + Rate;
                    }
                    else
                    {
                        Texts[i].text = i + 1 + "位:" + item + ":" + Rate;
                        BeforeRank = i + 1;
                    }
                    BeforeRate = Rate;
                    Texts[i].transform.SetParent(this.canvas.transform, false);
                    if (item == PlayerPrefs.GetString("PlayerName") || IndexNum == i)
                    {
                        Texts[i].text = i + 1 + "位:" + PlayerPrefs.GetString("PlayerName") + ":" + Rate;
                        Texts[i].color = Color.red;
                        RateisSame = false;
                        IndexNum = 0;
                    }
                    i++;
                }
                isSecond2 = true;

            }
        });

    }

    public void SerchNearRate()
    {
        if (isSecond)
        {
            return;
        }
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("PlayerRate");
        query.OrderByDescending("Rate");
        query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
        {
            if (e != null)
            {
                Debug.LogWarning("取得に失敗: " + e.ErrorMessage);
            }
            else
            {
                indexcount = 0;
                var rates = objList.Select(o => System.Convert.ToInt32(o["Rate"]));
                
                var names = objList.Select(o => System.Convert.ToString(o["Name"]));

                i = 0;
                foreach (var item in rates) //自分の場所をまず把握する
                {
                    i++;
                    if (PlayerPrefs.GetInt("PlayerRate") == item)
                    {
                        print("上から" + i + "番目にいる");
                        MyRank = i;
                        break;
                    }

                }

                int MaxLength = 4;
                int MaxInvest = MaxLength + MyRank;
                print("rank = " + MyRank);
                print(MaxInvest);
                bool Stop = false;
                int StopNum = 0;
                int NewRate = 0;
                int NewRank = 0;
                int CopyNewRank = 0;
                int[] Ranks = new int[9];
                int BeforeRating = 0;
                //先ほどの自分の順位を基に、周辺レートのプレイヤーを取ってくる
                foreach (var item in rates)
                {
                    j++;
                    if (NewRate != item)
                    {
                        NewRank=j;//レートが変化しているなら順位を変える
                    }
                    NewRate = item;

                    if (Mathf.Abs(MyRank - j) > MaxLength)
                    {
                        //表示するランク帯ではないので無視
                    }
                    else
                    {
                        if (!Stop)
                        {
                            StopNum = j; //j番目からランキング表示
                            CopyNewRank = NewRank;//いま取ってきた順位を取得、これは自分ではない（自分が1位ではない時に限る）
                            Ranks[indexcount] = NewRank;//表示するランク順位の初期値を設定
                            Stop = true;//この関数は一回しか呼ばれない
                        }

                        if (BeforeRating == NewRate) //BeforeRatingは1個前のitem つまり同じレートが続いてるときである
                        {
                            Ranks[indexcount] = Ranks[indexcount - 1];//一個前の順位を持ってくる
                        }
                        else if(indexcount != 0)//一個前とレートが異なり、indexcount!=0のとき（自分が1位のときは省く）
                        {
                            Ranks[indexcount] = NewRank; //同率かつ1位ではないので順位を下げる
                        }
                        float Bias = 50f;
                        Text UI = Instantiate(NameGene, new Vector2(TextPos2.anchoredPosition.x, TextPos2.anchoredPosition.y - (Bias * indexcount)), Quaternion.identity);
                        UI.text = NewRate + "";
                        Rivals[indexcount] = UI;
                        Rivals[indexcount].transform.SetParent(this.canvas.transform, false);
                        indexcount++;
                        BeforeRating = item;
                        if (j >= MaxInvest)
                        {
                            continue;
                        }
                    }
                }
                int MaxCount = indexcount; ;
                print(StopNum + "StopNUm");
                i = 0;
                indexcount = 0;

                foreach (var item in names)
                {
                    i++;
                    if (i >= StopNum)//STopNumは表示し始める一番最初の位置
                    {
                        if (indexcount >= MaxCount)
                        {
                            continue;
                        }
                        string Rate = "";
                        if (Rivals[indexcount].text != null)
                        {
                            Rate = Rivals[indexcount].text;
                        }
                       // print(Rate);
                        if (Rate == BeforeRate2)
                        {

                            if (PlayerPrefs.GetString("PlayerName") == item)
                            {
                                Rivals[indexcount].color = Color.red;
                            }
                            Rivals[indexcount].text = (Ranks[indexcount]) + "位:" + item + ":" + Rate;
                            print(Rivals[indexcount].text);
                        }
                        else
                        {

                            if (PlayerPrefs.GetString("PlayerName") == item)
                            {
                                Rivals[indexcount].color = Color.red;
                            }
                            Rivals[indexcount].text = (Ranks[indexcount]) + "位:" + item + ":" + Rate;
                            BeforeRank2 = i;
                            print(Rivals[indexcount].text);
                        }

                        if (i == MyRank)
                        {
                            Rivals[indexcount].color = Color.red;
                            Rivals[indexcount].text = MyRank + "位:" + PlayerPrefs.GetString("PlayerName") + ":" + PlayerPrefs.GetInt("PlayerRate");
                        }

                        BeforeRate2 = Rate;

                        Rivals[indexcount].transform.SetParent(this.canvas.transform, false);
                        indexcount++;

                  
                    }

                }
                isSecond = true;

            }

        });
    }

    public void SerchLogIns()
    {
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("LogIn");
        query.OrderByDescending("updateDate");
        query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
        {
            if (e != null)
            {
                Debug.LogWarning("取得に失敗: " + e.ErrorMessage);
            }
            else
            {            
                var Text = objList.Select(o => System.Convert.ToString(o["Text"]));
                int i = 0;
                foreach(var item in Text)
                {
                    if(i >= 9)
                    {
                        break;
                    }
                    LogIns[i].text = item;
                    i++;
                }
            }
        });



    }
}
