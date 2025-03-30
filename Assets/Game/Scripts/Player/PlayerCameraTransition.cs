using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
public class PlayerCameraTransition : MonoBehaviour
{
    public string triggerTag;
    public CinemachineCamera primaryCamera;
    public CinemachineCamera[] allCameras;
    private void Start(){
        switchToCamera(primaryCamera);
    }
    private void OnTriggerEnter2D(Collider2D other){
        print("trigger");
        if(other.CompareTag(triggerTag)){
            CinemachineCamera target = other.GetComponentInChildren<CinemachineCamera>();
            switchToCamera(target);
            print("change area");
        }
    }
    private void switchToCamera(CinemachineCamera target){
        foreach(var cam in allCameras){
            cam.enabled=cam==target;
        }
    }
}
