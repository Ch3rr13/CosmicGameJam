using UnityEngine;

public class BackgroundElement : MonoBehaviour
{
    [SerializeField]
    private float speed;

    // Update is called once per frame
    void Update()//Spawns enemies and moves them down the screen
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
    }

}
