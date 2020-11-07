using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button : MonoBehaviour
{

    public Text label;

    public RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            return _rectTransform;
        }
    }
    RectTransform _rectTransform;

    public Manager calcManager
    {
        get
        {
            if (_calcManager == null)
                _calcManager = GetComponentInParent<Manager>();
            return _calcManager;
        }
    }
    static Manager _calcManager;

    public void onTapped()
    {
        Debug.Log("Tapped: " + label.text);
        calcManager.buttonTapped(label.text[0]);
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
