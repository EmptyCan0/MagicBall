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
                        IndexNum = -1;
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
                        IndexNum = -1;
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
        if (currentRank < MaxLength) { NumSkip = 0; } else { NumSkip = currentRank - MaxLength; }

        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("PlayerRate");
        query.OrderByDescending("Rate");
        query.Skip = NumSkip;
        query.Limit = 2 * MaxLength + 1;
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

               


                int i = 0;
                for(i = 0; i < 10; i++)
                {
                    Rivals[i] = null;
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
                    NumSkip++;
                    if (i > 9) { continue; }
                    string name = Rivals[i];
                    Rivals[i] = (NumSkip + "位:" + name + ":" + item);
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

                    if (i == MaxLength -1) { UI.color = Color.red; }
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
