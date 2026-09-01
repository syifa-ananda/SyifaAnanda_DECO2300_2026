using UnityEngine;

public class RotateFurniture : MonoBehaviour
{
    public void Rotate90()
    {
        transform.Rotate(0f, 90f, 0f, Space.World);
    }
}