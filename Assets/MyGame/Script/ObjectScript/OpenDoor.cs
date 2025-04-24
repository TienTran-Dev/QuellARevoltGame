using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [SerializeField]
    private GameObject Door;
    private float valueRotation=2f;
    private float openAngle = 90f;
    private bool IsOpen = false;
    private bool IsOpenToOut = false;
    [SerializeField]
    private GameObject checkBox;
    private float targetAngle;

    private void Update()
    {

        CheckIn();

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsOpen = true;
           
            Debug.Log($"{IsOpen}");

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(WaitForCheckIn());
        }
    }

    private void CheckIn()
    {
        if (Door == null) return;
      targetAngle = IsOpen ? openAngle : IsOpenToOut ? -90f : 0f;

        Quaternion targetRot = Quaternion.Euler(0f, targetAngle, 0f);
            Door.transform.localRotation = Quaternion.Lerp(Door.transform.localRotation, targetRot, Time.deltaTime * valueRotation);
            // localrotation xoay trực tiếp child
      
    }

    private IEnumerator WaitForCheckIn()
    {
        yield return new WaitForSeconds(3);
        IsOpen = false;
    }


}
    


