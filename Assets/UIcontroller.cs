using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIcontroller : MonoBehaviour
{
    player player;
    Text distanceText;

    GameObject result;
    Text final_distance;

    private void Awake()
    {
        player = GameObject.Find("player").GetComponent<player>();
        distanceText = GameObject.Find("distanceText").GetComponent<Text>();
   
        final_distance = GameObject.Find("final_distance").GetComponent<Text>();
        result = GameObject.Find("result");
        result.SetActive(false);
        

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int distance = Mathf.FloorToInt(player.distance);
        distanceText.text = distance +" m";

        if (player.isdead)
        {
            result.SetActive(true);
            final_distance.text = distance + " m";
        }
    }
    public void quit()
    {
     SceneManager.LoadScene("menu");
    }

     public void retry()
    {
     SceneManager.LoadScene("samplescene");
    }
}
