using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;


public class Deck : MonoBehaviour
{
    [SerializeField]
    private GameObject[] playerCard;

    [SerializeField]
    private Transform cardPoint;

    [SerializeField]
    private Transform[] playerPoint;

    Vector3 mainCamPosition;
    float card;

    private List<GameObject> pokerCards = new List<GameObject>();
    private List<GameObject> myCard = new List<GameObject>();


    

    private void Awake()
    {
        foreach (var card in playerCard)
        {
            pokerCards.Add(card);
        }

        mainCamPosition = Camera.main.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("StartGame");
    }

    RaycastHit hit;

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // ¸¶¿ì½º ÁÂÇ¥¿¡¼­ ½î´Â ray

        if (Input.GetKeyDown(KeyCode.F1))
        {
            StartCoroutine("IEOneCard");
        }
        //if (Input.GetMouseButtonDown(0))
        //{
        //    StartCoroutine("IERotateCard");
        //}

        if (Input.GetMouseButton(0))
        {
            Debug.Log("Press");
            if (Physics.Raycast(ray, out hit))
            {
                Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 1f);

                if (hit.transform.tag == "PokerCard")
                {
                    hit.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    hit.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y - 0.001f, hit.transform.position.z + 0.001f);
                }
            }
        }


    }

    private IEnumerator StartGame()
    {
        foreach (var cardForeach in pokerCards)
        {
            float hight = (pokerCards.IndexOf(cardForeach));

            cardForeach.gameObject.transform.position =
                cardPoint.transform.position
                + new Vector3(0, hight / 500.0f, 0);

            yield return new WaitForSeconds(0.01f);
        }
    }


    private IEnumerator IEOneCard()
    {
        myCard.Add(pokerCards[pokerCards.Count - 1]);
        pokerCards.RemoveAt(pokerCards.Count - 1);
        
        for(int i= myCard.Count-1 ; i<myCard.Count; i++)
        {
            while (myCard[i].transform.position != playerPoint[myCard.IndexOf(myCard[i])].transform.position)
            {
                myCard[i].transform.position = Vector3.Lerp(myCard[i].transform.position, playerPoint[myCard.IndexOf(myCard[i])].transform.position, 10.0f * Time.deltaTime);
                myCard[i].transform.localRotation = Quaternion.Lerp(myCard[i].transform.localRotation, playerPoint[myCard.IndexOf(myCard[i])].transform.localRotation, 10.0f * Time.deltaTime);
                yield return null;
            }
        }

        yield return null;
    }

    private IEnumerator IERotateCard()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 1f);


            if (hit.transform.tag == "PokerCard")
            {
                Debug.Log(hit.transform.name);

                while (hit.transform.localRotation != Quaternion.Euler(new Vector3(0, 0, 180)))
                {
                    hit.transform.localRotation = Quaternion.Lerp(hit.transform.localRotation, Quaternion.Euler(new Vector3(-30, 0, 180)), 10.0f * Time.deltaTime);
                    yield return null;
                }
            }
        }
        yield return null;
    }
}
