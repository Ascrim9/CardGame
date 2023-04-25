using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Experimental.AI;

public class PokerCardManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] pokerCard;

    List<GameObject> LpokerCard;
    List<GameObject> LmyPokerCard;
    List<GameObject> LcomputerPokerCard;

    [SerializeField]
    private Transform pokerCardSpawnPoint;
    [SerializeField]
    private Transform TpokerCardView;

    [SerializeField]
    private Transform TpokerCardLR;
    [SerializeField]
    private Transform TpokerCardRL;

    [SerializeField]
    private Transform TpokerCardL;
    [SerializeField]
    private Transform TpokerCardR;

    [SerializeField]
    private Transform DieCardPoint;


    private Vector3 targetPos = default;
    private Quaternion targetRot = default;

    private RaycastHit hit;
        Vector3 dist;
    private Vector3 fixedMoveCard;

    [SerializeField]
    private Text mainText;



    private void Awake()
    {
        LpokerCard = new List<GameObject>();

        LmyPokerCard = new List<GameObject>();
        LcomputerPokerCard = new();

        fixedMoveCard = default;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetPokerDeck();

        StartCoroutine(nameof(PokerCardSpawn));
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 마우스 좌표에서 쏘는 ray



        if (Input.GetKeyDown(KeyCode.F12))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            StartCoroutine(nameof(PokerCardGetFive));
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            StartCoroutine(nameof(PokerCardGetOne));
        }

        if(Input.GetKeyDown(KeyCode.F2))
        {
            StartCoroutine(nameof(ChangedCard));
        }


        if (Physics.Raycast(ray, out hit))
        {
            Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 1f);

            if (hit.transform.CompareTag("PokerCard"))
            {
                if (dist == Vector3.zero) // 영벡터이면 dist를 계산한다.
                {
                    dist = hit.point - hit.transform.position;
                }

               /* hit.transform.position = hit.point - dist*/; // dist를 빼서 위치를 보정한다.


                Vector3 nextPos = new(hit.transform.position.x, 1.8f, hit.transform.position.z);
                //hit.transform.position = new Vector3(hit.transform.position.x, 1.85f, hit.transform.position.z);
                hit.transform.DOMove(nextPos, 0.1f);
            }

            foreach (var hit in LmyPokerCard)
            {   
            Vector3 originalPos = new(hit.transform.position.x, 1.7f, hit.transform.position.z);
            hit.transform.DOMove(originalPos, 0.1f);
            }
        }



    }

    private void LateUpdate()
    {
        CardRoundAlgorithm();


        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 마우스 좌표에서 쏘는 ray

            if (Physics.Raycast(ray, out hit))
            {
                Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 1f);

                if (hit.transform.CompareTag("PokerCard"))
                {
                    if (dist == Vector3.zero) // 영벡터이면 dist를 계산한다.
                    {
                        dist = hit.point - hit.transform.position;
                    }

                    hit.transform.position = hit.point - dist; // dist를 빼서 위치를 보정한다.

                    //hit.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y - 0.001f, hit.transform.position.z + 0.001f);
                
                    if(hit.transform.position.y >= 0.23)
                    {

                        Debug.Log(hit.transform.name);

                        GameObject testobject = hit.transform.gameObject;
                        LmyPokerCard.Remove(testobject);

                        testobject.transform.DOMove(DieCardPoint.position, 1.5f);
                        //testobject.transform.localRotation = DieCardPoint.localRotation;
                    }
                
                }
            }
        }
        else
        {
            dist = Vector3.zero;
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
        LmyPokerCard.Add(LpokerCard[^1]);
        //MoveTransform(LpokerCard[LpokerCard.Count - 1], TpokerCardView.position, 0.5f);
        LpokerCard.RemoveAt(LpokerCard.Count - 1);

        yield return null;

        Debug.Log("ALgorithm");

        CardRoundAlgorithm();
    }

    private IEnumerator PokerCardGetFive()
    {
        mainText.text = "---GameStart---";


        for (int i = 0; i < 5; i++)
        {
            LmyPokerCard.Add(LpokerCard[^1]);
            //MoveTransform(LpokerCard[LpokerCard.Count - 1], TpokerCardView.position, 0.5f);
            LpokerCard.RemoveAt(LpokerCard.Count - 1);
            CardRoundAlgorithm();

            yield return new WaitForSeconds(0.2f);

            LcomputerPokerCard.Add(LpokerCard[^1]);
            //MoveTransform(LpokerCard[LpokerCard.Count - 1], TpokerCardView.position, 0.5f);
            LpokerCard.RemoveAt(LpokerCard.Count - 1);
            CardRoundAlgorithm2();


            yield return new WaitForSeconds(0.5f);
        }

        yield return null;

        mainText.text = "---Change Cards?---";

    }

    private IEnumerator PokerCardChanged()
    {
        mainText.text = "---GoodLuck---";

        Debug.Log(LmyPokerCard.Count);


        for (int i = LmyPokerCard.Count; i < 5; i++)
        {
            LmyPokerCard.Add(LpokerCard[^1]);
            //MoveTransform(LpokerCard[LpokerCard.Count - 1], TpokerCardView.position, 0.5f);
            LpokerCard.RemoveAt(LpokerCard.Count - 1);
            CardRoundAlgorithm();

            yield return new WaitForSeconds(0.2f);
        }

        yield return null;

        mainText.text = "---Finish!!---";

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

    private void CardRoundAlgorithm2()
    {
        float[] objLersp = new float[LcomputerPokerCard.Count + 1];

        switch (LcomputerPokerCard.Count)
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
                float interval = 1.0f / (LcomputerPokerCard.Count - 1);
                for (int i = 0; i < LcomputerPokerCard.Count; i++)
                {
                    objLersp[i] = interval * i;
                }
                break;
        }

        for (int i = 0; i < LcomputerPokerCard.Count; i++)
        {
            targetPos = Vector3.Lerp(TpokerCardL.position, TpokerCardR.position, objLersp[i]);
            targetRot = Quaternion.Lerp(TpokerCardL.rotation, TpokerCardR.rotation , objLersp[i]);

            if (LcomputerPokerCard.Count >= 4)
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

            targetPos = new Vector3(targetPos.x, targetPos.y, targetPos.z - i * 0.001f);

            LcomputerPokerCard[i].transform.rotation = targetRot;
            LcomputerPokerCard[i].transform.DOMove(targetPos, 0.5f);
        }
    }

    private IEnumerator ChangedCard()
    {
        StartCoroutine(nameof(PokerCardChanged));

        yield return new WaitForSeconds(1.0f);
    }
}


