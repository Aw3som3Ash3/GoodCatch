using UnityEngine;
using UnityEngine.UI;

public class FishIcon : MonoBehaviour
{
    [SerializeField]
    Image icon;
    [SerializeField]
    Image border;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetIcon(Sprite sprite, Color color)
    {
        icon.sprite = sprite;
        border.color = color;
    }
}
