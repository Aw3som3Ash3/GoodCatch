using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Element", menuName = "Fish Monster/Elements", order = 2)]

public class Element : ScriptableObject
{


    public enum Effectiveness 
    {
        none,
        strong,
        veryStrong,
        weak,
        veryWeak,
        healing
    }

    [SerializeField]
    Element[] strong;
    [SerializeField]
    Element[] veryStrong;
    [SerializeField]
    Element[] weak;
    [SerializeField]
    Element[] veryWeak;
    [SerializeField]
    Sprite icon;
    public Sprite Icon { get { return icon; } }
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
    public Effectiveness GetEffectiveness(Element element)
    {
        if (strong.Contains(element))
        {
            return Effectiveness.strong;
        }
        else if (veryStrong.Contains(element))
        {
            return Effectiveness.veryStrong;
        }
        else if (weak.Contains(element))
        {
            return Effectiveness.weak;

        }
        else if (veryWeak.Contains(element))
        {
            return Effectiveness.veryWeak;
        }
        else
        {
            return Effectiveness.none;
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
