using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; 

public class PokerCardManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] pokerCard;

    List<GameObject> LpokerCard;
    List<GameObject> LmyPokerCard;

    [SerializeField]
    private Transform pokerCardSpawnPoint;
    [SerializeField]
    private Transform TpokerCardView;

    [SerializeField]
    private Transform TpokerCardLR;
    [SerializeField]
    private Transform TpokerCardRL;








    private Vector3 targetPos = default;
    private Quaternion targetRot = default;


    private void Awake()
    {
        LpokerCard = new List<GameObject>();

        LmyPokerCard = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetPokerDeck();

        StartCoroutine("PokerCardSpawn");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            Application.Quit();
        }


        if (Input.GetKeyDown(KeyCode.F1))
        {
            StartCoroutine("PokerCardGetOne");
        }
    }

    private void SetPokerDeck()
    {
        for(int i=0; i<pokerCard.Length; i++)
        {
            LpokerCard.Add(pokerCard[i]);
        }

        LpokerCard = Shuffle(LpokerCard);

        foreach (var poker in LpokerCard)
        {
            Console.WriteLine(poker);
        }

    }


    /// <summary>
    /// Shuffle Algorithm
    /// </summary>
    private List<T> Shuffle<T>(List<T> list)
    {
        int n = list.Count;

        T temp = default;

        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);

            temp = list[k];


            list[k] = list[n];
            list[n] = temp;
        }

        return list;
    }

    ///<summary>
    /// PokerCard SpawnPoint Move
    /// </summary>
    /// 
    private IEnumerator PokerCardSpawn()
    {
        foreach(var poker in LpokerCard)
        {
            float hight = LpokerCard.IndexOf(poker);

            poker.transform.position = pokerCardSpawnPoint.position
                                     + new Vector3(0,hight/ 600.0f , 0);
            
            yield return new WaitForSeconds(0.01f);
        }

        yield return null;
    }

    ///<summary>
    /// PC Get
    /// </summary>
    /// 

    private IEnumerator PokerCardGetOne()
    {
        LmyPokerCard.Add(LpokerCard[LpokerCard.Count - 1]);
        //MoveTransform(LpokerCard[LpokerCard.Count - 1], TpokerCardView.position, 0.5f);
        LpokerCard.RemoveAt(LpokerCard.Count - 1);

        yield return 0.6f;
        yield return null;

        Debug.Log("ALgorithm");

        CardRoundAlgorithm();
    }

    private void MoveTransform(GameObject _gm, Vector3 _vector, float dotTime)
    {
        _gm.transform.DOMove(_vector, dotTime);
    }



    private void CardRoundAlgorithm()
    {
        float[] objLersp = new float[LmyPokerCard.Count+1];

        switch (LmyPokerCard.Count)
        {
            case 1:
                objLersp = new float[] { 0.5f };
                break;
            case 2:
                objLersp = new float[] { 0.27f, 0.73f };
                break;
            case 3:
                objLersp = new float[] { 0.1f, 0.5f, 0.9f };
                break;
            default:
                float interval = 1.0f / (LmyPokerCard.Count - 1);
                for (int i = 0; i < LmyPokerCard.Count; i++)
                {
                    objLersp[i] = interval * i;
                }
                break;
        }

        for (int i = 0; i < LmyPokerCard.Count; i++)
        {
            targetPos = Vector3.Lerp(TpokerCardLR.position, TpokerCardRL.position, objLersp[i]);
            targetRot = Quaternion.Lerp(TpokerCardLR.rotation, TpokerCardRL.rotation, objLersp[i]);

            if (LmyPokerCard.Count >= 4)
            {
                //float curve = Mathf.Sqrt(Mathf.Pow(hight, 2) - Mathf.Pow(objLersp[i] - 0.5f, 2));
                //Debug.Log(Mathf.Pow(hight, 2));
                //Debug.Log(Mathf.Pow(objLersp[i] - 0.5f, 2));
                //Debug.Log(Mathf.Sqrt(Mathf.Pow(hight, 2) - Mathf.Pow(objLersp[i] - 0.5f, 2)));

                //curve = hight >= 0 ? curve : -curve;
                //targetPos.x += curve;

            }

            //_gm.transform.DOMove(_vector, dotTime);
            //LmyPokerCard[i].transform.SetPositionAndRotation(targetPos, targetRot);

            targetPos = new Vector3(targetPos.x, targetPos.y, targetPos.z - i*0.001f);

            LmyPokerCard[i].transform.rotation = targetRot;
            LmyPokerCard[i].transform.DOMove(targetPos, 0.5f);
        }
    }
}


