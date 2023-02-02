
using UnityEngine;

public class TimelineShip : MonoBehaviour
{
    private SpriteRenderer shipSprite;
    [SerializeField] private SpriteRenderer shipSelectionSprite;


    private void Start()
    {
        shipSprite = GetComponent<SpriteRenderer>();
    }
}
