using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button buttonCreate;

    void Awake()
    {
        if(PlayerPrefs.GetInt("MessageBoardCount") >= 5)
        {
            buttonCreate.gameObject.SetActive(false);
        } else
        {
            buttonCreate.gameObject.SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
