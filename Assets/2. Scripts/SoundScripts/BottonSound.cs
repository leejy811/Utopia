using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottonSound : MonoBehaviour
{
    // Start is called before the first frame update
    public void Click(string NAME)
    {
        AkSoundEngine.PostEvent(NAME, gameObject);
    }


}
