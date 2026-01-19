using System.Collections;
using TMPro;
using UnityEngine;

public class CameraVerticalScroller : MonoBehaviour
{
    public float startSpeed = 1f;
    public float maxSpeed = 5f;
    public float acceleration = 0.2f;
    
    [SerializeField] private TMP_Text text;

    [SerializeField] int delayToStart = 1;
    public bool canMove = false;

    private float currentSpeed;

    void Start()
    {
        currentSpeed = startSpeed;
        StartCoroutine(ActivateCameraMovement());
    }

    void Update()
    {
        if (!canMove) return;

        currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);

        transform.position += Vector3.up * currentSpeed * Time.deltaTime;
    }
    IEnumerator ActivateCameraMovement()
    {
        yield return new WaitForSeconds(delayToStart);
        canMove = true;
        if (text != null)text.gameObject.SetActive(false);
        
    }
}