using UnityEngine;
 
public class BackgroundElement : MonoBehaviour
{
    [SerializeField]
    private float speed;

    // Read by BackgroundDecorSpawner, so the decoration moves at exactly this speed.
    public float Speed
    {
        get { return speed; }
    }

    // Update is called once per frame
    void Update()//Spawns enemies and moves them down the screen
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
    }

}