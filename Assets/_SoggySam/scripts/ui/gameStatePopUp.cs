using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameStatePopUp : MonoBehaviour
{
    // this canvas group add a simple and fade opacity fader
    private CanvasGroup myUIgroup;

    // the list of the UI objects we want to show
    [SerializeField] private GameObject[] uiList;

    // fade accellerate float
    private float fadeAcceleration = 0f;

    // This hides all the objects, unhides the one we want and start the fade anim
    public void showPopup(int i)
    {
        // make group visible
        myUIgroup.alpha = 1;

        // fade accelleration reset
        fadeAcceleration = 0f;

        // turn off all elements, this might be 20+ one day
        if (uiList.Length > 0)
        {
            foreach(GameObject GO in uiList)
            {
                GO.SetActive(false);
            }
        }

        // turn on the right one
        uiList[i].SetActive(true);

    }

    // Start is called before the first frame update
    void Start()
    {
        //set canvas group
        myUIgroup = GetComponent<CanvasGroup>();
        showPopup(0);
    }

    // Update is called once per frame
    void Update()
    {
        // if we are fading
        if ( myUIgroup.alpha > 0f)
        {
            // fade a bit
            myUIgroup.alpha -= fadeAcceleration;

            // speed up the fade
            fadeAcceleration += Time.deltaTime * .001f;

            // if we have faded a lot, set to zero
            if (myUIgroup.alpha < 0.01f) myUIgroup.alpha = 0f;
        }
    }
}
