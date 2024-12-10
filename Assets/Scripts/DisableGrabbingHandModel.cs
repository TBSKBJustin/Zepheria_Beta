using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject leftHandModel;
    public GameObject rightHandModel;
    
    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(HideGrabbingHand);
        grabInteractable.selectExited.AddListener(ShowGrabbingHand);
    }

    public void HideGrabbingHand(SelectEnterEventArgs args)
    {
        if(args.interactableObject.transform.tag == "Left Hand")
        {
            leftHandModel.SetActive(false);
        }
        else if(args.interactableObject.transform.tag == "Right Hand")
        {
            rightHandModel.SetActive(false);
        }
    }

    public void ShowGrabbingHand(SelectExitEventArgs args)
    {
        if (args.interactableObject.transform.tag == "Left Hand")
        {
            leftHandModel.SetActive(true);
        }
        else if (args.interactableObject.transform.tag == "Right Hand")
        {
            rightHandModel.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
