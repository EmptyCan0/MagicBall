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
    string[] Rivals = new string[10];
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
                int rate = System.Convert.ToInt32(obj2["Rate"]);
                PlayerPrefs.SetInt("PlayerRate", rate);
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
        query.Limit = 10;
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
                string[] NamesChar = new string[10];
                int[] RateNum = new int[10];
                int[] RankNum = new int[10];
                i = 0;
                foreach (var item in rates)
                {
                    RateNum[i] = item;
                    i++;
                }
                i = 0;
                foreach (var item in names)
                {
                    NamesChar[i] = item;
                    i++;
                }
                i = 0;

                int Rank = 0;
                int BeforeRate = -1;
                bool SameAsMyRate = false;
                string CopyName = "";
                RankNum[0] = 1;
                for (i = 0; i < 10; i++)
                {
                    Rank++;
                    if(RateNum[i] == BeforeRate)
                    {
                        RankNum[i] = RankNum[i - 1];
                    }
                    else
                    {
                        RankNum[i] = Rank;
                    }
                    float Bias = 50f;
                    Text UI = Instantiate(NameGene, new Vector2(TextPos.anchoredPosition.x, TextPos.anchoredPosition.y - (Bias * i)), Quaternion.identity);
                    UI.transform.SetParent(this.canvas.transform, false);
                    UI.text = (RankNum[i] + "位:" + NamesChar[i] + ":" + RateNum[i]);

                    if (SameAsMyRate)
                    {
                        if (RateNum[i] == PlayerPrefs.GetInt("PlayerRate") && NamesChar[i] == PlayerPrefs.GetString("PlayerName"))
                        {
                            UI.text = (RankNum[i] + "位:" + CopyName + ":" + RateNum[i]);
                        }
                    }

                    if (RateNum[i] == PlayerPrefs.GetInt("PlayerRate") && !SameAsMyRate)
                    {
                        CopyName = NamesChar[i];
                        UI.text = (RankNum[i] + "位:" + PlayerPrefs.GetString("PlayerName") + ":" + PlayerPrefs.GetInt("PlayerRate"));
                        UI.color = Color.red;
                        SameAsMyRate = true;
                    }

                    BeforeRate = RateNum[i];

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

        int currentRank = 0;

        NCMBQuery<NCMBObject> rankQuery = new NCMBQuery<NCMBObject>("PlayerRate");
        rankQuery.WhereGreaterThan("Rate", PlayerPrefs.GetInt("PlayerRate"));
        rankQuery.CountAsync((int count, NCMBException e) => {
            if (e != null)
            {
                //件数取得失敗
                print("できなかった");
            }
            else
            {
                //件数取得成功
                print("調査はしている");
                currentRank = count + 1; // 自分よりスコアが上の人がn人いたら自分はn+1位
                print(currentRank);


                StartAroundSerch(currentRank);

            }
        });
                
 
    }

    public void StartAroundSerch(int currentRank)
    {
        int NumSkip;
        int MaxLength = 5;
        bool Skip = false;
        if (currentRank < MaxLength) { NumSkip = 0; Skip = false; } else { NumSkip = currentRank - MaxLength; Skip = true; }

        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("PlayerRate");
        query.OrderByDescending("Rate");
        query.Skip = NumSkip;
        query.Limit = 2 * MaxLength - 1;
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

                if (Skip)
                {
                    int i = 0;

                    int[] RateNum = new int[9];
                    int[] RankNum = new int[9];
                    foreach (var item in rates)
                    {
                        RateNum[i] = item;
                        i++;
                    }
                    i = 0;
                    int BeforeRate = RateNum[MaxLength - 1];
                    int k = currentRank;
                    RankNum[MaxLength - 1] = currentRank;
                    for (i = MaxLength - 2; i >= 0; i--)
                    {
                        k--;
                        if (RateNum[i] == BeforeRate)
                        {
                            RankNum[i] = RankNum[i + 1];
                        }
                        else
                        {
                            RankNum[i] = k;
                        }
                        BeforeRate = RateNum[i];
                    }
                    k = currentRank - 1;
                    BeforeRate = -1;
                    for (i = MaxLength - 1; i < 9; i++)
                    {
                        k++;
                        if (RateNum[i] == BeforeRate)
                        {
                            RankNum[i] = RankNum[i - 1];
                        }
                        else
                        {
                            RankNum[i] = k;
                        }
                        BeforeRate = RateNum[i];
                    }
                    for (i = 0; i < 9; i++)
                    {
                        print(RankNum[i] + "位");
                    }
                    i = 0;


                    foreach (var item in names)
                    {
                        if (i > 9) { continue; }
                        print(Rivals[i] + ":1回目");
                        Rivals[i] = item;
                        i++;
                    }



                    i = 0;
                    foreach (var item in rates)
                    {

                        if (i > 9) { continue; }
                        string name = Rivals[i];
                        Rivals[i] = (RankNum[i] + "位:" + name + ":" + item);
                        print(Rivals[i]);
                        i++;
                    }
                    i = 0;
                    for (i = 0; i < 9; i++)
                    {
                        float Bias = 50f;
                        Text UI = Instantiate(NameGene, new Vector2(TextPos2.anchoredPosition.x, TextPos2.anchoredPosition.y - (Bias * i)), Quaternion.identity);
                        UI.transform.SetParent(this.canvas.transform, false);
                        UI.text = Rivals[i];

                        if (i == MaxLength - 1)
                        {
                            UI.color = Color.red;
                            Rivals[i] = (currentRank + "位:" + PlayerPrefs.GetString("PlayerName") + ":" + PlayerPrefs.GetInt("PlayerRate"));
                            UI.text = Rivals[i];
                        }
                    }
                }
                else
                {
                    i = 0;
                    int k = 0;
                    int[] RateNum2 = new int[9];
                    int[] RankNum2 = new int[9];
                    foreach (var item in rates)
                    {
                        RateNum2[i] = item;
                        i++;
                    }
                    int BeforeRate = -1;
                    RankNum2[0] = 1;
                    for (i = 0; i < 9; i++)
                    {
                        k++;
                        if (RateNum2[i] == BeforeRate)
                        {
                            RankNum2[i] = RankNum2[i - 1]; 
                        }
                        else
                        {
                            RankNum2[i] = k;
                        }
                        BeforeRate = RateNum2[i];
                       
                    }
                    i = 0;
                    foreach (var item in names)
                    {
                        if (i > 9) { continue; }
                        
                        Rivals[i] = (RankNum2[i] + "位:" + item + ":" + RateNum2[i]);
                        print(Rivals[i]);
                        i++;
                    }
                    i = 0;

                    for (i = 0; i < 9; i++)
                    {
                        float Bias = 50f;
                        Text UI = Instantiate(NameGene, new Vector2(TextPos2.anchoredPosition.x, TextPos2.anchoredPosition.y - (Bias * i)), Quaternion.identity);
                        UI.transform.SetParent(this.canvas.transform, false);
                        UI.text = Rivals[i];

                        if (i == currentRank - 1)
                        {
                            UI.color = Color.red;
                            Rivals[i] = (currentRank + "位:" + PlayerPrefs.GetString("PlayerName") + ":" + PlayerPrefs.GetInt("PlayerRate"));
                            UI.text = Rivals[i];
                        }
                    }

                }


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
