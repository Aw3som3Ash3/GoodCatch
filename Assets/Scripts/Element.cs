using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Element", menuName = "Fish Monster/Elements", order = 2)]

public class Element : ScriptableObject
{
    [SerializeField]
    Element[] strong;
    [SerializeField]
    Element[] veryStrong;
    [SerializeField]
    Element[] weak;
    [SerializeField]
    Element[] veryWeak;

    public int CompareStrength(Element element)
    {
        if (strong.Contains(element))
        {
            return 2;
        }
        else if (veryStrong.Contains(element))
        {
            return 4;
        }
        else if (weak.Contains(element))
        {
            return 1;

        }
        else if (veryWeak.Contains(element))
        {
            return 3;
        }
        else
        {
            return 0;
        }

    }

    public float DamageModifier(Element element)
    {
        if (strong.Contains(element))
        {
            return 0.5f;
        }
        else if (veryStrong.Contains(element))
        {
            return 0;
        }
        else if (weak.Contains(element))
        {
            return 2;

        }
        else if (veryWeak.Contains(element))
        {
            return 4;
        }
        else
        {
            return 1;
        }
    }
}
