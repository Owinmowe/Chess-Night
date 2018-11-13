using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RotationOnZero : MonoBehaviour {

    private void OnDisable()
    {
        gameObject.GetComponent<RectTransform>().rotation = new Quaternion(0,0,0,0);
    }
}
