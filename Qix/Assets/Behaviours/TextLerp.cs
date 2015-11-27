using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextLerp : MonoBehaviour 
{
    public Color start;
    Color end = new Color(1,1,0,1);
    public float lerpTime = 2f;
    public float currentLerpTime;
    public bool change = false;

	// Use this for initialization
	void Start () 
    {
        start = GetComponent<Text>().color;
	}
	
	// Update is called once per frame
	void Update () 
    {

        
        //increment timer once per frame
        currentLerpTime += Time.deltaTime;
        if (currentLerpTime > lerpTime)
        {
            currentLerpTime = 0.0f;
            change = !change;
        }      
      
        float perc = currentLerpTime / lerpTime;
        if(change)   
        {
            GetComponent<Text>().color = Color.Lerp(GetComponent<Text>().color, start, perc);
        }
        else
        {
            GetComponent<Text>().color = Color.Lerp(GetComponent<Text>().color, end, perc);
        }


	}
}
